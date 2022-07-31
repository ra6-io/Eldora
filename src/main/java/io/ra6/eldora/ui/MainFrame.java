package io.ra6.eldora.ui;

import io.ra6.Eldora;
import io.ra6.eldora.components.EldoraTabComponent;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.KeyEvent;

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

		addKeyBinds();

		pack();
		setVisible(true);
	}

	private void addKeyBinds() {
		var inputMap = ((JPanel) getContentPane()).getInputMap(
				JComponent.WHEN_IN_FOCUSED_WINDOW);
		var actionMap = ((JPanel) getContentPane()).getActionMap();

		var reload = new AbstractAction() {

			@Override
			public void actionPerformed(ActionEvent e) {
				Eldora.reloadPlugins();
			}
		};

		var key = "RELOAD_PLUGINS";
		inputMap.put(KeyStroke.getKeyStroke("F5"), key);
		actionMap.put(key, reload);
/*
		var printHelloWorld = new AbstractAction() {
			@Override
			public void actionPerformed(ActionEvent e) {
				System.out.println("Hello World!!!");
			}
		};
		final var helloWorld = "helloWorld";
		inputMap.put(KeyStroke.getKeyStroke("F1"), helloWorld);
		actionMap.put(helloWorld, printHelloWorld);


		var printHelloMars = new AbstractAction() {
			@Override
			public void actionPerformed(ActionEvent e) {
				System.out.println("Hello Mars!!!");
			}
		};
		final var helloMars = "helloMars";
		inputMap.put(KeyStroke.getKeyStroke("F2"), helloMars);
		actionMap.put(helloMars, printHelloMars);
 */
	}

	public void addTab(EldoraTabComponent tab) {
		_rootPanel.addTab(tab);
	}

	public void clearTabs() {
		_rootPanel.clearTabs();
	}
}
