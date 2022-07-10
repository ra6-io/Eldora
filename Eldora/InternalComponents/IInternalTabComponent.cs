#region

using Eldora.PluginAPI;

#endregion

namespace Eldora.InternalComponents;

public interface IInternalTabComponent : ITabComponent
{
	public bool IsOnlyDebugVisible { get; }
}