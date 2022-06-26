#region

using Eto.Drawing;

#endregion

namespace Plugin.TokenCreator;

public static class BitmapExtensions
{
	public static PointF Center(this Image image)
	{
		var size = image.Size;
		return new PointF(size.Width / 2f, size.Height / 2f);
	}

	/// <summary>
	///     Extracts a portion of the bitmap with a format
	/// </summary>
	/// <param name="bitmap"></param>
	/// <param name="format"></param>
	/// <param name="rectangle"></param>
	/// <returns></returns>
	public static Bitmap CloneAndExtract(this Bitmap bitmap, PixelFormat format, Rectangle rectangle)
	{
		var result = new Bitmap(rectangle.Width, rectangle.Height, format);

		using var g = new Graphics(result);
		g.DrawImage(bitmap, rectangle, new PointF(0, 0));

		return result;
	}

	/// <summary>
	///     Clones the bitmap with a format
	/// </summary>
	/// <param name="bitmap"></param>
	/// <param name="format"></param>
	/// <returns></returns>
	public static Bitmap Clone(this Bitmap bitmap, PixelFormat format)
	{
		var result = new Bitmap(bitmap.Width, bitmap.Height, format);

		using var g = new Graphics(result);
		g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new PointF(0, 0));

		return result;
	}

	/// <summary>
	///     Creates a new raw bitmap
	/// </summary>
	/// <param name="bitmap"></param>
	/// <returns></returns>
	public static RawBitmap MakeRaw(this Bitmap bitmap)
	{
		return new RawBitmap(bitmap);
	}
}