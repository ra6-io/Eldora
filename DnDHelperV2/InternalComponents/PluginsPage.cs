#region

using Eto.Forms;

#endregion

namespace DnDHelperV2.InternalComponents;

public sealed class PluginsPage : IInternalTabComponent
{
	public string Title => "Plugins";

	public Panel GetRootPanel()
	{
		return new Panel();
	}

	public bool IsOnlyDebugVisible => false;
}