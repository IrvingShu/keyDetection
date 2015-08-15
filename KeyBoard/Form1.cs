using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace KeyBoard
{
    public partial class Form1 : Form
    {
        Image<Gray, Byte> imageGrayscale;
        private Image<Gray, Byte> dest;
        private Image<Rgb, Byte> src_color;
        //private Image src_color;
        private Image<Gray, Byte> dest_gray;
        private Image<Gray, Byte> dest_binary;
        private Image<Gray, Byte> dest_contours;
        private Image<Gray, Byte> dest_rect;
        private Image<Rgb, Byte> zoom_Img;


        bool mouseClickFlag = false;

        double zoomPara = 1.0F;


        public Form1()
        {
            InitializeComponent();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Image img = Image.FromFile(@"C:\Users\Administrator\Desktop\11.png");
            pictureBox1.Image = img;
            
                //!!!!!!!!!!!!!!!!!!!!!!!!!
            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);   //file:为鼠标移动定义一个事件处理过程"Form1_MouseMove"   
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            pictureBox1.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);

     
        }

        private Point start = new Point();//矩形起点 
        private Point end = new Point();//矩形终点 
        private bool catchStart = false;
        //鼠标按下   
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!catchStart)
                {
                    catchStart = true;
                    start.X = e.X;
                    start.Y = e.Y;
                    end.X = e.X;
                    end.Y = e.Y;
                }
            }
        }

        
        //鼠标释放  
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            catchStart = false;
            Rectangle rect = new Rectangle(start.X, start.Y, end.X - start.X, end.Y - start.Y);
            Image ChildImg = AcquireRectangleImage(pictureBox1.Image, rect);
            Image<Rgb, Byte> imageSource = new Image<Rgb, byte>((Bitmap)ChildImg);
            imageGrayscale = imageSource.Convert<Gray, Byte>();
            pictureBox2.Image = imageGrayscale.ToBitmap();

            dest_gray = imageGrayscale;//!!!!!!!!!!!!!!!!!!
            dest_rect = dest_gray;

            src_color = new Image<Rgb, byte>((Bitmap)ChildImg);
        }

        public static Image AcquireRectangleImage(Image source, Rectangle rect)
        {
            if (source == null || rect.IsEmpty) return null;
            Bitmap bmSmall = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            //Bitmap bmSmall = new Bitmap(rect.Width, rect.Height, source.PixelFormat);            
            using (Graphics grSmall = Graphics.FromImage(bmSmall))
            {
                grSmall.DrawImage(source, new System.Drawing.Rectangle(0, 0, bmSmall.Width, bmSmall.Height), rect, GraphicsUnit.Pixel);
                grSmall.Dispose();
            }
            return bmSmall;
        }
        //鼠标移动   
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = this.pictureBox1.CreateGraphics();

            if (catchStart)
            {
                this.Refresh();
                end.X = e.X;
                end.Y = e.Y;
                Rectangle rect = new Rectangle(start.X, start.Y, end.X - start.X, end.Y - start.Y);
                g.DrawRectangle(new Pen(Color.White), rect);

                //获取image的缩略图
                // Image.GetThumbnailImageAbort myCallback =new Image.GetThumbnailImageAbort(ThumbnailCallback);
                // Bitmap myBitmap = new Bitmap("Climber.jpg");
                // Image myThumbnail = pictureBox1.Image.GetThumbnailImage(40, 40, myCallback, IntPtr.Zero);
                // g.DrawImage(myThumbnail, 150, 75);
                // Image *imgRect = img.GetThumbnailImage(end.X - start.X, end.Y - start.Y,null,null);
            }
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //trackBar1.ValueChanged += new (TrackBar_ValueChanged);
            int iPos = trackBar1.Value;
            Image<Gray, Byte> imageGray = new Image<Gray, Byte>(imageGrayscale.ToBitmap());//why not imageGrayscale?
            dest_binary = new Image<Gray, Byte>(imageGray.Width, imageGray.Height);//these two sententences can not be removed to the out of  trackBar1_Scroll?
            CvInvoke.cvThreshold(imageGray, dest_binary, iPos, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);
            pictureBox2.Image = dest_binary.ToBitmap();
            textBox1.Text = Convert.ToString(iPos);


            
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            
          
          //int n = CvInvoke.cvFindContours(dest,);
          IntPtr Dyncontour = new IntPtr();
          IntPtr Dynstorage = CvInvoke.cvCreateMemStorage(0);
          int m = 88;
          int n = CvInvoke.cvFindContours(dest_binary, Dynstorage, ref Dyncontour, m, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL,Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE,new Point(0,0));
          
          Seq<Point> DyncontourTemp1 = new Seq<Point>(Dyncontour,null);
          Seq<Point> DyncontourTemp = DyncontourTemp1;
           for (; DyncontourTemp1 != null && DyncontourTemp1.Ptr.ToInt32() != 0; DyncontourTemp1 = DyncontourTemp1.HNext)
           {

               ;
               Rectangle rect = CvInvoke.cvBoundingRect(DyncontourTemp1, 0);
               CvInvoke.cvRectangleR(src_color, rect, new MCvScalar(255, 0, 0), 1, Emgu.CV.CvEnum.LINE_TYPE.CV_AA, 0);
               //CvInvoke.cvDrawContours(dest, DyncontourTemp1, new MCvScalar(128, 128, 128), new MCvScalar(255, 255, 255), 0, 1, Emgu.CV.CvEnum.LINE_TYPE.CV_AA, new Point(0, 0));

           }

           pictureBox2.Image = src_color.ToBitmap();
          //dest.Draw(bondContour, new MCvScalar(128,128,128),2);
           

   /*        Point point = new Point(1, 1);

           Emgu.CV.Seq<System.Drawing.Point> qqq = null;

           dest.Draw(, new Bgr(0,0,0), new Bgr(0,0,0), 2, 2);
           Rectangle rect = CvInvoke.cvBoundingRect(bondContour,true);
           Point leftPoint = new Point(rect.X,rect.Y);
           Point rightPoint = new Point(rect.X + rect.Width, rect.Y + rect.Height); 
           CvInvoke.cvRectangle(dest, leftPoint, rightPoint,new MCvScalar(250,0,0),1, Emgu.CV.CvEnum.LINE_TYPE.FOUR_CONNECTED, 0);
           pictureBox2.Image = dest.ToBitmap();
            
            //Image<Gray,Byte> imageTemp=imageThreshold.Copy();
            IntPtr storage = CvInvoke.cvCreateMemStorage(0);
            IntPtr ptrFirstChain = IntPtr.Zero;
            int total = CvInvoke.cvFindContours(dest.Ptr, storage, ref ptrFirstChain, 6553, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, new Point(0, 0));

            int aa = total;

           */ // CvInvoke.cvEndFindContours();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }



        //mouse_wheel
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            
            //if (mouseClickFlag == true)
            {
                if (e.Delta > 0) 
                { 
                    label1.Text = "正在向上滚动滑轮";
                    zoomPara += 0.05;
                      
                } 
  
                else
                { 
                    label1.Text = "正在向下滚动滑轮";
                    zoomPara -= 0.05;

                }
                //Size img_size = new Size((int)(src_color.Width*zoomPara),(int)(src_color.Height*zoomPara));
               // Bitmap zoom_Img = CvInvoke.cvCreateImage(img_size,Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_8U,3);

                Bitmap bit = new Bitmap((int)(src_color.Width * zoomPara), (int)(src_color.Height * zoomPara));
                zoom_Img = new Image<Rgb, byte>(bit);
                CvInvoke.cvResize(src_color, zoom_Img, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

                pictureBox2.Image = zoom_Img.ToBitmap();

             }
        }


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            label1.Text = "1111";
            this.pictureBox1.Focus();

            if (mouseClickFlag == false)
                mouseClickFlag = true;

            else
                mouseClickFlag = false;
        }


    }
}
