using System;

namespace WordToVec
{
    public class WordPair : IComparable
    {
        private string _word1;
        private string _word2;
        private double _relatedBy;

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

        public double GetRelatedBy()
        {
            return _relatedBy;
        }

        public void SetRelatedBy(double relatedBy)
        {
            _relatedBy = relatedBy;
        }

        public string GetWord1()
        {
            return _word1;
        }

        public string GetWord2()
        {
            return _word2;
        }
    }
}