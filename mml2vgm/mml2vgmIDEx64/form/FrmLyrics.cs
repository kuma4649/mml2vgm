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
        public int lyricsIndex = 0;
        private Color culColor = Color.FromArgb(192, 192, 255);


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

            rtbLyrics.Clear();
            GD3 gd3 = Audio.GetGD3();
            if (gd3 == null) return;

            if (gd3.Lyrics == null)
            {
                timer.Enabled = false;
            }
            else
            {
                lyrics = gd3.Lyrics;
                lyricsIndex = 0;
                skip = Audio.sm.GetDriverSeqCounterDelay();
                skip += Audio.getLatency();

                timer.Enabled = true;
            }
        }
        private long skip = 0;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (lyrics == null || lyrics.Count < 1) return;

            long cnt = Audio.GetDriverCounter() - skip;

            try
            {
                if (cnt >= lyrics[lyricsIndex].Item1)
                {

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
