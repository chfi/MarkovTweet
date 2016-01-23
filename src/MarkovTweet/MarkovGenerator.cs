using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkovTweet
{
    class MarkovGenerator
    {
        // the size of the prefix
        private int order;
        // A prefix is a set of n words, where n = the order of the generator.
        // A suffix is a single word, but a prefix can have an arbitrary number of suffixes.
        private Dictionary<List<string>, List<string>> prefixes;
        private Random rng;

        public MarkovGenerator(int order)
        {
            this.order = order;
            this.prefixes = new Dictionary<List<string>, List<string>>(comparer: new PrefixComparer());
            this.rng = new Random();
        }

        private class PrefixComparer : IEqualityComparer<List<string>>
        {
            // Two prefixes are equal if and only they have the same elements in the same order
            public bool Equals(List<string> x, List<string> y)
            {
                if (x.Count != y.Count)
                    return false;
                for (int i = 0; i < x.Count; i++)
                {
                    if (x[i] != y[i])
                        return false;
                }
                return true;
            }

            public int GetHashCode(List<string> obj)
            {
                int res = 0;
                foreach (string word in obj)
                {
                    res += word.GetHashCode() % 11;
                }
                return res;
            }
        }

        private void PrintPrefix(List<string> prefix)
        {
            foreach (var w in prefix)
            {
                Console.Write(w + " ");
            }
            Console.WriteLine();
        }

        private void PrintPrefixSuffixes(List<string> prefix)
        {
            List<string> Suffixes = prefixes[prefix];
            Console.Write("Prefix ");
            foreach (var w in prefix)
            {
                Console.Write(w + " ");
            }
            Console.WriteLine("has " + Suffixes.Count + " suffixes:");
            foreach (var s in Suffixes)
            {
                Console.WriteLine(s);
            }
        }

        private void AddPrefixSuffixPair(List<string> prefix, string suffix)
        {
            if (prefix.Count != order)
            {
                throw new ArgumentException("Prefix is of wrong size: Markov chain is order " + order + ", prefix length is " + prefix.Count);
            }
            prefixes[prefix].Add(suffix);
        }

        private String GetSuffix(List<string> prefix)
        {
            if (prefix.Count != order)
            {
                throw new ArgumentException("Prefix is of wrong size: Markov chain is order " + order + ", prefix length is " + prefix.Count);
            }
            var curPrefix = prefixes[prefix];

            int index = rng.Next(curPrefix.Count);

            return prefixes[prefix][index];
        }

        public void ReadInput(string input)
        {
            List<string> words = input.Split(' ').ToList();
            // Insert artificial data to make the control flow easier;
            // now we don't have to treat the beginning and end different
            for (int i = 0; i < order; i++)
            {
                words.Insert(0, "");
            }
            words.Add("");

            // fill the current prefix so we have something to start with
            List<string> curPrefix = words.Take(order).ToList();

            // now we step through each word, adding it to the suffix-list of the current prefix,
            // then updating the prefix, "shifting" it along the input text
            int WordCount = 0;
            foreach (string word in words)
            {
                // If we've counted more words than 'order', we're not at the start of the input
                // If we then also read an empty string, we must be at the end of the input, so we break
                if (WordCount > order && word == "")
                {
                    break;
                }

                // Add the prefix if it does not exist in the dictionary
                if (!prefixes.ContainsKey(curPrefix))
                {
                    prefixes.Add(curPrefix, new List<string>());
                }
               
                // Add the suffix to the prefix
                prefixes[curPrefix].Add(word);

                // create the next prefix by first copying the current one then "shifting" it along the input
                var newPrefix = new List<string>();
                newPrefix.AddRange(curPrefix);
                newPrefix.Add(word);
                newPrefix.RemoveAt(0);
                curPrefix = newPrefix;
            }
        }

        public String GenerateOutput()
        {
            var OutputBuilder = new StringBuilder("");
            var CurPrefix = new List<string>();

            // fill the initial prefix so that it must start from the beginning of the input
            for (int i = 0; i < order; i++)
            {
                CurPrefix.Add("");
            }

            int WordCount = 0;
            while (true)
            {
                var Suffix = GetSuffix(CurPrefix);

                // if the suffix is empty we might be at the end of the text,
                // like above this is checked by seeing if we've dealt with enough words
                if (Suffix != "")
                {
                    OutputBuilder.Append(Suffix + " ");
                }
                else if (WordCount > order)
                {
                    break;
                }
                
                // shift the prefix along the text
                CurPrefix.Add(Suffix);
                CurPrefix.RemoveAt(0);

                WordCount++;
            }
            return OutputBuilder.ToString();
        }
    }
}
