package io.ra6.eldora.ui;

import io.ra6.Eldora;

import javax.swing.*;
import java.awt.*;

public final class MainFrame extends JFrame {

	public void initialize() {
		// Load size from settings
		var size = new Dimension(1280, 720);

		setSize(size);
		setMinimumSize(size);

		setTitle(Eldora.TITLE);
		setDefaultCloseOperation(EXIT_ON_CLOSE);

		setContentPane(new RootPanel(this));

		Eldora.LOGGER.info("Showing UI");
		pack();
		setVisible(true);
	}
}
