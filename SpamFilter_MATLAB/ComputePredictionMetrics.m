% Copyright (c) 2016, Rakesh Kumbhashi
% All rights reserved.
% 
% Redistribution and use in source and binary forms, with or without
% modification, are permitted provided that the following conditions are
% met:
% 
%     * Redistributions of source code must retain the above copyright
%       notice, this list of conditions and the following disclaimer.
%     * Redistributions in binary form must reproduce the above copyright
%       notice, this list of conditions and the following disclaimer in
%       the documentation and/or other materials provided with the distribution
% 
% THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
% AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
% IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
% ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
% LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
% CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
% SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
% INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
% CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
% ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
% POSSIBILITY OF SUCH DAMAGE.

% The evaluation functions computed by this function are are:
% precision
% Precision = true_positive / (true_positive + false_positive)
% recall
% Recall = true_positive / (true_positive + false_negative)
% fscore
% F-score = 2 * Precision * Recall / (Precision + Recall)

function [precision, recall, fScore] =  ComputePredictionMetrics(y, yPredicted)

y_pos = find(y==1);

true_pos = sum(yPredicted(y_pos) == 1);
totalPredictPos  = sum(yPredicted==1);

yPredictZero = find(yPredicted == 0);
false_neg = sum(y(yPredictZero)==1);

precision = true_pos / totalPredictPos;
recall = true_pos / (true_pos + false_neg);

fScore = 2 * precision * recall / (precision + recall);
end