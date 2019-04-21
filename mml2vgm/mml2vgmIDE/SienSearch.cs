using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class SienSearch
    {
        private Sien sien;
        private List<SienItem> found = new List<SienItem>();
        private Tuple<string, Action<List<SienItem>>> req;
        private object lockObj = new object();
        private CancellationTokenSource tokenSource;
        private CancellationToken cancellationToken;
        private Task task = null;
        private string reqText;
        private Action<List<SienItem>> reqCallback;

        public SienSearch(string filename)
        {
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
        public void Request(string text, Action<List<SienItem>> callback)
        {
            lock (lockObj)
            {
                req = new Tuple<string, Action<List<SienItem>>>(text, callback);
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
                        break;

                }
            }
            reqCallback?.Invoke(found);
        }

    }
}
