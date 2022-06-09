namespace DnDHelperV2.PluginAPI;

public sealed record Dependency
{
	/// <summary>
	/// The name of the plugin.
	/// </summary>
	public string PluginName { get; init; } = null!;

	/// <summary>
	/// The Minial Version of a plugin.
	/// </summary>
	public Version MinimalVersion { get; init; } = null!;

	/// <summary>
	/// The Maximal Supported Version of a plugin.
	/// </summary>
	public Version? MaximalVersion { get; init; } = null;
}