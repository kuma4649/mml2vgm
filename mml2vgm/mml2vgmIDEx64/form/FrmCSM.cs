using NAudio.Dsp;
using NAudio.Wave;
using Sgry.Azuki.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDEx64.form
{
    public partial class FrmCSM : Form
    {
        private Action<Form> removeForm;
        private FrmEditor targetDocument;
        private AzukiControl ac;
        public FrmCSM()
        {
            InitializeComponent();
        }

        public FrmCSM(Action<Form> removeForm, FrmEditor dc)
        {
            InitializeComponent();
            this.removeForm = removeForm;

            if (dc == null) return;
            if (!(dc is FrmEditor)) return;
            targetDocument = (FrmEditor)dc;
            ac = ((FrmEditor)dc).azukiControl;
        }

            private void btnRef_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "waveファイル(*.wav)|*.wav|すべてのファイル(*.*)|*.*";
            ofd.Title = "waveファイルを選択";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            tbWaveFileName.Text = ofd.FileName;
        }

        private void tbSampleRate_Leave(object sender, EventArgs e)
        {
            try
            {
                int num;
                if(int.TryParse(tbSampleRate.Text,out num))
                {
                    tbSampleRate.Text = Math.Min(Math.Max(num, 4000), 192000).ToString();
                    return;
                }

                tbSampleRate.Text = "44100";
            }
            catch
            {
                tbSampleRate.Text = "44100";
            }
        }

        private void tbTempo_Leave(object sender, EventArgs e)
        {
            try
            {
                float num;
                if (float.TryParse(tbTempo.Text, out num))
                {
                    tbTempo.Text = Math.Min(Math.Max(num, 10f), 500f).ToString();
                    return;
                }

                tbTempo.Text = "110";
            }
            catch
            {
                tbTempo.Text = "110";
            }

        }

        private void tbReso_Leave(object sender, EventArgs e)
        {
            try
            {
                int num;
                if (int.TryParse(tbReso.Text, out num))
                {
                    tbReso.Text = Math.Min(Math.Max(num, 4), 384).ToString();
                    return;
                }

                tbReso.Text = "192";
            }
            catch
            {
                tbReso.Text = "192";
            }
        }

        private void tbAnalyzeSize_Leave(object sender, EventArgs e)
        {
            try
            {
                int num;
                if (int.TryParse(tbAnalyzeSize.Text, out num))
                {
                    num = Math.Min(Math.Max(num, 8), 9192);
                    int a = 8;
                    int dis = int.MaxValue;
                    while (true)
                    {
                        a <<= 1;
                        if (Math.Abs(num - a) > dis)
                        {
                            num = a >> 1;
                            break;
                        }
                        dis = Math.Abs(num - a);
                    }
                    tbAnalyzeSize.Text = num.ToString();
                    return;
                }

                tbAnalyzeSize.Text = "2048";
            }
            catch
            {
                tbAnalyzeSize.Text = "2048";
            }
        }

        private void tbTLlevelMul_Leave(object sender, EventArgs e)
        {
            try
            {
                float num;
                if (float.TryParse(tbTLlevelMul.Text, out num))
                {
                    tbTLlevelMul.Text = Math.Min(Math.Max(num, 0.0001f), 10f).ToString();
                    return;
                }

                tbTLlevelMul.Text = "2.0";
            }
            catch
            {
                tbTLlevelMul.Text = "2.0";
            }

        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbWaveFileName.Text))
                btnRef_Click(null, null);
            if(!File.Exists(tbWaveFileName.Text))
                btnRef_Click(null, null);
            if (!File.Exists(tbWaveFileName.Text))
                return;

            WaveFileReader reader = null;
            try
            {
                reader = new WaveFileReader(tbWaveFileName.Text);
            }
            catch
            {
                MessageBox.Show("Not support,this file format.");
                return;
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            try
            {
                string result = Analysis2(targetDocument.document.srcFileFormat);
                ac.Document.Replace(result);
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("Fail Message:{0}", ex.Message));
            }
        }

        private void FrmCSM_FormClosed(object sender, FormClosedEventArgs e)
        {
            removeForm?.Invoke(this);
        }


        private string Analysis()
        {
            string waveFileName = tbWaveFileName.Text;
            int samplerate =int.Parse(tbSampleRate.Text);
            int analyzeSize = int.Parse(tbAnalyzeSize.Text);
            float tempo = float.Parse(tbTempo.Text);
            int reso = int.Parse(tbReso.Text);
            float tlMul = float.Parse(tbTLlevelMul.Text);
            int indLen = (int)(samplerate * 60.0 * 4.0 / (tempo * reso));

            Tool.CSM.FFT fft = new Tool.CSM.FFT(analyzeSize, samplerate);
            float[] dat;
            using (WaveFileReader reader = new WaveFileReader(waveFileName))
            {
                dat = fft.ReadWave(reader);
            }
            int index = 0;

            string result;
            result = string.Format("'N3 T{0}@111EXONEX1234l{1}y$28,$F2 ; 4opの音色をセットしてね\r\n", tempo, reso);

            while (index < dat.Length)
            {
                float[] res = fft.FFTProcess(dat, index);
                List<Tool.CSM.FFT.Result> lst = fft.GetPeekList(res, 4);

                fft.CalcFnumTl(lst, tlMul);

                string r = "'N3 "
                + string.Format(" y$AD,${0:X02} y$A9,${1:X02} ", (byte)(lst[0].fnum >> 8), (byte)lst[0].fnum)
                + string.Format(" y$AC,${0:X02} y$A8,${1:X02} ", (byte)(lst[2].fnum >> 8), (byte)lst[2].fnum)
                + string.Format(" y$AE,${0:X02} y$AA,${1:X02} ", (byte)(lst[1].fnum >> 8), (byte)lst[1].fnum)
                + string.Format(" y$A6,${0:X02} y$A2,${1:X02} ", (byte)(lst[3].fnum >> 8), (byte)lst[3].fnum)
                + string.Format(" y$42,${0:X02}", (byte)lst[0].tl)
                + string.Format(" y$4A,${0:X02}", (byte)lst[2].tl)
                + string.Format(" y$46,${0:X02}", (byte)lst[1].tl)
                + string.Format(" y$4E,${0:X02}", (byte)lst[3].tl)
                + " R \r\n";

                result += r;
                index += indLen;
            }

            result += "'N3 y$28,$02 EXOF\r\n";

            return result;
        }

        private string Analysis2(EnmMmlFileFormat fmt)
        {
            string waveFileName = tbWaveFileName.Text;
            int samplerate = int.Parse(tbSampleRate.Text);
            int analyzeSize = int.Parse(tbAnalyzeSize.Text);
            float tempo = float.Parse(tbTempo.Text);
            int reso = int.Parse(tbReso.Text);
            float tlMul = float.Parse(tbTLlevelMul.Text);
            int indLen = (int)(samplerate * 60.0 * 4.0 / (tempo * reso));

            Tool.CSM.FFT fft = new Tool.CSM.FFT(analyzeSize, samplerate);
            float[] dat;
            using (WaveFileReader reader = new WaveFileReader(waveFileName))
            {
                dat = fft.ReadWave(reader);
            }
            int index = 0;

            string result;
            if (fmt == EnmMmlFileFormat.MUC)
            {
                result = string.Format("C C128 T{0} @2v15 S0,0,0,0 l{1}y$28,$F2 ; 4opの音色をセットしてね\r\n", tempo, reso);

                while (index < dat.Length)
                {
                    List<Tool.CSM.FFT.Result> lst = new List<Tool.CSM.FFT.Result>();

                    Complex[] trg = fft.GetTargetDat(dat, index);

                    while (true)
                    {
                        Complex[] fres = fft.FFTProcess2(trg, 0, out float[] res);
                        int peek = fft.GetPeek(res, out Tool.CSM.FFT.Result val);
                        lst.Add(val);
                        if (lst.Count == 16) break;

                        fres = fft.MakePeek(fres, peek);//ピークのみのデータを作成
                        Complex[] rres = fft.RFFTProcess(fres);//逆フーリエ
                        fft.DecDat(trg, rres);//ターゲットのデータからrresを減算
                    }

                    lst = fft.VoiceFilter(lst);

                    fft.CalcFnumTl(lst, tlMul);
                    string r = "C "
                    + string.Format(" y$AD,${0:X02} y$A9,${1:X02} ", (byte)(lst[0].fnum >> 8), (byte)lst[0].fnum)
                    + string.Format(" y$AC,${0:X02} y$A8,${1:X02} ", (byte)(lst[2].fnum >> 8), (byte)lst[2].fnum)
                    + string.Format(" y$AE,${0:X02} y$AA,${1:X02} ", (byte)(lst[1].fnum >> 8), (byte)lst[1].fnum)
                    + string.Format(" y$A6,${0:X02} y$A2,${1:X02} ", (byte)(lst[3].fnum >> 8), (byte)lst[3].fnum)
                    + string.Format(" y$42,${0:X02}", (byte)lst[0].tl)
                    + string.Format(" y$4A,${0:X02}", (byte)lst[2].tl)
                    + string.Format(" y$46,${0:X02}", (byte)lst[1].tl)
                    + string.Format(" y$4E,${0:X02}", (byte)lst[3].tl)
                    + " r \r\n";

                    result += r;
                    index += indLen;
                }

                result += "C y$28,$02 \r\n";
            }
            else
            {
                result = string.Format("'N3 T{0}@111EXONEX1234l{1}y$28,$F2 ; 4opの音色をセットしてね\r\n", tempo, reso);

                while (index < dat.Length)
                {
                    List<Tool.CSM.FFT.Result> lst = new List<Tool.CSM.FFT.Result>();

                    Complex[] trg = fft.GetTargetDat(dat, index);

                    while (true)
                    {
                        Complex[] fres = fft.FFTProcess2(trg, 0, out float[] res);
                        int peek = fft.GetPeek(res, out Tool.CSM.FFT.Result val);
                        lst.Add(val);
                        if (lst.Count == 16) break;

                        fres = fft.MakePeek(fres, peek);//ピークのみのデータを作成
                        Complex[] rres = fft.RFFTProcess(fres);//逆フーリエ
                        fft.DecDat(trg, rres);//ターゲットのデータからrresを減算
                    }

                    //string f = "; ";
                    //foreach(Tool.CSM.FFT.Result val in lst)
                    //{
                    //    f += string.Format(" {0}Hz", val.freq);
                    //}
                    //f += "\r\n";
                    lst = fft.VoiceFilter(lst);
                    //f += "filter ";
                    //foreach (Tool.CSM.FFT.Result val in lst)
                    //{
                    //    f += string.Format(" {0}Hz", val.freq);
                    //}
                    //f += "\r\n";

                    fft.CalcFnumTl(lst, tlMul);
                    string r = "'N3 "
                    + string.Format(" y$AD,${0:X02} y$A9,${1:X02} ", (byte)(lst[0].fnum >> 8), (byte)lst[0].fnum)
                    + string.Format(" y$AC,${0:X02} y$A8,${1:X02} ", (byte)(lst[2].fnum >> 8), (byte)lst[2].fnum)
                    + string.Format(" y$AE,${0:X02} y$AA,${1:X02} ", (byte)(lst[1].fnum >> 8), (byte)lst[1].fnum)
                    + string.Format(" y$A6,${0:X02} y$A2,${1:X02} ", (byte)(lst[3].fnum >> 8), (byte)lst[3].fnum)
                    + string.Format(" y$42,${0:X02}", (byte)lst[0].tl)
                    + string.Format(" y$4A,${0:X02}", (byte)lst[2].tl)
                    + string.Format(" y$46,${0:X02}", (byte)lst[1].tl)
                    + string.Format(" y$4E,${0:X02}", (byte)lst[3].tl)
                    + " R \r\n";

                    result += r;
                    //fresult += f;
                    index += indLen;
                }

                result += "'N3 y$28,$02 EXOF\r\n";
                //result += fresult;
            }

            return result;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
