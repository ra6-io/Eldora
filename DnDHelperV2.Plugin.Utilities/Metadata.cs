using DnDHelperV2.PluginAPI;

namespace DnDHelperV2.Plugin.Utilities;

public class Metadata : IMetadata
{
	public string PluginName => "Utilities";
	public Version PluginVersion => new(1, 0, 0);
	
	public string Author => "Ra6nar0k21";
	
	public string Description => "";
	
	public bool HasTabComponents => false;
	
	public List<Dependency> Dependencies => new();
}