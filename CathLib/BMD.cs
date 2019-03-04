using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CathLib
{
    public class BMD {
        const ushort EndFlag = 0xD821;
        const ushort NxtFlag = 0xD841;
        const ushort BrkFlag = 0x0000;
        const ushort SpcFlag = 0xFFE3;
        bool SpaceScape = false;


        byte[] Script;
        public BMD(byte[] Script) {
            this.Script = Script;
        }

        const uint HeaderLen = 0x1C;
        public virtual string[] Import() {
            MSGEntry[] Entries = Open();

            return Entries.SelectMany(x => x.Strings).ToArray();
        }

        public virtual MSGEntry[] Open() {
            if (Script.Length == 0)
                return new MSGEntry[0];

            StructReader Reader = new StructReader(new MemoryStream(Script), true, Encoding.ASCII);
            BMDStruct Info = new BMDStruct();
            Reader.ReadStruct(ref Info);

            long FirstOffset = long.MaxValue;
            long Pos = Reader.BaseStream.Position;
            List<MSGEntry> Entries = new List<MSGEntry>();
            while (Reader.BaseStream.Position < FirstOffset) {
                uint Offset = Tools.Reverse(Reader.ReadUInt32()) + HeaderLen;
                Pos = Reader.BaseStream.Position;
                Reader.BaseStream.Position = Offset;
                if (Offset != HeaderLen) {
                    if (FirstOffset == long.MaxValue)
                        FirstOffset = Offset;

                    MSGEntry Entry = new MSGEntry();
                    Reader.ReadStruct(ref Entry);
                    Entry.ID = Entries.LongCount();
                    Entry.Strings = new string[Entry.Count];
                    Entries.Add(Entry);
                }
                Reader.BaseStream.Position = Pos;
            }
            
            for (int i = 0; i < Entries.Count; i++) {
                for (int x = 0; x < Entries[i].Count; x++) {
                    Reader.BaseStream.Position = (Entries[i].StrOffset[x] + HeaderLen) - 2;
                    long NextOffset = x + 1 < Entries[i].Count ? ((Entries[i].StrOffset[x + 1] + HeaderLen) - 2) : Reader.BaseStream.Length - 2;


                    string Result = string.Empty;
                    while (Reader.BaseStream.Position < NextOffset) {
                        if (Reader.BaseStream.Position + 4 >= Reader.BaseStream.Length)
                            break;

                        ushort Char = Tools.Reverse(Reader.ReadUInt16());
                        ushort NChar = Tools.Reverse(Reader.ReadUInt16());
                        Reader.Seek(-2, SeekOrigin.Current);
                        bool Break = false;
                        if (Char == EndFlag)
                            break;
                        switch (Char) {
                            case BrkFlag:
                                Result += "\\n";
                                break;
                            case SpcFlag:
                                if (NChar == SpcFlag)
                                    Break = true;
                                SpaceScape = true;
                                Result += ' ';
                                break;
                            case 0x20:
                                SpaceScape = false;
                                Result += ' ';
                                break;
                            default:
                                Result += (char)Char;
                                break;
                        }

                        if (Break)
                            break;
                    }

                    Entries[i].Strings[x] = Result;
                }
            }

            return Entries.ToArray();
        }
    }
    
    //Offset = DWORD + 0x1C
    //Str End = 0xD821
    //Str Split = 0xD821D841

    public struct BMDStruct {
        public uint Magic;
        public uint Unk1;
        public uint Unk2;

        public uint FileSize;
        public uint UnkOffset1;//Allways points to the end of the last MSG
        public uint UnkCount;//At first look it's like the count of u32 after this, but this isn't the case in some files

        public uint Unk3;
        public uint Unk4;
        public uint Unk5;
        public uint Unk6;

    }

    public struct MSGEntry {
        [FString(Length = 0x20, TrimNull = true)]
        public string Label;

        public ushort Count;
        public ushort unk;//Actor?

        [RArray(FieldName = "Count")]
        public uint[] StrOffset;

        [Ignore]
        public string[] Strings;

        [Ignore]
        public long ID;
    }

}
