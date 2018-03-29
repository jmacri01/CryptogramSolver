using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptogramSolver.Console
{
    class Program
    {
        private static NetSpell.SpellChecker.Dictionary.WordDictionary oDict;
        private static NetSpell.SpellChecker.Spelling oSpell;
        private static Dictionary<string, bool> validWordsDict;
        private static Dictionary<string, bool> callsDict;

        static void Main(string[] args)
        {
            validWordsDict = new Dictionary<string, bool>();
            callsDict = new Dictionary<string, bool>();

            oDict = new NetSpell.SpellChecker.Dictionary.WordDictionary();
            oDict.DictionaryFile = "en-US.dic";
            oDict.Initialize();
            oSpell = new NetSpell.SpellChecker.Spelling();
            oSpell.Dictionary = oDict;

            var s = "aqqz";
            var filledIndexes = "aaaa";
            var alphabet = "abcdefghijklmnopqrstuvwxyz";

            var codes = CrackCode(s, filledIndexes, alphabet).ToList();

            var str = codes.Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");

            System.Console.Out.WriteLine(str);
        }

        static List<string> CrackCode(string s, string filledIndexes, string alphabet)
        {
            if(callsDict.ContainsKey($"{s} {filledIndexes} {alphabet}"))
                return new List<string>();

            callsDict.Add($"{s} {filledIndexes} {alphabet}", true);

            var l = new List<string>();

            var filled = true;

            if (!IsSentence(s, filledIndexes))
                return l;

            for (var i = 0; i < s.Length; ++i)
            {
                var rC = s[i];

                if (rC == ' ')
                    filledIndexes = $"{filledIndexes.Substring(0, i)}x{filledIndexes.Substring(i + 1)}";

                if (filledIndexes[i] == 'x')
                    continue;

                filled = false;

                for (var j = 0; j < alphabet.Length; ++j)
                {
                    var newStr = s;
                    var c = alphabet[j];
                    var newFilledIndexes = filledIndexes;
                    for (var k = 0; k < newFilledIndexes.Length; ++k)
                    {
                        if (rC == s[k] && newFilledIndexes[k] != 'x')
                        {
                            newStr = $"{newStr.Substring(0, k)}{c}{newStr.Substring(k + 1)}";
                            newFilledIndexes = $"{newFilledIndexes.Substring(0, k)}x{newFilledIndexes.Substring(k + 1)}";
                        }
                    }

                    var newAlphabet = $"{alphabet.Substring(0, j)}{alphabet.Substring(j + 1)}";
                    l.AddRange(CrackCode(newStr, newFilledIndexes, newAlphabet));
                }
            }

            if(filled)
                l.Add(s);

            return l;
        }

        static bool IsSentence(string s, string filledIndexes)
        {
            var currWord = string.Empty;
            for (var i = 0; i < filledIndexes.Length; ++i)
            {
                if (s[i] != ' ')
                {
                    currWord += s[i];
                }

                if (filledIndexes[i] != 'x')
                {
                    for (; i < filledIndexes.Length && s[i] != ' '; ++i) ;
                    currWord = string.Empty;
                }
                else if (s[i] == ' ' || i == s.Length - 1)
                {
                    if (!IsWord(currWord))
                        return false;

                    currWord = string.Empty;
                }
            }

            return true;
        }

        static bool IsWord(string s)
        {
            if (validWordsDict.ContainsKey(s))
                return validWordsDict[s];

            validWordsDict.Add(s, oSpell.TestWord(s));

            return validWordsDict[s];
        }
    }
}
