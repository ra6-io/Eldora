#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Eldora.InternalComponents;
using Eldora.PluginAPI;
using Eto.Forms;
using Newtonsoft.Json;
using Serilog;

#endregion

namespace Eldora;

public sealed class Eldora
{
	/// <summary>
	///     The root path for files
	/// </summary>
	public static readonly string RootPath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eldora");

	/// <summary>
	///     The path for plugins
	/// </summary>
	public static readonly string PluginPath = Path.Combine(RootPath, "plugins");

	/// <summary>
	///     The path for localizations
	/// </summary>
	public static readonly string LanguagePath = Path.Combine(RootPath, "languages");

	/// <summary>
	///     The path to the updater
	/// </summary>
	public static readonly string UpdaterPath = Path.Combine(RootPath, "updater");

	/// <summary>
	///     The path for logs
	/// </summary>
	public static readonly string LogPath = Path.Combine(RootPath, "logs");

	/// <summary>
	///     The path for the settings
	/// </summary>
	public static readonly string SettingPath = Path.Combine(RootPath, "settings.json");

	private const string DefaultResourceDataPath = "DefaultData";

	/// <summary>
	///     Gets parsed by config sometime
	/// </summary>
	private const bool DebugModeActivated = true;

	/// <summary>
	///     The version of the program
	/// </summary>
	public readonly Version Version = new(0, 0, 1);

	/// <summary>
	///     The plugin loader instance
	/// </summary>
	public PluginLoader PluginLoader { get; }

	/// <summary>
	///     The Singleton instance
	/// </summary>
	public static Eldora Instance { get; private set; }

	/// <summary>
	///     The platform handler
	/// </summary>
	public IPlatformHandler PlatformHandler { get; }

	/// <summary>
	///     The settings instance
	/// </summary>
	public SettingsObject Settings { get; private set; } = null!;

	private MainForm _mainForm;

	/// <summary>
	///     The main form
	/// </summary>
	public MainForm MainForm => _mainForm ??= new MainForm();

	public Eldora(IPlatformHandler platformHandler)
	{
		Instance = this;

		PlatformHandler = platformHandler;

		InitLogger();
		InitPaths();

		LoadSettings();
		LoadUpdater();

		PluginLoader = new PluginLoader();
		PluginLoader.LoadPlugins(PluginPath);

		// Enables all plugins
		PluginLoader.ForEachLoadedPlugin(data =>
		{
			try
			{
				data.plugin.OnEnable();
				Log.Information("Enabled plugin {Plugin}", data.metadata.PluginName);
			}
			catch (Exception e)
			{
				Log.Error(e, "Failed to enable plugin {Plugin}", data.metadata.PluginName);
			}
		});

		// Add components from plugins
		PluginLoader.ForEachLoadedPlugin(data =>
		{
			if (!data.metadata.HasTabComponents) return;

			try
			{
				var tabsToAdd = new List<TabPage>();

				foreach (var component in data.plugin.GetComponents())
				{
					if (component is not ITabComponent tabComponent) continue;

					var tab = new TabPage
					{
							Text = tabComponent.Title,
							Content = tabComponent.GetRootPanel()
					};
					Log.Debug("Adding tab {Title} from plugin {Plugin}", tabComponent.Title, data.metadata.PluginName);
					tabsToAdd.Add(tab);
				}

				tabsToAdd.ForEach(tab => ((TabControl)MainForm.Content).Pages.Add(tab));
			}
			catch (Exception e)
			{
				Log.Error(e, "Failed to add tabs from plugin {Plugin}", data.metadata.PluginName);
			}
		});

		// Adds all pages from internal components
		foreach (var type in Assembly.GetExecutingAssembly().GetExportedTypes())
		{
			if (type.GetInterface(nameof(IInternalTabComponent)) == null) continue;
			var tabComponent = (IInternalTabComponent)Activator.CreateInstance(type)!;

			var tab = new TabPage
			{
					Text = tabComponent.Title,
					Content = tabComponent.GetRootPanel(),
					// Visible if it is a debug page and Debug is activated or it is allways visible
					Visible = (tabComponent.IsOnlyDebugVisible && DebugModeActivated) || !tabComponent.IsOnlyDebugVisible
			};

			Log.Debug("Adding tab {Title} from internal", tabComponent.Title);
			((TabControl)MainForm.Content).Pages.Add(tab);
		}

		// Disables all plugins on closing
		MainForm.Closing += (_, _) =>
		{
			PluginLoader.ForEachLoadedPlugin(data =>
			{
				try
				{
					data.plugin.OnDisable();
					Log.Information("Disabled plugin {Plugin}", data.metadata.PluginName);
				}
				catch (Exception e)
				{
					Log.Error(e, "Failed to disable plugin {Plugin}", data.metadata.PluginName);
				}
			});
		};

		// Unloads all plugins on closed
		MainForm.Closed += (_, _) =>
		{
			PluginLoader.ForEachLoadedPlugin(data =>
			{
				try
				{
					data.plugin.OnUnload();
					Log.Information("Unloaded plugin {Plugin}", data.metadata.PluginName);
				}
				catch (Exception e)
				{
					Log.Error(e, "Failed to unload plugin {Plugin}", data.metadata.PluginName);
				}
			});

			Log.CloseAndFlush();
		};
	}

	/// <summary>
	///     Loading the updater
	/// </summary>
	private void LoadUpdater()
	{
		var assembly = Assembly.GetCallingAssembly();
		using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{DefaultResourceDataPath}.Updater.Updater.zip")!;
		//		using var streamReader = new StreamReader(stream);
		//		var buf = new byte[stream.Length];
		//		var bytes = stream.Read(buf, 0, buf.Length);

		Log.Information("Extracting Updater");
		using var file = new ZipArchive(stream);
		file.ExtractToDirectory(UpdaterPath, true);

		Log.Information($"v{Version.ToString(3)} Wpf \"{Directory.GetCurrentDirectory()}\"");

		var updaterPath = Path.Combine(UpdaterPath, "Eldora.Updater.exe");
		Process.Start(updaterPath, $"v{Version.ToString(3)} Wpf \"{Directory.GetCurrentDirectory()}\"");
	}

	/// <summary>
	///     Loads the settings
	/// </summary>
	private void LoadSettings()
	{
		Utils.WriteResourceToFileIfNotExists($"{DefaultResourceDataPath}.settings.json", SettingPath);

		try
		{
			Settings = JsonConvert.DeserializeObject<SettingsObject>(File.ReadAllText(SettingPath));
		}
		catch (JsonSerializationException e)
		{
			Log.Warning(e, "Failed to load settings {Path} writing default", SettingPath);
			//MessageBox.Show("Failed to load settings. Please check the log for more information.", "Error", MessageBoxButtons.OK, MessageBoxType.Error);
			//Environment.Exit(1);
			Utils.WriteResourceToFile($"{DefaultResourceDataPath}.settings.json", SettingPath);
			Settings = JsonConvert.DeserializeObject<SettingsObject>(File.ReadAllText(SettingPath));
		}
	}

	/// <summary>
	///     Initializes logger
	/// </summary>
	private static void InitLogger()
	{
		Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File(Path.Join(LogPath, "log.log"), rollingInterval: RollingInterval.Day)
				.CreateLogger();

		Log.Information("Initializing logger");
	}

	/// <summary>
	///     Creates directories if they don't exist
	/// </summary>
	private static void InitPaths()
	{
		Log.Information("Initializing paths");

		Directory.CreateDirectory(RootPath);
		Log.Debug("Created root path {RootPath}", RootPath);
		Directory.CreateDirectory(PluginPath);
		Log.Debug("Created plugin path {PluginPath}", PluginPath);
		Directory.CreateDirectory(LanguagePath);
		Log.Debug("Created language path {LanguagePath}", LanguagePath);
		Directory.CreateDirectory(LogPath);
		Log.Debug("Created log path {LogPath}", LogPath);
		Directory.CreateDirectory(UpdaterPath);
		Log.Debug("Created updater path {UpdaterPath}", UpdaterPath);

		Log.Information("Paths initialized");
	}
}