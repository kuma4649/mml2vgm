using HtmlAgilityPack;
using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;

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
        private Action<object,string[]> CompleteMethod = null;

        public void Start(object sender,string add, string url, Encoding encoding, string xpath, string nameNode, string paramsNode, Action<object,string[]> CompleteMethod)
        {
            this.sender = sender;
            this.add = add;
            this.url = url;
            this.encoding = encoding;
            this.xpath = xpath;
            this.nameNode = nameNode;
            this.paramsNode = paramsNode;
            this.CompleteMethod = CompleteMethod;

            WebClient wc = new WebClient();
            wc.Encoding = encoding;
            wc.DownloadStringCompleted += CompleteDownloadProc;
            wc.DownloadStringAsync(new Uri(url));
        }

        public void CompleteDownloadProc(Object sender, DownloadStringCompletedEventArgs e)
        {
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
                                foreach (string bsi in bs) bi.Add(int.Parse(bsi));
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
