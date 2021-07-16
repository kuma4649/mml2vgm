using System.Drawing;
using System.Windows.Forms;

namespace CustomControl
{
    public class MultiSelectTreeView : TreeView
    {
        private Color hotColor;
        private Color selectedColor;
        private Color checkedColor;

        private SolidBrush brushNodeBack;
        private SolidBrush brushHotNodeBack;
        private SolidBrush brushSelectedNodeBack;
        private SolidBrush brushCheckedNodeBack;
        private SolidBrush brushFontColor;
        private TreeNode hoverNode;
        private Bitmap backBmp = null;
        private StringFormat sf;

        public Color HotColor
        {
            get => hotColor;
            set
            {
                hotColor = value;
                if (brushHotNodeBack != null) brushHotNodeBack.Dispose();
                brushHotNodeBack = new SolidBrush(hotColor);
            }
        }

        public Color SelectedColor
        {
            get => selectedColor;
            set
            {
                selectedColor = value;
                if (brushSelectedNodeBack != null) brushSelectedNodeBack.Dispose();
                brushSelectedNodeBack = new SolidBrush(selectedColor);
            }
        }

        public Color CheckedColor
        {
            get => checkedColor;
            set
            {
                checkedColor = value;
                if (brushCheckedNodeBack != null) brushCheckedNodeBack.Dispose();
                brushCheckedNodeBack = new SolidBrush(checkedColor);
            }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                if (brushNodeBack != null) brushNodeBack.Dispose();
                brushNodeBack = new SolidBrush(base.BackColor);
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                if (brushFontColor != null) brushFontColor.Dispose();
                brushFontColor = new SolidBrush(base.ForeColor);
            }
        }



        public MultiSelectTreeView()
        {
            this.DoubleBuffered = true;
            sf = new StringFormat();
            sf.Trimming = StringTrimming.EllipsisCharacter;
            sf.FormatFlags = StringFormatFlags.NoWrap;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (backBmp != null) backBmp.Dispose();
            if (brushNodeBack != null) brushNodeBack.Dispose();
            if (brushHotNodeBack != null) brushHotNodeBack.Dispose();
            if (brushSelectedNodeBack != null) brushSelectedNodeBack.Dispose();
            if (brushCheckedNodeBack != null) brushCheckedNodeBack.Dispose();
            if (brushFontColor != null) brushFontColor.Dispose();
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            try
            {
                if (DrawMode != TreeViewDrawMode.OwnerDrawAll)
                {
                    base.OnDrawNode(e);
                    return;
                }

                if (e.Bounds.Width == 0 && e.Bounds.Height == 0) return;

                if (backBmp == null)
                    backBmp = new Bitmap(e.Bounds.Width, e.Bounds.Height);
                else if (backBmp.Width != e.Bounds.Width || backBmp.Height != e.Bounds.Height)
                {
                    backBmp.Dispose();
                    backBmp = new Bitmap(e.Bounds.Width, e.Bounds.Height);
                }

                Rectangle bnd = new Rectangle(0, 0, e.Bounds.Width, e.Bounds.Height);
                Graphics g = Graphics.FromImage(backBmp);

                if ((e.State & TreeNodeStates.Selected) != 0)
                {
                    if (brushSelectedNodeBack != null)
                        g.FillRegion(brushSelectedNodeBack, new Region(bnd));
                }
                else if (e.Node.Checked)
                {
                    if (brushCheckedNodeBack != null)
                        g.FillRegion(brushCheckedNodeBack, new Region(bnd));
                }
                else if (((e.State & TreeNodeStates.Hot) != 0) || hoverNode == e.Node)
                {
                    if (brushHotNodeBack != null)
                        g.FillRegion(brushHotNodeBack, new Region(bnd));
                }
                else
                {
                    if (brushNodeBack != null)
                        g.FillRegion(brushNodeBack, new Region(bnd));
                }

                bool showIcon = (e.Node.ImageIndex >= 0);
                int shift = e.Node.Level * Indent;
                int shiftPM = 16;
                int shiftPI = (showIcon ? 16 : 0);

                if (brushFontColor != null)
                {
                    g.DrawString(e.Node.Text, this.Font, brushFontColor
                    , new Rectangle(new Point(
                        bnd.X + shift + shiftPM + shiftPI
                        , bnd.Y + 0)
                    , bnd.Size)
                    , sf);
                }

                if (e.Node.Nodes.Count > 0)
                {
                    Bitmap bmp;
                    bmp = (e.Node.IsExpanded) ? Properties.Resources.Expand : Properties.Resources.Collapse;
                    bmp.MakeTransparent(Color.Black);
                    g.DrawImage(bmp, bnd.X + shift, bnd.Y, bnd.Height, bnd.Height);
                }


                if (showIcon)
                {
                    if (this.ImageList != null
                        && this.ImageList.Images != null
                        && this.ImageList.Images.Count > e.Node.ImageIndex)
                    {
                        g.DrawImage(this.ImageList.Images[e.Node.ImageIndex]
                            , bnd.X + shift + shiftPM
                            , bnd.Y
                            , bnd.Height
                            , bnd.Height);
                    }
                }

                e.Graphics.DrawImage(backBmp, e.Bounds.X, e.Bounds.Y);
            }
            catch { }
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);

            this.SelectedNode = e.Node;

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!HotTracking) return;

            TreeViewHitTestInfo info = this.HitTest(e.Location);
            if (hoverNode == info.Node) return;

            if (hoverNode != null) Invalidate(new Rectangle(0, hoverNode.Bounds.Y, Width, hoverNode.Bounds.Height));

            hoverNode = info.Node;

            if (hoverNode != null) Invalidate(new Rectangle(0, hoverNode.Bounds.Y, Width, hoverNode.Bounds.Height));
        }

    }
}
