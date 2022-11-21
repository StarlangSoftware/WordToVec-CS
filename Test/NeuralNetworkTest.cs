using System;
using Corpus;
using Dictionary.Dictionary;
using NUnit.Framework;
using WordToVec;

namespace Test
{
    public class NeuralNetworkTest
    {
        Corpus.Corpus turkish, english;
        SemanticDataSet mc, rg, ws, av, men, mturk, rare;

        [SetUp]
        public void Setup()
        {
            english = new Corpus.Corpus("../../../english-xs.txt");
            turkish = new Corpus.Corpus("../../../turkish-xs.txt");
            mc = new SemanticDataSet("../../../MC.txt");
            rg = new SemanticDataSet("../../../RG.txt");
            ws = new SemanticDataSet("../../../WS353.txt");
            men = new SemanticDataSet("../../../MEN.txt");
            mturk = new SemanticDataSet("../../../MTurk771.txt");
            rare = new SemanticDataSet("../../../RareWords.txt");
            av = new SemanticDataSet("../../../AnlamverRel.txt");
        }

        private VectorizedDictionary Train(AbstractCorpus corpus, bool cBow)
        {
            var parameter = new WordToVecParameter();
            parameter.SetCbow(cBow);
            var neuralNetwork = new NeuralNetwork(corpus, parameter);
            Console.WriteLine(neuralNetwork.VocabularySize());
            return neuralNetwork.Train();
        }

        [Test]
        public void TestTrainEnglishCBow()
        {
            var dictionary = Train(english, true);
            var mc2 = mc.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + mc.Size() + ") " + mc.SpearmanCorrelation(mc2));
            var rg2 = rg.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + rg.Size() + ") " + rg.SpearmanCorrelation(rg2));
            var ws2 = ws.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + ws.Size() + ") " + ws.SpearmanCorrelation(ws2));
            var men2 = men.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + men.Size() + ") " + men.SpearmanCorrelation(men2));
            var mturk2 = mturk.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + mturk.Size() + ") " + mturk.SpearmanCorrelation(mturk2));
            var rare2 = rare.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + rare.Size() + ") " + rare.SpearmanCorrelation(rare2));
        }
       
        [Test]
        public void TestWithWordVectors() {
            var dictionary = new VectorizedDictionary("../../../vectors-english-xs.txt", new EnglishWordComparator());
            Console.WriteLine(dictionary.Size());
            var mc2 = mc.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + mc.Size() + ") " + mc.SpearmanCorrelation(mc2));
            var rg2 = rg.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + rg.Size() + ") " + rg.SpearmanCorrelation(rg2));
            var ws2 = ws.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + ws.Size() + ") " + ws.SpearmanCorrelation(ws2));
            var men2 = men.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + men.Size() + ") " + men.SpearmanCorrelation(men2));
            var mturk2 = mturk.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + mturk.Size() + ") " + mturk.SpearmanCorrelation(mturk2));
            var rare2 = rare.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + rare.Size() + ") " + rare.SpearmanCorrelation(rare2));
        }

        [Test]
        public void TestTrainEnglishSkipGram() {
            var dictionary = Train(english, false);
            var mc2 = mc.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + mc.Size() + ") " + mc.SpearmanCorrelation(mc2));
            var rg2 = rg.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + rg.Size() + ") " + rg.SpearmanCorrelation(rg2));
            var ws2 = ws.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + ws.Size() + ") " + ws.SpearmanCorrelation(ws2));
            var men2 = men.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + men.Size() + ") " + men.SpearmanCorrelation(men2));
            var mturk2 = mturk.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + mturk.Size() + ") " + mturk.SpearmanCorrelation(mturk2));
            var rare2 = rare.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + rare.Size() + ") " + rare.SpearmanCorrelation(rare2));
        }
        
        [Test]
        public void TestTrainTurkishCBow() {
            var dictionary = Train(turkish, true);
            var av2 = av.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + av.Size() + ") " + av.SpearmanCorrelation(av2));
        }

        [Test]
        public void TestTrainTurkishSkipGram() {
            var dictionary = Train(turkish, false);
            var av2 = av.CalculateSimilarities(dictionary);
            Console.WriteLine("(" + av.Size() + ") " + av.SpearmanCorrelation(av2));
        }

    }
}