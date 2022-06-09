using System;
using System.IO;
using System.Reflection;
using DnDHelperV2.InternalComponents;
using DnDHelperV2.PluginAPI;
using Eto.Forms;
using Serilog;

namespace DnDHelperV2;

public sealed class DnDHelper
{
	/// <summary>
	/// The root path for files
	/// </summary>
	public static readonly string RootPath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DnDHelperV2");

	/// <summary>
	/// The path for plugins
	/// </summary>
	public static readonly string PluginPath = Path.Combine(RootPath, "plugins");

	/// <summary>
	/// The path for localizations
	/// </summary>
	public static readonly string LanguagePath = Path.Combine(RootPath, "languages");

	/// <summary>
	/// The path for logs
	/// </summary>
	public static readonly string LogPath = Path.Combine(RootPath, "logs");

	/// <summary>
	/// Gets parsed by config sometime
	/// </summary>
	private const bool DebugModeActivated = true;

	/// <summary>
	/// The plugin loader instance
	/// </summary>
	public PluginLoader PluginLoader { get; }

	/// <summary>
	/// The Singleton instance
	/// </summary>
	public static DnDHelper Instance { get; private set; }

	private MainForm _mainForm;

	/// <summary>
	/// The main form
	/// </summary>
	public MainForm MainForm => _mainForm ??= new MainForm();

	public DnDHelper()
	{
		Instance = this;

		InitLogger();
		InitPaths();

		PluginLoader = new PluginLoader();
		PluginLoader.LoadPlugins(PluginPath);

		// Enables all plugins
		PluginLoader.ForEachLoadedPlugin(data =>
		{
			data.plugin.OnEnable();
			Log.Information("Enabled plugin {Plugin}", data.metadata.PluginName);
		});

		// Add components from plugins
		PluginLoader.ForEachLoadedPlugin(data =>
		{
			if (!data.metadata.HasTabComponents) return;

			foreach (var component in data.plugin.GetComponents())
			{
				if (component is not ITabComponent tabComponent) continue;

				var tab = new TabPage
				{
						Text = tabComponent.Title,
						Content = tabComponent.GetRootPanel()
				};
				Log.Debug("Adding tab {Title} from plugin {Plugin}", tabComponent.Title, data.metadata.PluginName);
				((TabControl)MainForm.Content).Pages.Add(tab);
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
				data.plugin.OnDisable();
				Log.Information("Disabled plugin {Plugin}", data.metadata.PluginName);
			});
		};

		// Unloads all plugins on closed
		MainForm.Closed += (_, _) =>
		{
			PluginLoader.ForEachLoadedPlugin(data =>
			{
				data.plugin.OnUnload();
				Log.Information("Unloaded plugin {Plugin}", data.metadata.PluginName);
			});

			Log.CloseAndFlush();
		};
	}

	/// <summary>
	/// Initializes logger
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
	/// Creates directories if they don't exist
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

		Log.Information("Paths initialized");
	}
}