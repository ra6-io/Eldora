#region

using Newtonsoft.Json;

#endregion

namespace Eldora.Updater;

[JsonObject(MemberSerialization.OptIn)]
public sealed class AssetObject
{

	[JsonProperty("name")]
	public string Name { get; set; } = "";

	[JsonProperty("browser_download_url")]
	public string BrowserDownloadUrl { get; set; } = "";
}