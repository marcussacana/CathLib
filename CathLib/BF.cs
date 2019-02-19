using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.IO;
using VirtualStream;

namespace CathLib {
    public static class BF {
        public static Entry[] Open(Stream Packget) {
            Packget.Position = 0x10;
            bool BigEndian = Packget.ReadByte() == 0x00;
            Packget.Position = 0;

            StructReader Reader = new StructReader(Packget, BigEndian);
            BFHeader Header = new BFHeader();
            Reader.ReadStruct(ref Header);

            List<Entry> Entries = new List<Entry>();
            for (uint i = 0; i < Header.EntriesCount; i++) {
                Entry Entry = new Entry() {
                    Name = (i.ToString("X8") + (Header.Entries[i].BMD ? ".bmd" : ".dat")),
                    Content = new VirtStream(Packget, Header.Entries[i].Offset, Header.Entries[i].Length),
                    Length = Header.Entries[i].Length
                };

                Entries.Add(Entry);
            }

            return Entries.ToArray();
        }

#pragma warning disable 649
        struct BFHeader {
            public uint Unk1;
            public uint PackgetLength;

            public uint Magic;
            public uint Unk2;

            public uint EntriesCount;

            public ushort Unk3;
            public ushort Unk4;

            public uint Unk5;
            public uint Unk6;


            [StructField, RArray(FieldName = "EntriesCount")]
            public BFEntry[] Entries;
        }

        struct BFEntry {
            public uint Type;
            public uint SizeA;
            public uint SizeB;
            public uint Offset;

            [Ignore]
            public uint Length {
                get {
                    return SizeA * SizeB;
                }
            }

            [Ignore]
            public bool BMD {
                get {
                    return Type == 3;
                }
            }
        }

#pragma warning restore 649
    }
}
