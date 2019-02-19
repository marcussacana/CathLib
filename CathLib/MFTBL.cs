using AdvancedBinary;
using static CathLib.Remap;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CathLib {

    public class MFTBL {


        /// <summary>
        /// Needs setup the offset if you will use in other file that isn't the Catherine MFTBL.bin
        /// <para>See the MFTBL Offset.png in the source code directory</para>
        /// </summary>
        public uint BeginOffset = 0x3680;

        public byte[] Script;

        TBLEntry[] Entries;
        public MFTBL(byte[] Script) {
            this.Script = Script;
        }


        public string[] Import() {
            StructReader Reader = new StructReader(new MemoryStream(Script), false, Encoding.BigEndianUnicode);
            Reader.BaseStream.Position = BeginOffset;

            List<TBLEntry> Entries = new List<TBLEntry>();
            while (Reader.PeekLong() != 0x00) {
                TBLEntry Entry = new TBLEntry();
                Reader.ReadStruct(ref Entry);

                Entry.Question = Decode(Entry.Question);
                Entry.ReplyA = Decode(Entry.ReplyA);
                Entry.ReplyB = Decode(Entry.ReplyB);

                Entries.Add(Entry);
            }

            this.Entries = Entries.ToArray();

            return this.Entries.SelectMany(x => new string[] { x.Question, x.ReplyA, x.ReplyB }).ToArray();
        }
        public byte[] Export(string[] Strings) {
            for (int i = 0; i < Entries.Length; i++) {
                Entries[i].Question = Encode(Strings[(i * 3) + 0]);
                Entries[i].ReplyA   = Encode(Strings[(i * 3) + 1]);
                Entries[i].ReplyB   = Encode(Strings[(i * 3) + 2]);
            }

            using (MemoryStream Stream = new MemoryStream(Script))
            using (StructWriter Writer = new StructWriter(Stream, false, Encoding.BigEndianUnicode)) {
                Stream.Position = BeginOffset;

                for (int i = 0; i < Entries.Length; i++)
                    Writer.WriteStruct(ref Entries[i]);

                return Stream.ToArray();
            }
        }
    }

#pragma warning disable 649
    struct TBLEntry {
        public uint Unk1;
        public uint Unk2;

        [FString(Length = 0xD4)]
        public string Question;

        public uint Unk3;

        [FString(Length = 0x60)]
        public string ReplyA;

        public uint Unk4;

        [FString(Length = 0x60)]
        public string ReplyB;
    }
#pragma warning restore 649
}
