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
using System.IO;
using System.Linq;
using System.Text;
using Sims3.SimIFace;
using S3Launcher;

namespace S3ToolKit.GameFiles.Resources
{
    public class Resource_RIGBone
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scaling { get; set; }
        public string Name { get;  set; }
        public UInt32 NameHash { get; private set; }
        public Resource_RIGBone Parent { get; private set; }
        
        public UInt32 Flags { get; set; }

        private Int32 ParentIndex;
        public Int32 MirrorIndex { get; set; }

        public Resource_RIGBone()
        {

        }

        public void SetParentIndex(Int32 ParentIndex)
        {
            this.ParentIndex = ParentIndex;
        }

        public void Export(BinaryWriter Writer)
        {
            Writer.Write((Single)Position.x); 
            Writer.Write((Single) Position.y);
            Writer.Write((Single) Position.z);
            Writer.Write((Single)Rotation.Vector.x);
            Writer.Write((Single)Rotation.Vector.y);
            Writer.Write((Single)Rotation.Vector.z); 
            Writer.Write((Single)Rotation.Scaler);
            Writer.Write((Single)Scaling.x);
            Writer.Write((Single)Scaling.y);
            Writer.Write((Single)Scaling.z);

            Writer.Write((Int32)Name.Length);
            Writer.Write(Encoding.ASCII.GetBytes(Name));
            Writer.Write((Int32)MirrorIndex);
            Writer.Write((Int32)ParentIndex);            
            Writer.Write((UInt32)FNV.FNV32(Name));
            Writer.Write((UInt32)Flags);
        }

        public void Import(BinaryReader Reader)
        {
            Position = new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
            Rotation = new Quaternion(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
            Scaling = new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());

            UInt32 NameLen = Reader.ReadUInt32();
            Name = Encoding.ASCII.GetString(Reader.ReadBytes((int)NameLen));
            MirrorIndex = Reader.ReadInt32();
            ParentIndex = Reader.ReadInt32();
            NameHash = Reader.ReadUInt32();
            Flags = Reader.ReadUInt32();
        }

        public void SetParent(Resource_RIGBone Parent)
        {
            this.Parent = Parent;
        }

        public Int32 GetParentIndex()
        {
            return ParentIndex;
        }

        public override string ToString()
        {
            return string.Format("{0} P:{1} R:{2} S:{3}", Name, Position, Rotation, Scaling);
        }
    }
}

//    FLOAT[3] Position
//    FLOAT[4] Quaternion Orientation
//    FLOAT[3] Scaling?
//    DWORD Bone Name Length
//    STRING (Bone Name Length ASCII characters)
//    DWORD ???
//    DWORD Parent Index
//    DWORD Bone Name Hash (FNV 32)
//    DWORD Flags? Usually 0x23
