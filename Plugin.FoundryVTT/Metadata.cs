using DnDHelperV2.PluginAPI;

namespace Plugin.FoundryVTT;

public sealed class Metadata : IMetadata
{
	public string PluginName => "FoundryVTT Integration";
	public Version PluginVersion => new(1, 0, 0);
	public string Author => "Ra6nar0k21";

	public string Description => "";
	
	public bool HasTabComponents => true;

	public List<Dependency> Dependencies => new();
}