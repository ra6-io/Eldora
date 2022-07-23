package io.ra6.eldora.ui;

import io.ra6.Eldora;
import io.ra6.eldora.components.EldoraTabComponent;

import javax.swing.*;
import java.awt.*;

public final class MainFrame extends JFrame {

	private final RootPanel _rootPanel;

	public MainFrame() {
		_rootPanel = new RootPanel(this);
	}

	public void initialize() {
		// Load size from settings
		var size = new Dimension(1280, 720);

		setSize(size);
		setMinimumSize(size);

		setTitle(Eldora.TITLE);
		setDefaultCloseOperation(EXIT_ON_CLOSE);

		setContentPane(_rootPanel);

		Eldora.LOGGER.info("Showing UI");
		pack();
		setVisible(true);
	}

	public void addTab(EldoraTabComponent tab) {
		_rootPanel.addTab(tab);
	}
}
