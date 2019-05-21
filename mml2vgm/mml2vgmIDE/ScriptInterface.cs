using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Reflection;
using System.Diagnostics;

namespace mml2vgmIDE
{
    public static class ScriptInterface
    {
        private static ScriptEngine engine = Python.CreateEngine();

        public static void Init()
        {
        }

        public static string GetScriptTitle(string path)
        {
            try
            {
                ScriptSource source = engine.CreateScriptSourceFromFile(path);
                CompiledCode code = source.Compile();
                ScriptScope scope = engine.CreateScope();
                ScriptRuntime runtime = engine.Runtime;
                Assembly assembly = typeof(Program).Assembly;
                runtime.LoadAssembly(Assembly.LoadFile(assembly.Location));
                source.Execute(scope);

                dynamic pyClass = scope.GetVariable("Mml2vgmScript");
                dynamic mml2vgmScript = pyClass();
                return mml2vgmScript.title();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return Path.GetFileName(path);
            }
        }

        public static void run(string path, Mml2vgmInfo info)
        {
            try
            {
                ScriptSource source = engine.CreateScriptSourceFromFile(path);
                CompiledCode code = source.Compile();
                ScriptScope scope = engine.CreateScope();
                ScriptRuntime runtime = engine.Runtime;
                Assembly assembly = typeof(Program).Assembly;
                runtime.LoadAssembly(Assembly.LoadFile(assembly.Location));
                source.Execute(scope);

                dynamic pyClass = scope.GetVariable("Mml2vgmScript");
                dynamic mml2vgmScript = pyClass();
                ScriptInfo si = mml2vgmScript.run(info);
                if (si != null && info.document != null)
                {
                    info.document.editor.azukiControl.Document.Replace(si.responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private static void Reflect(ScriptInfo si)
        {
            if (si == null) return;
        }
    }

    public class ScriptInfo
    {
        /// <summary>
        /// スクリプトからエディタのキャレットの位置に挿入したい文字列を指定する
        /// </summary>
        public string responseMessage = "";
    }

    public class Mml2vgmInfo
    {
        /// <summary>
        /// アプリケーションフォルダー
        /// </summary>
        /// <remarks>アプリケーションのファイルが存在するフルパス</remarks>
        public string getApplicationFolder()
        {
            return Common.GetApplicationFolder();
        }

        /// <summary>
        /// アプリケーションデータフォルダー
        /// </summary>
        /// <remarks>アプリケーションの設定値保存向けフルパス</remarks>
        public string getApplicationDataFolder()
        {
            return Common.GetApplicationDataFolder();
        }

        /// <summary>
        /// アプリケーションテンポラリパス
        /// </summary>
        /// <remarks>一時的なワーク用のパス(毎回起動時に中身が削除されます)</remarks>
        public string getApplicationTempFolder()
        {
            return Path.Combine(Common.GetApplicationDataFolder(true), "temp");
        }

        /// <summary>
        /// 未使用
        /// </summary>
        public string name = "";

        /// <summary>
        /// スクリプト設定のルート
        /// </summary>
        private const string settingXmlName = "Setting";

        /// <summary>
        /// デフォルトファイル名(スクリプト間で共有する設定値)
        /// </summary>
        public string defaultXmlFilename = "scriptSetting.xml";

        /// <summary>
        /// 設定値(Dictionary)
        /// </summary>
        public Dictionary<string, string> settingData = new Dictionary<string, string>();

        /// <summary>
        /// アクティブなドキュメントのインスタンス
        /// </summary>
        public Document document = null;

        /// <summary>
        /// メインウィンドウのインスタンス
        /// </summary>
        public FrmMain parent = null;

        /// <summary>
        /// メッセージダイアログを表示する
        /// </summary>
        /// <param name="msg"></param>
        public void msg(string msg)
        {
            System.Windows.Forms.MessageBox.Show(msg);
        }

        /// <summary>
        /// ログウィンドウにメッセージを表示する
        /// </summary>
        /// <param name="msg"></param>
        public void msgLogWindow(string msg)
        {
            parent.MsgDisp(msg);
        }

        /// <summary>
        /// ログウィンドウにメッセージを表示する
        /// </summary>
        /// <param name="msg"></param>
        public void clearLogWindow()
        {
            parent.MsgClear();
        }

        /// <summary>
        /// デバッグウィンドウのログにメッセージを表示する。
        /// ログ(否ログウィンドウ)にも記録される
        /// </summary>
        /// <param name="msg"></param>
        public void msgDebugWindow(string msg)
        {
            log.Write(msg);
        }

        public byte[] ReadFileAllBytes(string fullPath)
        {
            return File.ReadAllBytes(fullPath);
        }

        public Mml2vgmInfo()
        {
        }

        public bool confirm(string message, string caption = "") {
            var result = System.Windows.Forms.MessageBox.Show(message, caption, System.Windows.Forms.MessageBoxButtons.YesNo);
            return result == System.Windows.Forms.DialogResult.Yes;
        }

        public string getCurrentFilepath() {
            return document.gwiFullPath;
        }

        public void runCommand(string cmdname, string arguments, bool waitEnd = false) {

            //if (waitEnd)
            //{
            //    using (process = new System.Diagnostics.Process())
            //    {
            //        process.StartInfo.FileName = cmdname;
            //        process.StartInfo.Arguments = arguments;
            //        process.StartInfo.UseShellExecute = false;
            //        process.StartInfo.RedirectStandardOutput = true;
            //        process.StartInfo.CreateNoWindow = true;
            //        process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_DataReceived);
            //        process.Exited += Process_Exited;
            //        process.EnableRaisingEvents = true;
            //        isProcessRunning = true;
            //        process.Start();
            //        process.BeginOutputReadLine();

            //        while (isProcessRunning)
            //        {
            //            System.Threading.Thread.Sleep(0);
            //            System.Windows.Forms.Application.DoEvents();
            //        }
            //        process.Close();
            //    }
            //}
            //else
            //{
            //    process = new System.Diagnostics.Process();
            //    process.StartInfo.FileName = cmdname;
            //    process.StartInfo.Arguments = arguments;
            //    //process.StartInfo.UseShellExecute = false;
            //    //process.StartInfo.RedirectStandardOutput = true;
            //    //process.StartInfo.CreateNoWindow = true;
            //    //process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_DataReceived);
            //    process.Start();
            //    //process.BeginOutputReadLine();
            //}

            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = cmdname;
            psi.Arguments = arguments;

            using (var p = System.Diagnostics.Process.Start(psi))
            {
                if (waitEnd)
                {
                    p.WaitForExit();
                }
                p.Close();
            }
        }

        //private Process process = null;
        //private bool isProcessRunning = false;
        //private void Process_Exited(object sender, EventArgs e)
        //{
        //    isProcessRunning = false;
        //}

        //private void process_DataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    parent.ProcessMsgDisp(e.Data);
        //}

        public string fileSelect(string title) {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = title;

            return ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK ? ofd.FileName : null;
        }

        public void loadSetting(string xmlFilename = null) {
            string xmlPath = getXmlPath(xmlFilename);
            if(!File.Exists(xmlPath)) return;

            var xe = System.Xml.Linq.XElement.Load(xmlPath);
            settingData = xe.Elements().ToDictionary(x => x.Name.LocalName, x => (string)x);
        }

        private string getXmlPath(string xmlFilename) {
            if(xmlFilename == null) xmlFilename = defaultXmlFilename;
            var xmlPath = Path.Combine(getApplicationDataFolder(), xmlFilename);
            return xmlPath;
        }

        public void saveSetting(string xmlFilename = null) {
            var xe = new System.Xml.Linq.XElement(settingXmlName);
            foreach(var k in settingData.Keys) {
                xe.Add(new System.Xml.Linq.XElement(k, settingData[k]));
            }

            var xmlPath = getXmlPath(xmlFilename);
            xe.Save(xmlPath);
        }

        public string getSettingValue(string key) {
            if(!settingData.ContainsKey(key)) return null;
            return settingData[key];
        }

        public void setSettingValue(string key, string value) {
            settingData[key] = value;
        }

        public void removeSetting(string key) {
            if(!settingData.ContainsKey(key)) return;
            settingData.Remove(key);
        }

    }
}
