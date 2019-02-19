using DirectXTexNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CathLib {
    public class DDS {

        byte[] Data;
        int TexCount;
        ScratchImage Texture;
        TexMetadata Metadata;
        DXGI_FORMAT dFormat;

        public DDS(Stream Data) {
            MemoryStream Mem = new MemoryStream();
            Data.CopyTo(Mem);
            this.Data = Mem.ToArray();
        }

        public DDS(byte[] Data) {
            this.Data = Data;
        }


        public Bitmap[] Import() {
            IntPtr DDS = new MemoryStream(Data).Alloc();
            Texture = TexHelper.Instance.LoadFromDDSMemory(DDS, Data.LongLength, DDS_FLAGS.NONE);
            Metadata = Texture.GetMetadata();

            TexCount = Texture.GetImageCount() / Metadata.MipLevels;
            if (TexCount == 0)
                throw new Exception("Unexpected DDS Entries/MipLevels");

            if (Metadata.IsCompressed()) {
                Texture = Texture.Decompress(0, DXGI_FORMAT.UNKNOWN);
                dFormat = Texture.GetMetadata().Format;
            }

            if ((Metadata.IsCompressed() ? dFormat : Metadata.Format) != DXGI_FORMAT.R8G8B8A8_UNORM)
                Texture = Texture.Convert(DXGI_FORMAT.R8G8B8A8_UNORM, TEX_FILTER_FLAGS.DEFAULT, 1f);
            
            Bitmap[] Textures = new Bitmap[TexCount];
            for (int i = 0; i < TexCount; i++) {
                Textures[i] = Texture.GetImage(i).GetBitmap();
            }
            return Textures;
        }

        public byte[] Export(Bitmap[] Tex) {
            for (int i = 0; i < TexCount; i++)
                Texture = Texture.SetBitmap(Tex[i], i);

            DXGI_FORMAT Format = (Metadata.IsCompressed() ? dFormat : Metadata.Format);
            if (Format != DXGI_FORMAT.R8G8B8A8_UNORM)
                Texture = Texture.Convert(Format, TEX_FILTER_FLAGS.DEFAULT, 1f);

            if (Metadata.IsCompressed())
                Texture = Texture.Compress(Metadata.Format, TEX_COMPRESS_FLAGS.DEFAULT, 1f);

            using (Stream Stream = Texture.SaveToDDSMemory(DDS_FLAGS.NONE)) {
                using (MemoryStream MStream = new MemoryStream()) {
                    Stream.CopyTo(MStream);
                    return MStream.ToArray();
                }
            }
        }

        public void Export(Stream Output, Bitmap[] Texture) {
            using (MemoryStream Stream = new MemoryStream(Export(Texture))) {
                Stream.CopyTo(Output);
                Stream.Close();
            }
        }
    }
}
