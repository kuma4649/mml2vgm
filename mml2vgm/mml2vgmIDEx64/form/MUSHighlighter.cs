using Sgry.Azuki;
using Sgry.Azuki.Highlighter;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static IronPython.Modules.PythonRegex;

namespace mml2vgmIDEx64
{
    public class MUSHighlighter : IHighlighter
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
        RegexPattern partPtn = new RegexPattern(new Regex("[ \\t0-9\\,\\-]+[ \\t]*\\["), false, new CharClass[] { CharClass.Keyword });//パート
        RegexPattern macroPtn = new RegexPattern(new Regex("\\$.+[ \\t]*\\["), false, new CharClass[] { CharClass.Keyword });//マクロ

        public MUSHighlighter()
        {
            RegexPattern rp;
            //rp= new RegexPattern(new Regex("^[ \\t0-9,\\-]+[ \\t]*\\["),false, new CharClass[] { CharClass.Keyword });//パート
            //recRegexPatterns.Add(rp);
            //rp = new RegexPattern(new Regex("^#\\s*\\* *[0-9]+"), false, new CharClass[] { CharClass.Keyword });//マクロ
            //recRegexPatterns.Add(rp);

            //rp = new RegexPattern(new Regex("^[#|!][@_A-Za-z0-9]*[ |\\t]"), false, new CharClass[] { CharClass.Keyword });//TAG
            //regexPatterns.Add(rp);
            //rp = new RegexPattern(new Regex("\\;.*"), false, new CharClass[] { CharClass.Comment });//comment
            //regexPatterns.Add(rp);

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

            bool part = false;
            int matchIndex = -1;

            for (int lin = lineIndexBegin; lin <= lineIndexEnd; lin++)
            {
                string lineContent = doc.GetLineContent(lin);//1行取り出す
                int offset = 0;
                int begin = doc.GetLineHeadIndex(lin);
                int end = begin + doc.GetLineLength(lin);

                while (offset < lineContent.Length)
                {
                    //コメントチェック
                    matchIndex = lineContent.IndexOf(';', offset);
                    if (matchIndex == offset)
                    {
                        for (int i = begin + matchIndex; i < end; i++)
                            doc.SetCharClass(i, CharClass.Comment);
                        break;
                    }

                    //パート外の状態の場合のチェック
                    if (!part)
                    {
                        //パート定義或いはマクロ定義開始部分があるかチェック
                        System.Text.RegularExpressions.Match match = partPtn.regex.Match(lineContent, offset);
                        if (!match.Success) match = macroPtn.regex.Match(lineContent, offset);
                        if (match.Success)
                        {
                            part = true;
                            offset += match.Length;
                            int patBegin = begin + match.Index;
                            int patEnd = begin + match.Index + match.Length;
                            for (int i = patBegin; i < patEnd; i++)
                                doc.SetCharClass(i, partPtn.klassList[0]);
                            continue;
                        }
                    }

                    //パート内の状態の場合のチェック
                    if (part)
                    {
                        // "チェック
                        matchIndex = lineContent.IndexOf('"', offset);
                        if (matchIndex >= 0)
                        {
                            offset = matchIndex + 1;
                            int patBegin = begin + matchIndex;
                            int patEnd = end;
                            doc.SetCharClass(patBegin, CharClass.EmbededScript);
                            for (int i = patBegin + 1; i < patEnd; i++)
                            {
                                doc.SetCharClass(i, CharClass.EmbededScript);
                                if (offset < lineContent.Length && lineContent[offset] == '"')
                                {
                                    offset++;
                                    break;
                                }
                                offset++;
                            }
                            continue;
                        }

                        //パート定義終了部分があるかチェック
                        matchIndex = lineContent.IndexOf(']', offset);
                        if (matchIndex >= 0)
                        {
                            part = false;
                            offset = matchIndex + 1;
                            matchIndex = begin + matchIndex;
                            doc.SetCharClass(matchIndex, CharClass.Keyword);
                            continue;
                        }

                    }

                    //何も該当しない場合は、その行のチェックを終える
                    offset++;
                }

            }
        }
    }
}