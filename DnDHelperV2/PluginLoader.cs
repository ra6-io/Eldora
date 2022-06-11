using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DnDHelperV2.PluginAPI;
using Serilog;

namespace DnDHelperV2;

public sealed class PluginLoader
{
	/// <summary>
	/// The loaded plugins
	/// </summary>
	private readonly List<AbstractPlugin> _loadedPlugins = new();

	/// <summary>
	/// Loads all plugins from the given directory.
	/// </summary>
	/// <param name="pluginPath">The path for the plugins</param>
	public void LoadPlugins(string pluginPath)
	{
		List<(Assembly assembly, IMetadata metadata)> plugins = new();

		var pluginFiles = Directory.GetFiles(pluginPath);

		// Checks for all files in the folder
		foreach (var pluginFile in pluginFiles)
		{
			if (!pluginFile.EndsWith(".dll")) continue;

			// loads plugin dll
			var dll = Assembly.LoadFile(pluginFile);

			// checks for all exported types for metadata
			foreach (var type in dll.GetExportedTypes())
			{
				if (type.GetInterface(nameof(IMetadata)) == null) continue;
				plugins.Add((dll, (IMetadata)Activator.CreateInstance(type)));
				break;
			}
		}
		
		CheckDependenciesAndLoad(plugins);
	}

	/// <summary>
	/// Checks if all dependencies are met and loads the plugins
	/// </summary>
	/// <param name="plugins"></param>
	private void CheckDependenciesAndLoad(List<(Assembly assembly, IMetadata metadata)> plugins)
	{
		// get all metadatas
		var metadatas = plugins.Select(x => x.metadata).ToList();

		var loadedPlugins = new List<AbstractPlugin>();

		plugins.ForEach(tuple =>
		{
			var assembly = tuple.assembly;
			var metadata = tuple.metadata;

			// check for all dependencies
			var allPresent = AllDependenciesPresent(metadata, metadatas);

			// If any missing error
			if (allPresent.Count != 0)
			{
				Log.Error("Plugin {PluginName} will not be loaded!: It has missing dependencies: {AllPresent}",
						metadata.PluginName,
						allPresent);
				return;
			}

			// checks for all exported abstract plugins and loads exactly one
			foreach (var type in assembly.GetExportedTypes())
			{
				if (!type.IsSubclassOf(typeof(AbstractPlugin))) continue;
				var plugin = (AbstractPlugin)Activator.CreateInstance(type)!;

				var folder = Path.Join(DnDHelper.PluginPath, metadata.PluginName);

				// Creates the plugin directory if not exist
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}

				// Sets data for plugin
				typeof(AbstractPlugin).GetField(nameof(plugin.Metadata))?.SetValue(plugin, metadata);
				typeof(AbstractPlugin).GetField(nameof(plugin.PluginFolder))?.SetValue(plugin, folder);
				typeof(AbstractPlugin).GetField(nameof(plugin.MainWindow))?.SetValue(plugin, DnDHelper.Instance.MainForm);
				typeof(AbstractPlugin).GetField(nameof(plugin.PlatformHandler))?.SetValue(plugin, DnDHelper.Instance.PlatformHandler);

				// Loads the plugin
				plugin.OnLoad();
				Log.Information("Loaded plugin {PluginName}", metadata.PluginName);

				loadedPlugins.Add(plugin);
				break;
			}
		});

		_loadedPlugins.AddRange(loadedPlugins);
		Log.Information("Loaded {Count} plugins", loadedPlugins.Count);
	}

	/// <summary>
	/// Checks if all dependencies are met
	/// </summary>
	/// <param name="metadata"></param>
	/// <param name="loadedMetadatas"></param>
	/// <returns></returns>
	private static List<string> AllDependenciesPresent(IMetadata metadata, IReadOnlyCollection<IMetadata> loadedMetadatas)
	{
		var missingDependencies = new List<string>();

		var dependencies = metadata.Dependencies;
		if (dependencies.Count == 0) return new List<string>();

		foreach (var dependency in dependencies)
		{
			// checks if dependency version is allowed
			var hasDependency = loadedMetadatas.Any(
					x => x.PluginName == dependency.PluginName &&
					     IsVersionBetween(x.PluginVersion, dependency.MinimalVersion, dependency.MaximalVersion));

			if (hasDependency) continue;

			missingDependencies.Add(
					$"{dependency.PluginName} v{(dependency.MaximalVersion == null ? $"{dependency.MinimalVersion}^" : $"{dependency.MinimalVersion}-{dependency.MaximalVersion}")}");
		}

		return missingDependencies;
	}

	/// <summary>
	/// For all loaded plugins execute action
	/// </summary>
	/// <param name="action"></param>
	public void ForEachLoadedPlugin(Action<(IMetadata metadata, AbstractPlugin plugin)> action)
	{
		foreach (var plugin in _loadedPlugins)
		{
			action((plugin.Metadata, plugin));
		}
	}

	/// <summary>
	/// Checks if version is between min and max.
	///
	/// If maximal is null, the value must be greater than minial
	/// </summary>
	/// <param name="value"></param>
	/// <param name="minimal"></param>
	/// <param name="maximal"></param>
	/// <returns></returns>
	private static bool IsVersionBetween(Version value, Version minimal, Version maximal = null)
	{
		if (value < minimal)
		{
			return false;
		}

		if (maximal != null && value > maximal)
		{
			return false;
		}

		return true;
	}

	// TODO: Get Plugin by name

}