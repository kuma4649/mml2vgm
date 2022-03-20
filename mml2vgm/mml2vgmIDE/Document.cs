using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    public class Document
    {
        public enum EnmCompileStatus
        {
            None,
            NeedCompile,
            Compiling,
            Success,
            Failed,
            ReqCompile
        }

        public string gwiFullPath = "";
        public string parentFullPath = "";
        public msgInfo[] errBox = null;
        public msgInfo[] wrnBox = null;
        public TreeNode gwiTree = null;
        public FrmEditor editor = null;
        public Tuple<string,string>[] tag= null;

        public bool isNew = false;
        public bool edit = false;
        public bool isMp3 = false;

        public EnmCompileStatus compileStatus = EnmCompileStatus.None;
        public EnmMmlFileFormat srcFileFormat = EnmMmlFileFormat.unknown;
        public EnmFileFormat dstFileFormat = EnmFileFormat.unknown;
        public object compiledData = null;

        public Document(Setting setting, EnmMmlFileFormat fmt)//,FrmSien sien)
        {
            gwiFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "(新規mmlファイル).gwi");
            errBox = null;
            wrnBox = null;
            InitFolderTree();
            editor = new FrmEditor(setting, fmt);//, sien);
            editor.Text = Path.GetFileName(gwiFullPath + "*");
            string filename = Path.Combine(System.Windows.Forms.Application.StartupPath, "Template.gwi");
            editor.azukiControl.Text = "";
            try { editor.azukiControl.Text = File.ReadAllText(filename); } catch {; }
            editor.azukiControl.ClearHistory();
            editor.Tag = this;
            editor.azukiControl.Tag = this;
            isNew = true;
            edit = false;
            compileStatus = EnmCompileStatus.None;
            srcFileFormat = fmt;
            dstFileFormat = EnmFileFormat.unknown;
            compiledData = null;
        }

        public bool InitOpen(string fullPath)
        {
            fullPath = fullPath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

            if (!File.Exists(fullPath))
            {
                return false;
            }

            gwiFullPath = fullPath;
            errBox = null;
            wrnBox = null;
            InitFolderTree();
            editor.Text = Path.GetFileName(fullPath);
            string txt="";
            switch (Path.GetExtension(fullPath).ToLower())
            {
                case ".muc":
                    srcFileFormat = EnmMmlFileFormat.MUC;
                    txt = File.ReadAllText(fullPath, Encoding.GetEncoding(932));
                    break;
                case ".mml":
                    srcFileFormat = EnmMmlFileFormat.MML;
                    txt = File.ReadAllText(fullPath, Encoding.GetEncoding(932));
                    break;
                case ".mdl":
                    srcFileFormat = EnmMmlFileFormat.MDL;
                    txt = File.ReadAllText(fullPath, Encoding.GetEncoding(932));
                    break;
                case ".gwi":
                default:
                    srcFileFormat = EnmMmlFileFormat.GWI;
                    txt = File.ReadAllText(fullPath);
                    break;
            }

            edit = false;
            if (editor.setting != null && editor.setting.other.ChangeEnterCode)
            {
                int len = txt.Length;
                txt = ChangeEnterCode(txt);
                if (len != txt.Length)
                {
                    edit = true;
                    editor.Text = Path.GetFileName(gwiFullPath + "*");
                }
            }

            editor.azukiControl.Text = txt;
            editor.azukiControl.ClearHistory();
            editor.Tag = this;
            editor.azukiControl.Tag = this;
            isNew = false;

            compileStatus = EnmCompileStatus.NeedCompile;
            dstFileFormat = EnmFileFormat.unknown;
            compiledData = null;
            
            return true;
        }

        private string ChangeEnterCode(string txt)
        {
            string buf = "";
            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i] != '\r' && txt[i] != '\n')
                {
                    buf += txt[i];
                    continue;
                }

                if (txt[i] == '\r' && i + 1 < txt.Length && txt[i + 1] == '\n')
                {
                    if (i + 1 < txt.Length && txt[i + 1] == '\n')
                    {
                        buf += txt[i];
                        buf += txt[i + 1];
                        i++;
                        continue;
                    }

                }

                buf += "\r\n";
                continue;
            }

            return buf;
        }

        public bool InitOpen(string fullPath,string[] buf,EnmMmlFileFormat srcFileFormat)
        {
            fullPath = fullPath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

            if (buf==null || buf.Length<1)
            {
                return false;
            }

            gwiFullPath = fullPath;
            errBox = null;
            wrnBox = null;
            InitFolderTree();
            editor.Text = Path.GetFileName(fullPath);
            this.srcFileFormat = srcFileFormat;
            editor.azukiControl.Text = string.Join("\r\n", buf);
            editor.azukiControl.ClearHistory();
            editor.Tag = this;
            editor.azukiControl.Tag = this;
            isNew = false;
            edit = false;

            compileStatus = EnmCompileStatus.NeedCompile;
            dstFileFormat = EnmFileFormat.unknown;
            compiledData = null;
            return true;
        }

        public bool InitOpen(string fullPath, string context)
        {
            fullPath = fullPath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

            gwiFullPath = fullPath;
            errBox = null;
            wrnBox = null;
            InitFolderTree();
            editor.Text = Path.GetFileName(fullPath);
            editor.azukiControl.Text = context;
            editor.azukiControl.ClearHistory();
            editor.Tag = this;
            editor.azukiControl.Tag = this;
            isNew = true;
            edit = true;

            return true;
        }

        private void InitFolderTree()
        {
            string path1 = Path.GetDirectoryName(gwiFullPath);
            path1 = string.IsNullOrEmpty(path1) ? gwiFullPath : path1;
            TreeNode tn = new TreeNode(path1, 1, 1);
            gwiTree = tn;
            tn.Expand();
            TreeNode ts = new TreeNode();
            DirectoryInfo dm = new DirectoryInfo(path1);

            try
            {
                foreach (DirectoryInfo ds in dm.GetDirectories())
                {
                    ts = new TreeNode(ds.Name, 1, 1);
                    ts.Nodes.Add("!dmy");
                    tn.Nodes.Add(ts);
                }
                foreach (FileInfo fi in dm.GetFiles())
                {
                    ts = new TreeNode(fi.Name, 0, 0);
                    tn.Nodes.Add(ts);
                }
            }
            catch { }

        }
    }

}
