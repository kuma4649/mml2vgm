using Sgry.Azuki;
using Sgry.Azuki.Highlighter;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace mml2vgmIDE
{
    public class MUCHighlighter : IHighlighter
    {
        class RegexPattern
        {
            public Regex regex;
            public IList<CharClass> klassList;
            public bool groupMatch;
            public RegexPattern(Regex regex,
                                 bool groupMatch,
                                 IList<CharClass> klassList)
            {
                this.regex = regex;
                this.groupMatch = groupMatch;
                this.klassList = klassList;
            }
        }

        List<RegexPattern> recRegexPatterns = new List<RegexPattern>(8);
        List<RegexPattern> regexPatterns = new List<RegexPattern>(8);


        public MUCHighlighter()
        {
            RegexPattern rp = new RegexPattern(new Regex("^[A-Za-z0-9]+[ |\\t]"),false, new CharClass[] { CharClass.Keyword });//パート
            recRegexPatterns.Add(rp);
            rp = new RegexPattern(new Regex("^#\\s*\\*[0-9]+"), false, new CharClass[] { CharClass.Keyword });//マクロ
            recRegexPatterns.Add(rp);

            rp = new RegexPattern(new Regex("^[#|!][@_A-Za-z0-9]*[ |\\t]"), false, new CharClass[] { CharClass.Keyword });//TAG
            regexPatterns.Add(rp);
            rp = new RegexPattern(new Regex("\\;.*"), false, new CharClass[] { CharClass.Comment });//comment
            regexPatterns.Add(rp);

        }

        bool IHighlighter.CanUseHook
        {
            get { return false; }
        }

        HighlightHook IHighlighter.HookProc
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        void IHighlighter.Highlight(Sgry.Azuki.Document doc, ref int dirtyBegin, ref int dirtyEnd)
        {
            int lineHeadIndexBegin = doc.GetLineHeadIndexFromCharIndex(dirtyBegin);
            int lineIndexBegin = doc.GetLineIndexFromCharIndex(lineHeadIndexBegin);

            int lineHeadIndexEnd = doc.GetLineHeadIndexFromCharIndex(dirtyEnd);
            int lineIndexEnd = doc.GetLineIndexFromCharIndex(lineHeadIndexEnd);

            for (int lin = lineIndexBegin; lin <= lineIndexEnd; lin++)
            {
                string lineContent = doc.GetLineContent(lin);
                int offset = 0;
                int begin = doc.GetLineHeadIndex(lin);
                int end = begin + doc.GetLineLength(lin);
                int nextSeekIndex = 0;

                foreach (RegexPattern pattern in recRegexPatterns)
                {
                    Match match = pattern.regex.Match(lineContent, offset);
                    if (match.Success == false || match.Index != offset)
                    {
                        continue;
                    }

                    int patBegin = begin + match.Index;
                    int patEnd = begin + match.Index + match.Length;
                    if (patBegin < patEnd)
                    {
                        for (int i = patBegin; i < patEnd; i++)
                        {
                            doc.SetCharClass(i, pattern.klassList[0]);
                        }
                        nextSeekIndex = Math.Max(nextSeekIndex, lineContent.Length);
                    }

                    //更にJコマンドがあればそこも
                    bool comment = false;
                    for (int i = patEnd; i < end; i++)
                    {
                        if (comment)
                            doc.SetCharClass(i, CharClass.Comment);
                        else if (doc.Text[i] == 'J')
                            doc.SetCharClass(i, CharClass.Annotation);
                        else if ((doc.Text[i] == 'S' || doc.Text[i] == 'M') && i + 1 < end && doc.Text[i + 1] == 'L')//SL,MLの時はLに反応しないように飛ばす
                        {
                            doc.SetCharClass(i, CharClass.Normal);
                            i++;
                        }
                        else if (doc.Text[i] == 'L')
                            doc.SetCharClass(i, CharClass.Annotation);
                        else if (doc.Text[i] == '!')
                            doc.SetCharClass(i, CharClass.Annotation);
                        else if (doc.Text[i] == ';')
                        {
                            doc.SetCharClass(i, CharClass.Comment);
                            comment = true;
                        }
                        else
                            doc.SetCharClass(i, CharClass.Normal);
                    }
                }

                for (int c = nextSeekIndex; c < lineContent.Length; c++)
                {
                    int m = c;
                    foreach (RegexPattern pattern in regexPatterns)
                    {
                        Match match = pattern.regex.Match(lineContent, c);
                        if (match.Success == false || match.Index != c)
                            continue;

                        int patBegin = begin + match.Index;
                        int patEnd = begin + match.Index + match.Length;
                        if (patBegin < patEnd)
                        {
                            for (int i = patBegin; i < patEnd; i++)
                            {
                                doc.SetCharClass(i, pattern.klassList[0]);
                            }
                            nextSeekIndex = Math.Max(nextSeekIndex, patEnd);
                        }

                        c += match.Length - 1;
                    }

                    //何もマッチしなかったときはハイライトを元に戻す
                    if (m == c)
                    {
                        doc.SetCharClass(c + begin, CharClass.Normal);
                    }
                }


            }
        }
    }
}