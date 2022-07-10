#region

using Newtonsoft.Json;

#endregion

namespace Eldora.Updater;

[JsonObject(MemberSerialization.OptIn)]
public sealed class ReleaseModel
{
	[JsonProperty("tag_name")]
	public string TagName { get; set; } = "";

	[JsonProperty("published_at")]
	public string PublishedAt { get; set; } = "";

	[JsonProperty("assets")]
	public AssetObject[] Assets { get; set; } = Array.Empty<AssetObject>();

	public Version Version => Updater.ParseVersion(TagName);
}