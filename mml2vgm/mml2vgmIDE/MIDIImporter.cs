using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class MIDIImporter
    {
        private int format;
        private int maxTrack;
        private int reso;
        private int ptr;

        private List<string> mml = new List<string>();
        private StringBuilder line = new StringBuilder();
        private List<Track> tracks = new List<Track>();
        private int tickCounter = 0;
        private int lastNoteOffTickCounter=0;
        private byte bcmd = 0;
        private List<Tuple<int, Tuple<int, byte, byte[]>>>[] note = new List<Tuple<int, Tuple<int, byte, byte[]>>>[0x80];

        private int cOct = 4;

        private class Track
        {
            public int ptr;
            public int startAdr;
            public int endAdr;
            public bool endFlg = false;
            public int deltaCounter = 0;
        }


        public MIDIImporter()
        {
            for(int i = 0; i < 0x80; i++)
            {
                note[i] = new List<Tuple<int, Tuple<int, byte, byte[]>>>();
            }
        }

        public string[] Import(byte[] buff)
        {
            mml.Clear();

            string errMsg = " Import error : ヘッダが不正です。";

            //チェックと情報収集
            if (!CheckHeader(buff)) return new string[] { errMsg };
            if (!CheckTrackHeader(buff)) return new string[] { errMsg };
            //順番に並んだイベントのデータを作成する
            List<Tuple<int, Tuple<int, byte, byte[]>>> seqEvents = GetSeqEvent(buff);

            mml.Add(
@"'{
    TitleName = test GM
    TitleNameJ = GMテスト
    GameName =
    GameNameJ =
    SystemName =
    SystemNameJ =
    Composer = 
    ComposerJ =
    ReleaseDate =
    Converted =
    Notes =

    Format = ZGM
    ClockCount = 192
}
");
            //コンバート
            Convert(seqEvents);

            return mml.ToArray();
        }

        private bool CheckHeader(byte[] buff)
        {
            string head = GetString(buff, 0, 4);
            if (head != "MThd")
            {
                return false;
            }

            ptr = 4;
            int dataLength = GetInt32(buff, ref ptr);
            if (dataLength != 6)
            {
                return false;
            }

            format = GetInt16(buff, ref ptr);
            maxTrack = GetInt16(buff, ref ptr);
            reso = GetInt16(buff, ref ptr);

            return true;
        }

        private bool CheckTrackHeader(byte[] buff)
        {

            Track trki = new Track();
            trki.ptr = ptr;
            trki.startAdr = ptr;
            tracks.Add(trki);

            for (int trk = 0; trk < maxTrack; trk++)
            {

                //MTrkのチェック
                string head = GetString(buff, ptr, 4);
                ptr += 4;
                if (head != "MTrk")
                {
                    return false;
                }

                int nextAddress = GetInt32(buff, ref ptr) + ptr;

                trki = new Track();
                trki.ptr = nextAddress;
                trki.startAdr = nextAddress;
                tracks.Add(trki);

                ptr = nextAddress;
            }

            if (ptr != buff.Length) return false;

            //実際の終了アドレスの次のアドレスが設定されるので注意
            for (int trk = 0; trk < maxTrack; trk++) tracks[trk].endAdr = tracks[trk + 1].startAdr;
            tracks.RemoveAt(tracks.Count - 1);
            for (int trk = 0; trk < maxTrack; trk++) tracks[trk].ptr = tracks[trk].startAdr + 4 + 4;//4:head 4:datalength

            return true;
        }

        private List<Tuple<int, Tuple<int, byte, byte[]>>> GetSeqEvent(byte[] buff)
        {
            List<Tuple<int, Tuple<int, byte, byte[]>>> ret = new List<Tuple<int, Tuple<int, byte, byte[]>>>();

            bool end;
            int minDelta;
            do
            {

                foreach (Track trk in tracks)
                {
                    if (trk.deltaCounter > 0) continue;

                    //deltaCounterが0の間、イベントを読み込む
                    while (trk.deltaCounter == 0)
                    {
                        trk.deltaCounter = GetDelta(buff, ref trk.ptr);
                        Tuple<byte, byte[]> evnt = GetEvents(buff, ref trk.ptr);
                        ret.Add(new Tuple<int, Tuple<int, byte, byte[]>>(
                            tickCounter
                            , new Tuple<int, byte, byte[]>(trk.deltaCounter, evnt.Item1, evnt.Item2)
                            ));

#if DEBUG
                        Console.WriteLine(" tc:{0} delta:{1} cmd:{2:x02} data[0]:{3:x02} ", tickCounter, trk.deltaCounter, evnt.Item1, evnt.Item2[0]);
#endif

                        if (trk.ptr >= trk.endAdr)
                        {
                            trk.endFlg = true;
                            break;
                        }
                    }
                }

                //全てのトラックの終了をチェック&最小デルタチェック
                end = true;
                minDelta = int.MaxValue;
                foreach (Track trk in tracks)
                {
                    if (trk.endFlg) continue;
                    end = false;

                    minDelta = Math.Min(minDelta, trk.deltaCounter);
                }

                //カウンターの更新
                tickCounter += minDelta;
                foreach (Track trk in tracks)
                {
                    if (trk.endFlg) continue;
                    trk.deltaCounter -= minDelta;
                }

            } while (!end);

            return ret;
        }


        private string GetString(byte[] buff, int ptr, int length)
        {
            return System.Text.Encoding.UTF8.GetString(buff, ptr, length);
        }

        private int GetInt32(byte[] buff, ref int ptr)
        {
            return buff[ptr++] * 0x100_00_00 + buff[ptr++] * 0x100_00 + buff[ptr++] * 0x100 + buff[ptr++];
        }

        private int GetInt16(byte[] buff, ref int ptr)
        {
            return buff[ptr++] * 0x100 + buff[ptr++];
        }

        private int GetDelta(byte[] buff, ref int ptr)
        {
            int ret = 0;
            byte v;
            do
            {
                ret <<= 7;
                v = buff[ptr++];
                ret += (v & 0x7f);
            } while ((v & 0x80) != 0);

            return ret;
        }

        private Tuple<byte, byte[]> GetEvents(byte[] buff, ref int ptr)
        {
            Tuple<byte, byte[]> ret;
            byte cmd;

            cmd = buff[ptr++];
            if ((cmd & 0x80) == 0)
            {
                cmd = bcmd;
                ptr--;
            }
            bcmd = cmd;

            List<byte> data = new List<byte>();
            byte cmdtype = (byte)(cmd & 0xf0);
            if (cmdtype != 0xf0)
            {
                data.Add(buff[ptr++]);
                if (cmdtype != 0xc0 && cmdtype != 0xd0)
                    data.Add(buff[ptr++]);
            }
            else if (cmd == 0xff)
            {
                byte meta = buff[ptr++];
                int length = GetDelta(buff, ref ptr);
                data.Add(meta);
                for (int i = 0; i < length; i++) data.Add(buff[ptr++]);
            }
            else
            {
                int length = GetDelta(buff, ref ptr);
                for (int i = 0; i < length; i++) data.Add(buff[ptr++]);
            }

            ret = new Tuple<byte, byte[]>(cmd, data.ToArray());
            return ret;
        }

        private void Convert(List<Tuple<int, Tuple<int, byte, byte[]>>> seqEvents)
        {
            for(int ch = 0; ch < 16; ch++)
            {

                tickCounter = 0;
                lastNoteOffTickCounter = 0;
                cOct = 4;
                mml.Add(string.Format("'M{0:d02} o4", ch + 1));

                for (int pos = 0; pos < seqEvents.Count; pos++)
                {
                    Tuple<int, Tuple<int, byte, byte[]>> evnt = seqEvents[pos];

                    if (tickCounter >= evnt.Item1)
                    {
                        //int delta = evnt.Item2.Item1;
                        byte cmd = evnt.Item2.Item2;
                        //byte[] data = evnt.Item2.Item3;
                        int cmdType = cmd & 0xf0;
                        int cmdCh = cmd & 0xf;

                        if (cmdType == 0xf0)
                        {
                            //TBD
                            continue;
                        }
                        else if (cmdCh != ch) continue;

                        switch (cmdType)
                        {
                            case 0x80:
                                noteOff(evnt);
                                break;
                            case 0x90:
                                noteOn(evnt);
                                break;
                            case 0xa0:
                                polyTouch(evnt);
                                break;
                            case 0xb0:
                                controlChange(evnt);
                                break;
                            case 0xc0:
                                programChange(evnt);
                                break;
                            case 0xd0:
                                channelTouch(evnt);
                                break;
                            case 0xe0:
                                pitchBend(evnt);
                                break;
                        }
                    }
                    else
                    {
                        tickCounter = evnt.Item1;
                    }

                    if (line.Length > 40)
                    {
                        mml.Add(string.Format("'M{0:d02} {1}", ch + 1, line));
                        line.Clear();
                    }
                }

                if (line.Length > 0)
                {
                    mml.Add(string.Format("'M{0:d02} {1}", ch + 1, line));
                    line.Clear();
                }
                mml.Add("");

            }
        }

        private void noteOff(Tuple<int, Tuple<int, byte, byte[]>> evnt)
        {
            int delta = evnt.Item2.Item1;
            byte cmd = evnt.Item2.Item2;
            byte[] data = evnt.Item2.Item3;
            int cmdType = cmd & 0xf0;
            int cmdCh = cmd & 0xf;
            
            List<Tuple<int, Tuple<int, byte, byte[]>>> lstNote = note[data[0]];

            if (lstNote.Count > 0)
            {
                Tuple<int, Tuple<int, byte, byte[]>> lastNote = lstNote[lstNote.Count - 1];
                int tick = evnt.Item1 - lastNote.Item1;
                int velo = lastNote.Item2.Item3[1];
                lastNoteOffTickCounter = evnt.Item1;

                int o = data[0] / 12;
                string oct="";
                while (cOct != o)
                {
                    if (cOct < o)
                    {
                        oct += ">";
                        cOct++;
                    }
                    else
                    {
                        oct += "<";
                        cOct--;
                    }
                }
                string nt="c c+d d+e f f+g g+a a+b ".Substring((data[0] % 12) * 2, 2).Trim();
                line.AppendFormat("{0}{1}#{2},{3}", oct, nt, tick, velo);
                note[data[0]].Clear();
            }
        }


        private void noteOn(Tuple<int, Tuple<int, byte, byte[]>> evnt)
        {
            //int delta = evnt.Item2.Item1;
            byte cmd = evnt.Item2.Item2;
            byte[] data = evnt.Item2.Item3;
            //int cmdType = cmd & 0xf0;
            //int cmdCh = cmd & 0xf;

            if (data[1] != 00) 
            {
                note[data[0]].Add(evnt);
            }
            else
            {
                noteOff(evnt);
            }

            if (evnt.Item1 - lastNoteOffTickCounter > 0)
            {
                line.AppendFormat("r#{0}", evnt.Item1 - lastNoteOffTickCounter);
                lastNoteOffTickCounter = evnt.Item1;
            }
        }

        private void polyTouch(Tuple<int, Tuple<int, byte, byte[]>> evnt)
        {
            if (evnt.Item1 - lastNoteOffTickCounter > 0)
            {
                line.AppendFormat("r#{0}", evnt.Item1 - lastNoteOffTickCounter);
                lastNoteOffTickCounter = evnt.Item1;
            }
        }

        private void controlChange(Tuple<int, Tuple<int, byte, byte[]>> evnt)
        {
            if (evnt.Item1 - lastNoteOffTickCounter > 0)
            {
                line.AppendFormat("r#{0}", evnt.Item1 - lastNoteOffTickCounter);
                lastNoteOffTickCounter = evnt.Item1;
            }
        }

        private void programChange(Tuple<int, Tuple<int, byte, byte[]>> evnt)
        {
            //int delta = evnt.Item2.Item1;
            byte cmd = evnt.Item2.Item2;
            byte[] data = evnt.Item2.Item3;
            //int cmdType = cmd & 0xf0;
            //int cmdCh = cmd & 0xf;

            if (evnt.Item1 - lastNoteOffTickCounter > 0)
            {
                line.AppendFormat("r#{0}", evnt.Item1 - lastNoteOffTickCounter);
                lastNoteOffTickCounter = evnt.Item1;
            }
            line.AppendFormat("@{0}", data[0]);
        }

        private void channelTouch(Tuple<int, Tuple<int, byte, byte[]>> evnt)
        {
            if (evnt.Item1 - lastNoteOffTickCounter > 0)
            {
                line.AppendFormat("r#{0}", evnt.Item1 - lastNoteOffTickCounter);
                lastNoteOffTickCounter = evnt.Item1;
            }
        }

        private void pitchBend(Tuple<int, Tuple<int, byte, byte[]>> evnt)
        {
            if (evnt.Item1 - lastNoteOffTickCounter > 0)
            {
                line.AppendFormat("r#{0}", evnt.Item1 - lastNoteOffTickCounter);
                lastNoteOffTickCounter = evnt.Item1;
            }
        }

    }
}