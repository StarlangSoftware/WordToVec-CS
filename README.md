# WordToVec-CS

Distributed representations (DR) of words (i.e., word embeddings) are used to capture semantic and syntactic regularities of the language by analyzing distributions of word relations within the textual data. Modeling methods generating DRs rely on the assumption that 'words that occur in similar contexts tend to have similar meanings' (distributional hypothesis) which stems from the nature of language itself. Due to their unsupervised nature, these modeling methods do not require any human judgement input to train, which allows researchers to train very large datasets in relatively low costs.

Traditional representations of words (i.e., one-hot vectors) are based on word-word (W x W) co-occurrence sparse matrices where W is the number of distinct words in the corpus. On the other hand, distributed word representations (DRs) (i.e., word embeddings) are word-context (W x C) dense matrices where C < W and C is the number of context dimensions which are determined by underlying model assumptions. Dense representations are arguably better at capturing generalized information and more resistant to overfitting due to context vectors representing shared properties of words. DRs are real valued vectors where each context can be considered as a continuous feature of a word. Due to their ability to represent abstract features of a word, DRs are considered as reusable across higher level tasks in ease, even if they are trained with totally different datasets.

Prediction based DR models gained much attention after Mikolov et al.’s neural network based SkipGram model in 2013. The secret behind the prediction based models is simple: never build a sparse matrix at all. Prediction based models construct dense matrix representations directly instead of reducing sparse ones to dense ones. These models are trained like any other supervised learning task by giving lots of positive and negative samples without adding any human supervision costs. Aim of these models is to maximize the probability of each context c with the same distributional assumptions on word-context co-occurrences, similar to count based models.

SkipGram is a prediction based distributional semantic model (DSM) consisting of a shallow neural network architecture inspired from neural language modeling (LM) intuitions. It is commonly known for its open-source implementation library word2vec. SkipGram acts like a log-linear classifier maximizing the prediction of the surrounding words of a word within a context (center window). Probabilistic word and sentence prediction by local neighbors of a word has been successfully applied on LM tasks under Markov assumption. SkipGram leverages the same idea by considering the words within the window as positive and negative instances and learning weights (for k contexts) which maximizes word predictions. In the training process, each word vector starts as a random vector, and then iteratively shifts to the neighboring vector.

------------------------------------------------

Detailed Description
============

Yapay sinir ağını initialize etmek için

	NeuralNetwork(Corpus corpus, WordToVecParameter parameter)

Sinir ağını eğitmek için ise

	VectorizedDictionary train()

## Cite
If you use this resource on your research, please cite the following paper: 

```
@article{ehsani18,
  title={Constructing a WordNet for {T}urkish Using Manual and Automatic Annotation},
  author={R. Ehsani and E. Solak and O.T. Yildiz},
  journal={ACM Transactions on Asian and Low-Resource Language Information Processing (TALLIP)},
  volume={17},
  number={3},
  pages={24},
  year={2018},
  publisher={ACM}
}

@inproceedings{bakay19b,
  title={Integrating {T}urkish {W}ord{N}et {K}e{N}et to {P}rinceton {W}ord{N}et: The Case of One-to-Many Correspondences},
  author={Ozge Bakay and Ozlem Ergelen and Olcay Taner Yildiz},
  booktitle={Innovations in Intelligent Systems and Applications},
  year={2019}
}

@inproceedings{bakay19a,
  title={Problems Caused by Semantic Drift in WordNet SynSet Construction},
  author={Ozge Bakay and Ozlem Ergelen and Olcay Taner Yildiz},
  booktitle={International Conference on Computer Science and Engineering},
  year={2019}
}

@inproceedings{ozcelik19,
  title={User Interface for {T}urkish Word Network {K}e{N}et},
  author={Riza Ozcelik and Selen Parlar and Ozge Bakay and Ozlem Ergelen and Olcay Taner Yildiz},
  booktitle={Signal Processing and Communication Applications Conference},
  year={2019}
}
