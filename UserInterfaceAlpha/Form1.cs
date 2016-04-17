using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace UserInterfaceAlpha
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			BackColor = Color.White;

			bgWorker.WorkerReportsProgress = true;
			bgWorker.DoWork += BgWorkerOnDoWork;
			bgWorker.ProgressChanged += BgWorkerOnProgressChanged;

			pictureBox1.Dock = DockStyle.Fill;
			WindowState = FormWindowState.Maximized;
			FormBorderStyle = FormBorderStyle.None;
		}

		private void BgWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
		{
			var width = pictureBox1.Width;
			var height = pictureBox1.Height;
			DrawFile(SourceFile, width, height, 10, 1, image => { bgWorker.ReportProgress(0, image); });
		}

		private void BgWorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
		{
			pictureBox1.Image = (Image) progressChangedEventArgs.UserState;
			pictureBox1.Invalidate();
		}

		private const string SourceFile = @"C:\temp\ocr\a-tale-of-two-cities.txt";
//		private const string SourceFile = @"C:\Users\Aaron\Downloads\unfiled\1719 Auburn House 2.zip";

		private readonly BackgroundWorker bgWorker = new BackgroundWorker();

		private void Form1_Load(object sender, EventArgs e)
		{
			bgWorker.RunWorkerAsync();
		}

		private void DrawFile(string filename, int width, int height, int dotSize, int borderSize, Action<Image> progress)
		{
			var dWidth = (double) width;
			var dHeight = (double) height;
			var nPerImage = borderSize == 0 ? (int)(dWidth * dHeight / dotSize ): (int) (dWidth*dHeight/dotSize/borderSize);


			using (var reader = File.OpenRead(filename))
			{
				int readCount;
				do
				{
					var buffer = new byte[nPerImage];
					readCount = reader.Read(buffer, 0, nPerImage);
//					readCount = Math.Min(readCount, 10);

					if (readCount > 0)
					{
						var img = new Bitmap(width, height);
						var g = Graphics.FromImage(img);

						for (var i = 0; i < readCount; i++)
						{
							var y = (int)(i/dWidth * dotSize) - borderSize;
							var x = (int)(i%dWidth * dotSize) - borderSize;
							var c = buffer[i];
							var color = Color.FromArgb(c, 0, 0);
							g.FillRectangle(new SolidBrush(color), x, y, dotSize-borderSize, dotSize-borderSize);
						}

						progress(img);
//						return;
						Thread.Sleep(500);
					}
				} while (readCount > 0);
			}
		}

		private void DrawFileAsSinglePixels(string filename, int width, int height, Action<Image> progress)
		{
			var nPerImage = width*height;

			using (var reader = File.OpenRead(filename))
			{
				int readCount;
				do
				{
					var buffer = new byte[nPerImage];
					readCount = reader.Read(buffer, 0, nPerImage);
//					readCount = Math.Min(readCount, 100);

					if (readCount > 0)
					{
						var img = new Bitmap(width, height);

						for (var i = 0; i < readCount; i++)
						{
							var y = i/(decimal) width;
							var x = i%(decimal) width;
							var c = buffer[i];
							var color = Color.FromArgb(c, 0, 0);
							img.SetPixel((int) x, (int) y, color);
						}

						progress(img);
						Thread.Sleep(500);
					}
				} while (readCount > 0);
			}
		}
	}
}