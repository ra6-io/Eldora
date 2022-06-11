using System.Runtime.InteropServices;
using Eto.Drawing;
using Newtonsoft.Json;

namespace Plugin.TokenCreator;

[JsonObject(MemberSerialization.OptIn)]
public sealed class Border
{

	/// <summary>
	/// The chroma threshold
	/// </summary>
	public const double CHROMA_KEY_THRESHOLD = 0.05;

	/// <summary>
	/// The name of the image inside the borders folder
	/// </summary>
	[JsonProperty("image")]
	public string ImageName { get; set; } = "";

	[JsonProperty("mask")]
	public string MaskName { get; set; } = "";
	
	/// <summary>
	/// The name of the border inside the editor
	/// </summary>
	[JsonProperty("name")]
	public string BorderName { get; set; } = "";

	public override string ToString()
	{
		return $"{BorderName} ({ImageName}) - {MaskName}";
	}

	/// <summary>
	/// Returns the image of the border
	/// </summary>
	/// <returns></returns>
	public Bitmap GetBitmap()
	{
		var path = Path.Combine(TokenCreatorPlugin.Instance.BordersImagesPath, ImageName);
		return new Bitmap(path);
	}

	/// <summary>
	/// Returns the mask for the border
	/// </summary>
	/// <returns></returns>
	public Bitmap GetMask()
	{
		var path = Path.Combine(TokenCreatorPlugin.Instance.BordersImagesPath, MaskName);
		return new Bitmap(path);
	}
	/*
	 * 		var chromaColor = GetChromaColor();
		var bmp = GetBitmap();

		using var data = bmp.Lock();

		var bytes = Math.Abs(data.ScanWidth) * bmp.Height;
		var rgba = new byte[bytes];

		Marshal.Copy(data.Data, rgba, 0, bytes);

		var pixels = Enumerable.Range(0, rgba.Length / 4).Select(x => new
		{
				B = rgba[x * 4],
				G = rgba[(x * 4) + 1],
				R = rgba[(x * 4) + 2],
				A = rgba[(x * 4) + 3],
				MakeTransparent = new Action(() => rgba[(x * 4) + 3] = 0)
		});

		pixels
				.AsParallel()
				.ForAll(p =>
				{
					// calculate the distance between the pixel and the chroma color
					var distance = Math.Sqrt(
							Math.Pow(p.R - chromaColor.R, 2) +
							Math.Pow(p.G - chromaColor.G, 2) +
							Math.Pow(p.B - chromaColor.B, 2)
						);
					
					// if the distance is less than the threshold, make the pixel transparent
					

				});

		Marshal.Copy(rgba, 0, data.Data, bytes);
		return bmp;
	 */
}