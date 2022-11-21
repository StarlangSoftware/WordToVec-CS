using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;

namespace WordToVec
{
    public class SemanticDataSet
    {
        private List<WordPair> _pairs;

        public SemanticDataSet()
        {
            _pairs = new List<WordPair>();
        }

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
        
        public int Size(){
            return _pairs.Count;
        }
        
        private void Sort(){
            _pairs.Sort();
        }

        public int Index(WordPair wordPair){
            for (int i = 0; i < _pairs.Count; i++){
                if (wordPair.Equals(_pairs[i])){
                    return i;
                }
            }
            return -1;
        }

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