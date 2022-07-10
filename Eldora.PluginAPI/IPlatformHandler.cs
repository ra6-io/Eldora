namespace Eldora.PluginAPI;

public interface IPlatformHandler
{
	/// <summary>
	///     Opens a folder and selects a file if wanted
	/// </summary>
	/// <param name="path"></param>
	/// <param name="select"></param>
	public void OpenFolder(string path, bool select = false);
}