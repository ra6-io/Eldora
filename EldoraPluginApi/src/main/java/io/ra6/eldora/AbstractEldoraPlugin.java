package io.ra6.eldora;

import io.ra6.eldora.components.EldoraTabComponent;
import org.apache.logging.log4j.Logger;

import java.util.ArrayList;

public abstract class AbstractEldoraPlugin {
	private Logger _logger = null;
	private final ArrayList<EldoraTabComponent> _eldoraTabComponents = new ArrayList<>();

	public final Logger getLogger() {
		return _logger;
	}

	protected final void addTabComponent(EldoraTabComponent tabComponent) {
		_eldoraTabComponents.add(tabComponent);
	}

	public final ArrayList<EldoraTabComponent> getEldoraTabComponents() {
		return _eldoraTabComponents;
	}

	public void onEnable() {
	}

	public void onDisable() {
	}

	public void onLoad(String pluginPath) {
	}

	public void onUnload() {
	}

	public final void setLogger(Logger logger) {
		if (_logger != null) return;
		_logger = logger;
	}
}
