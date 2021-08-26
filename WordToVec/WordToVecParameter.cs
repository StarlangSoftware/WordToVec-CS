namespace WordToVec
{
    public class WordToVecParameter
    {
        private int _layerSize = 100;
        private bool _cbow = true;
        private double _alpha = 0.025;
        private int _window = 5;
        private bool _hierarchicalSoftMax;
        private int _negativeSamplingSize = 5;
        private int _numberOfIterations = 3;
        private int _seed = 1;

        /**
         * <summary>Empty constructor for Word2Vec parameter</summary>
         */
        public WordToVecParameter()
        {
        }

        /**
         * <summary>Accessor for layerSize attribute.</summary>
         * <returns>Size of the word vectors.</returns>
         */
        public int GetLayerSize()
        {
            return _layerSize;
        }

        /**
         * <summary>Accessor for CBow attribute.</summary>
         * <returns>True is CBow will be applied, false otherwise.</returns>
         */
        public bool IsCbow()
        {
            return _cbow;
        }

        /**
         * <summary>Accessor for the alpha attribute.</summary>
         * <returns>Current learning rate alpha.</returns>
         */
        public double GetAlpha()
        {
            return _alpha;
        }

        /**
         * <summary>Accessor for the window size attribute.</summary>
         * <returns>Current window size.</returns>
         */
        public int GetWindow()
        {
            return _window;
        }

        /**
         * <summary>Accessor for the hierarchicalSoftMax attribute.</summary>
         * <returns>If hierarchical softmax will be applied, returns true; false otherwise.</returns>
         */
        public bool IsHierarchicalSoftMax()
        {
            return _hierarchicalSoftMax;
        }

        /**
         * <summary>Accessor for the negativeSamplingSize attribute.</summary>
         * <returns>Number of negative samples that will be withdrawn.</returns>
         */
        public int GetNegativeSamplingSize()
        {
            return _negativeSamplingSize;
        }

        /**
         * <summary>Accessor for the numberOfIterations attribute.</summary>
         * <returns>Number of epochs to train the network.</returns>
         */
        public int GetNumberOfIterations()
        {
            return _numberOfIterations;
        }

        /**
         * <summary>Accessor for the seed attribute.</summary>
         * <returns>Seed to train the network.</returns>
         */
        public int GetSeed()
        {
            return _seed;
        }

        /**
         * <summary>Mutator for the layerSize attribute.</summary>
         * <param name="layerSize">New size of the word vectors.</param>
         */
        public void SetLayerSize(int layerSize)
        {
            this._layerSize = layerSize;
        }

        /**
         * <summary>Mutator for cBow attribute</summary>
         * <param name="cbow">True if CBow applied; false if SkipGram applied.</param>
         */
        public void SetCbow(bool cbow)
        {
            this._cbow = cbow;
        }

        /**
         * <summary>Mutator for alpha attribute</summary>
         * <param name="alpha">New learning rate.</param>
         */
        public void SetAlpha(double alpha)
        {
            this._alpha = alpha;
        }

        /**
         * <summary>Mutator for the window size attribute.</summary>
         * <param name="window">New window size.</param>
         */
        public void SetWindow(int window)
        {
            this._window = window;
        }

        /**
         * <summary>Mutator for the hierarchicalSoftMax attribute.</summary>
         * <param name="hierarchicalSoftMax">True is hierarchical softMax applied; false otherwise.</param>
         */
        public void SetHierarchicalSoftMax(bool hierarchicalSoftMax)
        {
            this._hierarchicalSoftMax = hierarchicalSoftMax;
        }

        /**
         * <summary>Mutator for the negativeSamplingSize attribute.</summary>
         * <param name="negativeSamplingSize">New number of negative instances that will be withdrawn.</param>
         */
        public void SetNegativeSamplingSize(int negativeSamplingSize)
        {
            this._negativeSamplingSize = negativeSamplingSize;
        }

        /**
         * <summary>Mutator for the numberOfIterations attribute.</summary>
         * <param name="numberOfIterations">New number of iterations.</param>
         */
        public void SetNumberOfIterations(int numberOfIterations)
        {
            this._numberOfIterations = numberOfIterations;
        }
        
        /**
         * <summary>Mutator for the seed attribute.</summary>
         * <param name="seed">New seed.</param>
         */
        public void SetSeed(int seed)
        {
            this._seed = seed;
        }
    }
}