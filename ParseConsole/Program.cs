using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace ParseConsole
{
	class Program
	{
//		private const string SourceFile = @"c:\temp\ocr\a-tale-of-two-cities-R-full.bmp";
//		private const string TargetFile = @"c:\temp\ocr\a-tale-of-two-cities-R-full.txt";

		private const string SourceBase = @"c:\temp\ocr\a-tale-of-two-cities-RG-page-01";
		private const string SourceFile = SourceBase + ".bmp";
		private const string TargetFile = SourceBase + ".txt";

		static void Main(string[] args)
		{
			ParseRG(SourceFile, TargetFile, 10, 1);
			//System.Console.Read();
		}

		const int PopulatedR = 0xF0;
		const int PopulatedG = 0x0F;

		private static void ParseRG(string source, string target, int dotSize, int borderSize)
		{
			using (var writer = File.OpenWrite(target))
			{
				var img = (Bitmap) Image.FromFile(source);

				var width = img.Width;
				var height = img.Height;

				var dWidth = (double)width;
				var dHeight = (double)height;

				var dotWithBorder = dotSize + borderSize; // dot include right & bottom border

				var horizDots = Math.Floor((dWidth - borderSize) / dotWithBorder);
				var vertDots = Math.Floor((dHeight - borderSize) / dotWithBorder);

				for (var yDot = 0; yDot < vertDots; yDot++)
				{
					var y = yDot * dotWithBorder;
					y += borderSize; // bump down by border

					for (var xDot = 0; xDot < horizDots; xDot++)
					{
						var x = xDot*dotWithBorder;
						x += borderSize; // bump right by border

						var color = img.GetPixel(x, y);
						if (color.B == 0)
						{
							Debug.Fail("probably not a good read");
							break;
						}
						
						// assume R channel is always populated
						var rVal = color.R;
						writer.WriteByte(rVal);

						var bVal = color.B;
						if ((bVal & PopulatedG) == PopulatedG)
						{
							var gVal = color.G;
							writer.WriteByte(gVal);
						}
					}
				}
			}
		}

		private static void ParseR(string source, string target)
		{
			using (var writer = File.OpenWrite(target))
			{
				var img = (Bitmap) Image.FromFile(source);
				for (var y = 0; y < img.Height; y++)
				{
					for (var x = 0; x < img.Width; x++)
					{
						var color = img.GetPixel(x, y);
						if (color.B == 240)
						{
							break;
						}
						var b = color.R;
						writer.WriteByte(b);
					}
				}
			}
		}
	}
}