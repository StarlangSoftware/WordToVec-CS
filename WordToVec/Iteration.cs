using Corpus;

namespace WordToVec
{
    public class Iteration
    {
        private int _wordCount, _lastWordCount, _wordCountActual;
        private int _iterationCount;
        private int _sentencePosition, _sentenceIndex;
        private readonly double _startingAlpha;
        private double _alpha;
        public readonly Corpus.Corpus corpus;
        private readonly WordToVecParameter _wordToVecParameter;

        /**
         * <summary>Constructor for the {@link Iteration} class. Get corpus and parameter as input, sets the corresponding
         * parameters.</summary>
         * <param name="corpus">Corpus used to train word vectors using Word2Vec algorithm.</param>
         * <param name="wordToVecParameter">Parameters of the Word2Vec algorithm.</param>
         */
        public Iteration(Corpus.Corpus corpus, WordToVecParameter wordToVecParameter)
        {
            this.corpus = corpus;
            this._wordToVecParameter = wordToVecParameter;
            _startingAlpha = wordToVecParameter.GetAlpha();
            _alpha = wordToVecParameter.GetAlpha();
        }

        /**
         * <summary>Accessor for the alpha attribute.</summary>
         * <returns>Alpha attribute.</returns>
         */
        public double GetAlpha()
        {
            return _alpha;
        }

        /**
         * <summary>Accessor for the iterationCount attribute.</summary>
         * <returns>IterationCount attribute.</returns>
         */
        public int GetIterationCount()
        {
            return _iterationCount;
        }

        /**
         * <summary>Accessor for the sentenceIndex attribute.</summary>
         * <returns>SentenceIndex attribute</returns>
         */
        public int GetSentenceIndex()
        {
            return _sentenceIndex;
        }

        /**
         * <summary>Accessor for the sentencePosition attribute.</summary>
         * <returns>SentencePosition attribute</returns>
         */
        public int GetSentencePosition()
        {
            return _sentencePosition;
        }

        /**
         * <summary>Updates the alpha parameter after 10000 words has been processed.</summary>
         */
        public void AlphaUpdate()
        {
            if (_wordCount - _lastWordCount > 10000)
            {
                _wordCountActual += _wordCount - _lastWordCount;
                _lastWordCount = _wordCount;
                _alpha = _startingAlpha * (1 - _wordCountActual /
                    (_wordToVecParameter.GetNumberOfIterations() * corpus.NumberOfWords() + 1.0));
                if (_alpha < _startingAlpha * 0.0001)
                    _alpha = _startingAlpha * 0.0001;
            }
        }

        /**
         * <summary>Updates sentencePosition, sentenceIndex (if needed) and returns the current sentence processed. If one sentence
         * is finished, the position shows the beginning of the next sentence and sentenceIndex is incremented. If the
         * current sentence is the last sentence, the system shuffles the sentences and returns the first sentence.</summary>
         * <param name="currentSentence">Current sentence processed.</param>
         * <returns>If current sentence is not changed, currentSentence; if changed the next sentence; if next sentence is
         * the last sentence; shuffles the corpus and returns the first sentence.</returns>
         */
        public Sentence SentenceUpdate(Sentence currentSentence)
        {
            _sentencePosition++;
            if (_sentencePosition >= currentSentence.WordCount())
            {
                _wordCount += currentSentence.WordCount();
                _sentenceIndex++;
                _sentencePosition = 0;
                if (_sentenceIndex == corpus.SentenceCount())
                {
                    _iterationCount++;
                    _wordCount = 0;
                    _lastWordCount = 0;
                    _sentenceIndex = 0;
                    corpus.ShuffleSentences(1);
                }

                return corpus.GetSentence(_sentenceIndex);
            }

            return currentSentence;
        }
    }
}