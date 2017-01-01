using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureVocab_CS_Helper
{
    class Program
    {
        static void Main(string[] args)
        {
            const int TopNSpamFreqWords = 4000;

            string rootDir = @"C:\Development\Coursera_Machine_Learning\machine-learning-ex6\SpamAssassin_Apache_Exercise\MailDump";
            string[] spamDirs = new string[] { "20021010_spam", "20030228_spam", "20030228_spam_2", "20050311_spam_2" };
            string[] nonSpamDirs = new string[] { "20021010_easy_ham", "20021010_hard_ham", "20030228_easy_ham", "20030228_easy_ham_2", "20030228_hard_ham" };

            List<string> allDirs = new List<string>();
            allDirs.Add(spamDirs[0]);
            allDirs.Add(nonSpamDirs[0]);
            allDirs.Add(spamDirs[1]);
            allDirs.Add(nonSpamDirs[1]);
            allDirs.Add(spamDirs[2]);
            allDirs.Add(nonSpamDirs[2]);
            allDirs.Add(spamDirs[3]);
            allDirs.Add(nonSpamDirs[3]);
            allDirs.Add(nonSpamDirs[4]);


            Stopwatch watch = new Stopwatch();
            watch.Start();

            EmailProcessor emailProc = new EmailProcessor();

            foreach (string spamDir in spamDirs)
            {
                string spamDirPath = Path.Combine(rootDir, spamDir);
                foreach (var file in Directory.GetFiles(spamDirPath))
                {
                    emailProc.BuildSpamVocab(file);
                }
            }

            watch.Stop();
            Console.WriteLine("Total count:" + emailProc.wordFreqCount.Count);
            Console.WriteLine("Time Taken:" + watch.ElapsedMilliseconds);

            // Take top 4000 features and Create Vocab file
            var topNWords = emailProc.wordFreqCount.OrderByDescending(pair => pair.Value).Take(TopNSpamFreqWords);

            //using (StreamWriter writer = new StreamWriter(@"..\SpamVocab.txt"))
            //{
            //    int counter = 0;
            //    foreach (var pair in topNWords)
            //    {
            //        //Writes as <IndexNum>TAB<Word><IndexNum>TAB<Word><IndexNum>TAB<Word><IndexNum>TAB<Word> ... 
            //        counter++;
            //        writer.Write(counter);
            //        writer.Write("\t");
            //        writer.Write(pair.Key);
            //    }
            //}

            List<string> effectiveWordList = new List<string>();

            // Take top 4000 features and Create Vocab file , remove top 20 as they are not essentially features IN THIS CASE
            effectiveWordList.AddRange(topNWords.ToDictionary(x => x.Key, x => x.Value).Keys);
            effectiveWordList.RemoveRange(0, 20);

            //Thread safe / parallel List of feature vectors
            ConcurrentBag<int[]> concurrentBag = new ConcurrentBag<int[]>();

            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = Environment.ProcessorCount;

            watch.Restart();
            //Now build the feature vector - first column would be 1 (Spam) / 0 (Non-spam) - rest all would be feature vector indices for the email
            Parallel.ForEach( allDirs, po, Dir =>
            {
                bool spamFlag = false;
                if(Dir.Contains("spam"))
                    spamFlag = true;
                string DirPath = Path.Combine(rootDir, Dir);
                foreach (var file in Directory.GetFiles(DirPath))
                {
                    int[] featureVec = emailProc.GetFeatureVector(file, effectiveWordList, spamFlag);
                    concurrentBag.Add(featureVec);
                }
            });

            watch.Stop();
            Console.WriteLine("Time Taken for feature vector gen:" + watch.ElapsedMilliseconds);

            using (StreamWriter writer = new StreamWriter(@"..\..\DataX_Y.txt"))
            {
                foreach (var vector in concurrentBag)
                {
                    writer.WriteLine(string.Join(" ", vector));
                }
            }


        }
    }
}
