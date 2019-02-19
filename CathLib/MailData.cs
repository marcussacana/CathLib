using AdvancedBinary;
using static CathLib.Remap;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CathLib {
    public class MailData {

        byte[] Script;

        MailHeader Header = new MailHeader();

        MailEntry[] Entries;

        byte[] Sufix;
        public MailData(byte[] Script) {
            this.Script = Script;
        }

        public string[] Import() {
            StructReader Reader = new StructReader(new MemoryStream(Script), true, Encoding.BigEndianUnicode);
            Reader.ReadStruct(ref Header);

            uint MailLength = (uint)Tools.GetStructLength(new MailEntry());

            List<MailEntry> Mails = new List<MailEntry>();
            while (Reader.BaseStream.Position + MailLength < Script.Length) {
                MailEntry Mail = new MailEntry();
                Reader.ReadStruct(ref Mail);
                Mails.Add(Mail);
            }

            MemoryStream Sufix = new MemoryStream();
            Reader.BaseStream.CopyTo(Sufix);
            this.Sufix = Sufix.ToArray();

            Entries = Mails.ToArray();

            return Entries.SelectMany(x => new string[] { Decode(x.Sender), Decode(x.Subject), Decode(x.Message) }.
                                           Concat(from y in x.ReplyList where y.Enabled select Decode(y.Reply))).ToArray();
        }

        public byte[] Export(string[] Strings) {
            using (MemoryStream Stream = new MemoryStream()) {
                StructWriter Writer = new StructWriter(Stream, true, Encoding.BigEndianUnicode);
                Writer.WriteStruct(ref Header);

                for (int i = 0, x = 0; i < Entries.Length; i++) {
                    string Sender  = Strings[x++];
                    string Subject = Strings[x++];
                    string Message = Strings[x++];

                    Sender  = LimitString(Sender,  0x028);
                    Subject = LimitString(Subject, 0x04A);
                    Message = LimitString(Message, 0x35C);

                    Entries[i].Sender  = Encode(Sender);
                    Entries[i].Subject = Encode(Subject);
                    Entries[i].Message = Encode(Message);

                    for (int z = 0; z < Entries[i].ReplyList.Length; z++){
                        if (!Entries[i].ReplyList[z].Enabled)
                            continue;

                        string Reply = Encode(Strings[x++]);
                        Reply = LimitString(Reply, 0xAC);
                        Entries[i].ReplyList[z].Reply = Reply;
                    }

                    Writer.WriteStruct(ref Entries[i]);
                }

                Writer.Write(Sufix, 0, Sufix.Length);

                return Stream.ToArray();
            }
        }

        public string LimitString(string Content, int Length) {
            if (Content.Length > Length/2)
                return Content.Substring(0, Length/2);

            return Content;
        }

#pragma warning disable 649
        struct MailHeader {
            [PArray(PrefixType = Const.UINT32)]
            public byte[] Unk1;
            [FArray(Length = 0x21)]
            public byte[] Unk2;
        }        
        struct MailEntry {
            [FArray(Length = 0x24)]
            public byte[] Unk;

            [FString(Length = 0x28, TrimNull = true)]
            public string Sender;

            [FString(Length = 0x4A, TrimNull = true)]
            public string Subject;

            [FString(Length = 0x35C, TrimNull = true)]
            public string Message;

            [FArray(Length = 0x80), StructField]
            public MailReply[] ReplyList;
        }

        struct MailReply {
            public uint Flags;

            [FString(Length = 0xAC, TrimNull = true)]
            public string Reply;

            public bool Enabled {
                get {
                    return Flags != 0xFF000000;
                }
            }
        }
#pragma warning restore 649
    }
}
