using AdvancedBinary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VirtualStream;

namespace CathLib {
    public static class PAC {

        public static Encoding Encoding = Encoding.GetEncoding(932);
        public static Entry[] Open(Stream Packget) {
            List<Entry> Entries = new List<Entry>();

            StructReader Reader = new StructReader(Packget, Encoding: Encoding);
            long SizeOf = Tools.GetStructLength(new Entry());

            while (Reader.PeekByte() >= 0 && Reader.BaseStream.Position + SizeOf < Reader.BaseStream.Length) {
                Entry Entry = new Entry();
                Reader.ReadStruct(ref Entry);

                if (Entry.Length == 0)
                    break;

                Entry.Content = new VirtStream(Packget, Reader.BaseStream.Position, Entry.Length);

                Reader.Seek(Entry.Length, SeekOrigin.Current);

                while (Reader.BaseStream.Position % 64 != 0)
                    Reader.BaseStream.Position++;

                Entries.Add(Entry);
            }

            return Entries.ToArray();
        }

        public static void Save(Entry[] Entries, Stream Output) {
            StructWriter Writer = new StructWriter(Output);

            foreach (Entry Entry in Entries) {
                var lEntry = Entry;
                lEntry.Length = (uint)Entry.Content.Length;
                Writer.WriteStruct(ref lEntry);
                Entry.Content.CopyTo(Writer.BaseStream);

                while (Writer.BaseStream.Position % 64 != 0)
                    Writer.BaseStream.WriteByte(0x00);
            }

            //A null Entry = file end
            var tmp = new Entry() { Name = string.Empty, Length = 0 };
            Writer.WriteStruct(ref tmp);

            Writer.Flush();
        }
    }

    public struct Entry {
        [FString(Length = 0xFC, TrimNull = true)]
        public string Name;

        public uint Length;

        [Ignore]
        public Stream Content;
    }
}
