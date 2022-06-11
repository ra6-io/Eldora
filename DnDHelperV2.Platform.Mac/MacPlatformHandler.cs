using System.Diagnostics;
using DnDHelperV2.Annotations;
using DnDHelperV2.PluginAPI;

namespace DnDHelperV2.Mac;

public sealed class MacPlatformHandler : IPlatformHandler
{
	public void OpenFolder(string path, bool select)
	{
		Process.Start("open", path);
	}
}