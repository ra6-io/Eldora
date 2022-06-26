#region

using System.Reflection;
using Serilog;

#endregion

namespace DnDHelperV2.PluginAPI;

public sealed class Utils
{
	/// <summary>
	///     Writes a embedded resource to a file.
	/// </summary>
	/// <param name="resourcePath"></param>
	/// <param name="outputPath"></param>
	/// <param name="useAssemblyName">set to false if the resource path is complete</param>
	public static void WriteResourceToFile(string resourcePath, string outputPath, bool useAssemblyName = true)
	{
		var assembly = Assembly.GetCallingAssembly();

		if (useAssemblyName)
		{
			resourcePath = $"{assembly.GetName().Name}.{resourcePath}";
			Log.Information("Using assembly name for resource path: {ResourcePath}", resourcePath);
		}

		Log.Information("Copying {JsonName} to {External}", resourcePath, outputPath);
		using var stream = assembly.GetManifestResourceStream(resourcePath)!;
		using var streamReader = new StreamReader(stream);
		var buf = new byte[stream.Length];
		var bytes = stream.Read(buf, 0, buf.Length);

		File.WriteAllBytes(outputPath, buf);
		//Log.Information("Read {Bytes} bytes from {File}", bytes, resourcePath);
	}

	/// <summary>
	///     Writes a embedded resource to a file if it doesn't exist.
	/// </summary>
	/// <param name="resourcePath"></param>
	/// <param name="outputPath"></param>
	/// <param name="useAssemblyName">set to false if the resource path is complete</param>
	public static void WriteResourceToFileIfNotExists(string resourcePath, string outputPath, bool useAssemblyName = true)
	{
		if (File.Exists(outputPath))
		{
			Log.Information("File {File} already exists, skipping", outputPath);
			return;
		}

		var assembly = Assembly.GetCallingAssembly();

		if (useAssemblyName)
		{
			resourcePath = $"{assembly.GetName().Name}.{resourcePath}";
			Log.Information("Using assembly name for resource path: {ResourcePath}", resourcePath);
		}

		Log.Information("Copying {JsonName} to {External}", resourcePath, outputPath);
		using var stream = assembly.GetManifestResourceStream(resourcePath)!;
		using var streamReader = new StreamReader(stream);
		var buf = new byte[stream.Length];
		var bytes = stream.Read(buf, 0, buf.Length);

		File.WriteAllBytes(outputPath, buf);
		//Log.Information("Read {Bytes} bytes from {File}", bytes, resourcePath);
	}
}