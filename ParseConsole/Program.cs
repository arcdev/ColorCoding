using System.Drawing;
using System.IO;

namespace ParseConsole
{
	class Program
	{
		private const string SourceFile = @"c:\temp\ocr\a-tale-of-two-cities-R-full.bmp";
		private const string TargetFile = @"c:\temp\ocr\a-tale-of-two-cities-R-full.txt";

		static void Main(string[] args)
		{
			Parse(SourceFile, TargetFile);
			//System.Console.Read();
		}

		private static void Parse(string source, string target)
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