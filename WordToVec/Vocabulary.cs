using System;
using System.Collections.Generic;
using Corpus;
using DataStructure;
using Dictionary.Dictionary;

namespace WordToVec
{
    public class Vocabulary
    {
        private readonly List<VocabularyWord> _vocabulary;
        private Dictionary<string, int> _wordMap;
        private int[] _table;
        private int _totalNumberOfWords;

        /**
         * <summary>Constructor for the {@link Vocabulary} class. For each distinct word in the corpus, a {@link VocabularyWord}
         * instance is created. After that, words are sorted according to their occurrences. Unigram table is constructed,
         * where after Huffman tree is created based on the number of occurrences of the words.</summary>
         * <param name="corpus">Corpus used to train word vectors using Word2Vec algorithm.</param>
         */
        public Vocabulary(AbstractCorpus corpus)
        {
            _wordMap = new Dictionary<string, int>();
            var counts = new CounterHashMap<string>();
            corpus.Open();
            var sentence = corpus.GetSentence();
            while (sentence != null){
                for (var i = 0; i < sentence.WordCount(); i++){
                    counts.Put(sentence.GetWord(i).GetName());
                }
                _totalNumberOfWords += sentence.WordCount();
                sentence = corpus.GetSentence();
            }
            corpus.Close();
            _vocabulary = new List<VocabularyWord>();
            foreach (var word in counts.Keys){
                _vocabulary.Add(new VocabularyWord(word, counts[word]));
            }
            _vocabulary.Sort();
            CreateUniGramTable();
            ConstructHuffmanTree();
            _vocabulary.Sort(new TurkishWordComparator());
            for (var i = 0; i < _vocabulary.Count; i++){
                _wordMap[_vocabulary[i].GetName()] = i;
            }
        }

        /**
         * <summary>Returns number of words in the vocabulary.</summary>
         * <returns>Number of words in the vocabulary.</returns>
         */
        public int Size()
        {
            return _vocabulary.Count;
        }

        /**
         * <summary>Searches a word and returns the position of that word in the vocabulary. Search is done using binary search.</summary>
         * <param name="word">Word to be searched.</param>
         * <returns>Position of the word searched.</returns>
         */
        public int GetPosition(Word word)
        {
            return _wordMap[word.GetName()];
        }

        public int GetTotalNumberOfWords()
        {
            return _totalNumberOfWords;
        }
        
        /**
         * <summary>Returns the word at a given index.</summary>
         * <param name="index">Index of the word.</param>
         * <returns>The word at a given index.</returns>
         */
        public VocabularyWord GetWord(int index)
        {
            return _vocabulary[index];
        }

        /**
         * <summary>Constructs Huffman Tree based on the number of occurrences of the words.</summary>
         */
        private void ConstructHuffmanTree()
        {
            var count = new int[_vocabulary.Count * 2 + 1];
            var code = new int[VocabularyWord.MAX_CODE_LENGTH];
            var point = new int[VocabularyWord.MAX_CODE_LENGTH];
            var binary = new int[_vocabulary.Count * 2 + 1];
            var parentNode = new int[_vocabulary.Count * 2 + 1];
            for (var a = 0; a < _vocabulary.Count; a++)
                count[a] = _vocabulary[a].GetCount();
            for (var a = _vocabulary.Count; a < _vocabulary.Count * 2; a++)
                count[a] = 1000000000;
            var pos1 = _vocabulary.Count - 1;
            var pos2 = _vocabulary.Count;
            for (var a = 0; a < _vocabulary.Count - 1; a++)
            {
                int min1I;
                if (pos1 >= 0)
                {
                    if (count[pos1] < count[pos2])
                    {
                        min1I = pos1;
                        pos1--;
                    }
                    else
                    {
                        min1I = pos2;
                        pos2++;
                    }
                }
                else
                {
                    min1I = pos2;
                    pos2++;
                }

                int min2I;
                if (pos1 >= 0)
                {
                    if (count[pos1] < count[pos2])
                    {
                        min2I = pos1;
                        pos1--;
                    }
                    else
                    {
                        min2I = pos2;
                        pos2++;
                    }
                }
                else
                {
                    min2I = pos2;
                    pos2++;
                }

                count[_vocabulary.Count + a] = count[min1I] + count[min2I];
                parentNode[min1I] = _vocabulary.Count + a;
                parentNode[min2I] = _vocabulary.Count + a;
                binary[min2I] = 1;
            }

            for (var a = 0; a < _vocabulary.Count; a++)
            {
                var b = a;
                var i = 0;
                while (true)
                {
                    code[i] = binary[b];
                    point[i] = b;
                    i++;
                    b = parentNode[b];
                    if (b == _vocabulary.Count * 2 - 2)
                        break;
                }

                _vocabulary[a].SetCodeLength(i);
                _vocabulary[a].SetPoint(0, _vocabulary.Count - 2);
                for (b = 0; b < i; b++)
                {
                    _vocabulary[a].SetCode(i - b - 1, code[b]);
                    _vocabulary[a].SetPoint(i - b, point[b] - _vocabulary.Count);
                }
            }
        }

        /**
         * <summary>Constructs the unigram table based on the number of occurrences of the words.</summary>
         */
        private void CreateUniGramTable()
        {
            double total = 0;
            _table = new int[2 * _vocabulary.Count];
            foreach (var vocabularyWord in _vocabulary) {
                total += System.Math.Pow(vocabularyWord.GetCount(), 0.75);
            }
            var i = 0;
            var d1 = System.Math.Pow(_vocabulary[i].GetCount(), 0.75) / total;
            for (int a = 0; a < 2 * _vocabulary.Count; a++)
            {
                _table[a] = i;
                if (a / (2 * _vocabulary.Count + 0.0) > d1)
                {
                    i++;
                    d1 += System.Math.Pow(_vocabulary[i].GetCount(), 0.75) / total;
                }

                if (i >= _vocabulary.Count)
                    i = _vocabulary.Count - 1;
            }
        }

        /**
         * <summary>Accessor for the unigram table.</summary>
         * <param name="index">Index of the word.</param>
         * <returns>Unigram table value at a given index.</returns>
         */
        public int GetTableValue(int index)
        {
            return _table[index];
        }

        /**
         * <summary>Returns size of the unigram table.</summary>
         * <returns>Size of the unigram table.</returns>
         */
        public int GetTableSize()
        {
            return _table.Length;
        }
    }
}