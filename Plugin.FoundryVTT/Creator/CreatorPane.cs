using System.Text.Json.Serialization;
using DnDHelperV2.PluginAPI;
using Eto.Forms;

namespace Plugin.FoundryVTT.Creator;

public sealed class CreatorPane : ITabComponent
{
	public string Title => "Creator Pane";
	
	public Panel GetRootPanel()
	{
		return new Panel();
	}
}