using KAutoHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ToolsAF
{
    public partial class TAF : Form
    {
        public TAF()
        {
            InitializeComponent();
        }

        public bool CompareBitmapsFast(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (object.Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;

            int bytes = bmp1.Width * bmp1.Height * (Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

            bool result = true;
            byte[] b1bytes = new byte[bytes];
            byte[] b2bytes = new byte[bytes];

            BitmapData bitmapData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bitmapData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            Marshal.Copy(bitmapData1.Scan0, b1bytes, 0, bytes);
            Marshal.Copy(bitmapData2.Scan0, b2bytes, 0, bytes);

            for (int n = 0; n <= bytes - 1; n++)
            {
                if (b1bytes[n] - b2bytes[n] > 10 || b1bytes[n] - b2bytes[n] < -10)
                {
                    result = false;
                    break;
                }
            }

            bmp1.UnlockBits(bitmapData1);
            bmp2.UnlockBits(bitmapData2);

            return result;
        }
        public Bitmap MakeGrayscale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            Graphics g = Graphics.FromImage(newBitmap);


            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
           new float[] {.3f, .3f, .3f, 0, 0},
           new float[] {.59f, .59f, .59f, 0, 0},
           new float[] {.11f, .11f, .11f, 0, 0},
           new float[] {0, 0, 0, 1, 0},
           new float[] {0, 0, 0, 0, 1}
               });

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            g.Dispose();
            return newBitmap;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var handle = AutoControl.FindWindowHandle("Qt5QWindowIcon" == "" ? null : "Qt5QWindowIcon", "" == "" ? null : "");
            bool rod = false;
        LOOP:
            label1.Text = "Running...";
            Task.Delay(4000).Wait();
            Bitmap bmp = (Bitmap)CaptureHelper.CaptureWindow(handle);
            bmp = CaptureHelper.CropImage(bmp, new System.Drawing.Rectangle(780, 190, 40, 30));
            bmp = MakeGrayscale(bmp);
            for (int i = 0; i < 10000; i++)
            {
                Task.Delay(10).Wait();
                Bitmap bmpc = (Bitmap)CaptureHelper.CaptureWindow(handle);
                bmpc = CaptureHelper.CropImage(bmpc, new System.Drawing.Rectangle(780, 190, 40, 30));
                bmpc = MakeGrayscale(bmpc);
                if (CompareBitmapsFast(bmp, bmpc))
                {
                    label1.Text = "Waiting...";
                }
                else
                {
                    label1.Text = "Done!";
                    //AutoControl.SendKeyFocus(KeyCode.SPACE_BAR);
                    AutoControl.SendKeyBoardPress(handle, VKeys.VK_SPACE);
                    break;
                }

            }
            Task.Delay(5000).Wait();
            //AutoControl.SendKeyFocus(KeyCode.KEY_S);
            AutoControl.SendKeyBoardPress(handle, VKeys.VK_S);
            Task.Delay(500).Wait();
            //AutoControl.SendKeyFocus(KeyCode.KEY_S);
            AutoControl.SendKeyBoardPress(handle, VKeys.VK_S);
            Task.Delay(1000).Wait();
            //AutoControl.SendKeyFocus(KeyCode.KEY_O);
            AutoControl.SendKeyBoardPress(handle, VKeys.VK_O);
            Task.Delay(1000).Wait();
            //AutoControl.SendKeyFocus(KeyCode.KEY_3);
            AutoControl.SendKeyBoardPress(handle, VKeys.VK_3);
            Task.Delay(1000).Wait();
            //AutoControl.SendKeyFocus(KeyCode.KEY_B);
            if (rod)
            {
                AutoControl.SendKeyBoardPress(handle, VKeys.VK_B);
                rod = false;
            }
            else
            {
                AutoControl.SendKeyBoardPress(handle, VKeys.VK_A);
                rod = true;
            }
            Task.Delay(1000).Wait();
            //AutoControl.SendKeyFocus(KeyCode.SHIFT);
            AutoControl.SendKeyBoardPress(handle, VKeys.VK_SHIFT);
            Task.Delay(1000).Wait();
            goto LOOP;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Pause");
            var handle = AutoControl.FindWindowHandle("Qt5QWindowIcon" == "" ? null : "Qt5QWindowIcon", "" == "" ? null : "");
            Bitmap bmp = (Bitmap)CaptureHelper.CaptureWindow(handle);
            bmp = CaptureHelper.CropImage(bmp, new System.Drawing.Rectangle(780, 190, 40, 30));
            bmp.Save("test.png");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*Rectangle r = new Rectangle(800, 272,
                                        325, 180);

            
        LOOP:
            label1.Text = "Running...";
            Task.Delay(4000).Wait();
            Bitmap bmp = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(r.Left, r.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            bmp = MakeGrayscale(bmp);
            for (int i = 0; i < 10000; i++)
            {
                Task.Delay(10).Wait();
                Bitmap bmpc = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppArgb);
                Graphics gc = Graphics.FromImage(bmpc);
                gc.CopyFromScreen(r.Left, r.Top, 0, 0, bmpc.Size, CopyPixelOperation.SourceCopy);
                bmpc = MakeGrayscale(bmpc);
                if (CompareBitmapsFast(bmp, bmpc))
                {
                    label1.Text = "Waiting...";
                }
                else
                {
                    label1.Text = "Done!";
                    AutoControl.SendKeyFocus(KeyCode.SPACE_BAR);
                    break;
                }

            }
            Task.Delay(5000).Wait();
            AutoControl.SendKeyFocus(KeyCode.KEY_S);
            Task.Delay(500).Wait();
            AutoControl.SendKeyFocus(KeyCode.KEY_S);
            Task.Delay(1000).Wait();
            AutoControl.SendKeyFocus(KeyCode.KEY_O);
            Task.Delay(1000).Wait();
            AutoControl.SendKeyFocus(KeyCode.KEY_3);
            Task.Delay(1000).Wait();
            AutoControl.SendKeyFocus(KeyCode.KEY_B);
            Task.Delay(1000).Wait();
            AutoControl.SendKeyFocus(KeyCode.SHIFT);
            Task.Delay(1000).Wait();
            goto LOOP;*/

            var handle = AutoControl.FindWindowHandle("Qt5QWindowIcon" == "" ? null : "Qt5QWindowIcon", "" == "" ? null : "");
            AutoControl.SendKeyBoardPress(handle, VKeys.VK_SPACE);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            Task.Delay(3000).Wait();
            radioButton1.Checked = true;
            radioButton1.Text = "Connected";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            radioButton1.Text = "Connected";
        }
    }
}
