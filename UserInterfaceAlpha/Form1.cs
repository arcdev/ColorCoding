using System;
using System.ComponentModel;
using System.Diagnostics;
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
			DrawFile(SourceFile, width, height, image =>
			{
				bgWorker.ReportProgress(0, image);
			});
		}

		private void BgWorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
		{
			pictureBox1.Image = (Image) progressChangedEventArgs.UserState;
			pictureBox1.Invalidate();
		}

		//		private const string SourceFile = @"C:\temp\tale-of-two-cities-01.txt";
//		private const string SourceFile = @"C:\temp\A-Tale-of-Two-Cities.txt";
//		private const string SourceFile = @"C:\temp\ocr\a-tale-of-two-cities.txt";
		private const string SourceFile = @"C:\Users\Aaron\Downloads\unfiled\1719 Auburn House 2.zip";

		private BackgroundWorker bgWorker = new BackgroundWorker();

		private void Form1_Load(object sender, EventArgs e)
		{
//			var width = pictureBox1.Width;
//			var height = pictureBox1.Height;
//			DrawFile(SourceFile, width, height);

			bgWorker.RunWorkerAsync();
		}

		private void DrawFile(string filename, int width, int height, Action<Image> progress)
		{
			var nPerImage = width*height;

//			using (var reader = new FileStream(filename, FileMode.Open))
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
							var y = i / (decimal)width;
							var x = i % (decimal)width;
							var c = buffer[i];
							var color = Color.FromArgb(c, 0, 0);
							img.SetPixel((int)x, (int)y, color);
						}

						progress(img);

						//						pictureBox1.Image = img;
						//						pictureBox1.Invalidate();
						//						Debug.WriteLine("batch " + DateTime.Now);
						Thread.Sleep(500);
					}
				} while (readCount > 0);
			}
		}

		private void DrawFile(string filename, int width, int height)
		{
			var nPerImage = width*height;

			using (var reader = File.OpenRead(filename))
			{
				int readCount;
				do
				{
					var buffer = new byte[nPerImage];
					readCount = reader.Read(buffer, 0, nPerImage);

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

						pictureBox1.Image = img;
						pictureBox1.Invalidate();
						Debug.WriteLine("batch " + DateTime.Now);
						Thread.Sleep(500);
					}
				} while (readCount > 0);
			}
		}

		private void DrawFile1(string filename)
		{
			var content = File.ReadAllText(filename);

			var width = pictureBox1.Width;
			var height = pictureBox1.Height;

			var img = new Bitmap(width, height);

			for (var i = 0; i < content.Length; i++)
			{
				var y = i/(decimal) width;
				var x = i%(decimal) width;
				var c = content[i];
				Debug.WriteLine("{0:000}, {3:000}x{4:000} = {1} = {2}", i, c, (int) c, x, y);
				var color = Color.FromArgb(c, 0, 0);
				img.SetPixel((int) x, (int) y, color);
			}

			pictureBox1.Image = img;
		}

		private void DrawFile0(string filename)
		{
			var content = File.ReadAllText(filename);

			var width = pictureBox1.Width;
			var height = pictureBox1.Height;

			var img = new Bitmap(width, height);

			for (var i = 0; i < content.Length; i++)
			{
				var y = i/(decimal) width;
				var x = i%(decimal) width;
				var c = content[i];
				Debug.WriteLine("{0:000}, {3:000}x{4:000} = {1} = {2}", i, c, (int) c, x, y);
				var color = Color.FromArgb(c, c, c);
				img.SetPixel((int) x, (int) y, color);
			}

			pictureBox1.Image = img;
		}
	}
}