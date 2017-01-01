using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FeatureVocab_CS_Helper
{
    public class EmailProcessor
    {
        public Dictionary<string, int> wordFreqCount = new Dictionary<string, int>();
        Porter2 porter = new Porter2();

        //Adds to wordFreqCount - heling build spam Vocab
        public void BuildSpamVocab(string fileName)
        {
            string fileText = File.ReadAllText(fileName);


            string emailContent = fileText.ToLower();

            // Strip all HTML
            // Looks for any expression that starts with < and ends with > and replace
            // and does not have any < or > in the tag it with a space
            emailContent = Regex.Replace(emailContent, "<[^<>]+>", " ");

            // Handle Numbers
            // Look for one or more characters between 0-9
            emailContent = Regex.Replace(emailContent, "[0-9]+", "number");

            // Handle URLS
            // Look for strings starting with http:// or https://
            emailContent = Regex.Replace(emailContent, @"(http|https)://[^\s]*", "httpaddr");

            // Handle Email Addresses
            // Look for strings with @ in the middle
            emailContent = Regex.Replace(emailContent, @"[^\s]+@[^\s]+", "emailaddr");

            // Handle $ sign
            emailContent = Regex.Replace(emailContent, "[$]+", "dollar");

            string[] tokens = emailContent.Split(new char[] { '\r', '\n', ' ', '@', '$', '/', '#', '.', ':', '&', '*', '+', '=', '[', ']', '?', '!', '(', ')', '{', '}', ',', '\'', '\"', '>', '<', ';', '%' });
            foreach (string token in tokens)
            {
                // Remove any non alphanumeric characters
                string word = Regex.Replace(token, "[^a-zA-Z0-9]", "");

                // Stem the word 
                // (the porterStemmer sometimes has issues, so we use a try catch block)
                string stemmedWord = porter.stem(word.Trim());

                // Skip the word if it is too short
                if (stemmedWord.Length < 1)
                    continue;


                if (wordFreqCount.ContainsKey(stemmedWord))
                    wordFreqCount[stemmedWord] = wordFreqCount[stemmedWord] + 1;
                else
                    wordFreqCount[stemmedWord] = 1;


            }
        }

        //Builds Feature vector
        public int[] GetFeatureVector(string fileName, List<string> wordList, bool spamClassified)
        {
            string fileText = File.ReadAllText(fileName);

            string emailContent = fileText.ToLower();

            // Strip all HTML
            // Looks for any expression that starts with < and ends with > and replace
            // and does not have any < or > in the tag it with a space
            emailContent = Regex.Replace(emailContent, "<[^<>]+>", " ");

            // Handle Numbers
            // Look for one or more characters between 0-9
            emailContent = Regex.Replace(emailContent, "[0-9]+", "number");

            // Handle URLS
            // Look for strings starting with http:// or https://
            emailContent = Regex.Replace(emailContent, @"(http|https)://[^\s]*", "httpaddr");

            // Handle Email Addresses
            // Look for strings with @ in the middle
            emailContent = Regex.Replace(emailContent, @"[^\s]+@[^\s]+", "emailaddr");

            // Handle $ sign
            emailContent = Regex.Replace(emailContent, "[$]+", "dollar");

            int index;

            int[] featureVector = new int[wordList.Count+1];
            featureVector[0] = (spamClassified ? 1 : 0); //add a 1 at begin - if its spam, else add a 0

            string[] tokens = emailContent.Split(new char[] { '\r', '\n', ' ', '@', '$', '/', '#', '.', ':', '&', '*', '+', '=', '[', ']', '?', '!', '(', ')', '{', '}', ',', '\'', '\"', '>', '<', ';', '%' });
            foreach (string token in tokens)
            {
                // Remove any non alphanumeric characters
                string word = Regex.Replace(token, "[^a-zA-Z0-9]", "");

                // Stem the word 
                // (the porterStemmer sometimes has issues, so we use a try catch block)
                string stemmedWord = porter.stem(word.Trim());

                // Skip the word if it is too short
                if (stemmedWord.Length < 1)
                    continue;

                index = wordList.FindIndex(k => k.Equals(stemmedWord));
                if (index != -1)
                {
                    featureVector[index+1] = 1;
                }
            }

            return featureVector;

        }
    }
}
