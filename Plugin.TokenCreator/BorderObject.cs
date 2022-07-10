#region

using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Eto.Drawing;
using Newtonsoft.Json;

#endregion

namespace Plugin.TokenCreator;

[JsonObject(MemberSerialization.OptIn)]
public sealed class BorderObject
{
	/// <summary>
	///     The chroma threshold
	/// </summary>
	public const double CHROMA_KEY_THRESHOLD = 0.05;

	/// <summary>
	///     The colors for the checker board pattern
	/// </summary>
	private static Color _checkerBoardColor1 = Colors.White;

	private static Color _checkerBoardColor2 = Colors.DarkGray;

	/// <summary>
	///     The name of the image inside the borders folder
	/// </summary>
	[JsonProperty("image", Required = Required.Always)]
	public string ImageName { get; set; } = "";

	[JsonProperty("mask", Required = Required.Always)]
	public string MaskName { get; set; } = "";

	/// <summary>
	///     The name of the border inside the editor
	/// </summary>
	[JsonProperty("name", Required = Required.Always)]
	public string BorderName { get; set; } = "";

	public override string ToString()
	{
		return $"{BorderName} ({ImageName}) - {MaskName}";
	}

	/// <summary>
	///     The image as bitmap
	/// </summary>
	public Bitmap ImageBitmap { get; private set; } = null!;

	/// <summary>
	///     The mask as raw bitmap
	/// </summary>
	public RawBitmap MaskBitmap { get; private set; } = null!;

	/// <summary>
	///     The preview image as bitmap
	/// </summary>
	public Bitmap CheckerboardPreviewBitmap { get; private set; } = null!;

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		ImageBitmap = new Bitmap(Path.Combine(TokenCreatorPlugin.Instance.BordersImagesPath, ImageName));
		var mask = new Bitmap(Path.Combine(TokenCreatorPlugin.Instance.BordersImagesPath, MaskName));

		var offsetX = (int)(mask.Width / 2.0f - ImageBitmap.Width / 2.0f);
		var offsetY = (int)(mask.Height / 2.0f - ImageBitmap.Height / 2.0f);

		MaskBitmap = new RawBitmap(mask.CloneAndExtract(PixelFormat.Format32bppRgba, new Rectangle(offsetX, offsetY, ImageBitmap.Width, ImageBitmap.Height)));
		CalculateCheckerboardPreview();
	}

	/// <summary>
	///     Calculates the preview image with a checkerboard
	/// </summary>
	public void CalculateCheckerboardPreview()
	{
		var previewBitmap = new Bitmap(ImageBitmap.Width, ImageBitmap.Height, PixelFormat.Format32bppRgba);

		var checkerBoardSize = previewBitmap.Width / 16;

		using (var bitmapData = previewBitmap.Lock())
		{
			var size = bitmapData.ScanWidth * previewBitmap.Height;

			var bytes = new byte[size];
			Marshal.Copy(bitmapData.Data, bytes, 0, size);

			MaskBitmap.BitmapPixelDatas.AsParallel().ForAll(data =>
			{
				if (data.IsWhite)
				{
					bytes[data.Index * 4 + 3] = 0;
				}
				else
				{
					var color = _checkerBoardColor1;

					if ((data.LocX / checkerBoardSize % 2 == 0) ^ (data.LocY / checkerBoardSize % 2 == 0))
					{
						color = _checkerBoardColor2;
					}

					bytes[data.Index * 4 + 0] = (byte)color.Bb;
					bytes[data.Index * 4 + 1] = (byte)color.Gb;
					bytes[data.Index * 4 + 2] = (byte)color.Rb;
					bytes[data.Index * 4 + 3] = 255;
				}
			});
			Marshal.Copy(bytes, 0, bitmapData.Data, size);
		}

		CheckerboardPreviewBitmap = previewBitmap;
	}
}