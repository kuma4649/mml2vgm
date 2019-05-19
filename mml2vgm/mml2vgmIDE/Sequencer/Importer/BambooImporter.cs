using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace mml2vgmIDE.Sequencer.Importer
{
    public class BambooImporter : Importer
    {
        private string title;
        private string author;
        private string copyright;
        private string comment;
        private int tickFreq;
        private int stepHighlight1Distance;
        private int stepHighlight2Distance;

        private Dictionary<string, Func<object, byte[], int, int, int>> Section;

        public override string Convert(string fullPath)
        {
            //ファイル存在チェック
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException();
            }

            byte[] buf;
            buf = File.ReadAllBytes(fullPath);

            int adr = 0;
            if (Encoding.ASCII.GetString(buf, adr, 16) != "BambooTrackerMod") {
                throw new FileLoadException();
            }

            adr += 16;
            int eofOfs = (int)Common.getLE32(buf, (uint)adr);
            int eof = adr + eofOfs;
            adr += 4;
            int fileVersion = (int)Common.getLE32(buf, (uint)adr);
            if (fileVersion > 0x00010002) //Version::ofModuleFileInBCD()
            {
                throw new FileLoadException("このバージョンのインポートはサポートしていません");
            }
            adr += 4;

            object bambooSongData = new object();

            Section = new Dictionary<string, Func<object, byte[], int, int, int>>();
            Section.Add("MODULE  ", GetModuleData);
            Section.Add("INSTRMNT", GetInstrumentData);
            Section.Add("INSTPROP", GetInstrumentProperty);
            Section.Add("GROOVE  ", GetGrooveData);
            Section.Add("SONG    ", GetSongData);

            while (adr < eof)
            {
                string key = Encoding.ASCII.GetString(buf, adr, 8);
                if (Section.ContainsKey(key))
                {
                    adr = Section[key](bambooSongData, buf, adr + 8, fileVersion);
                }
                else
                {
                    throw new FileLoadException();
                }

            }

            return "";
        }

        private int GetSongData(object bambooSongData, byte[] buf, int adr, int fileVersion)
        {
            throw new NotImplementedException();
        }

        private int GetGrooveData(object bambooSongData, byte[] buf, int adr, int fileVersion)
        {
            throw new NotImplementedException();
        }

        private int GetInstrumentProperty(object bambooSongData, byte[] buf, int adr, int fileVersion)
        {
            throw new NotImplementedException();
        }

        private int GetInstrumentData(object bambooSongData, byte[] buf, int adr, int fileVersion)
        {
            throw new NotImplementedException();
        }

        private int GetModuleData(object bambooSongData, byte[] buf, int adr, int version)
        {
            int modOfs = (int)Common.getLE32(buf, (uint)adr);
            int modAdr = adr + 4;
            int len;

            len = (int)Common.getLE32(buf, (uint)modAdr);
            modAdr += 4;
            if (len > 0)
            {
                title = Encoding.ASCII.GetString(buf, modAdr, len);
                modAdr += len;
            }

            len = (int)Common.getLE32(buf, (uint)modAdr);
            modAdr += 4;
            if (len > 0)
            {
                author = Encoding.ASCII.GetString(buf, modAdr, len);
                modAdr += len;
            }

            len = (int)Common.getLE32(buf, (uint)modAdr);
            modAdr += 4;
            if (len > 0)
            {
                copyright = Encoding.ASCII.GetString(buf, modAdr, len);
                modAdr += len;
            }

            len = (int)Common.getLE32(buf, (uint)modAdr);
            modAdr += 4;
            if (len > 0)
            {
                comment = Encoding.ASCII.GetString(buf, modAdr, len);
                modAdr += len;
            }

            tickFreq = (int)Common.getLE32(buf, (uint)modAdr);
            modAdr += 4;

            stepHighlight1Distance = (int)Common.getLE32(buf, (uint)modAdr);
            modAdr += 4;

            if (version >= 0x00010003) //toBCD(1, 0, 3)
            {
                stepHighlight2Distance = (int)Common.getLE32(buf, (uint)modAdr);
                modAdr += 4;
            }
            else
            {
                stepHighlight2Distance = stepHighlight1Distance * 4;
            }

            return adr + modOfs;
        }
    }
}
