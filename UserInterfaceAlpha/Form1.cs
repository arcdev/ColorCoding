using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace UserInterfaceAlpha
{
	//TODO: new form for each image

	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			_bgWorker.WorkerReportsProgress = true;
			_bgWorker.DoWork += BgWorkerOnDoWork;
			_bgWorker.ProgressChanged += BgWorkerOnProgressChanged;

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
			DrawFile_RandG(SourceFile, width, height, 9, 1, image => { _bgWorker.ReportProgress(0, image); });
//			DrawFile_RandG(SourceFile, width, height, 1, 0, image => { bgWorker.ReportProgress(0, image); });
//			DrawFile_RandG(SourceFile, width, height, 12, 0, image => { bgWorker.ReportProgress(0, image); });
		}

		private void BgWorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
		{
			pictureBox1.Image = (Image) progressChangedEventArgs.UserState;
			pictureBox1.Invalidate();
		}

		private const string SourceFile = @"C:\temp\ocr\a-tale-of-two-cities.txt";
//		private const string SourceFile = @"C:\Users\Aaron\Downloads\unfiled\1719 Auburn House 2.zip";

		private readonly BackgroundWorker _bgWorker = new BackgroundWorker();

		private void Form1_Load(object sender, EventArgs e)
		{
			BackColor = Color.White;


			_bgWorker.RunWorkerAsync();
		}

		private const int PauseMilliseconds = 100;
		private const int PopulatedR = 0xF0;
		private const int PopulatedG = 0x0F;
		// todo: EOF marker?

		private void DrawFile_RandG(string filename, int width, int height, int dotSize, int borderSize, Action<Image> progress)
		{
			var dWidth = (double) width;
			var dHeight = (double) height;


			var dotWithBorder = dotSize + borderSize; // dot include right & bottom border

			var horizDots = Math.Floor((dWidth - borderSize)/dotWithBorder);
			var vertDots = Math.Floor((dHeight - borderSize)/dotWithBorder);
			var valuesPerImage = (int) (horizDots*vertDots);
			valuesPerImage *= 2; // 2 channels (Red & Green)

			var dotBrush = new SolidBrush(Color.Transparent); // initial

			using (var reader = File.OpenRead(filename))
			{
				int readCount;
				do
				{
					var buffer = new byte[valuesPerImage];
					readCount = reader.Read(buffer, 0, valuesPerImage);
					//readCount = Math.Min(readCount, 2000);

					if (readCount > 0)
					{
						var img = new Bitmap(width, height);
						var g = Graphics.FromImage(img);
						var dotIndexPerImage = 0;

						for (var bufferIndex = 0; bufferIndex < readCount; bufferIndex++)
						{
							var y = (int) Math.Floor(dotIndexPerImage/horizDots); // grid Y-value
							y *= dotWithBorder; // actual placement
							y += borderSize; // bump down by borderSize (top border)
							var x = (int) Math.Floor(dotIndexPerImage%horizDots); // grid X-value
							x *= dotWithBorder; // actual placement
							x += borderSize; // bump over by borderSize (left border)

							var bVal = PopulatedR; // assume R has a value

							var rVal = buffer[bufferIndex];
							byte gVal = 0;
							if (readCount > bufferIndex + 1)
							{
								gVal = buffer[bufferIndex + 1];
								bVal |= PopulatedG;
								bufferIndex++; // skip ahead one more
							}
							var color = Color.FromArgb(rVal, gVal, bVal);
							dotBrush.Color = color;
							g.FillRectangle(dotBrush, x, y, dotSize, dotSize);
							dotIndexPerImage++;
						}
						g.Dispose();

						progress(img);
//						return;
						Thread.Sleep(PauseMilliseconds);
					}
				} while (readCount > 0);
			}
		}
	}
}