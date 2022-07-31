package io.ra6.eldora.ui;

import io.ra6.eldora.components.EldoraTabComponent;
import io.ra6.eldora.ui.internal.PluginPanel;
import io.ra6.eldora.ui.internal.SettingsPanel;
import org.jetbrains.annotations.NotNull;

import javax.swing.*;
import java.awt.*;

public class RootPanel extends JPanel {
	private final MainFrame _parent;
	private final JTabbedPane _tabComponent;

	private int _defaultTabCount = -1;

	public RootPanel(MainFrame parentWindow) {
		_parent = parentWindow;
		_tabComponent = new JTabbedPane();

		setLayout(new BorderLayout());

		addComponents();
	}

	private void addComponents() {
		_tabComponent.setTabPlacement(JTabbedPane.LEFT);

		_tabComponent.addTab("Plugins", new PluginPanel());
		_tabComponent.addTab("Settings", new SettingsPanel());

		_defaultTabCount = _tabComponent.getTabCount();

		add(_tabComponent, BorderLayout.CENTER);
	}

	public void addTab(@NotNull EldoraTabComponent tab) {
		_tabComponent.addTab(tab.tabName(), tab.getComponent());
	}

	public void clearTabs() {
		for (var i = _defaultTabCount; i < _tabComponent.getTabCount(); i++) {
			_tabComponent.removeTabAt(i);
		}
	}
}
