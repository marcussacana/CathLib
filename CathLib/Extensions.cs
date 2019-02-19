using DirectXTexNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace CathLib {
#if DEBUG
    public static class Extensions {

#else
    static class Extensions {
#endif
        public static Bitmap GetBitmap(this DirectXTexNet.Image Image) {
            if (Image.Format != DXGI_FORMAT.R8G8B8A8_UNORM)
                throw new Exception("Only RGBA (UNORM) Is supported");

            int Size = TexHelper.Instance.BitsPerPixel(Image.Format)/8;
            IntPtr Address = Image.Pixels;
            Bitmap Result = new Bitmap(Image.Width, Image.Height);
            for (int y = 0; y < Result.Height; y++)
                for (int x = 0; x < Result.Width; x++) {
                    byte[] Arr = new byte[Size];
                    Marshal.Copy(Address, Arr, 0, Arr.Length);
                    Address = Address.Sum(Size);

                    Color Color = Color.FromArgb(Arr[3], Arr[0], Arr[1], Arr[2]);
                    Result.SetPixel(x, y, Color);
                }

            return Result;
        }

        public static ScratchImage SetBitmap(this ScratchImage Texture, Bitmap Content, int Index = 0) {
            var Meta = Texture.GetMetadata();
            if (Meta.Format != DXGI_FORMAT.R8G8B8A8_UNORM)
                throw new Exception("Only RGBA (UNORM) Is supported");

            int Size = TexHelper.Instance.BitsPerPixel(Meta.Format) / 8;
            byte[] Buffer = new byte[Content.Height * Content.Width * Size];
            for (int y = 0, z = 0; y < Content.Height; y++)
                for (int x = 0; x < Content.Width; x++) {
                    Color Pixel = Content.GetPixel(x, y);
                    new byte[] { Pixel.R, Pixel.G, Pixel.B, Pixel.A }.CopyTo(Buffer, z);
                    z += Size;
                }

            var Result = Texture.Resize(Content.Width, Content.Height, TEX_FILTER_FLAGS.DEFAULT);
            var Image = Result.GetImage(Index);
            Marshal.Copy(Buffer, 0, Image.Pixels, Buffer.Length);

            return Result;
        }


        public static IntPtr Alloc(this Stream Data) {
            byte[] Array = Data.ToArray();
            IntPtr Address = Marshal.AllocHGlobal(Array.Length);
            Marshal.Copy(Array, 0, Address, Array.Length);

            return Address;
        }

        public static byte[] ToArray(this Stream Data) {
            using (MemoryStream Temp = new MemoryStream()) {
                Data.Position = 0;
                Data.CopyTo(Temp);

                return Temp.ToArray();
            }
        }

        public static void Free(this IntPtr Ptr) => Marshal.FreeHGlobal(Ptr);

        public static IntPtr Sum(this IntPtr Ptr, long Value) => new IntPtr(Ptr.ToInt64() + Value);


        internal static Bitmap GetFrame(this KeyFrame Frame, Bitmap Texture) {
            if (Frame.Width == 0 || Frame.Heigth == 0)
                return null;

            Bitmap Result = new Bitmap(Frame.Width, Frame.Heigth);
            using (Graphics g = Graphics.FromImage(Result)) {
                Rectangle Target = new Rectangle(0, 0, Result.Width, Result.Height);
                Rectangle Source = new Rectangle(Frame.X1, Frame.Y1, Frame.Width, Frame.Heigth);
                g.DrawImage(Texture, Target, Source, GraphicsUnit.Pixel);
                g.Flush();
            }

            return Result;
        }

        public static TextureSheetInfo CreateSheet(this Bitmap[] Bitmaps) {
            TextureSheetInfo Info = new TextureSheetInfo();
            Info.Textures = (from x in Bitmaps orderby x.Height * x.Width descending select new TextureInfo() { Original = x }).ToArray();

            bool Swicher = true;
            for (int tid = 0; tid < Info.Textures.Length; tid++) {
                TextureInfo Texture = Info.Textures[tid];
                Rectangle[] Rectangles = Info.Textures.Take(tid).Select(x => x.Rectangle).ToArray();


                Rectangle Pos = new Rectangle(new Point(0, 0), Texture.Original.Size);
                if (Pos.Overlap(Rectangles)) {
                    Size CSize = GetTotalSize(Rectangles);

                    foreach (Rectangle Rect in Rectangles) {
                        Rectangle SkipedX = Pos.SkipX(Rect);
                        Size SXSize = GetTotalSize(Rectangles.Concat(new Rectangle[] { SkipedX }).ToArray());
                        Rectangle SkipedY = Pos.SkipY(Rect);
                        Size SYSize = GetTotalSize(Rectangles.Concat(new Rectangle[] { SkipedY }).ToArray());

                        if (!SkipedX.Overlap(Rectangles) && CSize == SXSize) {
                            Pos = SkipedX;
                            break;
                        }

                        if (!SkipedY.Overlap(Rectangles) && CSize == SYSize) {
                            Pos = SkipedY;
                            break;
                        }
                        
                    }

                    if (Pos.Overlap(Rectangles)) {
                        Size BSize = new Size(int.MaxValue, int.MaxValue);
                        foreach (Rectangle Rect in Rectangles) {
                            Rectangle SkipedX = Pos.SkipX(Rect);
                            Size SXSize = GetTotalSize(Rectangles.Concat(new Rectangle[] { SkipedX }).ToArray());
                            Rectangle SkipedY = Pos.SkipY(Rect);
                            Size SYSize = GetTotalSize(Rectangles.Concat(new Rectangle[] { SkipedY }).ToArray());
                            int XDiffWidth = SXSize.Width - CSize.Width;
                            int XDiffHeigh = SXSize.Height - CSize.Height;
                            int YDiffWidth = SYSize.Width - CSize.Width;
                            int YDiffHeigh = SYSize.Height - CSize.Height;


                            bool XMinor = XDiffHeigh + XDiffWidth < YDiffHeigh + YDiffWidth;

                            if (!SkipedX.Overlap(Rectangles) && (XMinor || SXSize == BSize)) {
                                if (BSize.Width == int.MaxValue || SXSize.Width * SXSize.Height < BSize.Width * BSize.Height) {
                                    BSize = SXSize;
                                    Pos = SkipedX;
                                }
                            }
                            if (!SkipedY.Overlap(Rectangles) && (!XMinor || SYSize == BSize)) {
                                if (BSize.Width == int.MaxValue || SYSize.Width * SYSize.Height < BSize.Width * BSize.Height) {
                                    BSize = SYSize;
                                    Pos = SkipedY;
                                }
                            }
                        }
                    }

                    if (Pos.Overlap(Rectangles)) {
                        Swicher = !Swicher;
                        if (Swicher)
                            Pos = new Rectangle(new Point(CSize.Width + 1, 0), Texture.Original.Size);
                        else
                            Pos = new Rectangle(new Point(0, CSize.Height + 1), Texture.Original.Size);
                    }

                    if (Pos.Overlap(Rectangles))
                        throw new Exception("Failed to create texture sheet");
                }
                Info.Textures[tid].Rectangle = Pos;

            }

            Info = Info.TrimSheet();

            Size Size = GetTotalSize((from x in Info.Textures select x.Rectangle).ToArray());
            Info.TextureSheet = new Bitmap(Size.Width, Size.Height);
            using (Graphics g = Graphics.FromImage(Info.TextureSheet)) {
                foreach (TextureInfo Texture in Info.Textures)
                    g.DrawImageUnscaled(Texture.Original, Texture.Rectangle);
                g.Flush();
            }

            return Info;
        }
        static Rectangle SkipX(this Rectangle Rectangle, Rectangle Target) {
            return new Rectangle(Target.X + Target.Width + 1, Target.Y, Rectangle.Width, Rectangle.Height);
        }
        static Rectangle SkipY(this Rectangle Rectangle, Rectangle Target) {
            return new Rectangle(Target.X, Target.Y + Target.Height + 1, Rectangle.Width, Rectangle.Height);
        }

        static TextureSheetInfo TrimSheet(this TextureSheetInfo Info) {
            bool Modified = true;
            while (Modified) {
                Modified = false;
                for (int i = 0; i < Info.Textures.Length; i++) {
                    if (Info.Textures[i].Rectangle.Width == 0 && Info.Textures[i].Rectangle.Height == 0)
                        continue;

                    while (true) {
                        Point NewPoint = new Point(Info.Textures[i].Rectangle.X - 1, Info.Textures[i].Rectangle.Y);
                        Rectangle NewRect = new Rectangle(NewPoint, Info.Textures[i].Rectangle.Size);
                        if (NewRect.Overlap(Info, new Rectangle[] { Info.Textures[i].Rectangle }))
                            break;
                        Modified = true;
                        Info.Textures[i].Rectangle = NewRect;
                    }
                    while (true) {
                        Point NewPoint = new Point(Info.Textures[i].Rectangle.X, Info.Textures[i].Rectangle.Y - 1);
                        Rectangle NewRect = new Rectangle(NewPoint, Info.Textures[i].Rectangle.Size);
                        if (NewRect.Overlap(Info, new Rectangle[] { Info.Textures[i].Rectangle }))
                            break;
                        Modified = true;
                        Info.Textures[i].Rectangle = NewRect;
                    }
                }
            }

            return Info;
        }

        
        static bool Overlap(this Rectangle Rectangle, Rectangle[] Rectangles) => Rectangle.Overlap(Rectangles, new Rectangle[0]);
        static bool Overlap(this Rectangle Rectangle, TextureSheetInfo Info, Rectangle[] Allow) => Rectangle.Overlap((from x in Info.Textures select x.Rectangle).ToArray(), Allow);
        static bool Overlap(this Rectangle Rectangle, Rectangle[] Rectangles, Rectangle[] Allow) {
            if (Rectangle.X < 0 || Rectangle.Y < 0)
                return true;

            foreach (Rectangle Rect in Rectangles) {
                if (Allow.Contains(Rect))
                    continue;

                if (Rectangle.IntersectsWith(Rect))
                    return true;
            }

            return false;
        }

        static Size GetTotalSize(this Rectangle[] Rectangles) {
            if (Rectangles.Length == 0)
                return new Size(1, 1);

            Rectangle MajorX = (from x in Rectangles orderby x.X + x.Width descending select x).First();
            Rectangle MajorY = (from x in Rectangles orderby x.Y + x.Height descending select x).First();

            return new Size(MajorX.X + MajorX.Width, MajorY.Y + MajorY.Height);
        }

        public static bool IsCompressed(this TexMetadata Metadata) {
            DXGI_FORMAT[] Compressions = new DXGI_FORMAT[] {
                    DXGI_FORMAT.BC1_TYPELESS,
                    DXGI_FORMAT.BC1_UNORM,
                    DXGI_FORMAT.BC1_UNORM_SRGB,
                    DXGI_FORMAT.BC2_TYPELESS,
                    DXGI_FORMAT.BC2_UNORM,
                    DXGI_FORMAT.BC2_UNORM_SRGB,
                    DXGI_FORMAT.BC3_TYPELESS,
                    DXGI_FORMAT.BC3_UNORM,
                    DXGI_FORMAT.BC3_UNORM_SRGB,
                    DXGI_FORMAT.BC4_SNORM,
                    DXGI_FORMAT.BC4_TYPELESS,
                    DXGI_FORMAT.BC4_UNORM,
                    DXGI_FORMAT.BC5_SNORM,
                    DXGI_FORMAT.BC5_TYPELESS,
                    DXGI_FORMAT.BC5_UNORM,
                    DXGI_FORMAT.BC6H_SF16,
                    DXGI_FORMAT.BC6H_TYPELESS,
                    DXGI_FORMAT.BC6H_UF16,
                    DXGI_FORMAT.BC7_TYPELESS,
                    DXGI_FORMAT.BC7_UNORM,
                    DXGI_FORMAT.BC7_UNORM_SRGB
                };

            return Compressions.Contains(Metadata.Format);
        }
        public struct TextureSheetInfo {
            public TextureInfo[] Textures;

            public Bitmap TextureSheet;
        }

        public struct TextureInfo {
            public Bitmap Original;
            public Rectangle Rectangle;
        }
    }
}
