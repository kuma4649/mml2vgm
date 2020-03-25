using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class CompileManager
    {
        public enum enmCompileCommand
        {
            compile, play
        }

        public class queItem
        {
            public enmCompileCommand cmd;
            public Document doc;
            public string srcText;
            public object param;
            private enmCompileCommand compile;

            public queItem(enmCompileCommand compile, Document doc, string srcText, object param)
            {
                this.compile = compile;
                this.doc = doc;
                this.srcText = srcText;
                this.param = param;
            }
        }

        private Queue<queItem> queCompile = new Queue<queItem>();
        private object lockobj = new object();
        private Thread _runner = null;
        public Thread runner
        {
            get
            {
                lock (lockobj) return _runner;
            }
            set
            {
                lock (lockobj) _runner = value;
            }
        }
        private Action<string> disp = null;

        public CompileManager(Action<string> disp)
        {
            this.disp = disp;
        }

        public void RequestCompile(Document doc, string srcText)
        {
            if (doc == null) return;
            if (doc.compileStatus == Document.EnmCompileStatus.Compiling) return;

            doc.compileStatus = Document.EnmCompileStatus.ReqCompile;
            queCompile.Enqueue(new queItem(enmCompileCommand.compile, doc, srcText, null));

            KickRunner();
        }

        private void KickRunner()
        {
            if (runner != null) return;

            runner = new Thread(run);
            runner.Start();
        }

        private void run()
        {
            while (queCompile.Count > 0)
            {
                queItem qi = queCompile.Dequeue();

                switch (qi.cmd)
                {
                    case enmCompileCommand.compile:
                        if (qi.doc == null) break;
                        if (qi.srcText == null)
                        {
                            qi.doc.compileStatus = Document.EnmCompileStatus.Success;//対象がない場合は無条件で成功
                            break;
                        }
                        Compile(qi);
                        break;
                    case enmCompileCommand.play:
                        if (qi.doc == null) break;
                        if (qi.doc.compileStatus != Document.EnmCompileStatus.Success) break;
                        Play(qi);
                        break;
                }
            }

            runner = null;
        }

        private void Play(queItem qi)
        {
        }

        private void Compile(queItem qi)
        {

            string ext = Path.GetExtension(qi.doc.gwiFullPath).ToLower();
            switch (ext)
            {
                case ".gwi":
                    Compile_GWI(qi);
                    break;
                case ".muc":
                    break;
            }

        }

        private void Compile_GWI(queItem qi)
        { 

            string stPath = System.Windows.Forms.Application.StartupPath;
            string[] activeMMLTextLines = qi.srcText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(qi.doc.gwiFullPath));
            string wrkPath = Path.GetDirectoryName(qi.doc.gwiFullPath);
            msgBox.clear();

            Mml2vgm mv = new Mml2vgm(activeMMLTextLines, tempPath, null, stPath, disp, wrkPath, false);
            int result = mv.Start();

            qi.doc.errBox = msgBox.getErr();
            qi.doc.wrnBox = msgBox.getWrn();

            disp("\r\n");
            disp(string.Format(msg.get("I0110"), msgBox.getErr().Length, msgBox.getWrn().Length));
            if (mv.desVGM.loopSamples != -1)
            {
                disp(string.Format(msg.get("I0111"), mv.desVGM.loopClock));
                if (mv.desVGM.info.format == enmFormat.VGM)
                    disp(string.Format(msg.get("I0112"), mv.desVGM.loopSamples, mv.desVGM.loopSamples / 44100L));
                else
                    disp(string.Format(msg.get("I0112"), mv.desVGM.loopSamples, mv.desVGM.loopSamples / (mv.desVGM.info.xgmSamplesPerSecond)));
            }
            disp(string.Format(msg.get("I0113"), mv.desVGM.lClock));
            if (mv.desVGM.info.format == enmFormat.VGM)
                disp(string.Format(msg.get("I0114"), mv.desVGM.dSample, mv.desVGM.dSample / 44100L));
            else
                disp(string.Format(msg.get("I0114"), mv.desVGM.dSample, mv.desVGM.dSample / (mv.desVGM.info.xgmSamplesPerSecond)));
            disp(msg.get("I0126"));

            if (result == 0)
            {
                if (mv.desVGM.info.format == enmFormat.VGM)
                {
                    uint EOFOffset = Common.getLE32(mv.desBuf, 0x04) + (uint)mv.desVGM.dummyCmdCounter;
                    Common.SetLE32(mv.desBuf, 0x04, EOFOffset);

                    uint GD3Offset = Common.getLE32(mv.desBuf, 0x14) + (uint)mv.desVGM.dummyCmdCounter;
                    Common.SetLE32(mv.desBuf, 0x14, GD3Offset);

                    uint LoopOffset = (uint)mv.desVGM.dummyCmdLoopOffset - 0x1c;
                    Common.SetLE32(mv.desBuf, 0x1c, LoopOffset);
                }

                qi.doc.dstFileFormat = mv.desVGM.info.format == enmFormat.VGM
                    ? EnmFileFormat.VGM
                    : (mv.desVGM.info.format == enmFormat.XGM
                    ? EnmFileFormat.XGM
                    : EnmFileFormat.ZGM);
                qi.doc.compiledData = mv.desBuf;
                qi.doc.compileStatus = Document.EnmCompileStatus.Success;
            }
            else
            {
                qi.doc.dstFileFormat = EnmFileFormat.unknown;
                qi.doc.compiledData = null;
                qi.doc.compileStatus = Document.EnmCompileStatus.Failed;
            }

        }
    }
}
