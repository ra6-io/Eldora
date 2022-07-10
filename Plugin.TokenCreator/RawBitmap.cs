#region

using System.Runtime.InteropServices;
using Eto.Drawing;
using Serilog;

#endregion

namespace Plugin.TokenCreator;

// TODO: Bitmap depth

public sealed class RawBitmap
{
	public Bitmap Bitmap { get; }

	private byte[] BitmapData { get; }

	public IEnumerable<RawBitmapPixelData> BitmapPixelDatas { get; }

	public RawBitmap(Bitmap bitmap)
	{
		Bitmap = bitmap;

		using var bitmapRaw = Bitmap.Lock();

		var resultBytes = bitmapRaw.ScanWidth * Bitmap.Height;
		BitmapData = new byte[resultBytes];

		Marshal.Copy(bitmapRaw.Data, BitmapData, 0, resultBytes);

		BitmapPixelDatas = Enumerable.Range(0, BitmapData.Length / 4).Select(i => new RawBitmapPixelData
		{
				B = BitmapData[i * 4 + 0],
				G = BitmapData[i * 4 + 1],
				R = BitmapData[i * 4 + 2],
				A = BitmapData[i * 4 + 3],
				IsBlack = BitmapData[i * 4 + 0] == 0 &&
				          BitmapData[i * 4 + 1] == 0 &&
				          BitmapData[i * 4 + 2] == 0,
				IsShade = BitmapData[i * 4 + 0] == BitmapData[i * 4 + 1] &&
				          BitmapData[i * 4 + 1] == BitmapData[i * 4 + 2],
				IsWhite = BitmapData[i * 4 + 0] == 255 &&
				          BitmapData[i * 4 + 1] == 255 &&
				          BitmapData[i * 4 + 2] == 255,
				Index = i,
				LocX = i % Bitmap.Width,
				LocY = i / Bitmap.Width
		});
	}

	public RawBitmap(int width, int height)
	{
		Bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgba);
		BitmapData = new byte[width * height * 4];

		BitmapPixelDatas = Enumerable.Range(0, BitmapData.Length / 4).Select(i => new RawBitmapPixelData
		{
				B = BitmapData[i * 4 + 0],
				G = BitmapData[i * 4 + 1],
				R = BitmapData[i * 4 + 2],
				A = BitmapData[i * 4 + 3],
				IsBlack = BitmapData[i * 4 + 0] == 0 &&
				          BitmapData[i * 4 + 1] == 0 &&
				          BitmapData[i * 4 + 2] == 0,
				IsShade = BitmapData[i * 4 + 0] == BitmapData[i * 4 + 1] &&
				          BitmapData[i * 4 + 1] == BitmapData[i * 4 + 2],
				IsWhite = BitmapData[i * 4 + 0] == 255 &&
				          BitmapData[i * 4 + 1] == 255 &&
				          BitmapData[i * 4 + 2] == 255,
				Index = i,
				LocX = i % Bitmap.Width,
				LocY = i / Bitmap.Width
		});
	}

	/// <summary>
	///     White will be masked ==> Will only be visible
	/// </summary>
	/// <param name="bitmap"></param>
	/// <param name="mask"></param>
	/// <returns></returns>
	public static RawBitmap Mask(RawBitmap bitmap, RawBitmap mask)
	{
		if (mask.BitmapData.Length > bitmap.BitmapData.Length)
		{
			Log.Information("Mask is larger than bitmap");
			return bitmap;
		}

		var result = new byte[mask.BitmapData.Length];

		mask.BitmapPixelDatas.AsParallel().ForAll(data =>
		{
			var i = data.Index;

			if (!data.IsWhite)
			{
				result[i * 4 + 3] = 0;
			}
			else
			{
				result[i * 4 + 0] = bitmap.BitmapData[i * 4 + 0];
				result[i * 4 + 1] = bitmap.BitmapData[i * 4 + 1];
				result[i * 4 + 2] = bitmap.BitmapData[i * 4 + 2];
				result[i * 4 + 3] = bitmap.BitmapData[i * 4 + 3];
			}
		});

		return FromByteData(result, mask.Bitmap.Width, mask.Bitmap.Height)!;
	}

	public RawBitmap Mask(RawBitmap mask)
	{
		return Mask(this, mask);
	}

	/// <summary>
	/// </summary>
	/// <param name="data"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <returns></returns>
	public static RawBitmap? FromByteData(byte[] data, int width, int height)
	{
		if (data.Length != width * height * 4)
		{
			Log.Information("Data length does not match width * height * 4");
			return null;
		}

		var bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgba);

		using (var bitmapRaw = bitmap.Lock())
		{
			Marshal.Copy(data, 0, bitmapRaw.Data, data.Length);
		}

		return new RawBitmap(bitmap);
	}

	public static byte[] Combine(RawBitmap bottom, RawBitmap top, BlendMode blendMode = BlendMode.Replace)
	{
		if (top.BitmapData.Length > bottom.BitmapData.Length)
		{
			return bottom.BitmapData;
		}

		var result = new byte[bottom.BitmapData.Length];

		// Ignores Alpha
		bottom.BitmapPixelDatas.AsParallel().ForAll(data =>
		{
			var i = data.Index;

			switch (blendMode)
			{
				case BlendMode.Replace:
					result[i * 4 + 0] = top.BitmapData[i * 4 + 0];
					result[i * 4 + 1] = top.BitmapData[i * 4 + 1];
					result[i * 4 + 2] = top.BitmapData[i * 4 + 2];
					break;
				case BlendMode.Addition:
					result[i * 4 + 0] = (byte)Math.Min(255, bottom.BitmapData[i * 4 + 0] + top.BitmapData[i * 4 + 0]);
					result[i * 4 + 1] = (byte)Math.Min(255, bottom.BitmapData[i * 4 + 1] + top.BitmapData[i * 4 + 1]);
					result[i * 4 + 2] = (byte)Math.Min(255, bottom.BitmapData[i * 4 + 2] + top.BitmapData[i * 4 + 2]);
					break;
				case BlendMode.Subtract:
					result[i * 4 + 0] = (byte)Math.Max(0, bottom.BitmapData[i * 4 + 0] - top.BitmapData[i * 4 + 0]);
					result[i * 4 + 1] = (byte)Math.Max(0, bottom.BitmapData[i * 4 + 1] - top.BitmapData[i * 4 + 1]);
					result[i * 4 + 2] = (byte)Math.Max(0, bottom.BitmapData[i * 4 + 2] - top.BitmapData[i * 4 + 2]);
					break;
				case BlendMode.Difference:
					result[i * 4 + 0] = (byte)Math.Abs(bottom.BitmapData[i * 4 + 0] - top.BitmapData[i * 4 + 0]);
					result[i * 4 + 1] = (byte)Math.Abs(bottom.BitmapData[i * 4 + 1] - top.BitmapData[i * 4 + 1]);
					result[i * 4 + 2] = (byte)Math.Abs(bottom.BitmapData[i * 4 + 2] - top.BitmapData[i * 4 + 2]);
					break;
				case BlendMode.Divide:
					result[i * 4 + 0] = (byte)Math.Min(255, bottom.BitmapData[i * 4 + 0] / top.BitmapData[i * 4 + 0]);
					result[i * 4 + 1] = (byte)Math.Min(255, bottom.BitmapData[i * 4 + 1] / top.BitmapData[i * 4 + 1]);
					result[i * 4 + 2] = (byte)Math.Min(255, bottom.BitmapData[i * 4 + 2] / top.BitmapData[i * 4 + 2]);
					break;
				case BlendMode.DarkenOnly:
					result[i * 4 + 0] = (byte)Math.Min(bottom.BitmapData[i * 4 + 0], top.BitmapData[i * 4 + 0]);
					result[i * 4 + 1] = (byte)Math.Min(bottom.BitmapData[i * 4 + 1], top.BitmapData[i * 4 + 1]);
					result[i * 4 + 2] = (byte)Math.Min(bottom.BitmapData[i * 4 + 2], top.BitmapData[i * 4 + 2]);
					break;
				case BlendMode.LightenOnly:
					result[i * 4 + 0] = (byte)Math.Max(bottom.BitmapData[i * 4 + 0], top.BitmapData[i * 4 + 0]);
					result[i * 4 + 1] = (byte)Math.Max(bottom.BitmapData[i * 4 + 1], top.BitmapData[i * 4 + 1]);
					result[i * 4 + 2] = (byte)Math.Max(bottom.BitmapData[i * 4 + 2], top.BitmapData[i * 4 + 2]);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(blendMode), blendMode, null);
			}
		});

		return result;
	}

	public static implicit operator Bitmap(RawBitmap b)
	{
		return b.Bitmap;
	}

	public enum BlendMode
	{
		Replace,
		Divide,
		Addition,
		Subtract,
		Difference,
		DarkenOnly,
		LightenOnly
	}

	public struct RawBitmapPixelData
	{
		public byte R { get; init; }
		public byte G { get; init; }
		public byte B { get; init; }
		public byte A { get; init; }

		public int LocX { get; init; }
		public int LocY { get; init; }
		public bool IsBlack { get; init; }
		public bool IsWhite { get; init; }
		public bool IsShade { get; init; }

		public int Index { get; init; }
	}
}