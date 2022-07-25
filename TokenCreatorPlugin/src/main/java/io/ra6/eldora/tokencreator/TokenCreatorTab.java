package io.ra6.eldora.tokencreator;

import io.ra6.eldora.components.EldoraTabComponent;
import org.jetbrains.annotations.NotNull;

import javax.swing.*;
import javax.swing.filechooser.FileFilter;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

public class TokenCreatorTab extends EldoraTabComponent {
	@Override
	public String tabName() {
		return "Token Creator";
	}

	private EditorCanvas _editorCanvas;

	@Override
	public JPanel getComponent() {
		var pane = new JPanel();
		pane.setLayout(new GridBagLayout());

		var c = new GridBagConstraints();
		c.fill = GridBagConstraints.BOTH;

		c.weightx = 100;
		c.weighty = 100;

		c.gridx = 0;

		c.weightx = 95;
		c.gridheight = 2;
		c.gridwidth = 1;
		c.gridy = 0;
		pane.add(createEditorPanel(), c);

		c.gridx = 1;
		c.weightx = 5;
		c.weighty = 25;
		c.gridwidth = 1;
		c.gridheight = 1;
		c.gridy = 0;
		pane.add(createPreviewPanel(), c);

		c.weighty = 75;
		c.gridx = 1;
		c.gridy = 1;
		pane.add(createToolPanel(), c);

		_editorCanvas.setBorderObject(TokenCreatorPlugin.getInstance().getLoadedBorders().get(0));
		return pane;
	}

	@NotNull
	private JPanel createPreviewPanel() {
		var pane = new JPanel();
		pane.setBorder(BorderFactory.createTitledBorder("Preview"));
		pane.setLayout(new BorderLayout());
		var thumb = new JLabel();
		thumb.setHorizontalAlignment(SwingConstants.CENTER);
		thumb.setVerticalAlignment(SwingConstants.CENTER);

		_editorCanvas.addPropertyChangeListener("preview", evt -> {
			var img = (BufferedImage) evt.getNewValue();
			var icon = new ImageIcon(img);
			thumb.setIcon(icon);
		});
		pane.add(thumb, BorderLayout.CENTER);

		return pane;
	}

	@NotNull
	private JPanel createEditorPanel() {
		_editorCanvas = new EditorCanvas();

		var pane = new JPanel();

		pane.setLayout(new BorderLayout());
		pane.setBorder(BorderFactory.createTitledBorder("Editor"));

		pane.add(_editorCanvas, BorderLayout.CENTER);
		return pane;
	}

	@NotNull
	private JPanel createToolPanel() {
		var pane = new JPanel();
		pane.setBorder(BorderFactory.createTitledBorder("Tools"));
		pane.setLayout(new GridBagLayout());

		var c = new GridBagConstraints();
		c.fill = GridBagConstraints.HORIZONTAL;

		c.weightx = 100;
		c.weighty = 100;

		c.gridx = 0;
		c.gridy = 0;

		var borderSelectionCB = new JComboBox<String>();
		for (BorderObject object : TokenCreatorPlugin.getInstance().getLoadedBorders()) {
			borderSelectionCB.addItem(object.getName());
		}
		borderSelectionCB.addActionListener(e -> {
			var newBorder = borderSelectionCB.getItemAt(borderSelectionCB.getSelectedIndex());
			_editorCanvas.setBorderObject(TokenCreatorPlugin.getInstance().getBorderFromName(newBorder));
		});

		pane.add(borderSelectionCB, c);

		var selectImageBtn = new JButton("Select Image");
		selectImageBtn.addActionListener(e -> {
			TokenCreatorPlugin.getInstance().getLogger().info("Clicked");

			var fc = new JFileChooser();
			fc.setFileFilter(new FileFilter() {
				@Override
				public boolean accept(File f) {
					return f.getName().endsWith(".png")
							|| f.getName().endsWith(".jpg")
							|| f.getName().endsWith(".bmp");
				}

				@Override
				public String getDescription() {
					return "Image";
				}
			});

			int returnVal = fc.showOpenDialog(pane);

			if (returnVal == JFileChooser.APPROVE_OPTION) {
				var file = fc.getSelectedFile();
				TokenCreatorPlugin.getInstance().getLogger().info("Opening: {}", file.getName());

				try {
					_editorCanvas.setTokenImage(file);
				} catch (IOException ex) {
					TokenCreatorPlugin.getInstance().getLogger().fatal(ex);
				}
			}
		});
		c.gridy = 1;
		pane.add(selectImageBtn, c);

		var exportTokenBtn = new JButton("Export Token");
		exportTokenBtn.addActionListener(e -> {
			var fc = new JFileChooser();
			int returnVal = fc.showSaveDialog(pane);

			if (returnVal == JFileChooser.APPROVE_OPTION) {
				var file = fc.getSelectedFile();
				TokenCreatorPlugin.getInstance().getLogger().info("Exporting to: {}", file.getName());

				String file_name = file.toString();
				if (!file_name.endsWith(".png"))
					file_name += ".png";

				try {
					_editorCanvas.exportTo(new File(file_name));
				} catch (IOException ex) {
					TokenCreatorPlugin.getInstance().getLogger().fatal(ex);
				}
			}
		});
		c.gridy = 2;
		pane.add(exportTokenBtn, c);

		return pane;
	}
}
