using AdvancedBinary;
using DirectXTexNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using VirtualStream;

namespace CathLib {
    public class SP2 {

        SPR0 Header = new SPR0();
        Stream Data;
        KeyFrame[] KeyFrames;

        TextureInfo[] Textures;
        public SP2(byte[] Data) {
            this.Data = new MemoryStream(Data);
        }

        public SP2(Stream Data) {
            this.Data = Data;
        }

        public Frame[] Import() {
            Data.Position = 0;
            StructReader Reader = new StructReader(Data, Encoding: Encoding.GetEncoding(932));
            Reader.ReadStruct(ref Header);

            if (Header.Signature != "SPR0")
                throw new Exception("Invalid Format");

            Header.Textures = Header.Textures.OrderBy(x => x.Offset).ToArray();

            Textures = new TextureInfo[Header.TextureCount];
            for (uint i = 0; i < Header.TextureCount; i++) {
                uint End = i + 1 >= Header.TextureCount ? Header.FileSize : Header.Textures[i + 1].Offset;
                uint Offset = Header.Textures[i].Offset;
                Textures[i] = new TextureInfo() {
                    Data = new VirtStream(Data, Offset, End - Offset)
                };
            }

            KeyFrames = new KeyFrame[Header.KeyFrameCount];
            for (uint i = 0; i < Header.KeyFrameCount; i++) {
                Reader.BaseStream.Position = Header.KeyFrames[i].Offset;
                KeyFrame KeyFrame = new KeyFrame();
                Reader.ReadStruct(ref KeyFrame);
                KeyFrame.ID = i;
                KeyFrames[i] = KeyFrame;
            }

            for (uint i = 0; i < Header.TextureCount; i++) {
                Textures[i].Address = Textures[i].Data.Alloc();
                Textures[i].Metadata = TexHelper.Instance.GetMetadataFromDDSMemory(Textures[i].Address, Textures[i].Data.Length, DDS_FLAGS.NONE);
                Textures[i].Texture = TexHelper.Instance.LoadFromDDSMemory(Textures[i].Address, Textures[i].Data.Length, DDS_FLAGS.NONE);

                var Texture = Textures[i].Texture;
                if (Texture.GetImageCount() != 1)
                    throw new Exception("Unexpected Texture");

                if (Textures[i].IsCompressed) {
                    Texture = Texture.Decompress(0, DXGI_FORMAT.UNKNOWN);
                    Textures[i].dFormat = Texture.GetMetadata().Format;
                }

                if ((Textures[i].IsCompressed ? Textures[i].dFormat : Textures[i].Metadata.Format) != DXGI_FORMAT.R8G8B8A8_UNORM)
                    Texture = Texture.Convert(DXGI_FORMAT.R8G8B8A8_UNORM, TEX_FILTER_FLAGS.DEFAULT, 1f);


                Textures[i].Bitmap = Texture.GetImage(0).GetBitmap();
            }            

            return (from x in KeyFrames where x.Width * x.Heigth != 0 select new Frame() {
                Name = x.Name,
                KeyFrameID = x.ID,
                Content = x.GetFrame(Textures[x.TextureID].Bitmap)
            }).ToArray();
        }


        public void Export(Stream Output, Frame[] Frames, bool Recompress = true) {
            for (int i = 0; i < Textures.Length; i++) {
                KeyFrame[] TexKFrames = (from x in KeyFrames where x.Width * x.Heigth != 0 && i == x.TextureID select x).ToArray();
                Bitmap[] TexFrames = (from x in Frames where 
                                      (from y in TexKFrames where x.KeyFrameID == y.ID select y).Count() == 1
                                      select x.Content).ToArray();

                if (TexFrames.Length < TexKFrames.Length)
                    throw new Exception($"Frames {TexKFrames.Length - TexFrames.Length} Missing...");
                if (TexFrames.Length != TexKFrames.Length)
                    throw new Exception($"Too Many Frames, {TexKFrames.Length} Expected");


                var Info = TexFrames.CreateSheet();
                Textures[i].Bitmap = Info.TextureSheet;

                for (int x = 0; x < Info.Textures.Length; x++) {
                    Frame Current = (from z in Frames where Info.Textures[x].Original == z.Content select z).Single();

                    KeyFrames[Current.KeyFrameID].X1 = Info.Textures[x].Rectangle.X;
                    KeyFrames[Current.KeyFrameID].Y1 = Info.Textures[x].Rectangle.Y;
                    KeyFrames[Current.KeyFrameID].X2 = Info.Textures[x].Rectangle.Right;
                    KeyFrames[Current.KeyFrameID].Y2 = Info.Textures[x].Rectangle.Bottom;

                }

                var Texture = Textures[i].Texture;

                if (Textures[i].IsCompressed)
                    Texture = Texture.Decompress(0, DXGI_FORMAT.UNKNOWN);

                var Format = (Textures[i].IsCompressed ? Textures[i].dFormat : Textures[i].Metadata.Format);
                if (Format != DXGI_FORMAT.R8G8B8A8_UNORM)
                    Texture = Texture.Convert(DXGI_FORMAT.R8G8B8A8_UNORM, TEX_FILTER_FLAGS.DEFAULT, 1f);

                Texture = Texture.SetBitmap(Info.TextureSheet);

                if (Format != DXGI_FORMAT.R8G8B8A8_UNORM) {
                    if (Textures[i].IsCompressed)
                        Texture = Texture.Convert(Textures[i].dFormat, TEX_FILTER_FLAGS.DEFAULT, 1f);
                    else
                        Texture = Texture.Convert(Textures[i].Metadata.Format, TEX_FILTER_FLAGS.DEFAULT, 1f);
                }

                if (Textures[i].IsCompressed && Recompress)
                    Texture = Texture.Compress(0, Textures[i].Metadata.Format, TEX_COMPRESS_FLAGS.PARALLEL, 1f);
                

                Textures[i].Texture = Texture;
            }

            Stream[] TexturesData = (from x in Textures select x.Texture.SaveToDDSMemory(0, DDS_FLAGS.NONE)).ToArray();

            uint KeyFramesOffset = (uint)Tools.GetStructLength(Header);
            uint TextureOffset = KeyFramesOffset + (uint)Sig.Length + (uint)(Tools.GetStructLength(new KeyFrame()) * Header.KeyFrameCount);
            
            for (int i = 0; i < KeyFrames.Length; i++)
                Header.KeyFrames[i].Offset = KeyFramesOffset + (uint)(Tools.GetStructLength(new KeyFrame()) * i);

            for (int i = 0; i < Textures.Length; i++) {
                Header.Textures[i].Offset = TextureOffset;
                TextureOffset += (uint)TexturesData[i].Length;
            }

            StructWriter Writer = new StructWriter(Output, Encoding: Encoding.GetEncoding(932));
            Header.FileSize = TextureOffset;
            Writer.WriteStruct(ref Header);
            for(int i = 0; i < KeyFrames.Length; i++)
                Writer.WriteStruct(ref KeyFrames[i]);

            Writer.Write(Sig);
            Writer.Flush();

            foreach (Stream Texture in TexturesData)
                Texture.CopyTo(Output);

            Output.Flush();
        }

        public byte[] Export(Frame[] Frames) {
            MemoryStream Stream = new MemoryStream();
            Export(Stream, Frames);

            return Stream.ToArray();
        }

        static byte[] Sig = new byte[] { 0x00, 0x45, 0x64, 0x69, 0x74, 0x65, 0x64, 0x20, 0x57, 0x69, 0x74, 0x68, 0x20, 0x43, 0x61, 0x74, 0x68, 0x4C, 0x69, 0x62, 0x00 };

        KeyFrame GetKeyFrame(Frame Frame) => KeyFrames[Frame.KeyFrameID];
    }
    public struct Frame {
        public string Name;
        public uint KeyFrameID;
        public Bitmap Content;
    }

    struct TextureInfo {
        public Stream Data;
        public IntPtr Address;
        public TexMetadata Metadata;
        public ScratchImage Texture;
        public Bitmap Bitmap;

        public DXGI_FORMAT dFormat;


        public bool IsCompressed {
            get {
                return Metadata.IsCompressed();
            }
        }
    }

#pragma warning disable 649, 169
    struct SPR0 {
        public ushort Flags;
        public ushort UID;
        public uint Unk;
        [FString(Length = 4, TrimNull = false)]
        public string Signature;        
        public uint HeaderSize;        
        public uint FileSize;        
        public ushort TextureCount;        
        public ushort KeyFrameCount;        
        public uint TexturePtrTableOffset;   
        public uint KeyFramePtrTableOffset;

        //Header End

        [RArray(FieldName = "TextureCount"), StructField()]
        public OffsetInfo[] Textures;

        [RArray(FieldName = "KeyFrameCount"), StructField()]
        public OffsetInfo[] KeyFrames;
    }

    struct OffsetInfo {
        public uint Type;
        public uint Offset;
    }

    struct KeyFrame {
        uint Unk0;
        [FString(Length = 16, TrimNull = true)]
        public string Name;
        public int TextureID;
        uint Unk1;
        uint Unk2;
        uint Unk3;
        uint Unk4;
        uint Unk5;
        uint Unk6;
        uint Unk7;
        uint Unk8;
        uint Unk9;
        uint Unk10;
        uint Unk11;
        uint Unk12;
        uint Unk13;
        uint Unk14;
        uint Unk15;
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;
        uint Unk16;
        uint Unk17;
        uint Unk18;
        uint Unk19;
        uint Unk20;
        uint Unk21;
        uint Unk22;

        [Ignore]
        public int Width { get { return X2 - X1; } }
        [Ignore]
        public int Heigth { get { return Y2 - Y1; } }

        [Ignore]
        public uint ID;
    }
#pragma warning restore 649
}
