package io.ra6.eldora.plugins;

import io.ra6.Eldora;
import io.ra6.Version;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.logging.Logger;
import java.util.zip.ZipFile;

//TODO: https://www.codegrepper.com/code-examples/java/java+load+jar+at+runtime

public class PluginHandler {

	public void loadPlugins(File pluginPath) {
		var files = pluginPath.listFiles((dir, name) -> name.endsWith(".jar"));

		if (files == null) return;

		for (var file : files) {
			try (var zipFile = new ZipFile(file)) {
				var entries = zipFile.entries();

				while (entries.hasMoreElements()) {
					var entry = entries.nextElement();
					if (entry.getName().equals("plugin.json")) {
						var reader = new BufferedReader(new InputStreamReader(zipFile.getInputStream(entry)));

						var metadata = parseMetadata(file, reader);
						if (metadata == null) break;

						loadPlugin(file, metadata);
					}
				}
			} catch (IOException e) {
				throw new RuntimeException(e);
			}
		}
	}

	private void loadPlugin(File file, PluginMetadata metadata) {
		Eldora.LOGGER.info("Loading Plugin {}.", metadata);
	}

	private PluginMetadata parseMetadata(File fileName, BufferedReader reader) throws IOException {
		var result = new StringBuilder();

		String line;
		while ((line = reader.readLine()) != null) {
			result.append(line);
		}
		reader.close();

		var plugin = new JSONObject(result.toString());
		if (!plugin.has("name")) {
			Eldora.LOGGER.error("plugin.json from file {} is invalid! Missing key {}", fileName, "name");
			return null;
		}
		if (!plugin.has("package")) {
			Eldora.LOGGER.error("plugin.json from file {} is invalid! Missing key {}", fileName, "package");
			return null;
		}
		if (!plugin.has("version")) {
			Eldora.LOGGER.error("plugin.json from file {} is invalid! Missing key {}", fileName, "version");
			return null;
		}

		var pluginName = plugin.getString("name");
		var pluginPackage = plugin.getString("package");
		var pluginVersion = plugin.getString("version");

		var pluginAuthors = new ArrayList<String>();

		if (plugin.has("authors")) {
			var rawAuthors = plugin.getJSONArray("authors");
			for (var i = 0; i < rawAuthors.length(); i++) {
				pluginAuthors.add(rawAuthors.getString(i));
			}
		}

		var pluginDescription = plugin.has("description") ? plugin.getString("description") : "";
		var pluginUrl = plugin.has("url") ? plugin.getString("url") : "";

		return new PluginMetadata(
				pluginName,
				pluginPackage,
				pluginAuthors.toArray(new String[]{}),
				new Version(pluginVersion),
				pluginDescription,
				pluginUrl
		);
	}

	private void onEnable() {
	}

	private void onDisable() {
	}

	private void onLoad() {
	}

	private void onUnload() {
	}
}
