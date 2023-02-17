using System.Windows.Forms;

namespace mml2vgmIDE
{
    public class DataGridViewImageCellEx : DataGridViewImageCell
    {
        public override object DefaultNewRowValue
        {
            get
            {
                return null;
            }
        }
    }
}