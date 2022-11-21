using NUnit.Framework;
using WordToVec;

namespace Test
{
    public class SemanticDataSetTest
    {
        [Test]
        public void TestSpearman() {
            SemanticDataSet semanticDataSet = new SemanticDataSet("../../../AnlamverRel.txt");
            Assert.AreEqual(1.0, semanticDataSet.SpearmanCorrelation(semanticDataSet), 0.0);
            semanticDataSet = new SemanticDataSet("../../../MC.txt");
            Assert.AreEqual(1.0, semanticDataSet.SpearmanCorrelation(semanticDataSet), 0.0);
            semanticDataSet = new SemanticDataSet("../../../MEN.txt");
            Assert.AreEqual(1.0, semanticDataSet.SpearmanCorrelation(semanticDataSet), 0.0);
            semanticDataSet = new SemanticDataSet("../../../MTurk771.txt");
            Assert.AreEqual(1.0, semanticDataSet.SpearmanCorrelation(semanticDataSet), 0.0);
            semanticDataSet = new SemanticDataSet("../../../RareWords.txt");
            Assert.AreEqual(1.0, semanticDataSet.SpearmanCorrelation(semanticDataSet), 0.0);
            semanticDataSet = new SemanticDataSet("../../../RG.txt");
            Assert.AreEqual(1.0, semanticDataSet.SpearmanCorrelation(semanticDataSet), 0.0);
            semanticDataSet = new SemanticDataSet("../../../WS353.txt");
            Assert.AreEqual(1.0, semanticDataSet.SpearmanCorrelation(semanticDataSet), 0.00001);
        }

    }
}