using DnDHelperV2.PluginAPI;
using Newtonsoft.Json;
using Serilog;

namespace Plugin.PointBuy;

public sealed class PointBuyPlugin : AbstractPlugin
{
	public static PointBuyPlugin Instance { get; private set; } = null!;

	private readonly Dictionary<string, PointBuyConfig> _pointBuyConfigs = new();

	private string _pointBuyConfigsPath = null!;
	private string _defaultConfigPath = null!;

	private PointBuyConfig? _selectedConfig;

	public PointBuyConfig DefaultConfig { get; private set; } = null!;

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

		// Default config
		DefaultConfig = new PointBuyConfig();

		_pointBuyConfigsPath = Path.Join(PluginFolder, "configs");
		_defaultConfigPath = Path.Join(_pointBuyConfigsPath, "rules_as_written.json");

		if (!Directory.Exists(_pointBuyConfigsPath))
		{
			Log.Information("Creating configs directory for Plugin {PluginName}", PluginFolder);
			Directory.CreateDirectory(_pointBuyConfigsPath);
		}

		// create default config if not exist
		if (!File.Exists(_defaultConfigPath))
		{
			File.WriteAllText(_defaultConfigPath,
					JsonConvert.SerializeObject(DefaultConfig, Formatting.Indented));
		}

		// Loads all files from the config path as configurations
		foreach (var file in Directory.GetFiles(_pointBuyConfigsPath))
		{
			var config = JsonConvert.DeserializeObject<PointBuyConfig>(File.ReadAllText(file));

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
	
	public PointBuyConfig SelectedConfig()
	{
		return _selectedConfig ?? DefaultConfig;
	}
}