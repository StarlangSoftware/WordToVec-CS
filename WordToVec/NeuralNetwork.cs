using System;
using Corpus;
using Dictionary.Dictionary;
using Math;

namespace WordToVec
{
    public class NeuralNetwork
    {
        private readonly Matrix _wordVectors;
        private readonly Matrix _wordVectorUpdate;
        private readonly Vocabulary _vocabulary;
        private readonly WordToVecParameter _parameter;
        private readonly CorpusStream _corpus;
        private double[] _expTable;
        private static int EXP_TABLE_SIZE = 1000;
        private static int MAX_EXP = 6;

        /**
         * <summary>Constructor for the {@link NeuralNetwork} class. Gets corpus and network parameters as input and sets the
         * corresponding parameters first. After that, initializes the network with random weights between -0.5 and 0.5.
         * Constructs vector update matrix and prepares the exp table.</summary>
         * <param name="corpus">Corpus used to train word vectors using Word2Vec algorithm.</param>
         * <param name="parameter">Parameters of the Word2Vec algorithm.</param>
         */
        public NeuralNetwork(CorpusStream corpus, WordToVecParameter parameter)
        {
            this._vocabulary = new Vocabulary(corpus);
            this._parameter = parameter;
            this._corpus = corpus;
            _wordVectors = new Matrix(_vocabulary.Size(), parameter.GetLayerSize(), -0.5, 0.5, 
                new Random(_parameter.GetSeed()));
            _wordVectorUpdate = new Matrix(_vocabulary.Size(), parameter.GetLayerSize());
            PrepareExpTable();
        }

        /**
         * <summary>Constructs the fast exponentiation table. Instead of taking exponent at each time, the algorithm will lookup
         * the table.</summary>
         */
        private void PrepareExpTable()
        {
            _expTable = new double[EXP_TABLE_SIZE + 1];
            for (var i = 0; i < EXP_TABLE_SIZE; i++)
            {
                _expTable[i] = System.Math.Exp((i / (EXP_TABLE_SIZE + 0.0) * 2 - 1) * MAX_EXP);
                _expTable[i] = _expTable[i] / (_expTable[i] + 1);
            }
        }

        /**
         * <summary>Main method for training the Word2Vec algorithm. Depending on the training parameter, CBox or SkipGram algorithm
         * is applied.</summary>
         * <returns>Dictionary of word vectors.</returns>
         */
        public VectorizedDictionary Train()
        {
            var result = new VectorizedDictionary(new TurkishWordComparator());
            if (_parameter.IsCbow())
            {
                TrainCbow();
            }
            else
            {
                TrainSkipGram();
            }

            for (var i = 0; i < _vocabulary.Size(); i++)
            {
                result.AddWord(new VectorizedWord(_vocabulary.GetWord(i).GetName(), _wordVectors.GetRow(i)));
            }

            return result;
        }

        /**
         * <summary>Calculates G value in the Word2Vec algorithm.</summary>
         * <param name="f">F value.</param>
         * <param name="alpha">Learning rate alpha.</param>
         * <param name="label">Label of the instance.</param>
         * <returns>Calculated G value.</returns>
         */
        private double CalculateG(double f, double alpha, double label)
        {
            if (f > MAX_EXP)
            {
                return (label - 1) * alpha;
            }

            if (f < -MAX_EXP)
            {
                return label * alpha;
            }

            return (label - _expTable[(int) ((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))]) * alpha;
        }

        /**
         * <summary>Main method for training the CBow version of Word2Vec algorithm.</summary>
         */
        private void TrainCbow()
        {
            var iteration = new Iteration(_corpus, _parameter);
            _corpus.Open();
            var currentSentence = _corpus.GetSentence();
            var random = new Random(_parameter.GetSeed());
            var outputs = new Vector(_parameter.GetLayerSize(), 0);
            var outputUpdate = new Vector(_parameter.GetLayerSize(), 0);
            while (iteration.GetIterationCount() < _parameter.GetNumberOfIterations())
            {
                iteration.AlphaUpdate(_vocabulary.GetTotalNumberOfWords());
                var wordIndex = _vocabulary.GetPosition(currentSentence.GetWord(iteration.GetSentencePosition()));
                var currentWord = _vocabulary.GetWord(wordIndex);
                outputs.Clear();
                outputUpdate.Clear();
                var b = random.Next(_parameter.GetWindow());
                var cw = 0;
                int lastWordIndex;
                for (var a = b; a < _parameter.GetWindow() * 2 + 1 - b; a++)
                {
                    var c = iteration.GetSentencePosition() - _parameter.GetWindow() + a;
                    if (a != _parameter.GetWindow() && currentSentence.SafeIndex(c))
                    {
                        lastWordIndex = _vocabulary.GetPosition(currentSentence.GetWord(c));
                        outputs.Add(_wordVectors.GetRow(lastWordIndex));
                        cw++;
                    }
                }

                if (cw > 0)
                {
                    outputs.Divide(cw);
                    int l2;
                    double f;
                    double g;
                    if (_parameter.IsHierarchicalSoftMax())
                    {
                        for (var d = 0; d < currentWord.GetCodeLength(); d++)
                        {
                            l2 = currentWord.GetPoint(d);
                            f = outputs.DotProduct(_wordVectorUpdate.GetRow(l2));
                            if (f <= -MAX_EXP || f >= MAX_EXP)
                            {
                                continue;
                            }

                            f = _expTable[(int) ((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))];

                            g = (1 - currentWord.GetCode(d) - f) * iteration.GetAlpha();
                            outputUpdate.Add(_wordVectorUpdate.GetRow(l2).Product(g));
                            _wordVectorUpdate.Add(l2, outputs.Product(g));
                        }
                    }
                    else
                    {
                        for (var d = 0; d < _parameter.GetNegativeSamplingSize() + 1; d++)
                        {
                            int target;
                            int label;
                            if (d == 0)
                            {
                                target = wordIndex;
                                label = 1;
                            }
                            else
                            {
                                target = _vocabulary.GetTableValue(random.Next(_vocabulary.GetTableSize()));
                                if (target == 0)
                                    target = random.Next(_vocabulary.Size() - 1) + 1;
                                if (target == wordIndex)
                                    continue;
                                label = 0;
                            }

                            l2 = target;
                            f = outputs.DotProduct(_wordVectorUpdate.GetRow(l2));
                            g = CalculateG(f, iteration.GetAlpha(), label);
                            outputUpdate.Add(_wordVectorUpdate.GetRow(l2).Product(g));
                            _wordVectorUpdate.Add(l2, outputs.Product(g));
                        }
                    }

                    for (var a = b; a < _parameter.GetWindow() * 2 + 1 - b; a++)
                    {
                        var c = iteration.GetSentencePosition() - _parameter.GetWindow() + a;
                        if (a != _parameter.GetWindow() && currentSentence.SafeIndex(c))
                        {
                            lastWordIndex = _vocabulary.GetPosition(currentSentence.GetWord(c));
                            _wordVectors.Add(lastWordIndex, outputUpdate);
                        }
                    }
                }

                currentSentence = iteration.SentenceUpdate(currentSentence);
            }
            _corpus.Close();
        }

        /**
         * <summary>Main method for training the SkipGram version of Word2Vec algorithm.</summary>
         */
        private void TrainSkipGram()
        {
            var iteration = new Iteration(_corpus, _parameter);
            _corpus.Open();
            var currentSentence = _corpus.GetSentence();
            var random = new Random(_parameter.GetSeed());
            var outputs = new Vector(_parameter.GetLayerSize(), 0);
            var outputUpdate = new Vector(_parameter.GetLayerSize(), 0);
            while (iteration.GetIterationCount() < _parameter.GetNumberOfIterations())
            {
                iteration.AlphaUpdate(_vocabulary.GetTotalNumberOfWords());
                var wordIndex = _vocabulary.GetPosition(currentSentence.GetWord(iteration.GetSentencePosition()));
                var currentWord = _vocabulary.GetWord(wordIndex);
                outputs.Clear();
                outputUpdate.Clear();
                var b = random.Next(_parameter.GetWindow());
                for (var a = b; a < _parameter.GetWindow() * 2 + 1 - b; a++)
                {
                    var c = iteration.GetSentencePosition() - _parameter.GetWindow() + a;
                    if (a != _parameter.GetWindow() && currentSentence.SafeIndex(c))
                    {
                        var lastWordIndex = _vocabulary.GetPosition(currentSentence.GetWord(c));
                        var l1 = lastWordIndex;
                        outputUpdate.Clear();
                        int l2;
                        double f;
                        double g;
                        if (_parameter.IsHierarchicalSoftMax())
                        {
                            for (int d = 0; d < currentWord.GetCodeLength(); d++)
                            {
                                l2 = currentWord.GetPoint(d);
                                f = _wordVectors.GetRow(l1).DotProduct(_wordVectorUpdate.GetRow(l2));
                                if (f <= -MAX_EXP || f >= MAX_EXP)
                                {
                                    continue;
                                }
                                else
                                {
                                    f = _expTable[(int) ((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))];
                                }

                                g = (1 - currentWord.GetCode(d) - f) * iteration.GetAlpha();
                                outputUpdate.Add(_wordVectorUpdate.GetRow(l2).Product(g));
                                _wordVectorUpdate.Add(l2, _wordVectors.GetRow(l1).Product(g));
                            }
                        }
                        else
                        {
                            for (int d = 0; d < _parameter.GetNegativeSamplingSize() + 1; d++)
                            {
                                int target;
                                int label;
                                if (d == 0)
                                {
                                    target = wordIndex;
                                    label = 1;
                                }
                                else
                                {
                                    target = _vocabulary.GetTableValue(random.Next(_vocabulary.GetTableSize()));
                                    if (target == 0)
                                        target = random.Next(_vocabulary.Size() - 1) + 1;
                                    if (target == wordIndex)
                                        continue;
                                    label = 0;
                                }

                                l2 = target;
                                f = _wordVectors.GetRow(l1).DotProduct(_wordVectorUpdate.GetRow(l2));
                                g = CalculateG(f, iteration.GetAlpha(), label);
                                outputUpdate.Add(_wordVectorUpdate.GetRow(l2).Product(g));
                                _wordVectorUpdate.Add(l2, _wordVectors.GetRow(l1).Product(g));
                            }
                        }

                        _wordVectors.Add(l1, outputUpdate);
                    }
                }

                currentSentence = iteration.SentenceUpdate(currentSentence);
            }
            _corpus.Close();
        }
    }
}