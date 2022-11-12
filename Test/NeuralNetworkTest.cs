using Corpus;
using Dictionary.Dictionary;
using NUnit.Framework;
using WordToVec;

namespace Test
{
    public class NeuralNetworkTest
    {
        CorpusStream turkish, english;

        [SetUp]
        public void Setup()
        {
            english = new CorpusStream("../../../english-similarity-dataset.txt");
            turkish = new CorpusStream("../../../turkish-similarity-dataset.txt");
        }

        private VectorizedDictionary Train(CorpusStream corpus, bool cBow)
        {
            var parameter = new WordToVecParameter();
            parameter.SetCbow(cBow);
            var neuralNetwork = new NeuralNetwork(corpus, parameter);
            return neuralNetwork.Train();
        }

        [Test]
        public void TestTrainEnglishCBow()
        {
            var dictionary = Train(english, true);
        }
        
        [Test]
        public void TestTrainEnglishSkipGram() {
            var dictionary = Train(english, false);
        }
        
        [Test]
        public void TestTrainTurkishCBow() {
            var dictionary = Train(turkish, true);
        }

        [Test]
        public void TestTrainTurkishSkipGram() {
            var dictionary = Train(turkish, false);
        }

    }
}