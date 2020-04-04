using System;
using Dictionary.Dictionary;

namespace WordToVec
{
    public class VocabularyWord : Word, IComparable
    {
        private readonly int _count;
        private readonly int[] _code;
        private readonly int[] _point;
        public static int MAX_CODE_LENGTH = 40;
        private int _codeLength;

        /**
         * <summary>Constructor for a {@link VocabularyWord}. The constructor gets name and count values and sets the corresponding
         * attributes. It also initializes the code and point arrays for this word.</summary>
         * <param name="name">Lemma of the word</param>
         * <param name="count">Number of occurrences of this word in the corpus</param>
         */
        public VocabularyWord(string name, int count) : base(name)
        {
            this._count = count;
            _code = new int[MAX_CODE_LENGTH];
            _point = new int[MAX_CODE_LENGTH];
            _codeLength = 0;
        }

        /**
         * <summary>Comparator interface to other VocabularyWord's.</summary>
         * <param name="o">Compared word</param>
         * <returns>If the number of occurrences of the current word is less than the number of occurences of o, returns 1.
         * If the number of occurrences of the current word is larger than the number of occurences of o, returns -1.
         * Otherwise, returns 0.</returns>
         */
        public int CompareTo(object o)
        {
            var word = (VocabularyWord) o;
            return word._count.CompareTo(_count);
        }

        /**
         * <summary>Accessor for the count attribute.</summary>
         * <returns>Number of occurrences of this word.</returns>
         */
        public int GetCount()
        {
            return _count;
        }

        /**
         * <summary>Mutator for codeLength attribute.</summary>
         * <param name="codeLength">New value for the codeLength.</param>
         */
        public void SetCodeLength(int codeLength)
        {
            this._codeLength = codeLength;
        }

        /**
         * <summary>Mutator for code attribute.</summary>
         * <param name="index">Index of the code</param>
         * <param name="value">New value for that indexed element of code.</param>
         */
        public void SetCode(int index, int value)
        {
            _code[index] = value;
        }

        /**
         * <summary>Mutator for point attribute.</summary>
         * <param name="index">Index of the point</param>
         * <param name="value">New value for that indexed element of point.</param>
         */
        public void SetPoint(int index, int value)
        {
            _point[index] = value;
        }

        /**
         * <summary>Accessor for the codeLength attribute.</summary>
         * <returns>Length of the Huffman code for this word.</returns>
         */
        public int GetCodeLength()
        {
            return _codeLength;
        }

        /**
         * <summary>Accessor for point attribute.</summary>
         * <param name="index">Index of the point.</param>
         * <returns>Value for that indexed element of point.</returns>
         */
        public int GetPoint(int index)
        {
            return _point[index];
        }

        /**
         * <summary>Accessor for code attribute.</summary>
         * <param name="index">Index of the code.</param>
         * <returns>Value for that indexed element of code.</returns>
         */
        public int GetCode(int index)
        {
            return _code[index];
        }
    }
}