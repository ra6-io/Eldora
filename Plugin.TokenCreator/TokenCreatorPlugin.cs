using System.Reflection;
using DnDHelperV2.PluginAPI;
using Eto.Drawing;
using Newtonsoft.Json;
using Serilog;

namespace Plugin.TokenCreator;

public sealed class TokenCreatorPlugin : AbstractPlugin
{
	public static TokenCreatorPlugin Instance { get; private set; } = null!;

	public override List<IComponent> GetComponents()
	{
		return new List<IComponent>
		{
				new TokenCreatorPage()
		};
	}

	public string BordersImagesPath { get; private set; } = null!;

	public const string TOKEN_BORDERS_PREFIX_KEY = "Plugin.TokenCreator.DefaultBorders";

	private static readonly List<string> DefaultTokens = new()
	{
			"10_sided_ring_1",
			"12_sided_ring_1",
			"abstract_double_ring_1",
			"abstract_double_ring_1_flat",
			"abstract_octagon_1",
			"abstract_octagon_1_flat",
			"abstract_ring_1",
			"abstract_ring_1_flat",
			"abstract_ring_2",
			"abstract_ring_2_flat",
			"abstract_ring_3",
			"abstract_ring_3_flat",
			"abstract_triangle_1",
			"cloud_ring_1",
			"double_ring_1",
			"fifthed_border_medium",
			"fifthed_border_medium_greyscale",
			"plain_hexagon_1",
			"plain_octagon_1",
			"plain_ring_1",
			"plain_ring_1_with_arrow_1",
			"plain_ring_2",
			"plain_ring_3",
			"plain_square_1",
			"plain_square_2",
			"spike_ring_1",
			"spike_ring_2",
			"wavey_ring_1",
	};

	private static readonly List<string> Masks = new()
	{
			"10_sided_ring_mask",
			"12_sided_ring_mask",
			"abstract_triangle_mask",
			"cloud_ring_1_mask",
			"plain_hexagon_mask",
			"plain_octagon_mask",
			"plain_ring_mask",
			"plain_square_mask",
			"wavey_ring_1_mask",
	};

	public readonly List<Border> Borders = new();

	public override void OnLoad()
	{
		Instance = this;

		BordersImagesPath = Path.Join(PluginFolder, "borders");

		if (!Directory.Exists(BordersImagesPath))
		{
			Directory.CreateDirectory(BordersImagesPath);
		}

		SaveDefaultBorders();

		foreach (var file in Directory.GetFiles(BordersImagesPath, "*.json"))
		{
			var border = JsonConvert.DeserializeObject<Border>(File.ReadAllText(file));

			if (border != null) Borders.Add(border);
			else Log.Error("Failed to load border {File}", file);
		}
	}

	/// <summary>
	/// Saves the default borders to plugin path
	/// </summary>
	private void SaveDefaultBorders()
	{
		var assembly = Assembly.GetExecutingAssembly();

		foreach (var defaultToken in DefaultTokens)
		{
			var jsonName = $"{defaultToken}.json";
			var imageName = $"{defaultToken}.png";

			var externalJsonPath = Path.Combine(BordersImagesPath, jsonName);
			var externalImagePath = Path.Combine(BordersImagesPath, imageName);


			if (!File.Exists(externalJsonPath))
			{
				Log.Information("Copying {JsonName} to {External}", jsonName, externalJsonPath);
				using var jsonStream = assembly.GetManifestResourceStream($"{TOKEN_BORDERS_PREFIX_KEY}.{jsonName}")!;
				using var jsonStreamReader = new StreamReader(jsonStream);
				var jsonBuf = new byte[jsonStream.Length];
				var readJsonBytes = jsonStream.Read(jsonBuf, 0, jsonBuf.Length);

				File.WriteAllBytes(externalJsonPath, jsonBuf);
				Log.Information("Read {Bytes} bytes from {Json}", readJsonBytes, jsonName);
			}

			if (!File.Exists(externalImagePath))
			{
				Log.Information("Copying {ImageName} to {External}", imageName, externalImagePath);
				using var imageStream = assembly.GetManifestResourceStream($"{TOKEN_BORDERS_PREFIX_KEY}.{imageName}")!;
				using var imageStreamReader = new StreamReader(imageStream);
				var imageBuf = new byte[imageStream.Length];
				var readImageBytes = imageStream.Read(imageBuf, 0, imageBuf.Length);
				File.WriteAllBytes(externalImagePath, imageBuf);
				Log.Information("Read {Bytes} bytes from {Image}", readImageBytes, imageName);
			}
		}

		foreach (var imageName in Masks.Select(mask => $"{mask}.png"))
		{
			using var manifestResourceStream =
					assembly.GetManifestResourceStream($"{TOKEN_BORDERS_PREFIX_KEY}.{imageName}")!;
			using var streamReader = new StreamReader(manifestResourceStream);
			var buf = new byte[manifestResourceStream.Length];
			var bytes = manifestResourceStream.Read(buf, 0, buf.Length);
			File.WriteAllBytes(Path.Combine(BordersImagesPath, imageName), buf);
			Log.Information("Read {Bytes} bytes from {Mask}", bytes, imageName);
		}

	}

}