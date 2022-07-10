#region

using Eldora.PluginAPI;
using Newtonsoft.Json;
using Serilog;

#endregion

namespace Plugin.TokenCreator;

// TODO: Fix performance

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
	private const string TokenBordersPrefixKey = "DefaultBorders";

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
			"wavey_ring_1"
	};

	private static readonly List<string> DefaultMasks = new()
	{
			"10_sided_ring_mask",
			"12_sided_ring_mask",
			"abstract_triangle_mask",
			"cloud_ring_1_mask",
			"plain_hexagon_mask",
			"plain_octagon_mask",
			"plain_ring_mask",
			"plain_square_mask",
			"wavey_ring_1_mask"
	};

	public readonly List<BorderObject> Borders = new();

	public override void OnLoad()
	{
		Instance = this;

		BordersImagesPath = Path.Join(PluginFolder, "borders");

		if (!Directory.Exists(BordersImagesPath))
		{
			Directory.CreateDirectory(BordersImagesPath);
		}

		SaveDefaultBorders();
		LoadBorders();
	}

	/// <summary>
	///     Loads all borders from the folder.
	/// </summary>
	private void LoadBorders()
	{
		foreach (var file in Directory.GetFiles(BordersImagesPath, "*.json"))
			try
			{
				var border = JsonConvert.DeserializeObject<BorderObject>(File.ReadAllText(file));

				if (border == null)
				{
					Log.Error("Failed to load border {File}", file);
					continue;
				}

				if (!File.Exists(Path.Combine(BordersImagesPath, border.ImageName)))
				{
					Log.Error("Image {ImageName} not found ({Path})",
							border.ImageName,
							Path.Combine(BordersImagesPath, border.ImageName));
					continue;
				}

				if (!File.Exists(Path.Combine(BordersImagesPath, border.MaskName)))
				{
					Log.Error("Mask {MaskName} not found ({Path})",
							border.MaskName,
							Path.Combine(BordersImagesPath, border.MaskName));
					continue;
				}

				Borders.Add(border);

				Log.Information("Loaded border {BorderName} with (ImageName: {ImageName} - MaskName: {MaskName})",
						border.BorderName,
						border.ImageName,
						border.MaskName);
			}
			catch (JsonSerializationException e)
			{
				Log.Warning(e, "Failed to load border {File}", file);
			}
	}

	/// <summary>
	///     Saves the default borders to plugin path
	/// </summary>
	private void SaveDefaultBorders()
	{
		foreach (var defaultToken in DefaultTokens)
		{
			var jsonName = $"{defaultToken}.json";
			var imageName = $"{defaultToken}.png";

			var externalJsonPath = Path.Combine(BordersImagesPath, jsonName);
			var externalImagePath = Path.Combine(BordersImagesPath, imageName);

			Utils.WriteResourceToFileIfNotExists($"{TokenBordersPrefixKey}.{jsonName}",
					externalJsonPath);

			Utils.WriteResourceToFileIfNotExists($"{TokenBordersPrefixKey}.{imageName}",
					externalImagePath);
		}

		foreach (var imageName in DefaultMasks.Select(mask => $"{mask}.png"))
		{
			var externalPath = Path.Combine(BordersImagesPath, $"{imageName}");

			Utils.WriteResourceToFileIfNotExists($"{TokenBordersPrefixKey}.{imageName}",
					externalPath);
		}

	}

}