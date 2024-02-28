using musicDriverInterface;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class GD3maker
    {
        public void make(List<outDatum> dat, Information info, string lyric)
        {
            //'Gd3 '
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x47));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x64));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x33));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x20));

            //GD3 Version
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x01));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));

            //GD3 Length(dummy)
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));

            //TrackName
            foreach (byte b in Encoding.Unicode.GetBytes(info.TitleName)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            foreach (byte b in Encoding.Unicode.GetBytes(info.TitleNameJ)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));

            //GameName
            foreach (byte b in Encoding.Unicode.GetBytes(info.GameName)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            foreach (byte b in Encoding.Unicode.GetBytes(info.GameNameJ)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));

            //SystemName
            foreach (byte b in Encoding.Unicode.GetBytes(info.SystemName)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            foreach (byte b in Encoding.Unicode.GetBytes(info.SystemNameJ)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));

            //Composer
            foreach (byte b in Encoding.Unicode.GetBytes(info.Composer)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            foreach (byte b in Encoding.Unicode.GetBytes(info.ComposerJ)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));

            //ReleaseDate
            foreach (byte b in Encoding.Unicode.GetBytes(info.ReleaseDate)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));

            //Converted
            foreach (byte b in Encoding.Unicode.GetBytes(info.Converted)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));

            //Notes
            foreach (byte b in Encoding.Unicode.GetBytes(info.Notes)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));

            //歌詞
            if (!string.IsNullOrEmpty(lyric))
            {
                foreach (byte b in Encoding.Unicode.GetBytes(lyric)) dat.Add(new outDatum(enmMMLType.unknown, null, null, b));
                dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
                dat.Add(new outDatum(enmMMLType.unknown, null, null, 0x00));
            }
        }

    }
}
