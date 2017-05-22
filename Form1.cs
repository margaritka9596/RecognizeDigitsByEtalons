using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //numberOfClasses & numberOfComponents
        int M, N;
        List<Bitmap> etalon = new List<Bitmap> { };
        List<string> labels = new List<string> { };
        //input signal
        List<int> X = new List<int> { };
        List<double> W = new List<double>();
        Bitmap experimentBitmap;
        static string pathWMatrix = @"C:\Users\Margo\Desktop\Mine\Study\m1_1\2_term\image processing\NeuronNetsTask1From4thCourse\WindowsFormsApplication1\W_array.txt";

        public Form1()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            int width = flowLayoutPanel1.ClientSize.Width;
            int height = flowLayoutPanel1.ClientSize.Height;
            Bitmap img1 = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img1);
            g.Clear(Color.White);
            g.Dispose();

           /* if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();

            pictureBox1.Image = img1;

            if (pictureBox2.Image != null)
                pictureBox2.Image.Dispose();

            pictureBox2.Image = img1;*/

        }
        private int[] binarizeBitmap(Bitmap image)
        {
            //задаем значение
            double backgroundColor = 1.0;
                   
            //бинаризация
            int h = image.Height;
            int w = image.Width;
           
            // Bitmap dst = new Bitmap(w, h);
                    
            int[] binarized = new int[h * w + 1];
            for (int j = 0; j < h; ++j)
                for (int i = 0; i < w; ++i)
                {
                    if (image.GetPixel(i, j).GetBrightness() < backgroundColor)
                        binarized[j * w + i] = 1;
                    else
                        binarized[j * w + i] = 0;
                    //dst.SetPixel(i, j, image.GetPixel(i, j).GetBrightness() < backgroundColor ? System.Drawing.Color.Black : System.Drawing.Color.White);
                }
            return binarized;
         }
        private double activationFunc(double x)
        {
            double res = 0, F = 9.9, alpha = 1/2.0;
            if (x < 0)
                res = 0;
            else
            {
                if (x > 0)
                    res = x;
                /*if (x > F)
                    res = 1;
                else
                    res = alpha * x;*/
            }
            return res;
        }
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Multiselect = true;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                foreach (string filename in openFileDialog1.FileNames)
                {
                    try
                    {
                        PictureBox pb = new PictureBox();
                        Image loadedImage = Image.FromFile(filename);
                        pb.Height = loadedImage.Height;
                        pb.Width = loadedImage.Width;
                        pb.Image = loadedImage;
                        flowLayoutPanel1.Controls.Add(pb);
                        Bitmap image = new Bitmap(filename);

                        //добавили все эталонные изображения в List<Bitmap>
                        //filename.Substring(101, filename.Length);
                        labels.Add(filename.Substring(filename.Length - 5, 1));
                        etalon.Add(image);
                    }
                    catch (IOException)
                    {
                    }
                }
             }
        }
        private void trainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //numberOfClasses
            M = etalon.Count();
            int[] el = new int[]{};
            
            //create matrix
            foreach (Bitmap img in etalon)
            {
                el = binarizeBitmap(img);
                for (int i = 0; i < el.Length; ++i)
                    X.Add(el[i]);
            }
            //numberOfComponents
            N = el.Length;

           /* using (StreamWriter writer = new StreamWriter(File.Open("C:/Users/Margo/Desktop/WindowsFormsApplication1/res/X_array.txt", FileMode.Create)))
            {

                for (int j = 0; j < M; ++j)
                {
                    writer.Write("---->" + N + "__" +  j + " = ");
                    for (int i = 0; i < N; ++i)
                    {
                        writer.Write(X[j * N + i]);
                    }
                    writer.Write("\n");
                }
            }*/
            //Create matrix W
            for (int j = 0; j < M; ++j)
                for (int i = 0; i < N; ++i)
                    W.Add((1 / (2.0)) * X[j * N + i]);

            using (StreamWriter writer = new StreamWriter(File.Open(pathWMatrix + "", FileMode.Create))) //"C:/Users/Margo/Desktop/WindowsFormsApplication1/res/W_array.txt"
            {
                for (int j = 0; j < M; ++j)
                {
                    writer.Write("---->" + j + " = ");
                    for (int i = 0; i < N; ++i)
                        writer.Write(W[j * N + i]);
                    writer.Write("\n");
                }
            }

            string message = "Net is ready. Continue?";
            string caption = "Message";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.
            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.No)
            {
                // Closes the parent form.
                this.Close();
            }
        }
        private void experimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //testing element, wrong signal
            //Bitmap experimentBitmap;

            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    PictureBox pb = new PictureBox();
                    Image loadedImage = Image.FromFile(file);
                    pb.Height = loadedImage.Height;
                    pb.Width = loadedImage.Width;
                    pb.Image = loadedImage;
                    flowLayoutPanel2.Controls.Add(pb);
                    experimentBitmap = new Bitmap(file);
                }
                catch (IOException)
                {
                }
            }
            int[] experiment = binarizeBitmap(experimentBitmap);

            //trouble maybe here
            //Net works
            //first floor
            double[] y1 = new double[M];
            for (int j = 0; j < M; ++j)
            {
                double sum = 0;
                for(int i = 0; i < N; ++i)
                {
                     sum += W[j * N + i] * experiment[i];
                }
                y1[j] = sum + N / (2.0);
            }
            /*using (StreamWriter writer = new StreamWriter(File.Open("C:/Users/Margo/Desktop/WindowsFormsApplication1/res/y1.txt", FileMode.Create)))
            {

                for (int j = 0; j < M; ++j)
                {
                    writer.Write("\nj = " + j + " ->");
                    writer.Write(y1[j]);
                }
            }*/
            //second floor
            double[] y2_prev = new double[M];
            y2_prev = y1;
            
            double[] y2_next = new double[M];

            /*using (StreamWriter writer = new StreamWriter(File.Open("C:/Users/Margo/Desktop/WindowsFormsApplication1/res/y2_next_____.txt", FileMode.Create)))
            {
                for (int k = 0; k < M; ++k)
                {
                    writer.Write("\n");
                    writer.Write(y2_next[k]);
                }
            }*/

            double eps = 0.05; // 0 <eps< 1/M
            int deadline = 1;
            int diffComp = deadline + 1;
            int count = 0;

           
            while (diffComp > deadline)
            {
                diffComp = 0;
                //покоординатно считаем новый вектор
                for (int j = 0; j < M; ++j)
                {
                    double argument = y2_prev[j];
                    double sum = 0;
                    /*for (int i = 0; (i < M) && (i != j); ++i)*/
                    for (int i = 0; i < M; ++i)
                    {
                        if(i != j)
                            sum += y2_prev[i];
                    }
                    sum *= eps;
                    argument -= sum;
                   
                    y2_next[j] = activationFunc(argument);
                    if (y2_prev[j] != y2_next[j])
                        diffComp++;
                   
                   /* using (StreamWriter writer = new StreamWriter(File.Open("C:/Users/Margo/Desktop/WindowsFormsApplication1/res/both_iter" + count + "_j_" + j + ".txt", FileMode.Create)))
                    {
                        writer.Write("diffcomp = " + diffComp + "____");
                        for (int k = 0; k < M; ++k)
                        {
                            writer.Write("\nj = " + j + " ->");
                            writer.Write(y2_prev[k] + "_&_" + y2_next[k] + "_arg_" + argument + "_sum_" + sum);
                        }
                    }*/
                }
                
                y2_next.CopyTo(y2_prev, 0);
                /*using (StreamWriter writer = new StreamWriter(File.Open("C:/Users/Margo/Desktop/WindowsFormsApplication1/res/y2_prev_iter" + count + ".txt", FileMode.Create)))
                {
                    for (int k = 0; k < M; ++k)
                    {
                        writer.Write("\n");
                        writer.Write(y2_prev[k]);
                    }
                }*/
                //--->
                count++;
                /*string message_1 = "Continue?" + "Iteration number " + count + ", diff = " + diffComp;
                string caption = "Message";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult r;
                // Displays the MessageBox.
                r = MessageBox.Show(message_1, caption, buttons);
                if (r == System.Windows.Forms.DialogResult.No)
                {
                    // Closes the parent form.
                    this.Close();
                }*/
                //<---
            }
            
            //find class number
            double max = 0;
            int classNum = -1;
            for (int k = 0; k < M; ++k)
            {
                if (y2_next[k] > max)
                {
                    max = y2_next[k];
                    classNum = k;
                }
                else
                {
                    if (y2_next[k] == max)
                        classNum = -2;
                }
            }
            string message;
            if (classNum == -2)
                message = "Net can't determine the class of experiment object =(";
            else
                message = "Class number is " + labels[classNum] + " ! =)";
                //"Are you sure that you would like to close the form?";
            //const string caption = "Form Closing";
            var res = MessageBox.Show(message);/* caption,*/
                                        /* MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);*/
            
            // If the no button was pressed ...
            /*if (res == DialogResult.No)
            {
                // cancel the closure of the form.
                e.Cancel = true;
            }*/
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
