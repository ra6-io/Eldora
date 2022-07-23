package io.ra6.eldora.ui;

import io.ra6.eldora.components.EldoraTabComponent;
import io.ra6.eldora.ui.internal.PluginPanel;
import io.ra6.eldora.ui.internal.SettingsPanel;

import javax.swing.*;
import java.awt.*;

public class RootPanel extends JPanel {
	private final MainFrame _parent;
	private final JTabbedPane _tabComponent;

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

		add(_tabComponent, BorderLayout.CENTER);
	}

	public void addTab(EldoraTabComponent tab) {
		_tabComponent.addTab(tab.tabName(), tab.getComponent());
	}
}
