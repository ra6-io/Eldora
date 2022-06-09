﻿namespace DnDHelperV2.PluginAPI;

public interface IMetadata
{
	/// <summary>
	/// The name of the Plugin
	/// </summary>
	public string PluginName { get; }

	/// <summary>
	/// The version of the Plugin
	/// </summary>
	public Version PluginVersion { get; }

	/// <summary>
	/// The author of the Plugin
	/// </summary>
	public string Author { get; }

	/// <summary>
	/// The description of the Plugin
	/// </summary>
	public string Description { get; }

	/// <summary>
	/// If the Plugin has visiual Components
	/// </summary>
	public bool HasTabComponents { get; }

	/// <summary>
	/// The dependencies of the Plugin
	/// </summary>
	public List<Dependency> Dependencies { get; }
}