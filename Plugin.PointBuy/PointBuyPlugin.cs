#region

using Eldora.PluginAPI;
using Newtonsoft.Json;
using Serilog;

#endregion

namespace Plugin.PointBuy;

public sealed class PointBuyPlugin : AbstractPlugin
{
	public static PointBuyPlugin Instance { get; private set; } = null!;

	private readonly Dictionary<string, PointBuyConfigObject> _pointBuyConfigs = new();

	private string _pointBuyConfigsPath = null!;
	private string _defaultConfigPath = null!;

	private PointBuyConfigObject? _selectedConfig;

	public PointBuyConfigObject DefaultConfig { get; private set; } = null!;

	public override List<IComponent> GetComponents()
	{
		return new List<IComponent>
		{
				new PointBuyPage()
		};
	}

	public override void OnLoad()
	{
		Instance = this;

		_pointBuyConfigsPath = Path.Join(PluginFolder, "configs");

		if (!Directory.Exists(_pointBuyConfigsPath))
		{
			Log.Information("Creating configs directory for Plugin {PluginName}", PluginFolder);
			Directory.CreateDirectory(_pointBuyConfigsPath);
		}

		// TODO: Extract all configs from default configs

		// Loads all files from the config path as configurations
		foreach (var file in Directory.GetFiles(_pointBuyConfigsPath))
		{
			var config = JsonConvert.DeserializeObject<PointBuyConfigObject>(File.ReadAllText(file));

			if (config == null)
			{
				Log.Error("Failed to deserialize PointBuyConfig from file {File}", file);
				continue;
			}

			_pointBuyConfigs.Add(Path.GetFileNameWithoutExtension(file), config);
		}

		// Log all configs
		foreach (var config in _pointBuyConfigs)
		{
			Log.Information("Loaded PointBuyConfig {ConfigName}", config.Key);
			Log.Information("\t {Config}", config.Value);
		}
	}

	public PointBuyConfigObject SelectedConfig()
	{
		return _selectedConfig ?? DefaultConfig;
	}
}