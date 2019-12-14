using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace _08._03._2014
{
    public partial class Form1 : Form
    {
        private class Decart // Tuple class is available since NET 4.0 
        {
            public int xx;
            public int yy;
        }

        int scrW = 0;
        int scrH = 0;
        int pictCount = 0;

        const int ID_BLINK_COUNT = 3;
        readonly string IDS_TAG_BLINK = "L4";
        Dictionary<Label, string> dict = null;
        delegate void AddFlowerDelegate();
        delegate void AddFlowerXYDelegate(Decart dec);
        delegate void AddTextDelegate(Label lb, char ch);

        AddFlowerDelegate addFlower;
        AddFlowerXYDelegate addFlowerXY;
        AddTextDelegate addTextDelegate = new AddTextDelegate((l, ch) => l.Text += ch );

        public Form1()
        {
            InitializeComponent();

            this.addFlower = new AddFlowerDelegate(this.addRandomFlower);
            this.addFlowerXY = new AddFlowerXYDelegate(this.addRandomFlowerXY);

            this.dict = new Dictionary<Label, string>()
            {
               { this.label1, "Наши милые лииингвистки, ..." },
               { this.label2, "Вы мудры как совы и прекрасны как бабочки..." },
               { this.label3, "Поздравляем вас со светлым праздником Весны 8 Марта !!!" },
               { this.label4, "Ваши програаааммисты! :)" }
            };
        }

        private void setLabelX(Label lb)
        {
            int lw = TextRenderer.MeasureText(lb.Text, lb.Font).Width;
            int sw = this.scrW;

            if (lb.Tag == null)
            {
                lb.Location = new Point((sw - lw) / 2, lb.Location.Y);
            }
            else if (string.Equals((string)lb.Tag, IDS_TAG_BLINK))
            {
                int l3w = TextRenderer.MeasureText(label3.Text, lb.Font).Width;
                int l3x = this.label3.Location.X;
                lb.Location = new Point(l3x + (l3w - lw), lb.Location.Y);
            }
        }

        private void label_TextChanged(object sender, EventArgs e)
        {
            this.setLabelX((Label)sender);
            ((Label)sender).BringToFront();
        }

        static int staticCount = 0;

        private void timerEventShow(Object myObject, EventArgs myEventArgs)
        {
            this.myTimer.Stop();
            Random random = new Random();
            staticCount++;

            foreach(Control c in this.Controls)
            {
                if (c is Label)
                {
                    if (string.Equals((string)c.Tag, IDS_TAG_BLINK))
                    {
                        if (staticCount == ID_BLINK_COUNT)
                        {
                            c.Visible = !c.Visible;
                            staticCount = 0;
                        }
                    }
                    else
                    {
                        c.ForeColor = Color.FromArgb(random.Next(byte.MaxValue), random.Next(byte.MaxValue), random.Next(byte.MaxValue));
                    }
                }
            }
            myTimer.Enabled = true;
        }

        private Random flRnd = new Random();

        private int getRandom(int x1, int x2)
        {
            return flRnd.Next(x1, x2);
        }

        private string GetPictureName()
        {
            return "pbox" + (++this.pictCount);
        }

        private Random imgRnd = new Random();

        private System.Drawing.Bitmap randomImage
        {
            get
            {
                string[] images =
                {
                    "_1_images", "_2_images", "_3_images", "_4_images",
                    "images", "images1", "images3", "images4", "_5_images",
                };
                object obj = _08._03._2014.Properties.Resources.ResourceManager.GetObject(images[imgRnd.Next(0, images.Length)]);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        private void addRandomFlower()
        {
            try
            {
                PictureBox rnd = new System.Windows.Forms.PictureBox();
                rnd.Image = this.randomImage;
                rnd.Name = GetPictureName();

                int imY = rnd.Image.Size.Height;
                int imX = rnd.Image.Size.Width;

                rnd.Location = new System.Drawing.Point(getRandom(-imX, scrW), getRandom(-imY, scrH));
                rnd.Size = new System.Drawing.Size(imX, imY);
                this.Controls.Add(rnd);
                rnd.BringToFront();
            }
            catch { }
        }

        private void addRandomFlowerXY(Decart dec)
        {
            try
            {
                PictureBox rnd = new System.Windows.Forms.PictureBox();
                rnd.Image = this.randomImage;
                rnd.Name = GetPictureName();

                int imY = rnd.Image.Size.Height;
                int imX = rnd.Image.Size.Width;

                rnd.Location = new System.Drawing.Point(dec.xx, dec.yy);
                rnd.Size = new System.Drawing.Size(imX, imY);
                this.Controls.Add(rnd);

                dec.xx = dec.xx + imX;
            }
            catch { }
        }

        private void ThreadEntry()
        {
            this.scrW = Screen.PrimaryScreen.WorkingArea.Width;
            this.scrH = Screen.PrimaryScreen.WorkingArea.Height;

            Invoke(new AddFlowerDelegate(() => Cursor.Hide()));
            Decart dec = new Decart();
            Object[] obj = new Object[] { dec };

            for (dec.yy = 0; dec.yy < scrH; dec.yy += 100)
            {
                for (dec.xx = 0; dec.xx < scrW;)
                {
                    Invoke(this.addFlowerXY, obj);
                    System.Threading.Thread.Sleep(10);
                }
            }
            for (int fl = 0; fl < 250; fl++)
            {
                Invoke(this.addFlower);
                System.Threading.Thread.Sleep(10);
            }

            Invoke(new AddFlowerDelegate(() =>
                {
                    int lh = TextRenderer.MeasureText(this.label1.Text, label1.Font).Height * 2;
                    int sh = Screen.PrimaryScreen.WorkingArea.Height / 4;

                    this.label1.Location = new Point(label1.Location.X, sh);
                    this.label2.Location = new Point(label1.Location.X, sh + lh);
                    this.label3.Location = new Point(label1.Location.X, sh + lh * 2);
                    this.label4.Location = new Point(label1.Location.X, sh + lh * 4);
                }
                ));

            AddTextDelegate showLabel = new AddTextDelegate((lb, x) =>
                {
                    lb.Visible = true;                    
                }
            );

            foreach (KeyValuePair<Label, string> pair in this.dict)
            {
                Invoke(showLabel, new Object[] { pair.Key, ' ' });

                for (int i = 0; i < pair.Value.Length; i++)
                {
                    Invoke(this.addTextDelegate, new Object[]{ pair.Key, pair.Value[i] });
                    System.Threading.Thread.Sleep(50);
                }
            }
            Invoke(new AddFlowerDelegate(() =>
                {
                    Cursor.Show();
                    this.myTimer.Tick += this.timerEventShow;
                    this.myTimer.Interval = 100;
                    this.myTimer.Start();
                }
                ));
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            new Thread(ThreadEntry){ IsBackground = true }.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                case Keys.Enter:
                case Keys.Escape:
                    this.Close();                
                    break;

                default:
                    break;
            }
        }
    }
}