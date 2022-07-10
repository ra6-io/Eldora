#region

using Eto.Forms;

#endregion

namespace Eldora.PluginAPI;

/// <summary>
///     Any component which has a tab needs to implement this interface
/// </summary>
public interface ITabComponent : IComponent
{
	/// <summary>
	///     The title of the tab
	/// </summary>
	public string Title { get; }

	/// <summary>
	///     The content of the tab
	/// </summary>
	/// <returns></returns>
	public Panel GetRootPanel();
}