#region

using DnDHelperV2.PluginAPI;

#endregion

namespace Plugin.TokenCreator;

public sealed class Metadata : IMetadata
{
	public string PluginName => "Token Creator";

	public Version PluginVersion => new(1, 0, 0);

	public string Author => "Ra6nar0k21";

	public string Description => "";

	public string Repository => "";

	public bool HasTabComponents => true;

	public List<Dependency> Dependencies => new();
}