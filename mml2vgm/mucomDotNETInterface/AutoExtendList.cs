using System.Collections.Generic;

namespace mucomDotNET.Interface
{
    public class AutoExtendList<T>
    {
        private List<T> buf;

        public int Count
        {
            get
            {
                return buf == null ? 0 : buf.Count;
            }
        }

        public AutoExtendList()
        {
            buf = new List<T>();
        }

        public void Set(int adr, T d)
        {
            if (adr >= buf.Count)
            {
                int size = adr + 1;
                for (int i = buf.Count; i < size; i++)
                    buf.Add(default(T));
            }
            buf[adr] = d;
        }

        public T Get(int adr)
        {
            if (adr >= buf.Count) return default(T);
            return buf[adr];
        }

        public T[] GetByteArray()
        {
            return buf.ToArray();
        }
    }
}
