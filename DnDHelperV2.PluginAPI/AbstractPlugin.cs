using Eto.Forms;

namespace DnDHelperV2.PluginAPI;

/// <summary>
/// Each plugin has to extend this class to be recognized
/// <br/>
/// Also each plugin needs to have a class that implements <see cref="IMetadata"/>
/// </summary>
public abstract class AbstractPlugin
{
	/// <summary>
	/// List of all the plugin's components
	/// </summary>
	public virtual List<IComponent> GetComponents() { return new List<IComponent>(); }

	/// <summary>
	/// Will be called when the plugin is loaded
	/// </summary>
	public virtual void OnLoad() {}

	/// <summary>
	/// Will be called when the plugin is enabled
	/// </summary>
	public virtual void OnEnable() {}

	/// <summary>
	/// Will be called when the plugin is unloaded
	/// </summary>
	public virtual void OnUnload() {}

	/// <summary>
	/// Will be called when the plugin is disabled
	/// </summary>
	public virtual void OnDisable() {}

#region UTILS

	/// <summary>
	/// The metadata of the plugin
	/// Will be set by the plugin loader
	/// </summary>
	public readonly IMetadata Metadata = null!;

	/// <summary>
	/// The Plugins folder path
	/// Will be set by the plugin loader
	/// </summary>
	public readonly string PluginFolder = null!;

	/// <summary>
	/// The main window object
	/// </summary>
	public readonly Form MainWindow = null!;

	/// <summary>
	/// The platform handler
	/// </summary>
	public readonly IPlatformHandler PlatformHandler = null!;

#endregion

}