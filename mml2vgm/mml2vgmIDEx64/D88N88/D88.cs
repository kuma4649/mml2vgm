using System;
using System.Collections.Generic;

namespace mml2vgmIDEx64.D88N88
{
    public class D88
    {
        public D88Header header;
        public D88Track[] tracks;

        public D88(byte[] d88b)
        {
            GetD88StrucFromByteArray(d88b);
        }

        public void GetD88StrucFromByteArray(byte[] d88b)
        {
            header = new D88Header(d88b);
            tracks = new D88Track[header.track_table.Length];
            for (int i = 0; i < header.track_table.Length; i++)
            {
                if (header.track_table[i] == 0) continue;
                tracks[i] = new D88Track(d88b, header.track_table[i]);
            }
        }

        public byte[] GetRaw()
        {
            List<byte> buf = new List<byte>();
            for (int i = 0; i < header.name.Length; i++) buf.Add(header.name[i]);
            for (int i = 0; i < header.reserve.Length; i++) buf.Add(header.reserve[i]);
            buf.Add(header.write_protect);
            buf.Add(header.disk_type);
            int n = header.disk_size;
            buf.Add((byte)n); buf.Add((byte)(n >> 8)); buf.Add((byte)(n >> 16)); buf.Add((byte)(n >> 24));
            for (int i = 0; i < header.track_table.Length; i++)
            {
                n = header.track_table[i];
                buf.Add((byte)n); buf.Add((byte)(n >> 8)); buf.Add((byte)(n >> 16)); buf.Add((byte)(n >> 24));
            }
            for (int i = header.track_table.Length; i < 164; i++)
            {
                buf.Add(0); buf.Add(0); buf.Add(0); buf.Add(0);
            }

            for (int i = 0; i < tracks.Length; i++)
            {
                if (tracks[i] == null) continue;
                for (int j = 0; j < tracks[i].sectors.Length; j++)
                {
                    buf.Add(tracks[i].sectors[j].c);
                    buf.Add(tracks[i].sectors[j].h);
                    buf.Add(tracks[i].sectors[j].r);
                    buf.Add(tracks[i].sectors[j].n);
                    n = tracks[i].sectors[j].number_of_sector;
                    buf.Add((byte)n); buf.Add((byte)(n >> 8));
                    buf.Add(tracks[i].sectors[j].density);
                    buf.Add(tracks[i].sectors[j].deleted_mark);
                    buf.Add(tracks[i].sectors[j].status);
                    for (int k = 0; k < tracks[i].sectors[j].reserve.Length; k++)
                        buf.Add(tracks[i].sectors[j].reserve[k]);
                    n = tracks[i].sectors[j].size_of_data;
                    buf.Add((byte)n); buf.Add((byte)(n >> 8));
                    for (int k = 0; k < tracks[i].sectors[j].data.Length; k++)
                        buf.Add(tracks[i].sectors[j].data[k]);
                }
            }

            return buf.ToArray();
        }
    }

    public class D88Header
    {
        public byte[] name = new byte[17];
        public byte[] reserve = new byte[9];
        public byte write_protect;
        public byte disk_type;
        public Int32 disk_size;
        public Int32[] track_table = null;

        public D88Header(byte[] d88b)
        {
            int ptr = 0;

            Array.Copy(d88b, ptr, name, 0, name.Length);
            ptr += name.Length;

            Array.Copy(d88b, ptr, reserve, 0, reserve.Length);
            ptr += reserve.Length;

            write_protect = d88b[ptr++];

            disk_type = d88b[ptr++];

            disk_size = d88b[ptr] + d88b[ptr + 1] * 0x100 + d88b[ptr + 2] * 0x100_00 + d88b[ptr + 3] * 0x100_00_00;
            ptr += 4;

            int trkN = 164;//2DD/2HD
            if (disk_type == 0) trkN = 84;//2D
            track_table = new Int32[trkN];

            for (int i = 0; i < trkN; i++)
            {
                track_table[i] = d88b[ptr] + d88b[ptr + 1] * 0x100 + d88b[ptr + 2] * 0x100_00 + d88b[ptr + 3] * 0x100_00_00;
                ptr += 4;
            }
        }
    }

    public class D88Track
    {
        public D88Sector[] sectors;

        public D88Track(byte[] d88b, int ptr)
        {
            int num = d88b[ptr + 4] + d88b[ptr + 5] * 0x100;
            sectors = new D88Sector[num];

            for (int i = 0; i < num; i++)
            {
                sectors[i] = new D88Sector();
                sectors[i].c = d88b[ptr++];
                sectors[i].h = d88b[ptr++];
                sectors[i].r = d88b[ptr++];
                sectors[i].n = d88b[ptr++];
                sectors[i].number_of_sector = (ushort)(d88b[ptr] + d88b[ptr + 1] * 0x100);
                ptr += 2;
                sectors[i].density = d88b[ptr++];
                sectors[i].deleted_mark = d88b[ptr++];
                sectors[i].status = d88b[ptr++];
                sectors[i].reserve = new byte[5];
                Array.Copy(d88b, ptr, sectors[i].reserve, 0, sectors[i].reserve.Length);
                ptr += sectors[i].reserve.Length;
                sectors[i].size_of_data = (ushort)(d88b[ptr] + d88b[ptr + 1] * 0x100);
                ptr += 2;
                sectors[i].data = new byte[sectors[i].size_of_data];
                Array.Copy(d88b, ptr, sectors[i].data, 0, sectors[i].data.Length);
                ptr += sectors[i].data.Length;
            }
        }

    }

    public class D88Sector
    {
        public byte c;
        public byte h;
        public byte r;
        public byte n;
        public ushort number_of_sector;
        public byte density;
        public byte deleted_mark;
        public byte status;
        public byte[] reserve = new byte[5];
        public ushort size_of_data;
        public byte[] data = null;
    }

}
