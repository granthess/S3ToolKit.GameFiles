/*
    Copyright 2012, Grant Hess

    This file is part of S3ToolKit.GameFiles.

    S3ToolKit.Utils is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with CC Magic.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S3ToolKit.GameFiles
{
    public class TS3GUID : IComparable<TS3GUID>, IComparable<string>
    {
        public byte[] Value { get; private set; }


        public TS3GUID(byte[] Value)
        {
            this.Value = Value;
        }

        public TS3GUID(byte[] Left, byte[] right)
        {
            Left.CopyTo(Value, 8);
            right.CopyTo(Value, 0);
        }

        public TS3GUID(string Value)
        {

        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(48);

            for (int i = 8; i < 16; i++)
            {
                sb.Append(string.Format("{0:x}", Value[i]));
            }

            for (int i = 0; i < 8; i++)
            {
                sb.Append(string.Format("{0:x}", Value[i]));
            }

            return sb.ToString();
        }



        #region Equality, IComparable
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is TS3GUID)
            {
                if ((obj as TS3GUID).Value != Value)
                {
                    return false;
                }                
                return true;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(TS3GUID a, TS3GUID b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Value == b.Value;
        }

        public static bool operator !=(TS3GUID a, TS3GUID b)
        {
            return !(a == b);  // Uses Our == operator above
        }

         public Int32 CompareTo(string B)
        {
            return ToString().CompareTo(B);
        }

        public Int32 CompareTo(TS3GUID B)
        {
            return ToString().CompareTo(B.ToString());
        }
        #endregion

    }
}
