#region

using System.IO.Compression;
using Newtonsoft.Json;

#endregion

namespace Eldora.Updater;

public static class Updater
{
	private static readonly HttpClient HttpClient = new();

	public static void Main(string[] args)
	{
		if (args.Length != 3)
		{
			Console.WriteLine("Usage: [version] [Wpf|Gtk|Mac] [ProgramPath]");
			return;
		}

		var releases = GenerateApiUrl("ra6-io", "eldora", "releases");
		CheckAndDownloadUpdate(ParseVersion(args[0]), args[1], releases, args[2]);
	}

	/// <summary>
	///     Parses a Version in this format <b>v1.0.0</b>
	/// </summary>
	/// <param name="version"></param>
	/// <returns></returns>
	public static Version ParseVersion(string version)
	{
		return Version.Parse(version.AsSpan(1));
	}

	public static void CheckAndDownloadUpdate(Version currentVersion, string platform, string releases, string programPath)
	{
		try
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, releases);

			// The default headers required to make a request to the Github site
			request.Headers.Add("Accept", "application/vnd.github+json");
			request.Headers.Add("User-Agent", "Eldora-CLI");
			var result = HttpClient.Send(request);

			// Reads the result of the get request and parses it to a model
			var resultContent = result.Content.ReadAsStringAsync().Result;
			var models = JsonConvert.DeserializeObject<ReleaseModel[]>(resultContent);

			var newestVersionModelVersion = currentVersion;

			// Checks the model versions and gets the newest one
			foreach (var releaseModel in models!)
					//Console.WriteLine($"Release Version: {releaseModel.TagName}");
				if (releaseModel.Version > newestVersionModelVersion)
				{
					newestVersionModelVersion = releaseModel.Version;
				}

			var newestModel = models.FirstOrDefault(releaseModel => releaseModel.Version == newestVersionModelVersion);

			Console.WriteLine($"Platform:        {platform}");
			Console.WriteLine($"Current Version: {currentVersion}");
			Console.WriteLine($"Newest Version:  {newestVersionModelVersion}");

			if (newestVersionModelVersion == currentVersion) return;

			Console.WriteLine("-----------------------------------------------------");
			Console.WriteLine("There is a new Version. Downloading the new Update...");

			foreach (var newestModelAsset in newestModel!.Assets)
			{
				// if the file name is not equal to the needed archive name
				if (!newestModelAsset.Name.EndsWith($"eldora-{currentVersion.ToString(3)}.platorm.{platform}.zip")) continue;
				//if (!newestModelAsset.Name.EndsWith(".zip")) continue;

				Console.WriteLine($"Downloading {newestModelAsset.Name}");
				Console.WriteLine($"From:       {newestModelAsset.BrowserDownloadUrl}");

				var filePath = $"{programPath}/{newestModelAsset.Name}";

				// Creates a get request to the download path and loads the file to local
				var response = HttpClient.GetAsync(newestModelAsset.BrowserDownloadUrl).Result;
				if (File.Exists(filePath)) File.Delete(filePath);

				using (var fs = new FileStream(filePath, FileMode.CreateNew))
				{
					response.Content.CopyTo(fs, null, CancellationToken.None);
				}

				foreach (var file in Directory.GetFiles(programPath))
				{
					if (file == $"{programPath}/{newestModelAsset.Name}") continue;

					if ((File.GetAttributes(file) & FileAttributes.Directory) != 0)
					{
						Directory.Delete(file);
						continue;
					}

					File.Delete(file);
				}

				Console.WriteLine("Extracting...");
				ZipFile.ExtractToDirectory(filePath, programPath);
				File.Delete(filePath);

				break;
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("ERROR");
			Console.WriteLine(e);
		}
	}

	public static string GenerateApiUrl(string owner, string repo, string path)
	{
		return $"https://api.github.com/repos/{owner}/{repo}/{path}";
	}
}