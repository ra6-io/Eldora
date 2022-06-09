using Eto.Forms;

namespace DnDHelperV2.PluginAPI;

public interface ITabComponent : IComponent
{
	/// <summary>
	/// The title of the tab
	/// </summary>
	public string Title { get; }
	
	/// <summary>
	/// The content of the tab
	/// </summary>
	/// <returns></returns>
	public Panel GetRootPanel();
}