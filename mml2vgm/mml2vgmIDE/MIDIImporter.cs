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
        private double mul = 0;
        private double length = 0;
        private List<MIDIEvent>[] note = new List<MIDIEvent>[0x80];

        private int cOct = 4;
        private int cVelo = 110;

        private class Track
        {
            public int ptr;
            public int startAdr;
            public int endAdr;
            public bool endFlg = false;
            public int deltaCounter = 0;
        }

        private class MIDIEvent
        {
            public int tickPos = 0;
            public int tickLength = 0;
            public byte cmd = 0;
            public byte[] data = null;
            public enmCmdType cmdType {
                get {
                    switch (cmd & 0xf0)
                    {
                        case 0x80:
                            return enmCmdType.NoteOff;
                        case 0x90:
                            return enmCmdType.NoteOn;
                        case 0xa0:
                            return enmCmdType.PolyTouch;
                        case 0xb0:
                            return enmCmdType.ControlChange;
                        case 0xc0:
                            return enmCmdType.ProgramChange;
                        case 0xd0:
                            return enmCmdType.ChannelTouch;
                        case 0xe0:
                            return enmCmdType.PitchBend;
                        case 0xf0:
                            return enmCmdType.SysEx;
                    }
                    return enmCmdType.Unknown;
                } 
            }
            public int cmdCh { get { return cmd & 0xf; } }

            public MIDIEvent(int tickPos, int tickLength = 0, byte cmd = 0, byte[] data = null)
            {
                this.tickPos = tickPos;
                this.tickLength = tickLength;
                this.cmd = cmd;
                this.data = data;
            }

            public override string ToString()
            {
                return String.Format("Ch:{0} Type:{1} {2}Pos:{3} Length:{4}",
                    cmdCh
                    , cmdType
                    , data == null ? "" : 
                        (data.Length < 1 ? "" : 
                        (data.Length < 2 ? string.Format("Data:{0:x02} ", data[0]) : 
                        (data.Length < 3 ? string.Format("Data:{0:x02} {1:x02} ", data[0], data[1]) : 
                        "")))
                    , tickPos
                    , tickLength);
            }
        }

        private enum enmCmdType : byte
        {
            Unknown=0x00
            , NoteOff = 0x80
            , NoteOn = 0x90
            , PolyTouch = 0xa0
            , ControlChange = 0xb0
            , ProgramChange = 0xc0
            , ChannelTouch = 0xd0
            , PitchBend = 0xe0
            , SysEx = 0xf0
        }

        public MIDIImporter()
        {
            for(int i = 0; i < 0x80; i++)
            {
                note[i] = new List<MIDIEvent>();
            }
        }

        public string[] Import(byte[] buff)
        {
            mml.Clear();

            string errMsg = " Import error : ヘッダが不正です。";

            //チェックと情報収集
            if (!CheckHeader(buff)) return new string[] { errMsg };
            if (!CheckTrackHeader(buff)) return new string[] { errMsg };
            
            List<MIDIEvent> seqEvents = GetSeqEvent(buff);//順番に並んだイベントのデータを作成する
            List<MIDIEvent>[] seqChEvents = DivideSeqEvents(seqEvents);//チャンネルごとのイベントに分ける

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
            Convert(seqChEvents);

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
            mul = 192.0 / (reso * 4);
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

        /// <summary>
        /// イベントごとのリストを生成する。
        /// その際、複数のトラックのデータをシーケンシャルな並びのイベントデータにまとめる
        /// </summary>
        private List<MIDIEvent> GetSeqEvent(byte[] buff)
        {
            List<MIDIEvent> ret = new List<MIDIEvent>();

            bool end;
            int minDelta;
            do
            {
                MIDIEvent lastEvent = null;
                foreach (Track trk in tracks)
                {
                    if (trk.endFlg) continue;
                    if (trk.deltaCounter > 0) continue;

                    if (trk.ptr == trk.startAdr + 4 + 4)
                        trk.deltaCounter = GetDelta(buff, ref trk.ptr);

                    //deltaCounterが0の間、イベントを読み込む
                    while (trk.deltaCounter == 0)
                    {
                        Tuple<byte, byte[]> evnt = GetEvents(buff, ref trk.ptr);
                        lastEvent = new MIDIEvent(tickCounter, 0, evnt.Item1, evnt.Item2);
                        ret.Add(lastEvent);

#if DEBUG
                        Console.WriteLine(lastEvent.ToString());
#endif
                        //最後まで読み込んだのであれば、このトラックの解析は完了
                        if (trk.ptr >= trk.endAdr)
                        {
                            trk.endFlg = true;
                            break;
                        }

                        trk.deltaCounter = GetDelta(buff, ref trk.ptr);
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

                if (lastEvent != null)
                {
                    lastEvent.tickLength = minDelta;
#if DEBUG
                    Console.WriteLine("lastEvent: {0}",lastEvent);
#endif
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

        private List<MIDIEvent>[] DivideSeqEvents(List<MIDIEvent> seqEvents)
        {

            List<MIDIEvent>[] ret = new List<MIDIEvent>[17];
            int[] delta = new int[17];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new List<MIDIEvent>();
                delta[i] = 0;
            }


            foreach (MIDIEvent evnt in seqEvents)
            {

                int usedCh;
                if (evnt.cmdType == enmCmdType.SysEx)
                {
                    //SysEx系はCh0にまとめる
                    ret[0].Add(evnt);
                    usedCh = 0;
                }
                else
                {
                    //その他は+1したChにまとめる
                    ret[evnt.cmdCh + 1].Add(evnt);
                    usedCh = evnt.cmdCh + 1;
                }

                int now = evnt.tickLength;
                for (int i = 0; i < ret.Length; i++)
                {
                    if (usedCh == i)
                    {
                        evnt.tickLength = delta[i];
                        delta[i] = now;
                    }
                    else
                    {
                        delta[i] += now;
                    }
                }

            }

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

        private void Convert(List<MIDIEvent>[] seqChEvents)
        {
            for(int ch = 0; ch < seqChEvents.Length; ch++)
            {

                tickCounter = 0;
                lastNoteOffTickCounter = 0;
                cOct = 4;
                length = 0;
                cVelo = 110;
                mml.Add(string.Format("'M{0:d02} o4 CH{1} U110", ch + 1, ch == 0 ? 1 : ch));
                List<MIDIEvent> seqEvents = seqChEvents[ch];

                for (int pos = 0; pos < seqEvents.Count; pos++)
                {
                    MIDIEvent evnt = seqEvents[pos];
                    tickCounter += evnt.tickLength;

                    {
                        Flash(seqEvents, pos);

                        if (evnt.cmdType == enmCmdType.SysEx)
                        {
                            continue;
                        }
                        else if (evnt.cmdCh != ch - 1) continue;

                        switch (evnt.cmdType)
                        {
                            case enmCmdType.NoteOff:
                                noteOff(evnt);
                                break;
                            case enmCmdType.NoteOn:
                                noteOn(evnt);
                                break;
                            case enmCmdType.PolyTouch:
                                polyTouch(evnt);
                                break;
                            case enmCmdType.ControlChange:
                                controlChange(evnt);
                                break;
                            case enmCmdType.ProgramChange:
                                programChange(evnt);
                                break;
                            case enmCmdType.ChannelTouch:
                                channelTouch(evnt);
                                break;
                            case enmCmdType.PitchBend:
                                pitchBend(evnt);
                                break;
                        }
                    }

                    if (line.Length > 70)
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

        private void Flash(List<MIDIEvent> seq,int pos)
        {
            if (tickCounter < 1) return;

            length = mul * tickCounter;
            int tick = (int)length;
            length -= tick;

            bool restflag = true;
            foreach(List<MIDIEvent> lstNevnt in note)
            {
                if (lstNevnt.Count < 1) continue;

                restflag = false;

                MIDIEvent evnt = lstNevnt[0];
                int o = evnt.data[0] / 12;
                string oct = "";
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
                string nt = "c c+d d+e f f+g g+a a+b ".Substring((evnt.data[0] % 12) * 2, 2).Trim();
                int velo = evnt.data[1];
                string sVelo = "";
                if (cVelo != velo)
                {
                    cVelo = velo;
                    sVelo = string.Format("U{0}", velo);
                }

                string tie = "";
                while (pos < seq.Count)
                {
                    if (seq[pos ].cmdType == enmCmdType.NoteOn && seq[pos].data[1] != 0)
                    {
                        tie = "&";
                        break;
                    }
                    else if(seq[pos].cmdType == enmCmdType.NoteOff
                            || (seq[pos ].cmdType == enmCmdType.NoteOn && seq[pos].data[1] == 0))
                    {
                        tie = "";
                        break;
                    }
                    else if (seq[pos].tickLength > 0)
                    {
                        tie = "&";
                        break;
                    }
                    pos++;
                }

                line.AppendFormat("{0}{1}{2}#{3}{4}", sVelo, oct, nt, tick,tie);

                break;
            }

            if (restflag) line.AppendFormat("r#{0}", tick);

            tickCounter = 0;
        }

        private void noteOff(MIDIEvent evnt)
        {
            note[evnt.data[0]].Clear();
        }


        private void noteOn(MIDIEvent evnt)
        {
            if (evnt.data[1] != 00) 
            {
                note[evnt.data[0]].Add(evnt);
            }
            else
            {
                noteOff(evnt);
            }
        }

        private void polyTouch(MIDIEvent evnt)
        {
        }

        private void controlChange(MIDIEvent evnt)
        {
        }

        private void programChange(MIDIEvent evnt)
        {
            line.AppendFormat("@{0}", evnt.data[0]);
        }

        private void channelTouch(MIDIEvent evnt)
        {
        }

        private void pitchBend(MIDIEvent evnt)
        {
        }

    }
}