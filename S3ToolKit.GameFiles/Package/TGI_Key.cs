using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S3ToolKit.GameFiles.Package
{
    /// <summary>
    /// Encapsulates a TGI Value for Package Resources
    /// </summary>
    /// <remarks>
    /// Implements IComparable %ltTGI_Key%gt and IComparable%ltstring%gt
    /// </remarks>
    public class TGI_Key : IComparable<TGI_Key> , IComparable<string>
    {
        public UInt32 Type { get; private set; }
        public UInt32 Group { get; private set; }
        public UInt64 Instance { get; private set; }

        #region Constructor
        public TGI_Key(UInt32 Type, UInt32 Group, UInt64 Instance)
        {
            this.Type = Type;
            this.Group = Group;
            this.Instance = Instance;
        }

        public TGI_Key(string Keyvalue)
        {
            if (Keyvalue == "")
                return;

            if (Keyvalue.ToLower().StartsWith("key:"))
            {
                Keyvalue = Keyvalue.Substring(4);
            }


            string T = Keyvalue.Substring(0, 8);
            string G = Keyvalue.Substring(9, 8);
            string I = Keyvalue.Substring(18);

            string I1 = I.Substring(0, 8);
            string I2 = I.Substring(8);

            this.Type = UInt32.Parse(T, System.Globalization.NumberStyles.HexNumber);
            this.Group = UInt32.Parse(G, System.Globalization.NumberStyles.HexNumber);

            this.Instance = UInt64.Parse(I1, System.Globalization.NumberStyles.HexNumber) << 32|
                            UInt64.Parse(I2, System.Globalization.NumberStyles.HexNumber);

            //this.Instance = UInt64.Parse(I, System.Globalization.NumberStyles.HexNumber);
        }
        #endregion

        #region Equality, IComparable
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is TGI_Key)
            {
                if ((obj as TGI_Key).Type != Type)
                {
                    return false;
                }
                if ((obj as TGI_Key).Group != Group)
                {
                    return false;
                }
                if ((obj as TGI_Key).Instance != Instance)
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

        public static bool operator ==(TGI_Key a, TGI_Key b)
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
            return a.Type == b.Type && a.Group == b.Group && a.Instance == b.Instance;
        }

        public static bool operator !=(TGI_Key a, TGI_Key b)
        {
            return !(a == b);  // Uses Our == operator above
        }

        public override string ToString()
        {
            return string.Format("{0:x8}-{1:x8}-{2:x16}", Type, Group, Instance);
        }

        public Int32 CompareTo(string B)
        {
            return ToString().CompareTo(B);
        }

        public Int32 CompareTo(TGI_Key B)
        {
            Int32 Result = Type.CompareTo(B.Type);

            if (Result != 0)
            {
                return Result;
            }

            Result = Group.CompareTo(B.Group);

            if (Result != 0)
            {
                return Result;
            }

            return Instance.CompareTo(B.Instance);
        }
        #endregion
    }
}
