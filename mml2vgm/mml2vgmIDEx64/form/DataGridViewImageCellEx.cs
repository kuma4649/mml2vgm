using System.Windows.Forms;

namespace mml2vgmIDEx64
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