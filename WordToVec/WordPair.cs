using System;

namespace WordToVec
{
    public class WordPair : IComparable
    {
        private string _word1;
        private string _word2;
        private double _relatedBy;

        /// <summary>
        /// Constructor of the WordPair object. WordPair stores the information about two words and their similarity scores.
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        /// <param name="relatedBy">Similarity score between first and second word.</param>
        public WordPair(string word1, string word2, double relatedBy)
        {
            _word1 = word1;
            _word2 = word2;
            _relatedBy = relatedBy;
        }

        public override bool Equals(object obj)
        {
            var second = (WordPair)obj;
            return _word1.Equals(second._word1) && _word2.Equals(second._word2);
        }

        public int CompareTo(object obj)
        {
            if (_relatedBy < ((WordPair)obj)._relatedBy)
            {
                return 1;
            }
            else
            {
                if (_relatedBy > ((WordPair)obj)._relatedBy)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Accessor for the similarity score.
        /// </summary>
        /// <returns>Similarity score.</returns>
        public double GetRelatedBy()
        {
            return _relatedBy;
        }

        /// <summary>
        /// Mutator for the similarity score.
        /// </summary>
        /// <param name="relatedBy">New similarity score</param>
        public void SetRelatedBy(double relatedBy)
        {
            _relatedBy = relatedBy;
        }

        /// <summary>
        /// Accessor for the first word.
        /// </summary>
        /// <returns>First word.</returns>
        public string GetWord1()
        {
            return _word1;
        }

        /// <summary>
        /// Accessor for the second word.
        /// </summary>
        /// <returns>Second word.</returns>
        public string GetWord2()
        {
            return _word2;
        }
    }
}