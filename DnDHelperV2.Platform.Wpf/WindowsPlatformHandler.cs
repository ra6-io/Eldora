using System.Diagnostics;
using DnDHelperV2.Annotations;
using DnDHelperV2.PluginAPI;

namespace DnDHelperV2.Wpf;

public sealed class WindowsPlatformHandler : IPlatformHandler
{
	public void OpenFolder(string path, bool select)
	{
		if (select)
		{
			var args = $"/select, \"{path}\"";
			Process.Start("explorer.exe", $"{args}");
		}
		else
		{
			Process.Start("explorer.exe", path);
		}
	}
}