#region

using System.Runtime.InteropServices;
using Eto.Drawing;

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