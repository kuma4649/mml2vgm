using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace mml2vgmIDE
{
    public class InstrumentAtValSound
    {
        private object sender = null;
        private string add = "";
        private string url = "";
        private Encoding encoding = Encoding.GetEncoding("Shift_JIS");
        private string xpath = "";
        private string nameNode = "";
        private string paramsNode = "";
        private Action<object, string[]> CompleteMethod = null;
        private FrmMain parent = null;
        public bool isFirst = false;

        public void Start(FrmMain parent, object sender, string add, string url, Encoding encoding, string xpath, string nameNode, string paramsNode, Action<object, string[]> CompleteMethod)
        {
            this.parent = parent;
            this.sender = sender;
            this.add = add;
            this.url = url;
            this.encoding = encoding;
            this.xpath = xpath;
            this.nameNode = nameNode;
            this.paramsNode = paramsNode;
            this.CompleteMethod = CompleteMethod;
            try
            {
                WebClient wc = new WebClient();
                wc.Encoding = encoding;
                wc.DownloadStringCompleted += CompleteDownloadProc;
                wc.DownloadStringAsync(new Uri(url));
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("音色の取得に失敗しました");
            }
        }

        public void CompleteDownloadProc(Object sender, DownloadStringCompletedEventArgs e)
        {
            if (parent.setting.OfflineMode)
            {
                return;
            }

            if (e.Error != null)
            {
                if (!isFirst) return;

                System.Windows.Forms.DialogResult res = System.Windows.Forms.MessageBox.Show(
                    @"
VAL-SOUND様からネットワーク経由での音色情報取得に失敗しました
Yes    : 再度接続に挑戦する
No     : オフラインモードに遷移する
Cancel : 永続的にオフラインモードにする
", "入力支援機能", System.Windows.Forms.MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Error);

                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                parent.setting.OfflineMode = true;
                if (res == System.Windows.Forms.DialogResult.Cancel)
                {
                    parent.setting.InfiniteOfflineMode = true;
                }
                return;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(e.Result);

            List<string> inst = new List<string>();
            string[] ret = null;
            try
            {
                foreach (var nd in doc.DocumentNode.SelectNodes(xpath))
                {
                    foreach (var cnd in nd.ChildNodes)
                    {
                        if (cnd == null || cnd.Name != nameNode) continue;

                        try
                        {
                            var a = cnd;
                            var b = cnd.NextSibling;
                            while (b != null && b.Name != paramsNode) b = b.NextSibling;
                            if (a != null && b != null)
                            {
                                string[] bs = b.InnerText.Replace("\n", ",").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                List<int> bi = new List<int>();
                                foreach (string bsi in bs)
                                {
                                    int bsii;
                                    if (!int.TryParse(bsi, out bsii)) continue;
                                    bi.Add(bsii);
                                }
                                string i = GetInstrumentString(a.InnerText.Replace("\n", "").Trim(), bi.ToArray());
                                inst.Add(i);
                            }
                        }
                        catch (Exception ex1)
                        {
                            log.ForcedWrite(ex1);
                        }
                    }

                }
                ret = inst.ToArray();
            }
            catch (Exception ex2)
            {
                log.ForcedWrite(ex2);
            }

            CompleteMethod?.Invoke(this.sender, ret);
        }

        private string GetInstrumentString(string name, int[] val)
        {

            string[] line = new string[5];

            for (int i = 0; i < 4; i++)
            {
                line[i] = "";
                for (int j = 0; j < 10; j++)
                {
                    line[i] += string.Format("{0:D3} ", val[i * 10 + j + 2]);
                }
            }
            line[4] = string.Format("{0:D3} {1:D3}", val[0], val[1]);

            return string.Format(
@"   {0}
'@ N 000
   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSGEG
'@ {1}000
'@ {2}000
'@ {3}000
'@ {4}000
   AL  FB
'@ {5}
", name + add, line[0], line[1], line[2], line[3], line[4]);

        }
    }
}
