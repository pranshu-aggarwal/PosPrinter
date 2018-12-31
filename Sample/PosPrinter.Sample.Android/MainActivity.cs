using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using static Android.Graphics.Bitmap;
using Java.IO;
using System.IO;
using System.Threading.Tasks;

namespace PosPrinter.Sample.Droid
{
    [Activity(Label = "PosPrinter.Sample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            App.GetTextBytes += App_GetTextBytes;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

        }

        byte[] App_GetTextBytes(string arg)
        {
            var bitmap = textAsBitmap(arg, 50, Color.Black, out int width);

            var newBitmap = Bitmap.CreateBitmap(570, 192, Bitmap.Config.Argb8888);
            Canvas can = new Canvas(newBitmap);
            can.DrawRGB(255, 255, 255);
            can.DrawBitmap(bitmap, 189, 0, null);

            return POS_PrintBMP(newBitmap, 570, 0);
        }


        public static byte[] POS_PrintBMP(Bitmap mBitmap, int nWidth, int nMode)
        {
            int width = ((nWidth + 7) / 8) * 8;
            int height = mBitmap.Height * width / mBitmap.Width;
            height = ((height + 7) / 8) * 8;

            Bitmap rszBitmap = mBitmap;
            if (mBitmap.Width != width)
            {
                rszBitmap = ImageProcessing.resizeImage(mBitmap, width, height);
            }

            Bitmap grayBitmap = ImageProcessing.toGrayscale(rszBitmap);

            byte[] dithered = thresholdToBWPic(grayBitmap);

            byte[] data = eachLinePixToCmd(dithered, width, nMode);

            return data;
        }

        private static byte[] thresholdToBWPic(Bitmap mBitmap)
        {
            int[] pixels = new int[mBitmap.Width * mBitmap.Height];
            byte[] data = new byte[mBitmap.Width * mBitmap.Height];

            mBitmap.GetPixels(pixels, 0, mBitmap.Width, 0, 0,
                    mBitmap.Width, mBitmap.Height);

            // for the toGrayscale, we need to select a red or green or blue color 
            ImageProcessing.format_K_threshold(pixels, mBitmap.Width,
                    mBitmap.Height, data);

            return data;
        }

        private static int[] p0 = { 0, 0x80 };
        private static int[] p1 = { 0, 0x40 };
        private static int[] p2 = { 0, 0x20 };
        private static int[] p3 = { 0, 0x10 };
        private static int[] p4 = { 0, 0x08 };
        private static int[] p5 = { 0, 0x04 };
        private static int[] p6 = { 0, 0x02 };

        private static byte[] eachLinePixToCmd(byte[] src, int nWidth, int nMode)
        {
            int nHeight = src.Length / nWidth;
            int nBytesPerLine = nWidth / 8;
            byte[] data = new byte[nHeight * (8 + nBytesPerLine)];
            int offset = 0;
            int k = 0;
            for (int i = 0; i < nHeight; i++)
            {
                offset = i * (8 + nBytesPerLine);
                data[offset + 0] = 0x1d;
                data[offset + 1] = 0x76;
                data[offset + 2] = 0x30;
                data[offset + 3] = (byte)(nMode & 0x01);
                data[offset + 4] = (byte)(nBytesPerLine % 0x100);
                data[offset + 5] = (byte)(nBytesPerLine / 0x100);
                data[offset + 6] = 0x01;
                data[offset + 7] = 0x00;
                for (int j = 0; j < nBytesPerLine; j++)
                {
                    data[offset + 8 + j] = (byte)(p0[src[k]] + p1[src[k + 1]]
                            + p2[src[k + 2]] + p3[src[k + 3]] + p4[src[k + 4]]
                            + p5[src[k + 5]] + p6[src[k + 6]] + src[k + 7]);
                    k = k + 8;
                }
            }

            return data;
        }

        public Bitmap textAsBitmap(String text, float textSize, Color textColor, out int width)
        {
            Paint paint = new Paint(PaintFlags.AntiAlias);
            paint.TextSize = textSize;
            paint.Color = textColor;
            paint.TextAlign = Paint.Align.Left;
            float baseline = -paint.Ascent(); // ascent() is negative
            width = (int)(paint.MeasureText(text) + 0.5f); // round
            int height = (int)(baseline + paint.Descent() + 0.5f);
            Bitmap image = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(image);
            canvas.DrawText(text, 0, baseline, paint);
            return image;
        }
    }

    class ImageProcessing
    {
        // 转成灰度图 
        public static Bitmap toGrayscale(Bitmap bmpOriginal)
        {
            int width, height;
            height = bmpOriginal.Height;
            width = bmpOriginal.Width;
            Bitmap bmpGrayscale = Bitmap.CreateBitmap(width, height,
                                                      Config.Argb8888);
            Canvas c = new Canvas(bmpGrayscale);
            Paint paint = new Paint();
            ColorMatrix cm = new ColorMatrix();
            cm.SetSaturation(0);
            ColorMatrixColorFilter f = new ColorMatrixColorFilter(cm);
            paint.SetColorFilter(f);
            c.DrawBitmap(bmpOriginal, 0, 0, paint);
            return bmpGrayscale;
        }

        // 缩放，暂时需要public以便调试，完成之后不用这个。 
        public static Bitmap resizeImage(Bitmap bitmap, int w, int h)
        {

            // load the origial Bitmap 
            Bitmap BitmapOrg = bitmap;

            int width = BitmapOrg.Width;
            int height = BitmapOrg.Height;
            int newWidth = w;
            int newHeight = h;

            // calculate the scale 
            float scaleWidth = ((float)newWidth) / width;
            float scaleHeight = ((float)newHeight) / height;

            // create a matrix for the manipulation 
            Matrix matrix = new Matrix();
            // resize the Bitmap 
            matrix.PostScale(scaleWidth, scaleHeight);
            // if you want to rotate the Bitmap 
            // matrix.postRotate(45); 

            // recreate the new Bitmap 
            Bitmap resizedBitmap = Bitmap.CreateBitmap(BitmapOrg, 0, 0, width,
              height, matrix, true);

            // make a Drawable from Bitmap to allow to set the Bitmap 
            // to the ImageView, ImageButton or what ever 
            return resizedBitmap;
        }

        // 16*16 
        private static int[,] Floyd16x16 = /* Traditional Floyd ordered dither */
        {
   { 0, 128, 32, 160, 8, 136, 40, 168, 2, 130, 34, 162, 10, 138, 42,
     170 },
   { 192, 64, 224, 96, 200, 72, 232, 104, 194, 66, 226, 98, 202, 74,
     234, 106 },
   { 48, 176, 16, 144, 56, 184, 24, 152, 50, 178, 18, 146, 58, 186,
     26, 154 },
   { 240, 112, 208, 80, 248, 120, 216, 88, 242, 114, 210, 82, 250,
     122, 218, 90 },
   { 12, 140, 44, 172, 4, 132, 36, 164, 14, 142, 46, 174, 6, 134, 38,
     166 },
   { 204, 76, 236, 108, 196, 68, 228, 100, 206, 78, 238, 110, 198, 70,
     230, 102 },
   { 60, 188, 28, 156, 52, 180, 20, 148, 62, 190, 30, 158, 54, 182,
     22, 150 },
   { 252, 124, 220, 92, 244, 116, 212, 84, 254, 126, 222, 94, 246,
     118, 214, 86 },
   { 3, 131, 35, 163, 11, 139, 43, 171, 1, 129, 33, 161, 9, 137, 41,
     169 },
   { 195, 67, 227, 99, 203, 75, 235, 107, 193, 65, 225, 97, 201, 73,
     233, 105 },
   { 51, 179, 19, 147, 59, 187, 27, 155, 49, 177, 17, 145, 57, 185,
     25, 153 },
   { 243, 115, 211, 83, 251, 123, 219, 91, 241, 113, 209, 81, 249,
     121, 217, 89 },
   { 15, 143, 47, 175, 7, 135, 39, 167, 13, 141, 45, 173, 5, 133, 37,
     165 },
   { 207, 79, 239, 111, 199, 71, 231, 103, 205, 77, 237, 109, 197, 69,
     229, 101 },
   { 63, 191, 31, 159, 55, 183, 23, 151, 61, 189, 29, 157, 53, 181,
     21, 149 },
   { 254, 127, 223, 95, 247, 119, 215, 87, 253, 125, 221, 93, 245,
     117, 213, 85 } };
        // 8*8 
        private static int[,] Floyd8x8 = { { 0, 32, 8, 40, 2, 34, 10, 42 },
   { 48, 16, 56, 24, 50, 18, 58, 26 },
   { 12, 44, 4, 36, 14, 46, 6, 38 },
   { 60, 28, 52, 20, 62, 30, 54, 22 },
   { 3, 35, 11, 43, 1, 33, 9, 41 },
   { 51, 19, 59, 27, 49, 17, 57, 25 },
   { 15, 47, 7, 39, 13, 45, 5, 37 },
   { 63, 31, 55, 23, 61, 29, 53, 21 } };
        // 4*4 
        private static int[,] Floyd4x4 = { { 0, 8, 2, 10 }, { 12, 4, 14, 6 },
   { 3, 11, 1, 9 }, { 15, 7, 13, 5 } };

        /**
         * 将256色灰度图转换为2值图 
         *  
         * @param orgpixels 
         * @param xsize 
         * @param ysize 
         * @param despixels 
         */
        public static void format_K_dither16x16(int[] orgpixels, int xsize,
          int ysize, byte[] despixels)
        {
            int k = 0;
            for (int y = 0; y < ysize; y++)
            {

                for (int x = 0; x < xsize; x++)
                {

                    if ((orgpixels[k] & 0xff) > Floyd16x16[x & 15, y & 15])
                        despixels[k] = 0;// black 
                    else
                        despixels[k] = 1;

                    k++;
                }
            }

        }

        /**
         * 将256色灰度图转换为2值图 
         *  
         * @param orgpixels 
         * @param xsize 
         * @param ysize 
         * @param despixels 
         */
        public static void format_K_dither8x8(int[] orgpixels, int xsize,
          int ysize, byte[] despixels)
        {
            int k = 0;
            for (int y = 0; y < ysize; y++)
            {

                for (int x = 0; x < xsize; x++)
                {

                    if (((orgpixels[k] & 0xff) >> 2) > Floyd8x8[x & 7, y & 7])
                        despixels[k] = 0;// black 
                    else
                        despixels[k] = 1;

                    k++;
                }
            }
        }

        public static void format_K_threshold(int[] orgpixels, int xsize,
          int ysize, byte[] despixels)
        {

            int graytotal = 0;
            int grayave = 128;
            int i, j;
            int gray;

            int k = 0;
            for (i = 0; i < ysize; i++)
            {

                for (j = 0; j < xsize; j++)
                {

                    gray = orgpixels[k] & 0xff;
                    graytotal += gray;
                    k++;
                }
            }
            grayave = graytotal / ysize / xsize;

            // 二值化 
            k = 0;
            for (i = 0; i < ysize; i++)
            {

                for (j = 0; j < xsize; j++)
                {

                    gray = orgpixels[k] & 0xff;

                    if (gray > grayave)
                        despixels[k] = 0;// white 
                    else
                        despixels[k] = 1;

                    k++;
                }
            }
        }

        /*
         * 对灰度图(ARGB_8888)执行平均阀值算法(滤去0和255不考虑) 
         *  
         * 可以先调用toGrayscale从彩色图片生成灰度图 再调用该函数，将灰度图片转成2值图片 
         */
        public static void format_K_threshold(Bitmap mBitmap)
        {

            int graytotal = 0;
            int grayave = 128;
            int graycnt = 1;
            int gray;

            int ysize = mBitmap.Height;
            int xsize = mBitmap.Width;

            int i, j;
            for (i = 0; i < ysize; ++i)
            {
                for (j = 0; j < xsize; ++j)
                {
                    gray = mBitmap.GetPixel(j, i) & 0xFF;
                    if (gray != 0 && gray != 255)
                    {
                        graytotal += gray;
                        ++graycnt;
                    }
                }
            }
            grayave = graytotal / graycnt;

            // 根据前面的计算，求得一个平均阀值 
            for (i = 0; i < ysize; i++)
            {

                for (j = 0; j < xsize; j++)
                {

                    gray = mBitmap.GetPixel(j, i) & 0xFF;

                    if (gray > grayave)
                        mBitmap.SetPixel(j, i, Color.White);
                    else
                        mBitmap.SetPixel(j, i, Color.Black);
                }
            }
        }

        public static Bitmap alignBitmap(Bitmap bitmap, int wbits, int hbits,
          int color)
        {
            // 已经是对齐的，可以直接返回。 
            if ((bitmap.Width % wbits == 0)
              && (bitmap.Height % hbits == 0))
                return bitmap;

            int width = bitmap.Width;
            int height = bitmap.Height;
            int[] pixels = new int[width * height];
            bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);

            int newwidth = (width + wbits - 1) / wbits * wbits;
            int newheight = (height + hbits - 1) / hbits * hbits;
            int[] newpixels = new int[newwidth * newheight];
            Bitmap newbitmap = Bitmap.CreateBitmap(newwidth, newheight,
                                                   Config.Argb8888);
            for (int i = 0; i < newheight; ++i)
            {
                for (int j = 0; j < newwidth; ++j)
                {
                    if ((i < height) && (j < width))
                        newpixels[i * newwidth + j] = pixels[i * width + j];
                    else
                        newpixels[i * newwidth + j] = color;
                }
            }
            newbitmap.SetPixels(newpixels, 0, newwidth, 0, 0, newwidth, newheight);
            return newbitmap;
        }
    }
}