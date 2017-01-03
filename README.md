# spam-filter-ml-project

## Spam filter project (spam assassin apache) using SVM in MATLAB and C# ##

This is an attempt to use LibSVM  for spam mails predictions. We are using the publicly available Spam assassin data set at apache.

* C# Helper project is used for arriving at Spam vocab (top 4000 words) and using it to extract features.
  This is performed in highly parallelized code.


* MATLAB code will train the model using svmTrain and optimize to find optimum C and gamma, also code for computing metrics - precision, recall and FScore 
  * Uses the OpenMP compiled version of the open source libSVM library. You can refer to instructions on how to compile LibSVM with OpenMP enabled

