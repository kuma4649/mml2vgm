using System;
using System.Drawing;

namespace mml2vgmIDE
{
    [Serializable]
    public class ColorScheme
    {

        public int Azuki_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int Azuki_BackColor = Color.FromArgb(40, 30, 60).ToArgb();
        public int Azuki_IconBarBack = Color.FromArgb(70, 60, 90).ToArgb();
        public int Azuki_LineNumberBack_Normal = Color.FromArgb(40, 30, 60).ToArgb();
        public int Azuki_LineNumberFore_Normal = Color.FromArgb(80, 170, 200).ToArgb();
        public int Azuki_SelectionBack_Normal = Color.FromArgb(120, 100, 90).ToArgb();
        public int Azuki_SelectionFore_Normal = Color.FromArgb(220, 230, 250).ToArgb();
        public int Azuki_MatchedBracketBack_Normal = Color.FromArgb(120, 80, 90).ToArgb();
        public int Azuki_MatchedBracketFore_Normal = Color.FromArgb(250, 230, 220).ToArgb();
        public int Azuki_LineNumberBack_Trace = Color.FromArgb(150, 180, 60).ToArgb();
        public int Azuki_LineNumberFore_Trace = Color.FromArgb(20, 40, 10).ToArgb();
        public int Azuki_Keyword = Color.FromArgb(255, 190, 60).ToArgb();
        public int Azuki_Keyword2 = Color.FromArgb(255, 220, 60).ToArgb();
        public int Azuki_Comment = Color.FromArgb(250, 190, 240).ToArgb();
        public int Azuki_DocComment = Color.FromArgb(230, 130, 230).ToArgb();
        public int Azuki_Number = Color.FromArgb(235, 235, 255).ToArgb();
        public int Azuki_Annotation = Color.FromArgb(60, 255, 60).ToArgb();

        public int StatusStripBack_Normal = Color.FromArgb(60, 90, 190).ToArgb();
        public int StatusStripBack_Trace = Color.FromArgb(100, 150, 10).ToArgb();

        public int ErrorList_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int ErrorList_BackColor = Color.FromArgb(40, 30, 60).ToArgb();

        public int SearchBox_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int SearchBox_BackColor = Color.FromArgb(40, 30, 60).ToArgb();

        public int Log_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int Log_BackColor = Color.FromArgb(40, 30, 60).ToArgb();

        public int PartCounter_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int PartCounter_BackColor = Color.FromArgb(40, 30, 60).ToArgb();
        public int PartCounter_SOLOROW_BackColor = Color.FromArgb(90, 10, 10).ToArgb();
        public int PartCounter_SOLO_BackColor = Color.FromArgb(90, 10, 10).ToArgb();
        public int PartCounter_MUTE_BackColor = Color.FromArgb(120, 90, 10).ToArgb();

        public int FolderTree_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int FolderTree_BackColor = Color.FromArgb(40, 30, 60).ToArgb();

        public int FrmSien_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int FrmSien_BackColor = Color.FromArgb(40, 30, 60).ToArgb();

        public ColorScheme Copy()
        {
            ColorScheme ret = new ColorScheme();

            ret.Azuki_ForeColor = this.Azuki_ForeColor;
            ret.Azuki_BackColor = this.Azuki_BackColor;
            ret.Azuki_IconBarBack = this.Azuki_IconBarBack;
            ret.Azuki_LineNumberBack_Normal = this.Azuki_LineNumberBack_Normal;
            ret.Azuki_LineNumberFore_Normal = this.Azuki_LineNumberFore_Normal;
            ret.Azuki_SelectionBack_Normal = this.Azuki_SelectionBack_Normal;
            ret.Azuki_SelectionFore_Normal = this.Azuki_SelectionFore_Normal;
            ret.Azuki_LineNumberBack_Trace = this.Azuki_LineNumberBack_Trace;
            ret.Azuki_LineNumberFore_Trace = this.Azuki_LineNumberFore_Trace;
            ret.StatusStripBack_Normal = this.StatusStripBack_Normal;
            ret.StatusStripBack_Trace = this.StatusStripBack_Trace;
            ret.Azuki_Keyword = this.Azuki_Keyword;
            ret.Azuki_Keyword2 = this.Azuki_Keyword2;
            ret.Azuki_Comment = this.Azuki_Comment;
            ret.Azuki_DocComment = this.Azuki_DocComment;
            ret.Azuki_Number = this.Azuki_Number;
            ret.Azuki_Annotation = this.Azuki_Annotation;

            ret.ErrorList_ForeColor = this.ErrorList_ForeColor;
            ret.ErrorList_BackColor = this.ErrorList_BackColor;

            ret.SearchBox_ForeColor = this.SearchBox_ForeColor;
            ret.SearchBox_BackColor = this.SearchBox_BackColor;

            ret.Log_ForeColor = this.Log_ForeColor;
            ret.Log_BackColor = this.Log_BackColor;

            ret.PartCounter_ForeColor = this.PartCounter_ForeColor;
            ret.PartCounter_BackColor = this.PartCounter_BackColor;
            ret.PartCounter_SOLOROW_BackColor = this.PartCounter_SOLOROW_BackColor;
            ret.PartCounter_SOLO_BackColor = this.PartCounter_SOLO_BackColor;
            ret.PartCounter_MUTE_BackColor = this.PartCounter_MUTE_BackColor;

            ret.FolderTree_ForeColor = this.FolderTree_ForeColor;
            ret.FolderTree_BackColor = this.FolderTree_BackColor;

            return ret;
        }
    }
}
