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
        public string gwiFullPath = "";
        public string[] errBox = null;
        public string[] wrnBox = null;
        public TreeNode gwiTree = null;
        public FrmEditor editor = null;
        public bool isNew = false;
        public bool edit = false;

        public Document()
        {
            gwiFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "(新規mmlファイル).gwi");
            errBox = null;
            wrnBox = null;
            InitFolderTree();
            editor = new FrmEditor();
            editor.Text = Path.GetFileName(gwiFullPath);
            editor.azukiControl1.Text = "";
            isNew = true;
            edit = false;
        }

        public bool InitOpen(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                return false;
            }

            gwiFullPath = fullPath;
            errBox = null;
            wrnBox = null;
            InitFolderTree();
            editor.Text = Path.GetFileName(fullPath);
            editor.azukiControl1.Text = File.ReadAllText(fullPath);
            isNew = false;
            edit = false;

            return true;
        }

        private void InitFolderTree()
        {
            TreeNode tn = new TreeNode(Path.GetFileName(Path.GetDirectoryName(gwiFullPath)));
            gwiTree = tn;
            TreeNode ts = new TreeNode();
            DirectoryInfo dm = new DirectoryInfo(Path.GetDirectoryName(gwiFullPath));

            try
            {
                foreach (DirectoryInfo ds in dm.GetDirectories())
                {
                    ts = new TreeNode(ds.Name);
                    tn.Nodes.Add(ts);
                }
                foreach (FileInfo fi in dm.GetFiles())
                {
                    ts = new TreeNode(fi.Name);
                    tn.Nodes.Add(ts);
                }
            }
            catch { }

        }
    }
}
