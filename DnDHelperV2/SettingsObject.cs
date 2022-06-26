#region

using Eto.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion

namespace DnDHelperV2;

[JsonObject(MemberSerialization.OptIn)]
public sealed class SettingsObject
{

	[JsonProperty("debug_mode_enabled")]
	public bool DebugModeEnabled { get; set; } = true;

	[JsonProperty("window", Required = Required.Always)]
	public WindowDataObject WindowData { get; set; }

	public sealed class WindowDataObject
	{
		[JsonProperty("width", Required = Required.Always)]
		public int Width { get; set; }

		[JsonProperty("height", Required = Required.Always)]
		public int Height { get; set; }

		[JsonProperty("state", Required = Required.Always)]
		[JsonConverter(typeof(StringEnumConverter))]
		public WindowState State { get; set; } = WindowState.Normal;
	}
}