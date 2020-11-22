using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class SienSearch
    {
        private Sien sien;
        private List<SienItem> found = new List<SienItem>();
        private Tuple<string, Action<List<SienItem>>, int, string, object> req;
        private object lockObj = new object();
        private CancellationTokenSource tokenSource;
        private CancellationToken cancellationToken;
        private Task task = null;
        private string reqText;
        private int reqParentID = -1;
        private string reqMMLFileFormat = "";
        private object reqOption = null;
        private Action<List<SienItem>> reqCallback;

        public SienSearch(string filename)
        {
            filename = filename.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            sien = JsonConvert.DeserializeObject<Sien>(System.IO.File.ReadAllText(filename));
            tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;
            task = new Task(Main, cancellationToken);
            task.Start();
        }

        ~SienSearch()
        {
            try
            {
                tokenSource.Cancel();
            }
            catch { }
        }

        private void Main()
        {
            try
            {
                while (true)
                {
                    while (req == null) Thread.Sleep(100);
                    lock (lockObj)
                    {
                        reqText = req.Item1;
                        reqCallback = req.Item2;
                        reqParentID = req.Item3;
                        reqMMLFileFormat = req.Item4;
                        reqOption = req.Item5;
                        req = null;
                    }
                    Search();
                }
            }
            catch { }
        }

        /// <summary>
        /// 候補の検索をリクエストする
        /// </summary>
        /// <param name="text">検索文字</param>
        /// <param name="callback">検索後に実行したいメソッド(別スレッドで呼ばれるので注意)</param>
        public void Request(string text, Action<List<SienItem>> callback,int parentID,string MMLFormat,object option)
        {
            lock (lockObj)
            {
                req = new Tuple<string, Action<List<SienItem>>, int, string, object>(text, callback, parentID, MMLFormat, option);
            }
        }

        private void Search()
        {
            found.Clear();
            if (string.IsNullOrEmpty(reqText))
            {
                reqCallback?.Invoke(null);
                return;
            }
            foreach (SienItem si in sien.list)
            {
                if (si.parentID != reqParentID) continue;
                if (si.supportMMLFormat.IndexOf(reqMMLFileFormat) < 0) continue;

                switch (si.patternType)
                {
                    case "Z":
                        if (si.pattern.IndexOf(reqText) == 0)
                        {
                            found.Add(si);
                            si.foundCnt = reqText.Length;
                        }
                        break;

                    case "B":
                        for (int i = 0; i < reqText.Length; i++)
                        {
                            string src = reqText.Substring(reqText.Length - i - 1);
                            if (si.pattern.IndexOf(src) == 0)
                            {
                                found.Add(si);
                                si.foundCnt = i + 1;
                                break;
                            }
                        }
                        if (reqOption != null) si.description = reqOption.ToString();
                        break;

                }
            }
            if (found.Count < 1) reqParentID = -1;
            reqCallback?.Invoke(found);
        }

    }
}
