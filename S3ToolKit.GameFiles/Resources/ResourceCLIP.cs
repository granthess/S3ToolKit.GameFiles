using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sims3.SimIFace;

namespace S3ToolKit.GameFiles.Resources
{
    public class ResourceCLIP
    {       

        #region Frames and Events Classes

        public class ClipRuleEntry
        {
            public UInt16 FrameIndex { get; set; }
            public UInt16 SignBits { get; set; }

            public Vector3 Translation { get; set; }
            public Quaternion Rotation { get; set; }
            public Vector3 Scale { get; set; }

            public string ExportText()
            {
                return string.Format("TR:{1}, ROT:{2}", FrameIndex, Translation, Rotation);
            }
        }
              

        public class ClipRule
        {
            public UInt32 RuleDataOffset { get; set; }
            public UInt32 NameHash { get; set; }
            public Single MovementOffset { get; set; }
            public Single MovementScale { get; set; }
            public UInt16 RuleFrameCount { get; set; }
            public UInt16 FrameType { get; set; }

            private UInt32 Size;

            private Dictionary<UInt32, string> HashTable;

            public List<ClipRuleEntry> Entries { get; private set; }

            public ClipRule()
            {
                Entries = new List<ClipRuleEntry>();
                this.HashTable = null;
            }

            public ClipRule(Dictionary<UInt32, string> HashTable)
                : this ()
            {
                this.HashTable = HashTable;
            }

            public void CalculateSize(UInt32 Offset)
            {
                this.Size = Offset - RuleDataOffset;

                float x = (float)Size / (float)RuleFrameCount;


                switch (FrameType)
                {
                    case 0x103: if (x != 10.0) throw new DataMisalignedException(); break;          // 2 WORD elements
                    case 0x10b: if (!float.IsNaN(x)) throw new DataMisalignedException(); break;    // no elements
                    case 0x112: if (x != 8.0) throw new DataMisalignedException(); break;           // 1 DWORD element
                    case 0x20c: if (!float.IsNaN(x)) throw new DataMisalignedException(); break;    // no elements
                    case 0x211: if (!float.IsNaN(x)) throw new DataMisalignedException(); break;    // no elements
                    case 0x214: if (x != 12.0) throw new DataMisalignedException(); break;          // 2 DWORD elements (4 WORD??)
                    case 0x705: if (x != 6.0) throw new DataMisalignedException(); break;           // 1 WORD element
                    case 0x709: if (!float.IsNaN(x)) throw new DataMisalignedException(); break;    // no elements
                    default: throw new NotImplementedException(); 
                }
            }

            public void Import(BinaryReader Reader, List<Single> Floats)
            {
                // Grab Header
                RuleDataOffset = Reader.ReadUInt32();
                NameHash = Reader.ReadUInt32();
                MovementOffset = Reader.ReadSingle();
                MovementScale = Reader.ReadSingle();
                RuleFrameCount = Reader.ReadUInt16();
                FrameType = Reader.ReadUInt16();

                string Bone = string.Format("0x{0:x8}", NameHash);
                if ((HashTable != null) && (HashTable.ContainsKey(NameHash)))
                {
                    Bone = HashTable[NameHash];
                }


                


                switch (FrameType)
                {
                    case 0x103: break;
                    case 0x10b: break;
                    case 0x112: break;
                    case 0x20c: break;
                    case 0x211: break;
                    case 0x214: break;
                    case 0x705: break;
                    case 0x709: break;
                    default: throw new NotImplementedException();
                }

                //if (RuleFrameCount == 0)
                //{
                    if (FrameType == 0x010b)
                    {
                        Console.WriteLine("Data, Frame {0:x4}, Bone {5}, Disp {3}, Scale {4}", FrameType, RuleFrameCount, RuleDataOffset, MovementOffset, MovementScale, Bone);
                    }
                    else if (FrameType == 0x0211)
                    {
                        Console.WriteLine("Data, Frame {0:x4}, Bone {5}, Disp {3}, Scale {4}", FrameType, RuleFrameCount, RuleDataOffset, MovementOffset, MovementScale, Bone);
                    }
                    else if (FrameType == 0x0709)
                    {
                        Console.WriteLine("Data, Frame {0:x4}, Bone {5}, Disp {3}, Scale {4}", FrameType, RuleFrameCount, RuleDataOffset, MovementOffset, MovementScale, Bone);
                    }
                //}

                long OldPosition = Reader.BaseStream.Position;
                Reader.BaseStream.Position = RuleDataOffset;



                // Now grab the data entries
                ClipRuleEntry Entry;
                for (int i = 0; i < RuleFrameCount; i++)
                {
                    
                    Entry = CreateEntry(Reader, FrameType, MovementOffset, MovementScale, Floats);
                    Entries.Add(Entry);

                    if (Entry.SignBits > 0xf)
                    {
                        Console.WriteLine("RULE: {0:x4} {1:x4} {2}", FrameType, Entry.SignBits, Bone);
                    }
                }


                Reader.BaseStream.Position = OldPosition;
            }

            private ClipRuleEntry CreateEntry(BinaryReader Reader, UInt16 FrameType, Single Offset, Single Scale, List<Single> Floats)
            {
                ClipRuleEntry temp = new ClipRuleEntry();
                
                // get the common header info
                temp.FrameIndex = Reader.ReadUInt16();
                temp.SignBits = Reader.ReadUInt16();

                //switch (temp.SignBits)
                //{
                //    case 0x0000: break;
                //    case 0x0001: break;
                //    case 0x0002: break;
                //    case 0x0004: break;
                //    case 0x0008: break;
                //    case 0x0010: break;
                //    case 0x0020: break;
                //    case 0x0040: break;
                //    case 0x0080: break;
                //    default: throw new NotImplementedException();
                //}

                // Console.WriteLine ("Data, 0x{0:x4}, 0x{1:x4}", FrameType, temp.SignBits);
                

                // read the data based on the stubble bubble
                switch (FrameType)
                {
                    case 0x103:
                        {
                            // Read indexed translation
                            Single X = (Floats[Reader.ReadUInt16()] * Scale) + Offset;
                            Single Y = (Floats[Reader.ReadUInt16()] * Scale) + Offset;
                            Single Z = (Floats[Reader.ReadUInt16()] * Scale) + Offset;

                            if ((temp.SignBits & 0x01) == 0x01)
                                X = -X;

                            if ((temp.SignBits & 0x02) == 0x02)
                                Y = -Y;

                            if ((temp.SignBits & 0x04) == 0x04)
                                Z = -Z;

                            if (temp.SignBits > 0x07)
                            {
                                throw new NotImplementedException();
                            }

                            temp.Translation = new Vector3(X, Y, Z);

                            //Console.WriteLine("I_TRAN {0} {1:x4} -- {2}", temp.FrameIndex, temp.SignBits, temp.Translation);
                        } break;
                    case 0x10b: throw new NotImplementedException();
                    case 0x112:
                        {
                            // Read Translation 10 bits at a time.
                            UInt32 TTemp = Reader.ReadUInt32();
                            Single X = (((TTemp & 0x3ff) >> 0) / 1023.0f * Scale) + Offset;
                            Single Y = (((TTemp & (0x3ff << 10)) >> 10) / 1023.0f * Scale) + Offset;
                            Single Z = (((TTemp & (0x3ff << 20)) >> 20) / 1023.0f * Scale) + Offset;

                            if ((temp.SignBits & 0x01) == 0x01)
                                X = -X;

                            if ((temp.SignBits & 0x02) == 0x02)
                                Y = -Y;

                            if ((temp.SignBits & 0x04) == 0x04)
                                Z = -Z;

                            if (temp.SignBits > 0x07)
                            {
                                //throw new NotImplementedException();
                            }

                            temp.Translation = new Vector3(X, Y, Z);
                            //Console.WriteLine("P_TRAN {0} {1:x4} -- {2}", temp.FrameIndex, temp.SignBits, temp.Translation);
                        } break;
                    case 0x20c: throw new NotImplementedException();
                    case 0x211: throw new NotImplementedException();
                    case 0x214:
                        {
                            // Read xyzw 12 bits at a time...
                            Single X = ((Reader.ReadUInt16() & 0x0fff) / 4095.0f * Scale) + Offset;
                            Single Y = ((Reader.ReadUInt16() & 0x0fff) / 4095.0f * Scale) + Offset;
                            Single Z = ((Reader.ReadUInt16() & 0x0fff) / 4095.0f * Scale) + Offset;
                            Single W = ((Reader.ReadUInt16() & 0x0fff) / 4095.0f * Scale) + Offset;

                            if ((temp.SignBits & 0x01) == 0x01)
                                X = -X;

                            if ((temp.SignBits & 0x02) == 0x02)
                                Y = -Y;

                            if ((temp.SignBits & 0x04) == 0x04)
                                Z = -Z;

                            if ((temp.SignBits & 0x08) == 0x08)
                                W = -W;

                            if (temp.SignBits > 0x0f)
                            {
                                //throw new NotImplementedException();
                            }

                            temp.Rotation = new Quaternion(X,Y,Z,W);
                            //Console.WriteLine("P_ROT  {0} {1:x4} -- {2}", temp.FrameIndex, temp.SignBits, temp.Rotation);
                        } break;
                    case 0x705:
                        {
                            // Unknownish valueses 
                            byte Value1 = Reader.ReadByte();
                            byte Value2 = Reader.ReadByte();
                            //Console.WriteLine("UNKNWN {0} {1:x4} -- {2:x2} {3:x2}", temp.FrameIndex, temp.SignBits, Value1,Value2);
                        } break;
                    case 0x709: throw new NotImplementedException();
                    default: throw new NotImplementedException();
                }



                return temp;
            }

            public override string ToString()
            {
                return string.Format ("Joint Rule -- Bone {0} ", NameHash) +
                string.Format("Offset {0}, Scale {1} ", MovementOffset, MovementScale)+
                string.Format("Frame type {0:x} for {1} frames", LookupFrameType(FrameType), RuleFrameCount);
                
            }

            private string LookupFrameType(UInt16 FrameType)
            {
                switch (FrameType)
                {
                    case 0x103: return "Ind-T";
                    case 0x10b: return "Nul-T";
                    case 0x112: return "Pak-T";
                    case 0x20c: return "Nul-R";
                    case 0x211: return "Ind-R";
                    case 0x214: return "Pak-R";
                    default: return string.Format("UNK-? 0x{0:x4}", FrameType);
                }
            }
        }

        #region Events Classes
        public class ClipEvent
        {
            public UInt32 EventType { get; private set; }
            public UInt32 ID { get; private set;}
            public Single TimeCode { get; private set; }
            public Single UnknownA { get; private set; }
            public Single UnknownB { get; private set; }
            public UInt32 UnknownC { get; private set; }
            public string EventName { get; private set; }

            public int FrameNumber { get { return (int)(TimeCode * 30.0); } }

            internal ClipEvent (UInt32 EventType)
            {
                this.EventType = EventType;
            }

            public virtual void Import(BinaryReader Reader)
            {
                UInt16 Sig = Reader.ReadUInt16();
                if (Sig != 0xc1e4)
                    throw new InvalidDataException();

                ID = Reader.ReadUInt32();
                TimeCode = Reader.ReadSingle();
                
                UnknownA = Reader.ReadSingle();
                UnknownB = Reader.ReadSingle();
                UnknownC = Reader.ReadUInt32();

                int ceNameLength = Reader.ReadInt32() + 1;  //+1 for the null                                
                EventName = ReadNullASCIIString(Reader, ceNameLength);

                while ((Reader.BaseStream.Position % 4) != 0)
                {
                    Reader.ReadByte();
                }
            }

            public virtual void ImportText(string Value)
            {
            }

            public virtual string ExportText()
            {
                return string.Format("ID:0x:{0:x4}, FR:{1}, UA:{2}, UB:{3}, UC:0x{4:x4}, NAME:{5}", ID, FrameNumber, UnknownA, UnknownB, UnknownC, EventName);
            }


            public override string ToString()
            {
                return string.Format("Event {0}={1} at {2}",ID,EventName,FrameNumber);
            }

        }

        public class ClipEventAttach : ClipEvent
        {
            public UInt32 PropHash { get; set; }
            public UInt32 ObjectHash { get; set; }
            public UInt32 SlotHash { get; set; }
            public UInt32 Unknown { get; set; }
            Matrix44 Matrix { get; set; }

            public ClipEventAttach()    
                : base(1)           
            {
                
            }

            public override void Import(BinaryReader Reader)
            {
                base.Import(Reader);
                
                PropHash = Reader.ReadUInt32();
                ObjectHash = Reader.ReadUInt32();
                SlotHash = Reader.ReadUInt32();
                Unknown = Reader.ReadUInt32();

                Matrix = new Matrix44(Reader);
            }

            public override string ToString()
            {
                return string.Format ("Attach Object {0}",base.ToString());
            }

            public override string ExportText()
            {
                return "EV1, " + base.ExportText() + string.Format(", prop:0x{0:x4}, obj:0x{1:x4}, slot:0x{2:x4}, U1:0x{3:x4}", PropHash, ObjectHash, SlotHash, Unknown);
            }
        }

        public class ClipEventUnParent : ClipEvent
        {
            public UInt32 ObjectHash { get; set; }

            public ClipEventUnParent() 
                :base(2)                
            {
             
            }

            public override void Import(BinaryReader Reader)
            {
                base.Import(Reader);
                ObjectHash = Reader.ReadUInt32();
            }

            public override string ToString()
            {
                return string.Format("Un-Parent {0}", base.ToString());
            }

            public override string ExportText()
            {
                return "EV2, " + base.ExportText() + string.Format(", obj:0x{0:x4}", ObjectHash);
            }
        }

        public class ClipEventPlaySound : ClipEvent
        {
            public string SoundName { get; set; }
            public ClipEventPlaySound()
                : base (3)
             
            {

            }

            public override void Import(BinaryReader Reader)
            {
                base.Import(Reader);
                SoundName = ReadNullASCIIString(Reader, 128);
            }

            public override string ToString()
            {
                return string.Format("Play Sound {0} - {1}", base.ToString(),SoundName);
            }

            public override string ExportText()
            {
                return "EV3, " + base.ExportText() + string.Format(", SND:{0}", SoundName);
            }
        }

        public class ClipEventSACS : ClipEvent
        {

            public ClipEventSACS()
                : base (4)
                
            {

            }

            public override void Import(BinaryReader Reader)
            {
                base.Import(Reader);
            }

            public override string ToString()
            {
                return string.Format("SACS Script {0}", base.ToString());
            }

            public override string ExportText()
            {
                return "EV4, " + base.ExportText();
            }
        }

        public class ClipEventPlayEffect : ClipEvent
        {
            public UInt32 Unknown1 { get; set; }
            public UInt32 Unknown2 { get; set; }
            public UInt32 EffectHash { get; set; }
            public UInt32 ActorHash { get; set; }
            public UInt32 SlotHash { get; set; }
            public UInt32 Unknown3 { get; set; }

            public ClipEventPlayEffect( )
                : base(5)
            {

            }

            public override void Import(BinaryReader Reader)
            {
                base.Import(Reader);

                Unknown1 = Reader.ReadUInt32();
                Unknown2 = Reader.ReadUInt32();
                EffectHash  = Reader.ReadUInt32();
                ActorHash = Reader.ReadUInt32();
                SlotHash = Reader.ReadUInt32();
                Unknown3 = Reader.ReadUInt32();                
            }

            public override string ToString()
            {
                return string.Format("Play Effect {0}", base.ToString());
            }

            public override string ExportText()
            {
                return "EV5, " + base.ExportText() + string.Format(", U1:{0}, U2:0x{1:x4}, Effect:0x{2:x4}, Actor:0x{3:x4}, Slot:0x{4:x4}, U3:0x{5:x4}",
                    Unknown1, Unknown2, EffectHash, ActorHash, SlotHash, Unknown3);
            }
        }

        public class ClipEventVisibility : ClipEvent
        {
            public Single Visibility { get; set; }

            public ClipEventVisibility()
                : base(6)
            {
            }

            public override void Import(BinaryReader Reader)
            {
                base.Import(Reader);

                Visibility = Reader.ReadSingle();
            }
            
            public override string ToString()
            {
                return string.Format("Set Visibility {0} -- {1}", base.ToString(), Visibility);
            }

            public override string ExportText()
            {
                return "EV6, " + base.ExportText() + string.Format(", VIS:{0}", Visibility);
            }
        }

        public class ClipEventDestroyProp : ClipEvent
        {
            public UInt32 ActorHash { get; set; }

            public ClipEventDestroyProp( )
                : base(9)
            {
            }

            public override void Import(BinaryReader Reader)
            {
                base.Import(Reader);

                ActorHash = Reader.ReadUInt32();
            }

            public override string ToString()
            {
                return string.Format("Destroy Prop {0}", base.ToString());
            }

            public override string ExportText()
            {
                return "EV9, " + base.ExportText() + string.Format(", Actor:0x{0:x4}", ActorHash);
            }
        }

        public class ClipEventStopEffect : ClipEvent
        {
            public UInt32 EffectHash { get; set; }
            public UInt32 Unknown1 { get; set; }

            public ClipEventStopEffect( )
                : base(10)
            {
            }

            public override void Import(BinaryReader Reader)
            {
                base.Import(Reader);

                EffectHash = Reader.ReadUInt32();
                Unknown1 = Reader.ReadUInt32();
            }

            public override string ToString()
            {
                return string.Format("Stop Effect {0}", base.ToString());
            }

            public override string ExportText()
            {
                return "EV10, " + base.ExportText() + string.Format(", Effect:0x{0:x4}, U1:0x{1:x4}", EffectHash, Unknown1);
            }
        }

        #endregion

        #endregion


        public string FileName { get; private set; }
        public string Name { get; private set; }

        public Single[] EndData { get; private set; }
        public UInt32 Unknown1 { get; private set; }
        public UInt32 Unknown2 { get; private set; }
        public string ActorName { get; private set; }

        public List<Tuple<UInt32, string, string>> SlotTable { get; private set; }

        public ClipEvent[] ClipTable { get; private set; }
        public ClipRule[] Rules { get; private set; }

        //public Dictionary<UInt16, ClipFrame> Frames { get; private set; }

        private Dictionary<UInt32, string> HashTable;

        public ResourceCLIP(Stream Source, Dictionary<UInt32, string> Table = null)
        {
            HashTable = Table;
            if (HashTable == null)
                HashTable = new Dictionary<uint, string>();

            //Frames = new Dictionary<ushort, ClipFrame>();
            FloatList = new List<float>();
            SlotTable = new List<Tuple<uint, string, string>>();
            Import(Source);
        }
        



        private void Import(Stream Source)
        {

            // Format from http://simswiki.info/wiki.php?title=Sims_3:0x6B20C4F3
            // Read Main Header


            //Stream F = File.Create(@"C:\temp\clips\test.clip");
            //Source.CopyTo(F);
            //F.Close();

            //Source.Position = 0;


            BinaryReader Reader = new BinaryReader(Source);

            UInt32 TID = Reader.ReadUInt32();
            if (TID != 0x6b20c4f3)
                throw new InvalidDataException();

            UInt32 Offset = Reader.ReadUInt32();
            UInt32 ClipSize = Reader.ReadUInt32();
            UInt32 ClipOffset = (UInt32)Reader.BaseStream.Position + Reader.ReadUInt32();
            UInt32 SlotOffset = (UInt32)Reader.BaseStream.Position + Reader.ReadUInt32();
            UInt32 ActorOffset = (UInt32)Reader.BaseStream.Position + Reader.ReadUInt32();
            UInt32 EventOffset = (UInt32)Reader.BaseStream.Position + Reader.ReadUInt32();

            Unknown1 = Reader.ReadUInt32();
            Unknown2 = Reader.ReadUInt32();

            UInt32 EndOffset = Reader.ReadUInt32();
            byte[] Buffer = Reader.ReadBytes(16);

            // get the end buffer values
            Reader.BaseStream.Position = EndOffset;
            EndData = new Single[4];
            EndData[0] = Reader.ReadSingle();
            EndData[1] = Reader.ReadSingle();
            EndData[2] = Reader.ReadSingle();
            EndData[3] = Reader.ReadSingle();

            // Get Clip
            Stream ClipStream = new SubStream(Source, ClipOffset, ClipSize);
            ImportClip(ClipStream);

            // Get Slot Table
            Stream SlotStream = new SubStream(Source, SlotOffset, ActorOffset - SlotOffset);
            ImportSlotTable(SlotStream);


            // Get Actor Name            
            Reader.BaseStream.Position = ActorOffset;
            

            int actorlen = (int)(EventOffset - ActorOffset);
            ActorName = ReadNullASCIIString(Reader, actorlen);

            // Get Event Table       
            Reader.BaseStream.Position = EventOffset;
            string ceSIG = Encoding.ASCII.GetString(Reader.ReadBytes(4));
            if (ceSIG != "=CE=")
                throw new InvalidDataException();

            UInt32 ceVersion = Reader.ReadUInt32();
            if (ceVersion != 0x0103)
                throw new InvalidDataException();

            UInt32 ceCount = Reader.ReadUInt32();
            UInt32 ceSize = Reader.ReadUInt32();
            UInt32 ceOffset = Reader.ReadUInt32();


            ClipTable = new ClipEvent[ceCount];

            for (int ceI = 0; ceI < ceCount; ceI++)
            {
                UInt16 ceType = Reader.ReadUInt16();

                ClipEvent Event;

                switch (ceType)
                {
                    case 1: Event = new ClipEventAttach(); break;
                    case 2: Event = new ClipEventUnParent(); break;
                    case 3: Event = new ClipEventPlaySound(); break;
                    case 4: Event = new ClipEventSACS(); break;
                    case 5: Event = new ClipEventPlayEffect(); break;
                    case 6: Event = new ClipEventVisibility(); break;
                    case 9: Event = new ClipEventDestroyProp(); break;
                    case 10: Event = new ClipEventStopEffect(); break;
                    default: throw new InvalidDataException();
                }

                ClipTable[ceI] = Event;

                Event.Import(Reader);               
                //Console.WriteLine(Event.ToString());                
            }


            Source.Close();
        }

        public List<Single> FloatList { get; private set; }
   
        private void ImportClip(Stream Source)
        {
            BinaryReader Reader = new BinaryReader(Source);

            string Sig = Encoding.ASCII.GetString(Reader.ReadBytes(8));
            if (Sig != "_pilC3S_")
            {
                throw new InvalidDataException();
            }

            UInt32 Version = Reader.ReadUInt32();
            if (Version != 2)
                throw new InvalidDataException();

            UInt32 Unknown1 = Reader.ReadUInt32();

            Single FrameDuration = Reader.ReadSingle();
            UInt16 FrameCount = Reader.ReadUInt16();
            UInt16 Unknown2 = Reader.ReadUInt16();

            UInt32 JointRuleCount = Reader.ReadUInt32();
            UInt32 FloatCount = Reader.ReadUInt32();

            UInt32 JointRuleOffset = Reader.ReadUInt32();
            UInt32 FrameDataOffset = Reader.ReadUInt32();

            UInt32 NameOffset = Reader.ReadUInt32();
            UInt32 FileNameOffset = Reader.ReadUInt32();

            // read the Name and FileName
            Reader.BaseStream.Position = NameOffset;
            Name = ReadNullASCIIString(Reader, (int)(FileNameOffset - NameOffset + 1));
            Reader.BaseStream.Position = FileNameOffset;
            FileName = ReadNullASCIIString(Reader, (int)(FrameDataOffset - FileNameOffset + 1));

            //Console.Error.WriteLine("_S3Clip_  {0} from {1} ", Name, FileName);

            // Read the Indexed Floats (if any)
            Reader.BaseStream.Position = FrameDataOffset;
            for (int i = 0; i < FloatCount; i++)
            {
                FloatList.Add(Reader.ReadSingle());
            }


            // Read the joint rules
            Reader.BaseStream.Position = JointRuleOffset;

            Rules = new ClipRule[JointRuleCount];

            for (int i = 0; i < JointRuleCount; i++)
            {
                Rules[i] = new ClipRule(HashTable);
                Rules[i].Import(Reader, FloatList);

                if (i > 0)
                {
                    Rules[i - 1].CalculateSize(Rules[i].RuleDataOffset);
                }

                //Console.WriteLine(Rules[i].ToString());

                //UInt32 RuleDataOffset = Reader.ReadUInt32();
                //UInt32 NameHash = Reader.ReadUInt32();
                //Single MovementOffset = Reader.ReadSingle();
                //Single MovementScale = Reader.ReadSingle();
                //UInt16 RuleFrameCount = Reader.ReadUInt16();
                //UInt16 FrameType = Reader.ReadUInt16();

                //Console.Write ("Joint Rule -- Bone {0} ",NameHash);
                //Console.Write ("Offset {0}, Scale {1} ", MovementOffset, MovementScale);
                //Console.WriteLine("Frame type {0:x} for {1} frames", LookupFrameType(FrameType), RuleFrameCount);

                //Console.WriteLine("LOC: 0x{0:x}", RuleDataOffset);
            }
        }




        private void ImportSlotTable(Stream Source)
        {
            BinaryReader Reader = new BinaryReader(Source);

            UInt32 PrimaryCount = Reader.ReadUInt32();
            
            UInt32[] PrimaryOffsets = new UInt32[PrimaryCount];
            UInt32 PrimaryBase = (UInt32) Reader.BaseStream.Position;
            for (int i = 0; i < PrimaryCount; i++)
            {
                PrimaryOffsets[i] = Reader.ReadUInt32() + PrimaryBase;
            }

            foreach (UInt32 Offset in PrimaryOffsets)
            {
                Reader.BaseStream.Position = Offset;
                UInt32 Pad = Reader.ReadUInt32();
                UInt32 SecondaryCount = Reader.ReadUInt32();

                UInt32[] SecondaryOffsets = new UInt32[SecondaryCount];
                UInt32 SecondaryBase = (UInt32)Reader.BaseStream.Position;
                for (int i = 0; i < SecondaryCount; i++)
                {
                    SecondaryOffsets[i] = Reader.ReadUInt32() + SecondaryBase;
                }

                foreach (UInt32 Secondary in SecondaryOffsets)
                {
                    Reader.BaseStream.Position = Secondary;
                    UInt32 Index = Reader.ReadUInt32();
                    string Actor = ReadNullASCIIString(Reader, 512);
                    string Slot = ReadNullASCIIString(Reader, 512);

                    SlotTable.Add(new Tuple<uint, string, string>(Index, Actor, Slot));
                }
            }

        }

        private static string ReadNullASCIIString(BinaryReader Reader, int namelen)
        {
            byte[] namebuf = Reader.ReadBytes(namelen);
            StringBuilder Namebuilder = new StringBuilder(namelen);

            foreach (byte b in namebuf)
            {
                if ((b == 0x00) | (b == 0x7e))
                    break;
                Namebuilder.Append((char)b);
            }

            return Namebuilder.ToString();
        }

        public void ExportToBlender(string OutputName, Dictionary<UInt32,string> HashTable = null)
        {
            // Generate a separate file for each set of data.
            
            // The main header file == filename  + ".BA"
            ExportMainToBlender(Path.ChangeExtension(OutputName, ".BA"));

            // The Event table file == filename  + ".BA0"
            ExportEventTableToBlender(Path.ChangeExtension(OutputName, ".BA0"));
            
            // The clip data   file == filename  + ".BA2"
            ExportClipToBlender(Path.ChangeExtension(OutputName, ".BA1") ,HashTable);


        }

        private void ExportMainToBlender(string OutputName)
        {
            List<string> Output = new List<string>();

            // we only need to save things we can't generate ourselves...            
            //
            // Values Unknown1, Unknown2, Actor Name and the 4x single end values

            Output.Add("#CLIP Import/Export Master File");
            Output.Add("#The only user editable value in this file is the ActorName");
            Output.Add("#And possibly the slot table");
            Output.Add(string.Format ("Name={0}", Name));
            Output.Add(string.Format("FileName={0}", FileName));
            Output.Add(string.Format("ActorName={0}", ActorName));
            Output.Add(string.Format("Unknown1={0}", Unknown1));
            Output.Add(string.Format("Unknown2={0}", Unknown2));
            Output.Add(string.Format("End0={0} {1} {2} {3}", EndData[0], EndData[1], EndData[2], EndData[3]));
            Output.Add(string.Empty);

            Output.Add("#CLIP Import/Export Slot Table File");
            foreach (var Entry in SlotTable)
            {
                Output.Add(string.Format("{0}:{1}={2}", Entry.Item1, Entry.Item2, Entry.Item3));
            }


            // Finally write the file out
            File.WriteAllLines(OutputName, Output);
        }

        private void ExportEventTableToBlender(string OutputName)
        {
            List<string> Output = new List<string>();

            // we only need to save things we can't generate ourselves...            
            //
            // The Slot Table
            Output.Add("#CLIP Import/Export Event Table File");

            var SortedClipTable = from Item in ClipTable
                                  orderby Item.FrameNumber
                                  select Item;


            foreach (var Event in SortedClipTable)
            {
                Output.Add(Event.ExportText());
            }

            // Finally write the file out
            File.WriteAllLines(OutputName, Output);
        }

        private void ExportClipToBlender(string OutputName, Dictionary<UInt32, string> HashTable = null)
        {
            List<string> Output = new List<string>();

            // we only need to save things we can't generate ourselves...            
            //
            // The Clip itself
            Output.Add("#CLIP Import/Export Animation Clip File");

            List<Tuple<UInt16, string, string>> SortList = new List<Tuple<ushort, string, string>>();

            foreach (var Rule in Rules)
            {
                // Get the bone name if we have it...
                string Name = string.Format("0x{0:x4}", Rule.NameHash);
                if (HashTable != null)
                {
                    if (HashTable.ContainsKey(Rule.NameHash))
                    {
                        Name = HashTable[Rule.NameHash];
                    }

                }

                // Output.Add (string.Format ("Bone: {0} FT:0x{1:x4}, NUM:{2}",Name, Rule.FrameType, Rule.RuleFrameCount));

                

                foreach (var Entry in Rule.Entries)
                {
                    SortList.Add (new Tuple<ushort,string,string>(Entry.FrameIndex,Name, string.Format ("FR:{0}, BONE:{1}, {2}",Entry.FrameIndex, Name, Entry.ExportText())));
                }


            }


            var bob = from Item in SortList
                      orderby Item.Item1
                      select Item.Item1;

            var count2 = bob.Distinct().Count();
                      


            var SortList2 = from Item in SortList
                            orderby Item.Item1, Item.Item2
                            select Item.Item3;

            int Count = SortList2.Count();

            foreach (string Entry in SortList2)
            {
                Output.Add(Entry);
            }

            // Finally write the file out
            File.WriteAllLines(OutputName, Output);
        }
    }
}
