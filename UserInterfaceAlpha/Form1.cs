using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace UserInterfaceAlpha
{
	//TODO: new form for each image
	//TODO: layer the R & G units
	//	B & 0xF0 => means R has no value
	//	B & 0x0F => means G has no value

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
//			DrawFile_Ronly(SourceFile, width, height, 2, 0, image => { bgWorker.ReportProgress(0, image); });
//			DrawFile_RandG(SourceFile, width, height, 2, 0, image => { bgWorker.ReportProgress(0, image); });
//			DrawFile_RandG(SourceFile, width, height, 12, 1, image => { bgWorker.ReportProgress(0, image); });
			DrawFile_RandG(SourceFile, width, height, 10, 1, image => { bgWorker.ReportProgress(0, image); });
//			DrawFile_RandG(SourceFile, width, height, 12, 0, image => { bgWorker.ReportProgress(0, image); });
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

		private const int PauseMilliseconds = 100;
		private const int PopulatedR = 0xF0;
		private const int PopulatedG = 0x0F;
		//const int Empty = EmptyR | EmptyG;
		// todo: EOF marker?

		private void DrawFile_RandG(string filename, int width, int height, int dotSize, int borderSize, Action<Image> progress)
		{
			var dWidth = (double) width;
			var dHeight = (double) height;


			var dotWithBorder = dotSize + borderSize; // dot include right & bottom border

			var horizDots = Math.Floor((dWidth- borderSize) /dotWithBorder);
			var vertDots = Math.Floor((dHeight- borderSize) /dotWithBorder);
			var unitsPerImage = (int)(horizDots*vertDots);
			unitsPerImage *= 2; // 2 channels (Red & Green)

			var dotBrush = new SolidBrush(Color.Transparent); // initial

			using (var reader = File.OpenRead(filename))
			{
				int readCount;
				do
				{
					var buffer = new byte[unitsPerImage];
					readCount = reader.Read(buffer, 0, unitsPerImage);
					//readCount = Math.Min(readCount, 2000);

					if (readCount > 0)
					{
						var img = new Bitmap(width, height);
						var g = Graphics.FromImage(img);

						for (var i = 0; i < readCount; i++)
						{
							var y = (int)Math.Floor(i/horizDots); // grid Y-value
							y *= dotWithBorder; // actual placement
							y += borderSize; // bump down by borderSize (top border)
							var x = (int)Math.Floor(i %horizDots); // grid X-value
							x *= dotWithBorder; // actual placement
							x += borderSize; // bump over by borderSize (left border)

							var bVal = PopulatedR; // assume R has a value

							var rVal = buffer[i];
							byte gVal = 0;
							if (readCount > i + 1)
							{
								gVal = buffer[i + 1];
								bVal |= PopulatedG;
								i++; // skip ahead one more
							}
							var color = Color.FromArgb(rVal, gVal, bVal);
							dotBrush.Color = color;
							g.FillRectangle(dotBrush, x, y, dotSize, dotSize);
						}
						g.Dispose();

						progress(img);
//						return;
						Thread.Sleep(PauseMilliseconds);
					}
				} while (readCount > 0);
			}
		}

		private void DrawFile_RandG0(string filename, int width, int height, int dotSize, int borderSize, Action<Image> progress)
		{
			var dWidth = (double) width;
			var dHeight = (double) height;
			var nPerImage = borderSize == 0 ? (int)(dWidth * dHeight / dotSize ): (int) (dWidth*dHeight/dotSize/borderSize);
			nPerImage *= 2; // one for R and one for G


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

						// fill entire image with value that means "neither R-layer nor G-layer has data"
						//g.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, Empty)), 0, 0, width, height);

						for (var i = 0; i < readCount; i++)
						{
							var y = (int)(i/dWidth * dotSize);
							var x = (int)(i%dWidth * dotSize);

							var bVal = PopulatedR; // assume R has a value

							var rVal = buffer[i];
							byte gVal = 0;
							if (readCount < i + 1)
							{
								gVal = buffer[i + 1];
								bVal |= PopulatedG;
								i++; // skip ahead one more
							}
							var color = Color.FromArgb(rVal, gVal, bVal);
							g.FillRectangle(new SolidBrush(color), x, y, dotSize, dotSize);
							if (borderSize != 0)
							{
								g.DrawRectangle(new Pen(Color.White, 1), x, y, dotSize - borderSize, dotSize - borderSize);
							}
						}
						g.Dispose();

						progress(img);
//						return;
						Thread.Sleep(250);
					}
				} while (readCount > 0);
			}
		}

		private void DrawFile_Ronly(string filename, int width, int height, int dotSize, int borderSize, Action<Image> progress)
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