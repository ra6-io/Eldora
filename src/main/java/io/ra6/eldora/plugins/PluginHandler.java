package io.ra6.eldora.plugins;

import com.sun.jdi.event.ExceptionEvent;
import io.ra6.Eldora;
import io.ra6.Version;
import io.ra6.eldora.AbstractEldoraPlugin;
import io.ra6.eldora.components.EldoraTabComponent;
import org.jetbrains.annotations.NotNull;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.lang.reflect.InvocationTargetException;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLClassLoader;
import java.util.ArrayList;
import java.util.zip.ZipFile;

//TODO: https://www.codegrepper.com/code-examples/java/java+load+jar+at+runtime

public class PluginHandler {

	private final ArrayList<LoadedPlugin> _loadedPlugins = new ArrayList<>();

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
			}
		}
	}

	private void loadPlugin(@NotNull File file, PluginMetadata metadata) {
		try {
			Eldora.LOGGER.info("Loading Plugin {}.", metadata);

			URLClassLoader child = null;
			child = new URLClassLoader(
					new URL[]{file.toURI().toURL()},
					this.getClass().getClassLoader()
			);
			var classToLoad = Class.forName(metadata.getCompletePackage(), true, child);
			//if (classToLoad.getDeclaredAnnotation(EldoraPlugin.class) != null) {
			if (AbstractEldoraPlugin.class.isAssignableFrom(classToLoad)) {
				AbstractEldoraPlugin instance;
				try {
					instance = (AbstractEldoraPlugin) classToLoad.getDeclaredConstructor().newInstance();
					Eldora.LOGGER.info("Loaded Plugin: {}", instance.getClass().getName());
					var loadedPlugin = new LoadedPlugin(instance, metadata);
					_loadedPlugins.add(loadedPlugin);
				} catch (IllegalAccessException | NoSuchMethodException | NoSuchFieldException e) {
					Eldora.LOGGER.error("Could not load plugin {}: {}", metadata.getName(), e);
				}
			} else {
				Eldora.LOGGER.error("{} does not extend: {}", metadata.getCompletePackage(), AbstractEldoraPlugin.class.getName());
			}

			onLoad();
		} catch (MalformedURLException | ClassNotFoundException |
		         InstantiationException | InvocationTargetException e) {
			Eldora.LOGGER.error("Could not load plugin {}: {}", metadata.getName(), e);
		}
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
		if (!plugin.has("main")) {
			Eldora.LOGGER.error("plugin.json from file {} is invalid! Missing key {}", fileName, "main");
			return null;
		}
		if (!plugin.has("version")) {
			Eldora.LOGGER.error("plugin.json from file {} is invalid! Missing key {}", fileName, "version");
			return null;
		}

		var pluginName = plugin.getString("name");
		var pluginPackage = plugin.getString("package");
		var pluginMain = plugin.getString("main");
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
				pluginMain,
				pluginAuthors.toArray(new String[]{}),
				new Version(pluginVersion),
				pluginDescription,
				pluginUrl
		);
	}

	public void onEnable() {
		_loadedPlugins.forEach(loadedPlugin -> {
			try {
				Eldora.LOGGER.info("Enabling plugin {}", loadedPlugin.getMetadata().getName());
				loadedPlugin.onEnable();
				Eldora.LOGGER.info("Enabled plugin {}", loadedPlugin.getMetadata().getName());
			} catch (Exception e) {
				Eldora.LOGGER.error("Error enabling plugin {}: {}", loadedPlugin.getMetadata().getName(), e);
			}
		});
	}

	public void onDisable() {
		_loadedPlugins.forEach(loadedPlugin -> {
			try {
				Eldora.LOGGER.info("Disabling plugin {}", loadedPlugin.getMetadata().getName());
				loadedPlugin.onDisable();
				Eldora.LOGGER.info("Disabled plugin {}", loadedPlugin.getMetadata().getName());
			} catch (Exception e) {
				Eldora.LOGGER.error("Error enabling plugin {}: {}", loadedPlugin.getMetadata().getName(), e);
			}
		});
	}

	public void onLoad() {
		_loadedPlugins.forEach(loadedPlugin -> {
			try {
				Eldora.LOGGER.info("Loading plugin {}", loadedPlugin.getMetadata().getName());
				loadedPlugin.onLoad();
				Eldora.LOGGER.info("Loaded plugin {}", loadedPlugin.getMetadata().getName());
			} catch (Exception e) {
				Eldora.LOGGER.error("Error enabling plugin {}: {}", loadedPlugin.getMetadata().getName(), e);
			}
		});
	}

	public void onUnload() {
		_loadedPlugins.forEach(loadedPlugin -> {
			try {
				Eldora.LOGGER.info("Unloading plugin {}", loadedPlugin.getMetadata().getName());
				loadedPlugin.onUnload();
				Eldora.LOGGER.info("Unloaded plugin {}", loadedPlugin.getMetadata().getName());
			} catch (Exception e) {
				Eldora.LOGGER.error("Error enabling plugin {}: {}", loadedPlugin.getMetadata().getName(), e);
			}
		});
	}

	public ArrayList<EldoraTabComponent> getAllTabs() {
		var result = new ArrayList<EldoraTabComponent>();
		for (LoadedPlugin loadedPlugin : _loadedPlugins) {
			Eldora.LOGGER.info("Adding tabs of plugin {}", loadedPlugin.getMetadata().getName());
			for (EldoraTabComponent tab : loadedPlugin.getPluginInstance().getEldoraTabComponents()) {
				Eldora.LOGGER.info("Adding {}", tab.tabName());
				result.add(tab);
			}
		}
		return result;
	}
}
