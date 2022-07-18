package io.ra6.eldora.ui;

import io.ra6.eldora.ui.internal.PluginPanel;
import io.ra6.eldora.ui.internal.SettingsPanel;

import javax.swing.*;
import java.awt.*;

public class RootPanel extends JPanel {

	private final MainFrame _parent;

	public RootPanel(MainFrame parentWindow) {
		_parent = parentWindow;

		setLayout(new BorderLayout());

		addComponents();
	}

	private void addComponents() {
		var tabComponent = new JTabbedPane();
		tabComponent.setTabPlacement(JTabbedPane.LEFT);

		tabComponent.addTab("Plugins", new PluginPanel());
		tabComponent.addTab("Settings", new SettingsPanel());

		add(tabComponent, BorderLayout.CENTER);
	}
}
