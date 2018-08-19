using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public interface IPoint4D : IPoint3D
    {
        int M { get; }
    }

    [Parsable]
    public struct Point4D : IPoint4D, IComparable, IComparable<Point4D>
    {
        internal int m_X;
        internal int m_Y;
        internal int m_Z;
        internal int m_M;

        public static readonly Point4D Zero = new Point4D(0, 0, 0, 0);

        public Point4D(int x, int y, int z, int m)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
            m_M = m;
        }

        public Point4D(IPoint4D p)
            : this(p.X, p.Y, p.Z, p.M)
        { }

        public Point4D(IPoint3D p, int m)
            : this(p.X, p.Y, p.Z, m)
        { }

        public Point4D(IPoint2D p, int z, int m)
            : this(p.X, p.Y, z, m)
        { }

        [CommandProperty(AccessLevel.Counselor)]
        public int X { get { return m_X; } set { m_X = value; } }

        [CommandProperty(AccessLevel.Counselor)]
        public int Y { get { return m_Y; } set { m_Y = value; } }

        [CommandProperty(AccessLevel.Counselor)]
        public int Z { get { return m_Z; } set { m_Z = value; } }

        [CommandProperty(AccessLevel.Counselor)]
        public int M { get { return m_M; } set { m_M = value; } }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", m_X, m_Y, m_Z, m_M);
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is IPoint4D))
            {
                return false;
            }

            IPoint4D p = (IPoint4D)o;

            return m_X == p.X && m_Y == p.Y && m_Z == p.Z && m_M == p.M;
        }

        public override int GetHashCode()
        {
            return m_X ^ m_Y ^ m_Z ^ m_M;
        }

        public static Point4D Parse(string value)
        {
            int start = value.IndexOf('(');
            int end = value.IndexOf(',', start + 1);

            string param1 = value.Substring(start + 1, end - (start + 1)).Trim();

            start = end;
            end = value.IndexOf(',', start + 1);

            string param2 = value.Substring(start + 1, end - (start + 1)).Trim();

            start = end;
            end = value.IndexOf(')', start + 1);

            string param3 = value.Substring(start + 1, end - (start + 1)).Trim();

            start = end;
            end = value.IndexOf(')', start + 1);

            string param4 = value.Substring(start + 1, end - (start + 1)).Trim();

            return new Point4D(Convert.ToInt32(param1), Convert.ToInt32(param2), Convert.ToInt32(param3), Convert.ToInt32(param4));
        }

        public static bool operator ==(Point4D l, Point4D r)
        {
            return l.m_X == r.m_X && l.m_Y == r.m_Y && l.m_Z == r.m_Z && l.m_M == r.m_M;
        }

        public static bool operator !=(Point4D l, Point4D r)
        {
            return l.m_X != r.m_X || l.m_Y != r.m_Y || l.m_Z != r.m_Z || l.m_M != r.m_M;
        }

        public static bool operator ==(Point4D l, IPoint4D r)
        {
            if (ReferenceEquals(r, null))
            {
                return false;
            }

            return l.m_X == r.X && l.m_Y == r.Y && l.m_Z == r.Z && l.m_M == r.M;
        }

        public static bool operator !=(Point4D l, IPoint4D r)
        {
            if (ReferenceEquals(r, null))
            {
                return false;
            }

            return l.m_X != r.X || l.m_Y != r.Y || l.m_Z != r.Z || l.m_M != r.M;
        }

        public int CompareTo(Point4D other)
        {
            int v = (m_X.CompareTo(other.m_X));

            if (v == 0)
            {
                v = (m_Y.CompareTo(other.m_Y));

                if (v == 0)
                {
                    v = (m_Z.CompareTo(other.m_Z));

                    if (v == 0)
                    {
                        v = (m_M.CompareTo(other.m_M));
                    }
                }
            }

            return v;
        }

        public int CompareTo(object other)
        {
            if (other is Point4D)
            {
                return CompareTo((Point4D)other);
            }
            else if (other == null)
            {
                return -1;
            }

            throw new ArgumentException();
        }
    }
}
