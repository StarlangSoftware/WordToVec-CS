using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;

namespace WordToVec
{
    public class SemanticDataSet
    {
        private List<WordPair> _pairs;

        /// <summary>
        /// Empty constructor for the semantic dataset.
        /// </summary>
        public SemanticDataSet()
        {
            _pairs = new List<WordPair>();
        }

        /// <summary>
        /// Constructor for the semantic dataset. Reads word pairs and their similarity scores from an input file.
        /// </summary>
        /// <param name="fileName">Input file that stores the word pair and similarity scores.</param>
        public SemanticDataSet(string fileName)
        {
            _pairs = new List<WordPair>();
            var streamReader = new StreamReader(new FileStream(fileName, FileMode.Open));
            var line = streamReader.ReadLine();
            while (line != null)
            {
                var items = line.Split(" ");
                _pairs.Add(new WordPair(items[0], items[1], double.Parse(items[2])));
                line = streamReader.ReadLine();
            }
        }
        
        /// <summary>
        /// Calculates the similarities between words in the dataset. The word vectors will be taken from the input
        /// vectorized dictionary.
        /// </summary>
        /// <param name="dictionary">Vectorized dictionary that stores the word vectors.</param>
        /// <returns>Word pairs and their calculated similarities stored as a semantic dataset.</returns>
        public SemanticDataSet CalculateSimilarities(VectorizedDictionary dictionary) {
            var result = new SemanticDataSet();
            double similarity;
            for (var i = 0; i < _pairs.Count; i++){
                var word1 = _pairs[i].GetWord1();
                var word2 = _pairs[i].GetWord2();
                var vectorizedWord1 = (VectorizedWord) dictionary.GetWord(word1);
                var vectorizedWord2 = (VectorizedWord) dictionary.GetWord(word2);
                if (vectorizedWord1 != null && vectorizedWord2 != null){
                    similarity = vectorizedWord1.GetVector().CosineSimilarity(vectorizedWord2.GetVector());
                    result._pairs.Add(new WordPair(word1, word2, similarity));
                } else {
                    _pairs.RemoveAt(i);
                    i--;
                }
            }
            return result;
        }
        
        /// <summary>
        /// Returns the size of the semantic dataset.
        /// </summary>
        /// <returns>The size of the semantic dataset.</returns>
        public int Size(){
            return _pairs.Count;
        }
        
        /// <summary>
        /// Sorts the word pairs in the dataset according to the WordPairComparator.
        /// </summary>
        private void Sort(){
            _pairs.Sort();
        }

        /// <summary>
        /// Finds and returns the index of a word pair in the pairs array list. If there is no such word pair, it
        /// returns -1.
        /// </summary>
        /// <param name="wordPair">Word pair to search in the semantic dataset.</param>
        /// <returns>Index of the given word pair in the pairs array list. If it does not exist, the method returns -1.</returns>
        public int Index(WordPair wordPair){
            for (var i = 0; i < _pairs.Count; i++){
                if (wordPair.Equals(_pairs[i])){
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Calculates the Spearman correlation coefficient with this dataset to the given semantic dataset.
        /// </summary>
        /// <param name="semanticDataSet">Given semantic dataset with which Spearman correlation coefficient is calculated.</param>
        /// <returns>Spearman correlation coefficient with the given semantic dataset.</returns>
        public double SpearmanCorrelation(SemanticDataSet semanticDataSet){
            double sum = 0;
            int rank1, rank2;
            Sort();
            semanticDataSet.Sort();
            for (var i = 0; i < _pairs.Count; i++){
                rank1 = i + 1;
                rank2 = semanticDataSet.Index(_pairs[i]) + 1;
                double di = rank1 - rank2;
                sum += 6 * di * di;
            }
            double n = _pairs.Count;
            var ratio = sum / (n * (n * n - 1));
            return 1 - ratio;
        }

    }
}