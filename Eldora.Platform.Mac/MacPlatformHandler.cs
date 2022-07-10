#region

using System.Diagnostics;
using Eldora.PluginAPI;

#endregion

namespace Eldora.Mac;

public sealed class MacPlatformHandler : IPlatformHandler
{
	public void OpenFolder(string path, bool select)
	{
		Process.Start("open", path);
	}
}