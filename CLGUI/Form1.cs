using CathLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CLGUI {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                textBox1.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
            } catch { }
        }

        bool XML = false;
        bool DAT = false;
        bool TBL = false;
        BMDTL Script;
        XML DB;
        MailData Mail;
        MFTBL MF;
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "All BMD Files|*.bmd|All XML Files|*.xml|All DAT Files|*.dat|All MFTBL.bin Files|*MFTBL.bin";
            if (fd.ShowDialog() != DialogResult.OK)
                return;

            XML = fd.FileName.ToLower().EndsWith(".xml");
            DAT = fd.FileName.ToLower().EndsWith(".dat");
            TBL = fd.FileName.ToLower().EndsWith(".bin");
            byte[] Scr = File.ReadAllBytes(fd.FileName);

            if (XML) {
                DB = new XML(Scr);
                listBox1.Items.Clear();
                listBox1.Items.AddRange(DB.Import());
                return;
            }

            if (DAT) {
                Mail = new MailData(Scr);
                listBox1.Items.Clear();
                listBox1.Items.AddRange(Mail.Import());
                return;
            }

            if (TBL) {
                MF = new MFTBL(Scr);
                listBox1.Items.Clear();
                listBox1.Items.AddRange(MF.Import());
                return;
            }

            Script = new BMDTL(Scr);

            listBox1.Items.Clear();
            listBox1.Items.AddRange(Script.Import());
        }

        private void generateStringPatchxmlToolStripMenuItem_Click(object sender, EventArgs e) {
            FolderBrowserDialog FB = new FolderBrowserDialog();
            FB.SelectedPath = "C:\\Program Files (x86)\\Steam\\SteamApps\\Common\\CatherineClassic";
            FB.Description = "Give the Catherine game directory";
            if (FB.ShowDialog() != DialogResult.OK)
                return;

            FolderBrowserDialog SD = new FolderBrowserDialog();
            SD.Description = "Directory to save all XMLs";
            SD.ShowNewFolderButton = true;

            if (SD.ShowDialog() != DialogResult.OK)
                return;

            if (!SD.SelectedPath.EndsWith("\\"))
                SD.SelectedPath += '\\';

            object[] Data = GetStrings(FB.SelectedPath);

            Text = "Generating XMLs...";
            Application.DoEvents();

            for (int i = 0; i < Data.Length; i++) {
                string FN = (string)((object[])Data[i])[0];
                string[] Strings = (string[])((object[])Data[i])[1];

                string Target = $"{SD.SelectedPath}[{i.ToString("X4")}] {Path.GetFileNameWithoutExtension(FN)}.xml";

                int Entries = 0;
                using (TextWriter Writer = new StreamWriter(File.Create(Target), Encoding.UTF8)) {
                    Writer.WriteLine("<Patches>");
                    List<string> Parsed = new List<string>();
                    foreach (string Str in Strings) {
                        string[] ParseRst = ParseStr(Str);
                        foreach (string Parse in ParseRst) {
                            if (Parsed.Contains(Parse))
                                continue;
                            if (Parse.Length <= 1)
                                continue;

                            Parsed.Add(Parse);

                            Writer.WriteLine("\t<Patch>");
                            Writer.WriteLine("\t\t<!--{0} -->", Str.Replace("--", "ー"));
                            Writer.WriteLine("\t\t<ID>-1</ID>");
                            Writer.WriteLine("\t\t<Match>");
                            Writer.WriteLine("\t\t\t<String>{0}</String>", Parse);
                            Writer.WriteLine("\t\t</Match>");
                            Writer.WriteLine("\t\t<Replace>");
                            Writer.WriteLine("\t\t\t<String>{0}</String>", Parse);
                            Writer.WriteLine("\t\t</Replace>");
                            Writer.WriteLine("\t</Patch>");
                            Entries++;
                        }
                    }
                    Writer.WriteLine("</Patches>");

                    Writer.Close();
                }

                if (Entries == 0)
                    File.Delete(Target);
            }

            Text = "CLGUI";
            MessageBox.Show("XML Generated...");
        }

        private string PackXML(string[] XMLs) {
            const string Prefix = "<Patches>";
            const string Sufix = "</Patches>";

            StringBuilder SB = new StringBuilder();
            SB.AppendLine(Prefix);
            foreach (string Path in XMLs) {
                string Data = File.ReadAllText(Path, Encoding.UTF8);
                Data = Data.Substring(Data.IndexOf(Prefix) + Prefix.Length);
                Data = Data.Substring(0, Data.IndexOf(Sufix));

                SB.Append(Data);
            }
            SB.AppendLine(Sufix);

            return SB.ToString();
        }

        //{0xD828}Vincent{0xD829}\n{0xDA8A}{0x0003}{0x0010}{0xDA61}눊{0x0001}...!? I almost fell asleep. 
        private string[] ParseStr(string Str) {
            const string SetColorTag = "{0xD901}";
            int Chars = 0;
            bool InTag = false;
            string Result = string.Empty;
            string Tag = string.Empty;

            foreach (char c in Str) {
                if (c == '{') {
                    if (Chars == 1)//If have only one char after last tag
                        Result = Result.Substring(0, Result.Length - 1);

                    Tag = "{";
                    InTag = true;
                    continue;
                }
                if (c == '}') {
                    Tag += c;
                    InTag = false;
                    Chars = 0;

                    if (Tag == SetColorTag) {
                        if (Result.Length > 0) {
                            Result += Tag;
                        }
                    }
                    continue;
                }
                if (InTag) {
                    Tag += c;
                    continue;
                }

                Result += c;
                Chars++;
            }

            Result = Result.Replace("\\n", "\n").Trim();

            if (Result.Contains("\n\n"))
                Result = Result.Substring(0, Result.IndexOf("\n\n"));

            string Name = null;
            if (Str.Contains("{0xD829}\\n")) {//Name
                if (Str.StartsWith("{0xD828}")) {
                    int Ind = Result.IndexOf("\n");
                    if (Ind > 0) {
                        Name = Result.Substring(0, Ind);
                        if (string.IsNullOrWhiteSpace(Name))
                            Name = null;
                    }
                }
                Result = Result.Substring(Result.IndexOf("\n") + 1);
            }

            List<Range> Jap = new List<Range>() {
                Range.Hiragana, Range.Katakana,
                Range.KatakanaPhoneticExtensions,
                Range.CJKUnifiedIdeographs
            };

            int JapCount = (from x in Result where Jap.Contains(UnicodeRanges.GetRange(Result.First())) select x).Count();
            bool InUpper = (from x in Result where char.IsUpper(x) select x).Count() > Result.Length / 2;

            string Ori = Result;
            if (!InUpper) {
                while (Result.Length >= 1 && !(char.IsLetter(Result.First()) && char.IsUpper(Result.First())) && (Result.First() != '”')) {

                    if (Result.First() == ' ' || JapCount >= Ori.Length / 2) {
                        Result = Ori;
                        break;
                    }
                    Result = Result.Substring(1);
                }

                if (Result.Length >= 1 && char.IsLetter(Result.First()) && char.IsUpper(Result.First())) {
                    int Ind = Result.IndexOf(" ");
                    if (Ind < 0)
                        Ind = Result.Length;

                    string First = Result.Substring(0, Ind);
                    Again:;
                    while (First.Length >= 1 && ((char.IsLetter(First.Last()) && char.IsLower(First.Last())) || char.IsPunctuation(First.Last()) || First.Last() == '\n'))
                        First = First.Substring(0, First.Length - 1);

                    First = First.Substring(0, First.Length - 1);

                    if (First.EndsWith("-"))
                        goto Again;

                    Result = Result.Substring(First.Length);
                }

            }

            if (string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(Ori))
                Result = Ori;

            Result = EscapeXML(Result);

            var Results = (from x in Result.Replace(SetColorTag, "\x0").Split('\x0') where !string.IsNullOrWhiteSpace(x) select x.Trim()).ToArray();
            if (Name != null)
                Results = new string[] { EscapeXML(Name) }.Concat(Results).ToArray();
            return Results;
        }

        private string EscapeXML(string Str) => Str.Replace("&", "&amp;").Replace("\n", "&#xA;").Replace("\r", "&#xD;").Replace("<", "&lt;").Replace(">", "&gt;");

        private object[] GetStrings(string Directory) {

            Text = "Enumerating Files...";
            Application.DoEvents();
            string[] Files = GetFiles(Directory);

            List<object> Strings = new List<object>();
            for (long i = 0; i < Files.LongLength; i++) {
                Text = $"Reading Files... {i}/{Files.LongLength} ({(int)((double)i / Files.LongLength * 100)}%)";
                Application.DoEvents();
                string FN = Path.GetFileName(Files[i]).ToLower();
                if (FN.StartsWith("i_") || FN.StartsWith("g_") || FN.StartsWith("s_") || FN.StartsWith("f_"))
                    continue;

                Strings.Add(new object[] { FN, Process(Files[i], File.Open(Files[i], FileMode.Open)) });
            }

            return Strings.ToArray();
        }

        private string[] Process(string File, Stream Content) {
            if (File.ToLower().EndsWith(".bmd"))
                using (MemoryStream Buffer = new MemoryStream()) {
                    Content.CopyTo(Buffer);
                    BMDTL Script = new BMDTL(Buffer.ToArray());
                    return Script.Import();
                }
            if (File.ToLower().EndsWith(".pac")) {
                List<string> Strings = new List<string>();
                foreach (Entry Entry in PAC.Open(Content))
                    Strings.AddRange(Process(Entry.Name, Entry.Content));

                return Strings.ToArray();
            }
            if (File.ToLower().EndsWith(".bf")) {
                List<string> Strings = new List<string>();
                foreach (Entry Entry in BF.Open(Content))
                    Strings.AddRange(Process(Entry.Name, Entry.Content));

                return Strings.ToArray();
            }

            return new string[0];
        }

        private string[] GetFiles(string Directory, string Filter = "*.bmd;*.pac;*.bf") {
            List<string> Entries = new List<string>();
            foreach (string Ext in Filter.Split(';'))
                Entries.AddRange(System.IO.Directory.GetFiles(Directory, Ext, SearchOption.AllDirectories));

            return Entries.ToArray();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = XML ? "All XML Files|*.xml" : (TBL ? "ALL MFTBL.BIN Files|*MFTBL.BIN" : "ALL DAT Files|*.dat");
            if (fd.ShowDialog() != DialogResult.OK)
                return;

            string[] Strings = listBox1.Items.Cast<string>().ToArray();

            if (XML)
                File.WriteAllBytes(fd.FileName, DB.Export(Strings));
            else if (DAT)
                File.WriteAllBytes(fd.FileName, Mail.Export(Strings));
            else if (TBL)
                File.WriteAllBytes(fd.FileName, MF.Export(Strings));
            else
                throw new Exception("BMD IS READ ONLY");

            MessageBox.Show("Saved");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Return) {
                e.Handled = true;
                e.SuppressKeyPress = true;

                listBox1.Items[listBox1.SelectedIndex] = textBox1.Text;
            }
        }
        private void optimizeXMLToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "All XML Files|*.xml";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All XML Files|*.xml";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            if (ofd.FileName == sfd.FileName && MessageBox.Show("This will delete any entry that isn't modified, are you sure?", "CLGUI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;


            byte[] Scr = Encoding.UTF8.GetBytes(PackXML(ofd.FileNames));

            var DB = new XML(Scr);
            File.WriteAllBytes(sfd.FileName, DB.Optimize());
            MessageBox.Show("Finished");
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All SP2 Files|*.SP2";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            foreach (string FileName in ofd.FileNames) {
                string OutDir = FileName + "~\\";

                if (!Directory.Exists(OutDir))
                    Directory.CreateDirectory(OutDir);

                using (Stream Stream = File.Open(FileName, FileMode.Open)) {
                    SP2 SP2 = new SP2(Stream);
                    var Textures = SP2.Import();

                    int i = 0;
                    foreach (Frame Frame in Textures) {
                        string Name = GetValidName(Frame.Name);
                        Frame.Content.Save(OutDir + $"[{i++}] " + Name + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
            MessageBox.Show("Extracted");
        }
        private string GetValidName(string Name) {
            char[] Invalid = Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).Concat(new char[] { '・' }).ToArray();
            string Rst = string.Empty;
            foreach (char c in Name) {
                if (Invalid.Contains(c))
                    Rst += $"[0x{((ushort)c).ToString("X4")}]";
                else
                    Rst += c;
            }

            return Rst;
        }

        private void createRectanglesToolStripMenuItem_Click(object sender, EventArgs e) {
#if DEBUG
            bool Swich = false;

            Bitmap[] Array = new Bitmap[new Random().Next(10, 30)];
            byte[] Rand = new byte[4 * Array.Length];
            new Random().NextBytes(Rand);
            for (int i = 0; i < Array.Length; i++) {
                int Width = Rand[(i * 4) + 0];
                int Heigth = Rand[(i * 4) + 2];
                Array[i] = new Bitmap(Width, Heigth);

                using (Graphics g = Graphics.FromImage(Array[i])) {
                    g.DrawRectangle(Swich ? Pens.Black : Pens.DarkBlue, new Rectangle(new Point(0, 0), new Size(Array[i].Size.Width - 1, Array[i].Size.Height - 1)));
                    g.Flush();
                }
            }

            var Result = Array.CreateSheet();
            Result.TextureSheet.Save("sheet.png");
            MessageBox.Show("Genareted");
#endif
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All SP2 Files|*.SP2";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.SelectedPath = Path.GetDirectoryName(ofd.FileName);


            foreach (string FileName in ofd.FileNames) {
                bool Auto = false;
                if (!Directory.Exists(FileName + "~\\")) {
                    if (fb.ShowDialog() != DialogResult.OK)
                        return;
                } else Auto = true;

                SaveFileDialog sfd = new SaveFileDialog();
                if (!Auto) {
                    sfd.Filter = "All SP2 Files|*.SP2";
                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;
                }

                string InDir = Auto ? FileName + "~\\" : fb.SelectedPath;
                if (!InDir.EndsWith("\\"))
                    InDir += "\\";

                using (Stream Input = File.Open(ofd.FileName, FileMode.Open)) {
                    SP2 Texture = new SP2(Input);
                    Frame[] Frames = Texture.Import();
                    for (int i = 0; i < Frames.Length; i++) {
                        string TFile = InDir + $"[{i}] " + GetValidName(Frames[i].Name) + ".png";
                        if (File.Exists(TFile))
                            Frames[i].Content = new Bitmap(TFile);

                    }

                    using (Stream Output = File.Create(Auto ? Path.GetDirectoryName(FileName) + "\\" + Path.GetFileNameWithoutExtension(FileName) + "-new" + Path.GetExtension(FileName) : sfd.FileName)) {
                        Texture.Export(Output, Frames, recompressToolStripMenuItem.Checked, !keepFramePointsToolStripMenuItem.Checked);
                        Output.Close();
                    }

                    foreach (var Frame in Frames)
                        Frame.Content.Dispose();
                }
            }

            MessageBox.Show("SP2 Repacked");
        }

        private void openToolStripMenuItem2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All DDS Files|*.DDS";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            foreach (string FileName in ofd.FileNames) {
                string OutFileA = Path.GetDirectoryName(FileName) + "\\" + Path.GetFileNameWithoutExtension(FileName) + ".png";
                string OutFileB = Path.GetDirectoryName(FileName) + "\\" + Path.GetFileNameWithoutExtension(FileName) + "_{0}.png";

                byte[] Texture = File.ReadAllBytes(FileName);
                DDS Reader = new DDS(Texture);
                Bitmap[] Texs = Reader.Import();
                if (Texs.Length > 1) {
                    for (int i = 0; i < Texs.Length; i++) {
                        Texs[i].Save(string.Format(OutFileB, i));
                    }
                } else
                    Texs.Single().Save(OutFileA);
            }

            MessageBox.Show("Converted");
        }

        private void saveToolStripMenuItem2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All DDS Files|*.DDS";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;



            foreach (string FileName in ofd.FileNames) {
                string PNGA = Path.GetDirectoryName(FileName) + "\\" + Path.GetFileNameWithoutExtension(FileName) + ".png";
                string PNGB = Path.GetDirectoryName(FileName) + "\\" + Path.GetFileNameWithoutExtension(FileName) + "_{0}.png";
                string DDS = Path.GetDirectoryName(FileName) + "\\" + Path.GetFileNameWithoutExtension(FileName) + "-new" + Path.GetExtension(FileName);

                byte[] Texture = File.ReadAllBytes(FileName);
                DDS Reader = new DDS(Texture);
                var Texs = Reader.Import();

                for (int i = 0; i < Texs.Length; i++) {
                    string PNG = Texs.Length == 1 ? PNGA : string.Format(PNGB, i);

                    using (var Img = new Bitmap(PNG))
                        Texs[i] = Img;

                }

                Texture = Reader.Export(Texs);

                File.WriteAllBytes(DDS, Texture);
            }

            MessageBox.Show("Converted");
        }

        private void processToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            using (Stream File = System.IO.File.Open(ofd.FileName, FileMode.Open)) {
                string[] Strs = Process(ofd.FileName, File);
                listBox1.Items.Clear();
                listBox1.Items.AddRange(Strs);
            }
        }

        private void extractPackgetToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            string Path = ofd.FileName;
            string OutDir = Path + "~\\";

            if (!Directory.Exists(OutDir))
                Directory.CreateDirectory(OutDir);

            using (Stream Content = File.Open(Path, FileMode.Open)) {
                if (Path.ToLower().EndsWith(".pac")) {
                    foreach (Entry Entry in PAC.Open(Content)) {
                        using (Stream Output = File.Create(OutDir + GetValidName(Entry.Name))) {
                            Entry.Content.CopyTo(Output);
                            Output.Close();
                        }
                    }
                }
                if (Path.ToLower().EndsWith(".bf")) {
                    foreach (Entry Entry in BF.Open(Content)) {
                        using (Stream Output = File.Create(OutDir + GetValidName(Entry.Name))) {
                            Entry.Content.CopyTo(Output);
                            Output.Close();
                        }
                    }
                }
                Content.Close();
            }
            MessageBox.Show("Extracted");
        }

        private void repackPACToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All PAC Files|*.pac";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            string Path = ofd.FileName;
            string FilesDir = Path + "~\\";
            string OutPath = System.IO.Path.GetDirectoryName(Path) + "\\" + System.IO.Path.GetFileNameWithoutExtension(Path) + "-new" + System.IO.Path.GetExtension(Path);
            using (Stream Output = File.Create(OutPath))
            using (Stream Content = File.Open(Path, FileMode.Open)) {
                if (Path.ToLower().EndsWith(".pac")) {
                    var Entries = PAC.Open(Content);
                    for (int i = 0; i < Entries.Length; i++) {
                        string FN = FilesDir + GetValidName(Entries[i].Name);
                        if (File.Exists(FN))
                            Entries[i].Content = File.Open(FN, FileMode.Open);
                    }

                    PAC.Save(Entries, Output);

                    foreach (Entry Entry in Entries)
                        Entry.Content.Close();
                }
                if (Path.ToLower().EndsWith(".bf")) {
                    MessageBox.Show("Not Supported Yet");
                    return;
                }

                Content.Close();
                Output.Close();
            }

            MessageBox.Show("Repacked");
        }

        private void dumpDDSToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All SP2 Files|*.SP2";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            foreach (string FileName in ofd.FileNames) {
                string OutDir = FileName + "~\\";

                if (!Directory.Exists(OutDir))
                    Directory.CreateDirectory(OutDir);

                using (Stream Input = File.Open(FileName, FileMode.Open)) {
                    SP2 SP2 = new SP2(Input);

                    var TexsData = SP2.GetTextures();
                    for (int i = 0; i < TexsData.Length; i++) {
                        DDS DDS = new DDS(TexsData[i]);
                        Bitmap[] Textures = DDS.Import();
                        for (int x = 0; x < Textures.Length; x++) {
                            Textures[x].Save(OutDir + $"{i}-{x}.png", System.Drawing.Imaging.ImageFormat.Png);
                            Textures[x].Dispose();
                        }
                    }
                }
            }

            MessageBox.Show("Dumped");
        }
    }
}
