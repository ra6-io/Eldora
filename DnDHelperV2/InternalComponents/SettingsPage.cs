using DnDHelperV2.PluginAPI;
using Eto.Forms;

namespace DnDHelperV2.InternalComponents;

public sealed class SettingsPage : IInternalTabComponent
{
	public string Title => "Settings";

	// TODO: Add debug enable option
	// TODO: Save and load settings
	// TODO: Add plugin page
	
	public Panel GetRootPanel()
	{
		return new Panel();
	}

	public bool IsOnlyDebugVisible => false;
}