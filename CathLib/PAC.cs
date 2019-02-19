using AdvancedBinary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
                try {
                    while (Reader.BaseStream.Position % 0x10 != 0 || Reader.PeekByte() == 0x00 || !ValidFileName(Reader.PeekString(StringStyle.FString, EntryName)))
                        Reader.BaseStream.Position++;
                    
                } catch { }

                Entries.Add(Entry);
            }

            return Entries.ToArray();
        }
        private static bool ValidFileName(string FileName) {
            if (FileName == string.Empty)
                return true;

            string[] Parts = FileName.Split('.');
            if (Parts.Length != 2)
                return false;

            if (FileName.Contains("\\") || FileName.Contains("/"))
                return false;

            if (Parts[1].Length > 3)
                return false;



            List<Range> AllowedRanges = new List<Range>() {
                Range.BasicLatin,
                Range.LatinExtendedA,
                Range.Katakana,
                Range.Hiragana,
                Range.KatakanaPhoneticExtensions,
                Range.CJKUnifiedIdeographs,
                Range.GeneralPunctuation
            };

            foreach (char Char in FileName)
                if (!AllowedRanges.Contains(UnicodeRanges.GetRange(Char)))
                    return false;

            return true;
        }

        static FieldInfo EntryName = (from x in typeof(Entry).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) where x.Name == "Name" select x).Single();
    }

    public struct Entry {
        [FString(Length = 0xFC, TrimNull = true)]
        public string Name;

        public uint Length;

        [Ignore]
        public Stream Content;
    }
}
