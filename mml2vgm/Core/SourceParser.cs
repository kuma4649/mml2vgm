using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SourceParser
    {
        public List<Line> lnInformation = null;
        public List<Line> lnInstrument = null;
        public List<Line> lnAlies = null;
        public List<Line> lnPart = null;
        public List<string> lnChipPartName = null;

        public int Parse(List<Line> src)
        {
            log.Write("テキスト解析開始");

            bool multiLine = false;

            lnInformation = new List<Line>();
            lnInstrument = new List<Line>();
            lnAlies = new List<Line>();
            lnPart = new List<Line>();
            lnChipPartName = new List<string>();

            foreach (Line line in src)
            {
                string s = line.Txt;

                if (multiLine)
                {
                    if (s.Trim() == "") continue;
                    lnInformation.Add(line);
                    if (s.IndexOf("}") < 0) continue;
                    multiLine = false;
                    continue;
                }

                // 行頭が'以外は読み飛ばす
                if (s.TrimStart().IndexOf("'") != 0) continue;

                s = s.TrimStart().Substring(1).TrimStart();

                // 'のみの行も読み飛ばす
                if (s.Trim() == "") continue;

                if (s.IndexOf("{") == 0)
                {
                    multiLine = true;
                    lnInformation.Clear();
                    lnInformation.Add(line);

                    if (s.IndexOf("}") > -1) multiLine = false;
                    continue;
                }
                else if (s.IndexOf("@") == 0)
                {
                    lnInstrument.Add(line);
                    continue;
                }

                if (s.IndexOf("%") == 0)
                {
                    lnAlies.Add(line);
                    continue;
                }

                if (s.IndexOf("+") == 0)
                {
                    // Includeはとばす
                    continue;
                }
                else
                {
                    lnPart.Add(line);
                    continue;
                }

            }

            DivChipParts();

            log.Write("テキスト解析完了");
            return 0;

        }

        /// <summary>
        /// ざっくりと使用パート名を音源指定部分のみ列挙する
        /// </summary>
        private void DivChipParts()
        {

            foreach (Line ln in lnPart)
            {
                if (ln == null || string.IsNullOrEmpty(ln.Txt) || ln.Txt.Length < 1) continue;

                string parts = ln.Txt.Substring(1).Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();

                try
                {
                    // 複合パート名指定に使用できる文字
                    //   英字(大文字小文字) 小文字は大文字の直後のみ有効
                    //   数字
                    //   ','(コンマ) '-'(ハイフン)
                    // 空白やタブを含めることはできない。
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i] >= 'A' && parts[i] <= 'Z')
                        {
                            string a = parts[i].ToString();

                            //2文字目が小文字ならばそれもパート名に含める
                            if (i + 1 < parts.Length && parts[i + 1] >= 'a' && parts[i + 1] <= 'z')
                            {
                                a += parts[i + 1].ToString();
                                i++;
                            }

                            if (!lnChipPartName.Contains(a))
                            {
                                lnChipPartName.Add(a);
                            }

                        }
                        else if (parts[i] >= '0' && parts[i] <= '9')
                        {
                            continue;
                        }
                        else if (parts[i] == ',' || parts[i] == '-')
                        {
                            continue;
                        }
                        else break;
                    }
                }
                catch
                {
                    //パート解析に失敗 
                    msgBox.setErrMsg(string.Format(msg.get("E02010"), parts), ln.Lp);
                }
            }

        }


    }
}
