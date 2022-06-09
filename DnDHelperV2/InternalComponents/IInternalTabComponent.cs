using DnDHelperV2.PluginAPI;

namespace DnDHelperV2.InternalComponents;

public interface IInternalTabComponent : ITabComponent
{
	public bool IsOnlyDebugVisible { get; }
}