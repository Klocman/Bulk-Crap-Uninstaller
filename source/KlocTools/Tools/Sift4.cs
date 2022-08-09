using System;
using System.Collections.Generic;
using System.Linq;

namespace Klocman.Tools
{
    /// <summary>
    /// Algorithm by Siderite
    /// https://siderite.blogspot.com/2014/11/super-fast-and-accurate-string-distance.html#at2217133354
    /// </summary>
    public class Sift4
    {
        private readonly Options _options;

        public Sift4(Options options)
        {
            if (options == null) options = new Options();
            if (options.Tokenizer == null)
            {
                options.Tokenizer = s => s == null
                    ? Array.Empty<string>()
                    : s.ToCharArray().Select(c => c.ToString()).ToArray();
            }
            if (options.TokenMatcher == null)
            {
                options.TokenMatcher = (t1, t2) => t1 == t2;
            }
            if (options.MatchingEvaluator == null)
            {
                options.MatchingEvaluator = (t1, t2) => 1;
            }
            if (options.LocalLengthEvaluator == null)
            {
                options.LocalLengthEvaluator = l => l;
            }
            if (options.TranspositionCostEvaluator == null)
            {
                options.TranspositionCostEvaluator = (c1, c2) => 1;
            }
            if (options.TranspositionsEvaluator == null)
            {
                options.TranspositionsEvaluator = (l, t) => l - t;
            }
            _options = options;
        }

        /// <summary>
        /// General distance algorithm uses all the parameters in the options object and works on tokens
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="maxOffset"></param>
        /// <returns></returns>
        public double GeneralDistance(string s1, string s2, int maxOffset)
        {
            var t1 = _options.Tokenizer(s1);
            var t2 = _options.Tokenizer(s2);

            var l1 = t1.Length;
            var l2 = t2.Length;

            if (l1 == 0) return _options.LocalLengthEvaluator(l2);
            if (l2 == 0) return _options.LocalLengthEvaluator(l1);

            var c1 = 0;  //cursor for string 1
            var c2 = 0;  //cursor for string 2
            var lcss = 0.0;  //largest common subsequence
            var localCs = 0.0; //local common substring
            var trans = 0.0;  //cost of transpositions ('axb' vs 'xba')
            var offsetArr = new LinkedList<OffsetPair>();  //offset pair array, for computing the transpositions

            while ((c1 < l1) && (c2 < l2))
            {
                if (_options.TokenMatcher(t1[c1], t2[c2]))
                {
                    localCs += _options.MatchingEvaluator(t1[c1], t2[c2]);
                    var isTransposition = false;
                    var op = offsetArr.First;
                    while (op != null)
                    {  //see if current match is a transposition
                        var ofs = op.Value;
                        if (c1 <= ofs.C1 || c2 <= ofs.C2)
                        {
                            // when two matches cross, the one considered a transposition is the one with the largest difference in offsets
                            isTransposition = Math.Abs(c2 - c1) >= Math.Abs(ofs.C2 - ofs.C1);
                            if (isTransposition)
                            {
                                trans += _options.TranspositionCostEvaluator(c1, c2);
                            }
                            else
                            {
                                if (!ofs.IsTransposition)
                                {
                                    ofs.IsTransposition = true;
                                    trans += _options.TranspositionCostEvaluator(ofs.C1, ofs.C2);
                                }
                            }
                            break;
                        }
                        var nextOp = op.Next;
                        if (c1 > ofs.C2 && c2 > ofs.C1)
                        {
                            offsetArr.Remove(op);
                        }
                        op = nextOp;
                    }
                    offsetArr.AddLast(new OffsetPair(c1, c2)
                    {
                        IsTransposition = isTransposition
                    });
                }
                else
                {
                    lcss += _options.LocalLengthEvaluator(localCs);
                    localCs = 0;
                    if (c1 != c2)
                    {
                        c1 = c2 = Math.Min(c1, c2);  //using min allows the computation of transpositions
                    }
                    //if matching tokens are found, remove 1 from both cursors (they get incremented at the end of the loop)
                    //so that we can have only one code block handling matches 
                    for (var i = 0; i < maxOffset && (c1 + i < l1 || c2 + i < l2); i++)
                    {
                        if ((c1 + i < l1) && _options.TokenMatcher(t1[c1 + i], t2[c2]))
                        {
                            c1 += i - 1;
                            c2--;
                            break;
                        }
                        if ((c2 + i < l2) && _options.TokenMatcher(t1[c1], t2[c2 + i]))
                        {
                            c1--;
                            c2 += i - 1;
                            break;
                        }
                    }
                }
                c1++;
                c2++;
                if (_options.MaxDistance != null)
                {
                    var temporaryDistance = _options.LocalLengthEvaluator(Math.Max(c1, c2)) - _options.TranspositionsEvaluator(lcss, trans);
                    if (temporaryDistance >= _options.MaxDistance) return Math.Round(temporaryDistance, MidpointRounding.AwayFromZero);
                }
                // this covers the case where the last match is on the last token in list, so that it can compute transpositions correctly
                if ((c1 >= l1) || (c2 >= l2))
                {
                    lcss += _options.LocalLengthEvaluator(localCs);
                    localCs = 0;
                    c1 = c2 = Math.Min(c1, c2);
                }
            }
            lcss += _options.LocalLengthEvaluator(localCs);
            return Math.Round(_options.LocalLengthEvaluator(Math.Max(l1, l2)) - _options.TranspositionsEvaluator(lcss, trans), MidpointRounding.AwayFromZero); //apply transposition cost to the final result
        }

        /// <summary>
        /// Static distance algorithm working on strings, computing transpositions as well as stopping when maxDistance was reached.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="maxOffset"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        public static double CommonDistance(string s1, string s2, int maxOffset, int maxDistance = 0)
        {
            var l1 = s1 == null ? 0 : s1.Length;
            var l2 = s2 == null ? 0 : s2.Length;

            if (l1 == 0) return l2;
            if (l2 == 0) return l1;

            var c1 = 0;  //cursor for string 1
            var c2 = 0;  //cursor for string 2
            var lcss = 0;  //largest common subsequence
            var localCs = 0; //local common substring
            var trans = 0;  //number of transpositions ('axb' vs 'xba')
            var offsetArr = new LinkedList<OffsetPair>();  //offset pair array, for computing the transpositions

            while ((c1 < l1) && (c2 < l2))
            {
                if (s1![c1] == s2![c2])
                {
                    localCs++;
                    var isTransposition = false;
                    var op = offsetArr.First;
                    while (op != null)
                    {  //see if current match is a transposition
                        var ofs = op.Value;
                        if (c1 <= ofs.C1 || c2 <= ofs.C2)
                        {
                            // when two matches cross, the one considered a transposition is the one with the largest difference in offsets
                            isTransposition = Math.Abs(c2 - c1) >= Math.Abs(ofs.C2 - ofs.C1);
                            if (isTransposition)
                            {
                                trans++;
                            }
                            else
                            {
                                if (!ofs.IsTransposition)
                                {
                                    ofs.IsTransposition = true;
                                    trans++;
                                }
                            }
                            break;
                        }
                        var nextOp = op.Next;
                        if (c1 > ofs.C2 && c2 > ofs.C1)
                        {
                            offsetArr.Remove(op);
                        }
                        op = nextOp;
                    }
                    offsetArr.AddLast(new OffsetPair(c1, c2)
                    {
                        IsTransposition = isTransposition
                    });
                }
                else
                {
                    lcss += localCs;
                    localCs = 0;
                    if (c1 != c2)
                    {
                        c1 = c2 = Math.Min(c1, c2);  //using min allows the computation of transpositions
                    }
                    //if matching tokens are found, remove 1 from both cursors (they get incremented at the end of the loop)
                    //so that we can have only one code block handling matches 
                    for (var i = 0; i < maxOffset && (c1 + i < l1 || c2 + i < l2); i++)
                    {
                        if ((c1 + i < l1) && s1[c1 + i] == s2[c2])
                        {
                            c1 += i - 1;
                            c2--;
                            break;
                        }
                        if ((c2 + i < l2) && s1[c1] == s2[c2 + i])
                        {
                            c1--;
                            c2 += i - 1;
                            break;
                        }
                    }
                }
                c1++;
                c2++;
                if (maxDistance > 0)
                {
                    var temporaryDistance = Math.Max(c1, c2) - (lcss - trans);
                    if (temporaryDistance >= maxDistance) return temporaryDistance;
                }
                // this covers the case where the last match is on the last token in list, so that it can compute transpositions correctly
                if ((c1 >= l1) || (c2 >= l2))
                {
                    lcss += localCs;
                    localCs = 0;
                    c1 = c2 = Math.Min(c1, c2);
                }
            }
            lcss += localCs;
            return Math.Max(l1, l2) - (lcss - trans); //apply transposition cost to the final result
        }

        /// <summary>
        /// Standard Sift algorithm, using strings and taking only maxOffset as a parameter
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="maxOffset"></param>
        /// <returns></returns>
        public static int SimplestDistance(string s1, string s2, int maxOffset)
        {
            var l1 = s1 == null ? 0 : s1.Length;
            var l2 = s2 == null ? 0 : s2.Length;

            if (l1 == 0) return l2;
            if (l2 == 0) return l1;

            var c1 = 0;  //cursor for string 1
            var c2 = 0;  //cursor for string 2
            var lcss = 0;  //largest common subsequence
            var localCs = 0; //local common substring

            while ((c1 < l1) && (c2 < l2))
            {
                if (s1![c1] == s2![c2])
                {
                    localCs++;
                }
                else
                {
                    lcss += localCs;
                    localCs = 0;
                    if (c1 != c2)
                    {
                        c1 = c2 = Math.Max(c1, c2);
                    }
                    //if matching tokens are found, remove 1 from both cursors (they get incremented at the end of the loop)
                    //so that we can have only one code block handling matches 
                    for (var i = 0; i < maxOffset && (c1 + i < l1 && c2 + i < l2); i++)
                    {
                        if ((c1 + i < l1) && s1[c1 + i] == s2[c2])
                        {
                            c1 += i - 1;
                            c2--;
                            break;
                        }
                        if ((c2 + i < l2) && s1[c1] == s2[c2 + i])
                        {
                            c1--;
                            c2 += i - 1;
                            break;
                        }
                    }
                }
                c1++;
                c2++;
            }
            lcss += localCs;
            return Math.Max(l1, l2) - lcss;
        }

        private class OffsetPair
        {
            public int C1 { get; set; }
            public int C2 { get; set; }
            public bool IsTransposition { get; set; }

            public OffsetPair(int c1, int c2)
            {
                C1 = c1;
                C2 = c2;
                IsTransposition = false;
            }
        }

        public class Options
        {
            /// <summary>
            /// If set, the algorithm will stop if the distance reaches this value
            /// </summary>
            public int? MaxDistance { get; set; }

            /// <summary>
            /// The function that turns strings into a list of tokens (also strings)
            /// </summary>
            public Func<string, string[]> Tokenizer { get; set; }

            /// <summary>
            /// The function that determines if two tokens are matching (similar to characters being the same in the simple algorithm)
            /// </summary>
            public Func<string, string, bool> TokenMatcher { get; set; }

            /// <summary>
            /// The function that determines the value of a match of two tokens (the equivalent of adding 1 to the lcss when two characters match)
            /// This assumes that the TokenMatcher function is a lot less expensive than this evaluator. If that is not the case, 
            /// you can optimize the speed of the algorithm by using only the matching evaluator and then calculating if two tokens match on the returned value.
            /// </summary>
            public Func<string, string, double> MatchingEvaluator { get; set; }

            /// <summary>
            /// Determines if the local value (computed on subsequent matched tokens) must be modified.
            /// In case one wants to reward longer matched substrings, for example, this is what you need to change.
            /// </summary>
            public Func<double, double> LocalLengthEvaluator { get; set; }

            /// <summary>
            /// The function determining the cost of an individual transposition, based on its counter positions.
            /// </summary>
            public Func<int, int, double> TranspositionCostEvaluator { get; set; }

            /// <summary>
            /// The function determining how the cost of transpositions affects the final result
            /// Change it if it doesn't suit you.
            /// </summary>
            public Func<double, double, double> TranspositionsEvaluator { get; set; }
        }
    }
}