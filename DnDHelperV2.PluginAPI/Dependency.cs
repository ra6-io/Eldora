namespace DnDHelperV2.PluginAPI;

/// <summary>
/// Any dependency will be declared as this
/// </summary>
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