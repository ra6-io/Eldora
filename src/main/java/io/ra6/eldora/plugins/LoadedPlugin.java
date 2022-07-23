package io.ra6.eldora.plugins;

import io.ra6.Eldora;
import io.ra6.eldora.AbstractEldoraPlugin;
import io.ra6.eldora.components.EldoraTabComponent;

import java.util.ArrayList;

public class LoadedPlugin {
	private final PluginMetadata _pluginMetadata;
	private final AbstractEldoraPlugin _pluginInstance;

	public LoadedPlugin(AbstractEldoraPlugin instance, PluginMetadata metadata) throws IllegalAccessException, NoSuchFieldException {
		_pluginMetadata = metadata;
		_pluginInstance = instance;

		_pluginInstance.setLogger(Eldora.LOGGER);
	}

	public void onEnable() {
		_pluginInstance.onEnable();
	}

	public void onDisable() {
		_pluginInstance.onDisable();
	}

	public void onLoad() {
		_pluginInstance.onLoad();
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
}
