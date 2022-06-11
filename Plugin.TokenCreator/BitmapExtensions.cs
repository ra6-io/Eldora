using Eto.Drawing;
using Eto.IO;

namespace Plugin.TokenCreator;

public static class BitmapExtensions
{
	public static PointF Center(this Image image)
	{
		var size = image.Size;
		return new PointF(size.Width / 2f, size.Height / 2f);
	}

	public static Bitmap Extract(this Bitmap bitmap, int x, int y, int width, int height)
	{
		var result = new Bitmap(width, height, PixelFormat.Format32bppRgba);

		using var g = new Graphics(result);
		g.DrawImage(bitmap, new Rectangle(x, y, width, height), new PointF(0, 0));
		
		return result;
	}
}