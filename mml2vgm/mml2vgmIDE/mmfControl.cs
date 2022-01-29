using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Security.AccessControl;
using System.Text;


namespace mml2vgmIDE
{
    public class mmfControl
    {
        private object lockobj = new object();
        private static Dictionary<string, Tuple<MemoryMappedFile, byte[]>> _map = new Dictionary<string, Tuple<MemoryMappedFile, byte[]>>();
        public string mmfName = "dummy";
        public int mmfSize = 1024;

        public mmfControl()
        {
        }

        public mmfControl(bool isClient, string mmfName, int mmfSize)
        {
            this.mmfName = mmfName;
            this.mmfSize = mmfSize;
            if (!isClient) Open(mmfName, mmfSize);
        }

        public void Open(string mmfName, int mmfSize)
        {
            if (_map.ContainsKey(mmfName)) return;
            MemoryMappedFile mp;
            byte[] mmfBuf = new byte[mmfSize];

            lock (lockobj)
            {
                mp = MemoryMappedFile.CreateNew(mmfName, mmfSize);
                MemoryMappedFileSecurity permission = mp.GetAccessControl();
                permission.AddAccessRule(
                  new AccessRule<MemoryMappedFileRights>("Everyone",
                    MemoryMappedFileRights.FullControl, AccessControlType.Allow));
                mp.SetAccessControl(permission);
            }
            _map.Add(mmfName, new Tuple<MemoryMappedFile, byte[]>(mp, mmfBuf));

            return;
        }

        public void Close()
        {
            //lock (lockobj)
            //{
            //    if (_map == null) return;
            //    _map.Dispose();
            //    _map = null;
            //}
        }

        public string GetMessage()
        {
            string msg = "";

            lock (lockobj)
            {
                using (MemoryMappedViewAccessor view = _map[mmfName].Item1.CreateViewAccessor())
                {
                    view.ReadArray(0, _map[mmfName].Item2, 0, _map[mmfName].Item2.Length);
                    msg = Encoding.Unicode.GetString(_map[mmfName].Item2);
                    msg = msg.Substring(0, msg.IndexOf('\0'));
                    Array.Clear(_map[mmfName].Item2, 0, _map[mmfName].Item2.Length);
                    view.WriteArray(0, _map[mmfName].Item2, 0, _map[mmfName].Item2.Length);
                }
            }

            return msg;
        }

        public void SendMessage(string msg)
        {
            byte[] ary = Encoding.Unicode.GetBytes(msg);
            if (ary.Length > mmfSize) throw new ArgumentOutOfRangeException();

            if (!_map.ContainsKey(mmfName))
            {
                ;
                var mp = MemoryMappedFile.OpenExisting(mmfName);
                _map.Add(mmfName, new Tuple<MemoryMappedFile, byte[]>(mp, null));
            }
            //using (var map = MemoryMappedFile.OpenExisting(mmfName))
            using (var view = _map[mmfName].Item1.CreateViewAccessor())
                view.WriteArray(0, ary, 0, ary.Length);
        }

        public void SetBytes(byte[] buf)
        {
            if (buf.Length > mmfSize) throw new ArgumentOutOfRangeException();

            if (!_map.ContainsKey(mmfName))
            {
                ;
                var mp = MemoryMappedFile.OpenExisting(mmfName);
                _map.Add(mmfName, new Tuple<MemoryMappedFile, byte[]>(mp, null));
            }
            //using (var map = MemoryMappedFile.OpenExisting(mmfName))
            using (var view = _map[mmfName].Item1.CreateViewAccessor())
                view.WriteArray(0, buf, 0, buf.Length);
        }
    }
}
