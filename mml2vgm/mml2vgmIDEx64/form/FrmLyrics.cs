using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDEx64
{
    public partial class FrmLyrics : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action parentUpdate = null;
        public Setting setting = null;
        public List<Tuple<int, int, string>> lyrics = null;
        private bool isMuapMode;
        public int lyricsIndex = 0;
        private Color culColor = Color.FromArgb(192, 192, 255);
        private long skip = 0;
        private string muapOldText = "";
        private int oldComlength;

        public FrmLyrics(Setting setting, ThemeBase theme)
        {
            InitializeComponent();
            rtbLyrics.BackColor = Color.FromArgb(setting.ColorScheme.Log_BackColor);
            rtbLyrics.ForeColor = Color.FromArgb(setting.ColorScheme.Log_ForeColor);
            culColor = Color.FromArgb(setting.ColorScheme.Log_ForeColor);

            this.setting = setting;
            rtbLyrics.GotFocus += RichTextBox1_GotFocus;

            Common.SetDoubleBuffered(rtbLyrics);

        }

        protected override string GetPersistString()
        {
            return this.Name;
        }

        private void FrmLyrics_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            parentUpdate?.Invoke();
        }

        private void RichTextBox1_GotFocus(object sender, EventArgs e)
        {
            lblDummy.Focus();
        }

        public void update()
        {
            isMuapMode=false;
            muapOldText = "";

            rtbLyrics.Clear();
            GD3 gd3 = Audio.GetGD3();
            if (gd3 == null) return;

            if (gd3.Lyrics == null)
            {
                timer.Enabled = false;
                return;
            }

            lyrics = gd3.Lyrics;
            lyricsIndex = 0;
            skip = Audio.sm.GetDriverSeqCounterDelay();
            skip += Audio.getLatency();

            timer.Enabled = true;
        }

        public void update(GD3Tag gd3tag)
        {
            isMuapMode = false;
            muapOldText = "";
            rtbLyrics.Clear();

            if (gd3tag == null || gd3tag.dicItem == null
                || !gd3tag.dicItem.ContainsKey(enmTag.Lyric)
                || gd3tag.dicItem[enmTag.Lyric].Length < 1
                || gd3tag.dicItem[enmTag.Lyric][0] != "MUS:UseLyric"
                )
            {
                timer.Enabled = false;
                return;
            }

            isMuapMode = true;
            lyricsIndex = 0;
            skip = Audio.sm.GetDriverSeqCounterDelay();
            skip += Audio.getLatency();

            timer.Enabled = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isMuapMode)
            {
                muapTimer_Tick();
                return;
            }

            if (lyrics == null || lyrics.Count < 1) return;

            long cnt = Audio.GetDriverCounter() - skip;

            try
            {
                if (cnt < lyrics[lyricsIndex].Item1) return;

                //lblLyrics.Text = lyrics[lyricsIndex].Item3;
                rtbLyrics.Clear();

                int ind = 0;
                rtbLyrics.SelectionColor = culColor;
                while (ind < lyrics[lyricsIndex].Item3.Length)
                {
                    char c = lyrics[lyricsIndex].Item3[ind];
                    if (c == '\\')
                    {
                        ind++;
                        c = lyrics[lyricsIndex].Item3[ind];
                        switch (c)
                        {
                            case '"':
                            case '\\':
                                break;
                            case 'c':
                                ind++;
                                string n = lyrics[lyricsIndex].Item3[ind++].ToString();
                                int r, g, b;
                                if (n == "s")
                                {
                                    r = 192;
                                    g = 192;
                                    b = 255; //192,192,255 system color
                                }
                                else
                                {
                                    n += lyrics[lyricsIndex].Item3[ind++].ToString();
                                    r = Int32.Parse(n, System.Globalization.NumberStyles.HexNumber);
                                    n = lyrics[lyricsIndex].Item3[ind++].ToString();
                                    n += lyrics[lyricsIndex].Item3[ind++].ToString();
                                    g = Int32.Parse(n, System.Globalization.NumberStyles.HexNumber);
                                    n = lyrics[lyricsIndex].Item3[ind++].ToString();
                                    n += lyrics[lyricsIndex].Item3[ind++].ToString();
                                    b = Int32.Parse(n, System.Globalization.NumberStyles.HexNumber);
                                }
                                culColor = Color.FromArgb(r, g, b);
                                rtbLyrics.SelectionColor = culColor;
                                continue;
                        }
                    }
                    rtbLyrics.SelectedText = c.ToString();
                    ind++;
                }

                lyricsIndex++;

                if (lyricsIndex == lyrics.Count)
                {
                    timer.Enabled = false;
                }
            }
            catch
            {
                try
                {
                    rtbLyrics.Clear();
                    rtbLyrics.SelectedText = "LYLIC PARSE ERROR";
                }
                catch { }
            }

        }

        private void muapTimer_Tick()
        {
            //List<Tuple<string, string>> ret = Audio.GetTagsDriver();
            //if (ret == null || ret.Count < 2 || ret[0] == null) return;

            List<Tuple<long, int, string>> lyrics = Audio.GetMuapLyrics();
            if (lyrics == null || lyrics.Count < 1) return;
            long cnt = Audio.EmuSeqCounter;// - skip;

            if (lyricsIndex < 0) lyricsIndex = 0;
            while (lyricsIndex+1 < lyrics.Count && cnt >= lyrics[lyricsIndex + 1].Item1) lyricsIndex++;
            if (lyricsIndex >= lyrics.Count) return;

            string ly = lyrics[lyricsIndex].Item3;
            int comlength = lyrics[lyricsIndex].Item2;
            ly = ly.Replace("\0", "");
            if (comlength == 255)
            {
                rtbLyrics.ForeColor = Color.White;
                rtbLyrics.Text = ly;
                oldComlength = -1;
                return;
            }

            if (oldComlength == comlength) return;

            oldComlength = comlength;
            rtbLyrics.SuspendLayout();
            rtbLyrics.Clear();
            if (comlength != 0)
            {
                rtbLyrics.SelectionColor = Color.White;
                rtbLyrics.SelectedText = ly.Substring(0, Math.Min(ly.Length, comlength));
            }
            if (ly.Length > comlength)
            {
                rtbLyrics.SelectionColor = Color.Blue;
                rtbLyrics.SelectedText = ly.Substring(comlength);
            }
            rtbLyrics.ResumeLayout();

            //if (muapOldText == muapNewText) return;

            //rtbLyrics.Clear();
            //rtbLyrics.SelectedText = muapNewText;
            //muapOldText = muapNewText;
        }

        private void FrmLyrics_SizeChanged(object sender, EventArgs e)
        {
            if (!this.Created) return;
            if (rtbLyrics.Font == null) return;
            if (this.Height < 1) return;

            Font oldfont = rtbLyrics.Font;
            rtbLyrics.Font = new Font(rtbLyrics.Font.OriginalFontName, Math.Max(this.Height / 2, 1), FontStyle.Bold);
            try
            {
                oldfont.Dispose();
            }
            catch { }
        }

    }
}
