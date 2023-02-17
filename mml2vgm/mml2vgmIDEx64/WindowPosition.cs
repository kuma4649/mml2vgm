using System;
using System.Drawing;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    [Serializable]
    public class WindowPosition
    {

        private string _name = "";
        //ウィンドウの位置と大きさ(フロート状態の位置と大きさを含む)
        private Rectangle _rect = Rectangle.Empty;
        //ウィンドウの状態(通常/最大化/最小化)
        private FormWindowState _windowState = FormWindowState.Normal;
        //非表示か
        private bool _hide = false;

        public string Name { get => _name; set => _name = value; }
        public Rectangle Rect { get => _rect; set => _rect = value; }
        public FormWindowState WindowState { get => _windowState; set => _windowState = value; }
        public bool Hide { get => _hide; set => _hide = value; }

        public WindowPosition()
        {
        }

        public WindowPosition Copy()
        {
            WindowPosition ret = new WindowPosition();
            ret.Name = this.Name;
            ret.Rect = Rectangle.Empty;
            if (this.Rect != Rectangle.Empty)
            {
                ret.Rect = new Rectangle(this.Rect.X, this.Rect.Y, this.Rect.Width, this.Rect.Height);
            }
            ret.WindowState = this.WindowState;
            ret.Hide = this.Hide;

            return ret;
        }
    }
}
