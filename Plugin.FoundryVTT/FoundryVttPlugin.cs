using DnDHelperV2.PluginAPI;
using Newtonsoft.Json;
using Plugin.FoundryVTT.Creator;
using Serilog;

namespace Plugin.FoundryVTT;

public sealed class FoundryVttPlugin : AbstractPlugin
{
	public static FoundryVttPlugin Instance { get; private set; } = null!;

	public override List<IComponent> GetComponents()
	{
		return new List<IComponent>
		{
				new CreatorPane()
		};
	}

	public override void OnLoad()
	{
		Instance = this;
	}
}