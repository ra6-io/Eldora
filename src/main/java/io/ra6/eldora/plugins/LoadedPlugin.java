package io.ra6.eldora.plugins;

import io.ra6.Eldora;
import io.ra6.eldora.AbstractEldoraPlugin;

import java.io.File;

public class LoadedPlugin {
	private final PluginMetadata _pluginMetadata;
	private final AbstractEldoraPlugin _pluginInstance;
	private final File _pluginFolder;

	public LoadedPlugin(AbstractEldoraPlugin instance, PluginMetadata metadata, File pluginFolder) throws IllegalAccessException, NoSuchFieldException {
		_pluginMetadata = metadata;
		_pluginInstance = instance;
		_pluginFolder = pluginFolder;

		_pluginInstance.setLogger(Eldora.LOGGER);
	}



	public void onEnable() {
		_pluginInstance.onEnable();
	}

	public void onDisable() {
		_pluginInstance.onDisable();
	}

	public void onLoad(String pluginPath) {
		_pluginInstance.onLoad(pluginPath);
	}

	public void onUnload() {
		_pluginInstance.onUnload();
	}

	public AbstractEldoraPlugin getPluginInstance() {
		return _pluginInstance;
	}

	public PluginMetadata getMetadata() {
		return _pluginMetadata;
	}

	public File getPluginFolder() {
		return _pluginFolder;
	}
}
