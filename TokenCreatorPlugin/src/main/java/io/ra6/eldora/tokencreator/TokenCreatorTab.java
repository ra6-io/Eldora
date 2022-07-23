package io.ra6.eldora.tokencreator;

import io.ra6.eldora.components.EldoraTabComponent;

import javax.swing.*;
import java.awt.*;

public class TokenCreatorTab extends EldoraTabComponent {
	@Override
	public String tabName() {
		return "Token Creator";
	}

	@Override
	public JPanel getComponent() {
		var panel = new JPanel();
		panel.setLayout(new GridBagLayout());

		var toolsPanel = new JPanel();
		toolsPanel.setBorder(BorderFactory.createTitledBorder("Tools"));

		var editorPanel = new JPanel();
		editorPanel.setBorder(BorderFactory.createTitledBorder("Editor"));

		var previewPanel = new JPanel();
		previewPanel.setBorder(BorderFactory.createTitledBorder("Preview"));

		var c = new GridBagConstraints();
		c.fill = GridBagConstraints.HORIZONTAL;

		return panel;
	}
}
