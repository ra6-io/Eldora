#region

using System;
using Eldora.PluginAPI;

#endregion

namespace Eldora.Gtk;

public sealed class LinuxPlatformHandler : IPlatformHandler
{

	public void OpenFolder(string path, bool select)
	{
		throw new NotImplementedException();
	}
}