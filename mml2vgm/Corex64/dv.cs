using System;

namespace Corex64
{
    public interface IDv
    {
        bool eq();
        void rst();
    }

    public class Dbool : IDv
    {
        public bool eq()
        {
            return s == v;
        }

        public void rst()
        {
            s = v;
        }

        public bool? v;
        private bool? s;
    }

    public class Dint : IDv
    {
        public Dint(int val)
        {
            this.val = val;
            s = null;
        }

        public bool eq()
        {
            return s == val;
        }

        public void rst()
        {
            s = val;
        }

        public int? val;
        private int? s;

        public static implicit operator Dint(int v)
        {
            throw new NotImplementedException();
        }

    }

    public class Dlong : IDv
    {
        public bool eq()
        {
            return s == v;
        }

        public void rst()
        {
            s = v;
        }

        public long? v;
        private long? s;
    }

    public class Dfloat : IDv
    {
        public bool eq()
        {
            return s == v;
        }

        public void rst()
        {
            s = v;
        }

        public float? v;
        private float? s;
    }

    public class Dbyte : IDv
    {
        public bool eq()
        {
            return s == v;
        }

        public void rst()
        {
            s = v;
        }

        public byte? v;
        private byte? s;
    }

    public class Dchar : IDv
    {
        public bool eq()
        {
            return s == v;
        }

        public void rst()
        {
            s = v;
        }

        public char? v;
        private char? s;
    }

    public class Ddouble : IDv
    {
        public bool eq()
        {
            return s == v;
        }

        public void rst()
        {
            s = v;
        }

        public double? v;
        private double? s;
    }

}
