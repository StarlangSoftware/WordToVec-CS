using System;
using Corpus;
using Dictionary.Dictionary;
using Math;

namespace WordToVec
{
    public class NeuralNetwork
    {
        private readonly double[,] _wordVectors;
        private readonly double[,] _wordVectorUpdate;
        private readonly Vocabulary _vocabulary;
        private readonly WordToVecParameter _parameter;
        private readonly AbstractCorpus _corpus;
        private double[] _expTable;
        private static int EXP_TABLE_SIZE = 1000;
        private static int MAX_EXP = 6;
        private int _vectorLength;

        /**
         * <summary>Constructor for the {@link NeuralNetwork} class. Gets corpus and network parameters as input and sets the
         * corresponding parameters first. After that, initializes the network with random weights between -0.5 and 0.5.
         * Constructs vector update matrix and prepares the exp table.</summary>
         * <param name="corpus">Corpus used to train word vectors using Word2Vec algorithm.</param>
         * <param name="parameter">Parameters of the Word2Vec algorithm.</param>
         */
        public NeuralNetwork(AbstractCorpus corpus, WordToVecParameter parameter)
        {
            var random = new Random(parameter.GetSeed());
            _vectorLength = parameter.GetLayerSize();
            _vocabulary = new Vocabulary(corpus);
            _parameter = parameter;
            _corpus = corpus;
            var row = _vocabulary.Size();
            _wordVectors = new double[row,_vectorLength];
            for (var i = 0; i < row; i++) {
                for (var j = 0; j < _vectorLength; j++) {
                    _wordVectors[i, j] = -0.5 + random.NextDouble();
                }
            }
            _wordVectorUpdate = new double[row,_vectorLength];
            PrepareExpTable();
        }

        /// <summary>
        /// Returns the vocabulary size.
        /// </summary>
        /// <returns>The vocabulary size.</returns>
        public int VocabularySize(){
            return _vocabulary.Size();
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
                var vector = new Vector(0, 0);
                for (var j = 0; j < _vectorLength; j++){
                    vector.Add(_wordVectors[i, j]);
                }
                result.AddWord(new VectorizedWord(_vocabulary.GetWord(i).GetName(), vector));
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

        /// <summary>
        /// Calculate the update of outputs for word indexed with l2. It also calculates the word vector updates for word
        /// indexed at l2.
        /// </summary>
        /// <param name="outputUpdate">Output update to be added.</param>
        /// <param name="outputs">Current outputs.</param>
        /// <param name="l2">Index of the input</param>
        /// <param name="g">Multiplier for the update.</param>
        private void UpdateOutput(double[] outputUpdate, double[] outputs, int l2, double g){
            for (var j = 0; j < _vectorLength; j++){
                outputUpdate[j] += _wordVectorUpdate[l2, j] * g;
            }
            for (var j = 0; j < _vectorLength; j++){
                _wordVectorUpdate[l2, j] += outputs[j] * g;
            }
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
            var outputs = new double[_vectorLength];
            var outputUpdate = new double[_vectorLength];
            while (iteration.GetIterationCount() < _parameter.GetNumberOfIterations())
            {
                iteration.AlphaUpdate(_vocabulary.GetTotalNumberOfWords());
                var wordIndex = _vocabulary.GetPosition(currentSentence.GetWord(iteration.GetSentencePosition()));
                var currentWord = _vocabulary.GetWord(wordIndex);
                Array.Fill(outputs, 0);
                Array.Fill(outputUpdate, 0);
                var b = random.Next(_parameter.GetWindow());
                var cw = 0;
                int lastWordIndex;
                for (var a = b; a < _parameter.GetWindow() * 2 + 1 - b; a++)
                {
                    var c = iteration.GetSentencePosition() - _parameter.GetWindow() + a;
                    if (a != _parameter.GetWindow() && currentSentence.SafeIndex(c))
                    {
                        lastWordIndex = _vocabulary.GetPosition(currentSentence.GetWord(c));
                        for (var j = 0; j < _vectorLength; j++){
                            outputs[j] += _wordVectors[lastWordIndex, j];
                        }
                        cw++;
                    }
                }

                if (cw > 0)
                {
                    for (var j = 0; j < _vectorLength; j++){
                        outputs[j] /= cw;
                    }
                    int l2;
                    double f;
                    double g;
                    if (_parameter.IsHierarchicalSoftMax())
                    {
                        for (var d = 0; d < currentWord.GetCodeLength(); d++)
                        {
                            l2 = currentWord.GetPoint(d);
                            f = 0;
                            for (var j = 0; j < _vectorLength; j++){
                                f += outputs[j] * _wordVectorUpdate[l2, j];
                            }
                            if (f <= -MAX_EXP || f >= MAX_EXP)
                            {
                                continue;
                            }

                            f = _expTable[(int) ((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))];

                            g = (1 - currentWord.GetCode(d) - f) * iteration.GetAlpha();
                            UpdateOutput(outputUpdate, outputs, l2, g);
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
                            f = 0;
                            for (var j = 0; j < _vectorLength; j++){
                                f += outputs[j] * _wordVectorUpdate[l2, j];
                            }
                            g = CalculateG(f, iteration.GetAlpha(), label);
                            UpdateOutput(outputUpdate, outputs, l2, g);
                        }
                    }

                    for (var a = b; a < _parameter.GetWindow() * 2 + 1 - b; a++)
                    {
                        var c = iteration.GetSentencePosition() - _parameter.GetWindow() + a;
                        if (a != _parameter.GetWindow() && currentSentence.SafeIndex(c))
                        {
                            lastWordIndex = _vocabulary.GetPosition(currentSentence.GetWord(c));
                            for (var j = 0; j < _vectorLength; j++){
                                _wordVectors[lastWordIndex, j] += outputUpdate[j];
                            }
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
            var outputUpdate = new double[_vectorLength];
            while (iteration.GetIterationCount() < _parameter.GetNumberOfIterations())
            {
                iteration.AlphaUpdate(_vocabulary.GetTotalNumberOfWords());
                var wordIndex = _vocabulary.GetPosition(currentSentence.GetWord(iteration.GetSentencePosition()));
                var currentWord = _vocabulary.GetWord(wordIndex);
                Array.Fill(outputUpdate, 0);
                var b = random.Next(_parameter.GetWindow());
                for (var a = b; a < _parameter.GetWindow() * 2 + 1 - b; a++)
                {
                    var c = iteration.GetSentencePosition() - _parameter.GetWindow() + a;
                    if (a != _parameter.GetWindow() && currentSentence.SafeIndex(c))
                    {
                        var lastWordIndex = _vocabulary.GetPosition(currentSentence.GetWord(c));
                        var l1 = lastWordIndex;
                        Array.Fill(outputUpdate, 0);
                        int l2;
                        double f;
                        double g;
                        if (_parameter.IsHierarchicalSoftMax())
                        {
                            for (int d = 0; d < currentWord.GetCodeLength(); d++)
                            {
                                l2 = currentWord.GetPoint(d);
                                f = 0;
                                for (var j = 0; j < _vectorLength; j++)
                                {
                                    f += _wordVectors[l1, j] * _wordVectorUpdate[l2, j];
                                }
                                if (f <= -MAX_EXP || f >= MAX_EXP)
                                {
                                    continue;
                                }
                                else
                                {
                                    f = _expTable[(int) ((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))];
                                }

                                g = (1 - currentWord.GetCode(d) - f) * iteration.GetAlpha();
                                for (var j = 0; j < _vectorLength; j++){
                                    outputUpdate[j] += _wordVectorUpdate[l2, j] * g;
                                }
                                for (var j = 0; j < _vectorLength; j++){
                                    _wordVectorUpdate[l2, j] += _wordVectors[l1, j] * g;
                                }
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
                                f = 0;
                                for (var j = 0; j < _vectorLength; j++)
                                {
                                    f += _wordVectors[l1, j] * _wordVectorUpdate[l2, j];
                                }
                                g = CalculateG(f, iteration.GetAlpha(), label);
                                for (var j = 0; j < _vectorLength; j++){
                                    outputUpdate[j] += _wordVectorUpdate[l2, j] * g;
                                }
                                for (var j = 0; j < _vectorLength; j++){
                                    _wordVectorUpdate[l2, j] += _wordVectors[l1, j] * g;
                                }
                            }
                        }

                        for (var j = 0; j < _vectorLength; j++){
                            _wordVectors[l1, j] += outputUpdate[j];
                        }
                    }
                }

                currentSentence = iteration.SentenceUpdate(currentSentence);
            }
            _corpus.Close();
        }
    }
}