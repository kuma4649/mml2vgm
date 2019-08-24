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
        public Dictionary<enmChipType, Tuple<string, string, List<string>>> dicChipPartName = null;

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
            dicChipPartName = GetChipPartNames();

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


        private Dictionary<enmChipType, Tuple<string, string, List<string>>> GetChipPartNames()
        {
            Dictionary<enmChipType, Tuple<string, string, List<string>>> cpn = new Dictionary<enmChipType, Tuple<string, string, List<string>>>();
            ClsChip cp;

            //default
            cp = new Conductor(null, 0, null, null, false);
            cpn.Add(enmChipType.CONDUCTOR, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "Cn" })));
            cp = new YM2151(null, 0, null, null, false);
            cpn.Add(enmChipType.YM2151, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "X", "Xs" })));
            cp = new YM2203(null, 0, null, null, false);
            cpn.Add(enmChipType.YM2203, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "N", "Ns" })));
            cp = new YM2608(null, 0, null, null, false);
            cpn.Add(enmChipType.YM2608, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "P", "Ps" })));
            cp = new YM2610B(null, 0, null, null, false);
            cpn.Add(enmChipType.YM2610B, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "T", "Ts" })));
            cp = new YM2612(null, 0, null, null, false);
            cpn.Add(enmChipType.YM2612, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "F", "Fs" })));
            cp = new SN76489(null, 0, null, null, false);
            cpn.Add(enmChipType.SN76489, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "S", "Ss" })));
            cp = new RF5C164(null, 0, null, null, false);
            cpn.Add(enmChipType.RF5C164, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "R", "Rs" })));
            cp = new segaPcm(null, 0, null, null, false);
            cpn.Add(enmChipType.SEGAPCM, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "Z", "Zs" })));
            cp = new HuC6280(null, 0, null, null, false);
            cpn.Add(enmChipType.HuC6280, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "H", "Hs" })));
            cp = new YM2612X(null, 0, null, null, false);
            cpn.Add(enmChipType.YM2612X, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "E", "Es" })));
            cp = new YM2413(null, 0, null, null, false);
            cpn.Add(enmChipType.YM2413, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "L", "Ls" })));
            cp = new C140(null, 0, null, null, false);
            cpn.Add(enmChipType.C140, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "Y", "Ys" })));
            cp = new AY8910(null, 0, null, null, false);
            cpn.Add(enmChipType.AY8910, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "A", "As" })));
            cp = new K051649(null, 0, null, null, false);
            cpn.Add(enmChipType.K051649, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "K", "Ks" })));
            cp = new QSound(null, 0, null, null, false);
            cpn.Add(enmChipType.QSound, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "Q", "Qs" })));
            cp = new K053260(null, 0, null, null, false);
            cpn.Add(enmChipType.K053260, new Tuple<string, string, List<string>>(cp.Name, cp.ShortName, new List<string>(new string[] { "O", "Os" })));

            foreach (Line ln in lnInformation)
            {
                string[] nn = ln.Txt.Split('=');
                if (nn.Length != 2) continue;

                string wrd = nn[0].Trim().ToUpper();
                foreach (enmChipType val in Enum.GetValues(typeof(enmChipType)))
                {
                    if (!cpn.ContainsKey(val)) continue;

                    int n = -1;
                    if (wrd.IndexOf(Information.PARTNAME + cpn[val].Item1) == 0) n = 0;
                    if (wrd.IndexOf(Information.PARTNAME + cpn[val].Item2) == 0) n = 1;
                    if (n == -1) continue;

                    n = -1;
                    if (wrd == Information.PARTNAME + cpn[val].Item1) n = 0;
                    if (wrd == Information.PARTNAME + cpn[val].Item2) n = 0;
                    if (wrd == Information.PARTNAME + cpn[val].Item1 + "SECONDARY") n = 1;
                    if (wrd == Information.PARTNAME + cpn[val].Item2 + "SECONDARY") n = 1;
                    if (n == -1) continue;

                    List<string> npn = nn[1].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                    if (npn.Count < 1) continue;
                    if (n == 1 && npn.Count > 1) continue;//Secondaryの場合は複数一括定義を認めない
                    for (int i = 0; i < npn.Count; i++)
                    {
                        npn[i] = npn[i].Trim();
                        //書式のチェック
                        if (
                            (npn[i][0] < 'A' || npn[i][0] > 'Z')//１文字目は大文字である故t
                            || (npn[i].Length == 2 && (npn[i][1] < 'a' || npn[i][1] > 'z'))//２文字目は小文字であること
                            )
                        {
                            npn.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    if (npn.Count < 1) continue;

                    //既に定義済みか調べ、定義済みならそれを削除(空文字に)する
                    foreach (enmChipType e in cpn.Keys)
                    {
                        for (int i = 0; i < cpn[e].Item3.Count; i++)
                        {
                            if (string.IsNullOrEmpty(cpn[e].Item3[i])) continue;
                            for (int j = 0; j < npn.Count; j++)
                            {
                                if (npn[j] == cpn[e].Item3[i])
                                {
                                    cpn[e].Item3.RemoveAt(i);
                                    i--;
                                    break;
                                }
                            }
                        }
                    }

                    if (n == 1)
                    {
                        while (cpn[val].Item3.Count < 2)
                        {
                            cpn[val].Item3.Add("");
                        }
                        cpn[val].Item3[1] = npn[0];
                        continue;
                    }

                    if (npn.Count < 2)
                    {
                        if (cpn[val].Item3.Count < 1)
                        {
                            cpn[val].Item3.Add("");
                        }
                        cpn[val].Item3[0] = npn[0];
                        continue;
                    }

                    cpn[val].Item3.Clear();
                    foreach(string np in npn)
                    {
                        cpn[val].Item3.Add(np);
                    }
                }
            }

            return cpn;
        }
    }
}
