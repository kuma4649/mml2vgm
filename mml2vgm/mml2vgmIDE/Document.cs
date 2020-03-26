using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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
        public bool isNew = false;
        public bool edit = false;

        public EnmCompileStatus compileStatus = EnmCompileStatus.None;
        public EnmMmlFileFormat srcFileFormat = EnmMmlFileFormat.unknown;
        public EnmFileFormat dstFileFormat = EnmFileFormat.unknown;
        public object compiledData = null;

        public Document(Setting setting,bool isMUC)
        {
            gwiFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "(新規mmlファイル).gwi");
            errBox = null;
            wrnBox = null;
            InitFolderTree();
            editor = new FrmEditor(setting, isMUC);
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
            srcFileFormat = EnmMmlFileFormat.GWI;
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
            switch (Path.GetExtension(fullPath))
            {
                case ".muc":
                    srcFileFormat = EnmMmlFileFormat.MUC;
                    editor.azukiControl.Text = File.ReadAllText(fullPath,Encoding.GetEncoding(932));
                    break;
                case ".gwi":
                default:
                    srcFileFormat = EnmMmlFileFormat.GWI;
                    editor.azukiControl.Text = File.ReadAllText(fullPath);
                    break;
            }
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

        public bool InitOpen(string fullPath,string context)
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
            TreeNode tn = new TreeNode(Path.GetFileName(Path.GetDirectoryName(gwiFullPath)),1,1);
            gwiTree = tn;
            tn.Expand();
            TreeNode ts = new TreeNode();
            DirectoryInfo dm = new DirectoryInfo(Path.GetDirectoryName(gwiFullPath));

            try
            {
                foreach (DirectoryInfo ds in dm.GetDirectories())
                {
                    ts = new TreeNode(ds.Name,1,1);
                    ts.Nodes.Add("!dmy");
                    tn.Nodes.Add(ts);
                }
                foreach (FileInfo fi in dm.GetFiles())
                {
                    ts = new TreeNode(fi.Name,0,0);
                    tn.Nodes.Add(ts);
                }
            }
            catch { }

        }
    }

}
