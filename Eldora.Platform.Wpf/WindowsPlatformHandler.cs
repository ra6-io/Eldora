#region

using System.Diagnostics;
using Eldora.PluginAPI;

#endregion

namespace Eldora.Wpf;

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