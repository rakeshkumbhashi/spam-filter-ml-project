
%% Machine Learning Online Class - Additional exercise - using LibSVM
%  Use the spam asssassin public data set from http://spamassassin.apache.org/publiccorpus/  as a training exercise to classify data set.
%
%  Instructions
%  ------------
%  Download spam assassin from Apache or from Github zip and extract
%  Run the Charp Helper project (in same repository) to build vocabulary of
%  frequent words in Spam mails
%  Extract features using this vocabulary from all emails
%  Compile LibSVM as per instructions at
%  https://www.csie.ntu.edu.tw/~cjlin/libsvm/ ( see specific instructions
%  for compiling on Windows as well as using OpenMP for parallelized execution)
% Run this code using MATLAB

%% Initialization
clear ; close all; clc

%% =================== Generating Vocab ===============================

% Go through all the spam emails and find hte most frequently used words
% Find the frequency

%% ==== Part 1: Run through all spam email and we generated the SpamVocab - 
%  top 4000 words using a CSharp Helper as it was much faster to enable parallel File Read and Vocab generation === 


%% ==================== Part 2: Feature Extraction ====================
%  Using CSharp helper we have generated Feature vector which is compiled 
%  by extracting features and saved as space delimited text file - 1 row
%  for each email


%% =========== Part 3: Train Linear SVM for Spam Classification ========
%  In this section, you will train a linear classifier to determine if an
%  email is Spam or Not-Spam. I am using OpenMP compiled version, so its parallelized and much faster

X_Y = load('DataX_Y.txt');

%Randomly swaps different rows - so we have a good distribution for Cross validation, and Training sets
X_Y= randswap(X_Y);
y = X_Y(:,1);
X = X_Y(:, 2:end);

% Use standard 60-20-20 split for training, CV and test sets
train_limit = floor(length(y) * 0.6);
cv_limit= floor(length(y) * 0.8);
ytrain = y(1:train_limit);
Xtrain = X(1:train_limit, :);

yvalid = y(train_limit+1:cv_limit);
Xvalid = X(train_limit+1:cv_limit, :);

ytest = y(cv_limit+1:end);
Xtest = X(cv_limit+1:end, :);

fprintf('\nTraining Linear SVM (Spam Classification)\n')
fprintf('(this may take 1 to 2 minutes) ...\n')

% now add libsvm path   
addpath('C:\Development\Coursera_Machine_Learning\OpenMP_libsvm-3.22\matlab');

 % create model - see the first arg is Label vector, second is matrix,
 % third is options
 
% %% Calling LibSVM method - optimize parameter c on validation set, use default gamma
% % LibsVM train by default uses Gaussian / RBF kernel
% % How to assess output to get back to parameters in ML class? %   w = model.SVs' * model.sv_coef; %   b = -model.rho;
% 
% %Used below code to arrive at optimum C and gamma -- loop through c and gamma values
% n = 3:12;
% gV = -15:-6;
% 
% highestAccuracy = 0;
% OptimumC = 0;
% OptimumGamma = 0;
% 
% for i = 1:numel(n)
%      c=2^n(i); 
%      
%      
%      for j=1:numel(gV)
%         gamma=2^gV(j);
%         % create model
%         model = svmtrain(ytrain, Xtrain,['-c ' num2str(c) ' -g ' num2str(gamma) ]); 
% 
%         %predict model on cross validation set to capture accuracy and
%         %arrive at right C and Gamma/Sigma
%         [lbl, acc, dec] = svmpredict(yvalid, Xvalid, model);
%         
%         if(acc(1) > highestAccuracy )
%            highestAccuracy = acc(1);
%            OptimumC = c;
%            OptimumGamma = gamma; 
%          end
%         
%          fprintf('C = %f, sigma = %f', c, gamma);
%         
%     end
%     
% end
% 
% fprintf('\Highest Accuracy  -- C = %f, sigma/gamma = %f, highest Accuracy = %f', OptimumC, OptimumGamma, highestAccuracy);

% train model wiht computed optimum C and Gamma
%optimimC = 8, OptimumGamma = 8
OptimumC = 8;
OptimumGamma =  0.0020;

fprintf('\nRunning SVM train on training set ...\n')

%Train model - Uses the libsvm OpenMP library from the path indicated above
model = svmtrain(ytrain, Xtrain,['-c ' num2str(OptimumC) ' -g ' num2str(OptimumGamma)]); 

%predict model on test Data set
fprintf('\nVerifying prediction and computing metrics on CV set ...\n')

[lbl, acc, ~] = svmpredict(ytrain, Xtrain, model);
[cvPrecision, cvRecall, cvFScore] = ComputePredictionMetrics( ytrain, lbl);

fprintf('Cross Validation Accuracy: %f\n', acc(1));
fprintf('Cross Validation Precision: %f , Recall: %f, FScore: %f\n', acc(1), cvPrecision, cvRecall, cvFScore);


%% =================== Part 4: Test Data set================
%  After training the classifier, we can evaluate it on a test set that was
%  earlier created

fprintf('\nEvaluating the trained SVM on a test set ...\n')

%predict model on test Data set
[lbl, acc, ~] = svmpredict(ytest, Xtest, model);
[testPrecision, testRecall, testFScore] = ComputePredictionMetrics( ytest, lbl);

fprintf('Test Accuracy: %f\n', acc(1));
fprintf('Test Set Precision: %f , Recall: %f, FScore: %f\n', acc(1), testPrecision, testRecall, testFScore);
