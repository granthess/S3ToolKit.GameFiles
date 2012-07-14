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
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace S3ToolKit.GameFiles.Resources
{
    public class Resource_RIG
    {
        public string Name { get; private set; }
        public List<Resource_RIGBone> Bones { get; private set; }


        public Resource_RIG()
        {
            Bones = new List<Resource_RIGBone>();
        }

        public Resource_RIG(string Name)
            : this()
        {

            this.Name = Name;
        }

        public Resource_RIG(Stream Source)
            : this()
        {
            BinaryReader Reader = new BinaryReader(Source);

            Import(Reader);  // DON'T CLOSE READER!
        }

        public void Export(BinaryWriter Writer)
        {
            Writer.Write((UInt32)4);
            Writer.Write((UInt32)2);

            Writer.Write((Int32)Bones.Count);

            foreach (Resource_RIGBone Bone in Bones)
            {
                Bone.Export(Writer);
            }

            Writer.Write((Int32)Name.Length);
            Writer.Write(Encoding.ASCII.GetBytes(Name));

            Writer.Write((UInt32)0);

        }

        public void Import(BinaryReader Reader)
        {
            Bones.Clear();

            UInt32 VersionA = Reader.ReadUInt32();
            UInt32 VersionB = Reader.ReadUInt32();

            if ((VersionA != 4) | (VersionB != 2))
            {

                if (VersionA > 0x1000)
                {
                    return;
                }
                else
                {

                }
            }


            UInt32 BoneCount = Reader.ReadUInt32();

            for (int i = 0; i < BoneCount; i++)
            {
                Resource_RIGBone Temp = new Resource_RIGBone();
                Temp.Import(Reader);
                Bones.Add(Temp);
            }

            foreach (Resource_RIGBone Entry in Bones)
            {
                if (Entry.GetParentIndex() != -1)
                {
                    Entry.SetParent(Bones[(int)Entry.GetParentIndex()]);
                }
            }

            UInt32 NameLen = Reader.ReadUInt32();
            Name = Encoding.ASCII.GetString(Reader.ReadBytes((int)NameLen));

            UInt32 ChainLength = Reader.ReadUInt32();
        }

        public void SaveToFile(string FileName)
        {
            using (BinaryWriter Writer = new BinaryWriter ( File.OpenWrite(FileName)))
            {
                Export(Writer);
            }
        }

        public void SaveToStream(Stream OutputStream)
        {
            BinaryWriter Writer = new BinaryWriter(OutputStream);
            Export(Writer);  

            // DO NOT CLOSE WRITER!
        }


        private int LookupBoneName(string Name)
        {            
            int i =0;
            foreach (Resource_RIGBone Bone in Bones )
            {
                if (Bone.Name == Name)
                {
                    return i;
                }
                i++;
            }

            return 0;
        }

        public void FixupUnknown()
        {            
            foreach (Resource_RIGBone Bone in Bones)
            {
                string OtherBone;
                if (Bone.Name.Contains("_L_"))
                {
                    OtherBone = Bone.Name.Replace("_L_", "_R_");
                    Bone.MirrorIndex = LookupBoneName(OtherBone);          
                }
                else if (Bone.Name.Contains("_R_"))
                {
                    OtherBone = Bone.Name.Replace("_R_", "_L_");
                    Bone.MirrorIndex = LookupBoneName(OtherBone);
                    Bone.Flags = 0x3f;
                }               
            }
        }
    }
 
}


//DWORD Major Version? 4
//DWORD Minor Version? 2
//DWORD Bone Count
//--repeat(Bone Count)
//    FLOAT[3] Position
//    FLOAT[4] Quaternion Orientation
//    FLOAT[3] Scaling?
//    DWORD Bone Name Length
//    STRING (Bone Name Length ASCII characters)
//    DWORD ???
//    DWORD Parent Index
//    DWORD Bone Name Hash (FNV 32)
//    DWORD Flags? Usually 0x23
//DWORD Skeleton Name Length
//STRING (Skeleton Name Length ASCII characters)
//DWORD IK Chain Count
//--repeat(IK Chain Count)
//    DWORD Type
//    --insert type-specific data