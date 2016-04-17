using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserInterfaceAlpha
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private Graphics gForm;
		private const int blockSize = 20;
		private const int scale = 1;

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			gForm = e.Graphics;
		}

		private void btnGo_Click(object sender, EventArgs e)
		{
			var image = ConvertFileToBitmap(SourceFile);

			pictureBox1.Image = image;
			return;

			var scaledImage = new Bitmap(image.Width * scale, image.Height * scale);


			var g = Graphics.FromImage(scaledImage);
			g.InterpolationMode = InterpolationMode.Bilinear;
			g.DrawImage(image, 0, 0, scaledImage.Width, scaledImage.Height);
			pictureBox1.Image = scaledImage;
		}


		private const string SourceFile = @"C:\temp\tale-of-two-cities-01.txt";

		private Image ConvertFileToBitmap(string source)
		{
			var width = pictureBox1.Width/scale;
			var height = pictureBox1.Height/scale;

			//var content = File.ReadAllText(source);
			var content = "abcdefghijklmnopqrstuvwxyz";
			var colors = ConvertStringToColorGrid(content.ToCharArray(), width);

			var bitmap = new Bitmap(width, height);
			{
				for (int y = 0; y < colors.GetLength(1); y++)
				{
					for (int x = 0; x < colors.GetLength(0); x++)
				{
					
						var color = colors[x, y];
						bitmap.SetPixel(x, y, color);
					}
				}
				return bitmap;
			}

		}

		private Color[,] ConvertStringToColorGrid(char[] chars, int width)
		{
			var height = (int)Math.Ceiling(chars.Length/(double) width);
			var rtn = new Color[width,height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var c = chars[(x*y) + y];
					rtn[x,y] = ConvertCharToColor(c);
				}
			}

			return rtn;
		}

		private Color[,] ConvertStringToColorGrid0(char[] chars, int cols)
		{
			var rows = (int)Math.Ceiling(chars.Length/(double) cols);
			var rtn = new Color[cols,rows];

			var row = 0;
			var col = 0;
			foreach (var c in chars)
			{
				while (col < cols)
				{
					rtn[col, row] = ConvertCharToColor(c);
					col++;
				}
				row++;
			}

			return rtn;
		}

		private List<Color> ConvertStringToColorSequence(IEnumerable<char> chars)
		{
			return chars.Select(ConvertCharToColor).ToList();
		}

		private Color ConvertCharToColor(char c)
		{
			var rtn = Color.FromArgb(c, c, c);
			return rtn;
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		private void Form1_Load(object sender, EventArgs e)
		{
			DrawFile();
		}

		private void DrawFile()
		{
			var content = "It was the best of times".ToCharArray();

			var width = pictureBox1.Width;
			var height = pictureBox1.Height;

			var img = new Bitmap(width, height);

			for (int i = 0; i < content.Length; i++)
			{
				var y = i/(decimal) width;
				var x =i%(decimal) width;
				var c = content[i];
				Debug.WriteLine("{0:000}, {3:000}x{4:000} = {1} = {2}", i, c, (int)c, x, y);
				var color = Color.FromArgb(c, c, c);
				img.SetPixel((int)x, (int)y, color);
			}


			pictureBox1.Image = img;
		}

		private void DrawFile0()
		{
			var content = "It was the best of times".ToCharArray();

			var width = pictureBox1.Width;
			var height = pictureBox1.Height;

			var img = new Bitmap(width, height);


			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					var index = y*x + x;
					if (index >= content.Length)
					{
						break;
						Debug.WriteLine("{0:000} = {1} = {2}", index, "0", "");
						img.SetPixel(x, y, Color.Transparent);
						continue;
					}
					var c = content[y*x + x];
					Debug.WriteLine("{0:000} = {1} = {2}", index, c, (int)c);
					var color = Color.FromArgb(c, c, c);
					img.SetPixel(x, y, color);
				}
			}

			pictureBox1.Image = img;
		}
	}
}
