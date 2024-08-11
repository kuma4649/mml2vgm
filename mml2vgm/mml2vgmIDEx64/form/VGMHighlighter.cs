using Sgry.Azuki.Highlighter;
using Sgry.Azuki;
using System.Text.RegularExpressions;
using System.Drawing;

namespace mml2vgmIDE
{
    public class VGMHighlighter : IHighlighter
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


        public VGMHighlighter()
        {
            recRegexPatterns.Add(new RegexPattern(new Regex("^[^'].*"), false, new CharClass[] { CharClass.DocComment }));
            recRegexPatterns.Add(new RegexPattern(new Regex("^'[A-Za-z0-9\\-\\,\\+]+_*[ |\\t]"), false, new CharClass[] { CharClass.Keyword }));
            recRegexPatterns.Add(new RegexPattern(new Regex("^'[A-Za-z0-9\\-\\,\\+]+~*[ |\\t]"), false, new CharClass[] { CharClass.Keyword }));
            recRegexPatterns.Add(new RegexPattern(new Regex("^'@[ |\\t]"), false, new CharClass[] { CharClass.Keyword2 }));
            recRegexPatterns.Add(new RegexPattern(new Regex("^'%\\S+[ |\\t]"), false, new CharClass[] { CharClass.Keyword }));
            recRegexPatterns.Add(new RegexPattern(new Regex(";.*"), false, new CharClass[] { CharClass.DocComment }));
            recRegexPatterns.Add(new RegexPattern(new Regex("^'\\{"), false, new CharClass[] { CharClass.Comment }));

            //keywordHighlighter.AddEnclosure("'{", "}", CharClass.Comment, true);
        }

        bool IHighlighter.CanUseHook
        {
            get { return false; }
        }

        HighlightHook IHighlighter.HookProc
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        void IHighlighter.Highlight(Sgry.Azuki.Document doc, ref int dirtyBegin, ref int dirtyEnd)
        {
            int lineHeadIndexBegin = doc.GetLineHeadIndexFromCharIndex(dirtyBegin);
            int lineIndexBegin = doc.GetLineIndexFromCharIndex(lineHeadIndexBegin);

            int lineHeadIndexEnd = doc.GetLineHeadIndexFromCharIndex(dirtyEnd);
            int lineIndexEnd = doc.GetLineIndexFromCharIndex(lineHeadIndexEnd);

            bool comment = false;
            char t;
            char tb;
            string lineContent;
            int begin;
            int end;
            bool found;
            int offset;
            int nextSeekIndex;
            int lin;

            //ブロックコメント中かどうか現在位置から遡って調べる
            for (lin = lineIndexBegin; lin >= 0; lin--)
            {
                lineContent = doc.GetLineContent(lin);
                begin = doc.GetLineHeadIndex(lin);
                end = begin + doc.GetLineLength(lin);
                found = false;

                if (string.IsNullOrEmpty(lineContent))
                    continue;

                if (lineContent.IndexOf("'{") == 0)
                {
                    comment = true;
                    found = true;
                }
                else
                {
                    for (int i = end - 1 - begin; i >= 0; i--)
                    {
                        t = lineContent[i];
                        if (t == '}')
                        {
                            comment = false;
                            found = true;
                            break;
                        }
                    }
                }
                if (found) break;
            }

            for (lin = lineIndexBegin; lin <= lineIndexEnd; lin++)
            {
                lineContent = doc.GetLineContent(lin);
                offset = 0;
                begin = doc.GetLineHeadIndex(lin);
                end = begin + doc.GetLineLength(lin);
                nextSeekIndex = 0;

                if (comment)
                {
                    for (int i = begin, j = 0; i < end; i++, j++)
                    {
                        doc.SetCharClass(i, CharClass.Comment);
                        t = lineContent[j];
                        if (t == '}')
                        {
                            comment = false;
                            break;
                        }
                    }
                    continue;
                }

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

                    if (pattern.klassList[0] == CharClass.Comment)
                    {
                        comment = true;
                        for (int i = patEnd; i < end; i++)
                        {
                            doc.SetCharClass(i, CharClass.Comment);
                        }
                        break;
                    }
                    if (pattern.klassList[0] == CharClass.Keyword2)
                    {
                        for (int i = patEnd, j = patEnd - begin; i < end; i++, j++)
                        {
                            t = lineContent[j];
                            if (t == ';')
                            {
                                for (; i < end; i++)
                                    doc.SetCharClass(i, CharClass.Comment);
                                break;
                            }
                            doc.SetCharClass(i, CharClass.Normal);
                        }
                    }
                    if (pattern.klassList[0] != CharClass.Keyword) continue;

                    //更にJコマンドがあればそこも
                    tb = '\0';
                    for (int i = patEnd, j = patEnd - begin; i < end; i++, j++)
                    {
                        t = lineContent[j];

                        if (t == ';')
                        {
                            for (; i < end; i++)
                                doc.SetCharClass(i, CharClass.Comment);
                            break;
                        }
                        if (t == 'J')
                            doc.SetCharClass(i, CharClass.Annotation);
                        else if (t == 'L' && (tb != 'T' && tb != 'S' && tb != 'M' && tb != 'O'))//TL,SL,ML,pOL
                            doc.SetCharClass(i, CharClass.Annotation);
                        else if (t == '!')
                            doc.SetCharClass(i, CharClass.Annotation);
                        else
                            doc.SetCharClass(i, CharClass.Normal);

                        tb = t;
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
