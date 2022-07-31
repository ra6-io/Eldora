package io.ra6.eldora.tokencreator;

import javax.imageio.ImageIO;
import javax.swing.*;
import java.awt.*;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import java.awt.event.MouseWheelEvent;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

public class EditorCanvas extends JComponent {

	private BorderObject _borderObject;
	private Color _canvasBackground = Color.LIGHT_GRAY;

	private float _imageScale = 1.0f;
	private static final float SCALE_FACTOR = 0.1f;

	private BufferedImage _tokenImage;

	private boolean _dragging = false;
	private Point _dragStart = new Point();
	private final Point _dragPosition = new Point(40, 40);

	private boolean _colorPickerEnabled = false;

	public EditorCanvas() {
		enableEvents(
				AWTEvent.MOUSE_EVENT_MASK |
						AWTEvent.MOUSE_MOTION_EVENT_MASK |
						AWTEvent.MOUSE_WHEEL_EVENT_MASK |
						AWTEvent.KEY_EVENT_MASK
		);
		//setDoubleBuffered(true);
	}

	@Override
	public void paint(Graphics g) {
		if (_borderObject == null) return;

		g.setColor(_canvasBackground);
		g.fillRect(0, 0, getWidth(), getHeight());

		if (_tokenImage != null) {
			var imgW = (int) (_tokenImage.getWidth() * _imageScale);
			var imgH = (int) (_tokenImage.getHeight() * _imageScale);
			g.drawImage(_tokenImage,
					_dragPosition.x - imgW / 2,
					_dragPosition.y - imgH / 2,
					imgW,
					imgH, this);
		}

		g.drawImage(_borderObject.getTokenImage(),
				getWidth() / 2 - _borderObject.getTokenImage().getWidth() / 2,
				getHeight() / 2 - _borderObject.getTokenImage().getHeight() / 2,
				_borderObject.getTokenImage().getWidth(),
				_borderObject.getTokenImage().getHeight(),
				this);
	}

	@Override
	protected void processMouseEvent(MouseEvent e) {
		if (isColorPickerEnabled()) {
			if (e.getID() == MouseEvent.MOUSE_CLICKED) {

			}
			return;
		}

		switch (e.getID()) {
			case MouseEvent.MOUSE_PRESSED -> {
				_dragging = true;
				_dragStart = e.getPoint();
			}
			case MouseEvent.MOUSE_RELEASED -> _dragging = false;
		}

		firePropertyChange("preview", null, getPreview());
		super.processMouseEvent(e);
	}

	@Override
	protected void processMouseMotionEvent(MouseEvent e) {
		if (!_dragging) return;

		var diff = Utils.getDifference(e.getPoint(), _dragStart);

		_dragPosition.translate(diff.x, diff.y);
		_dragStart = e.getPoint();

		repaint();

		firePropertyChange("preview", null, getPreview());
		super.processMouseMotionEvent(e);
	}

	@Override
	protected void processMouseWheelEvent(MouseWheelEvent e) {
		if (_tokenImage == null) return;

		_imageScale += -e.getWheelRotation() * SCALE_FACTOR;
		// clamps between 0.1f and 10.0f
		_imageScale = Math.min(Math.max(_imageScale, 0.1f), 10.0f);
		repaint();

		firePropertyChange("preview", null, getPreview());
		super.processMouseWheelEvent(e);
	}

	@Override
	protected void processKeyEvent(KeyEvent e) {
		super.processKeyEvent(e);
	}

	public boolean isColorPickerEnabled() {
		return _colorPickerEnabled;
	}

	public void setColorPickerEnabled(boolean colorPickerEnabled) {
		_colorPickerEnabled = colorPickerEnabled;
	}

	public void setBorderObject(BorderObject borderObject) {
		_borderObject = borderObject;

		repaint();
	}

	public void setTokenImage(File file) throws IOException {
		_tokenImage = ImageIO.read(file);

		repaint();
	}

	public BufferedImage getPreview() {
		var img = new BufferedImage(getWidth(), getHeight(), BufferedImage.TYPE_INT_ARGB);

		var g = img.createGraphics();
		print(g);
		g.dispose();

		var w = _borderObject.getTokenImage().getWidth();
		var h = _borderObject.getTokenImage().getHeight();

		var subImage = img.getSubimage(
				getWidth() / 2 - w / 2,
				getHeight() / 2 - h / 2,
				w, h);

		applyMask(subImage, _borderObject.getMaskImage());

		g = subImage.createGraphics();
		g.drawImage(_borderObject.getTokenImage(), 0, 0, _borderObject.getTokenImage().getWidth(), _borderObject.getTokenImage().getHeight(), this);
		g.dispose();

		return subImage;
	}

	private void applyMask(BufferedImage subImage, BufferedImage mask) {
		var width = subImage.getWidth();
		var height = subImage.getHeight();

		var imagePixels = subImage.getRGB(0, 0, width, height, null, 0, width);
		var maskPixels = mask.getRGB(0, 0, width, height, null, 0, width);

		for (var i = 0; i < imagePixels.length; i++) {
			var color = maskPixels[i] & 0x00ffffff; // Mask preexisting alpha
			if (color != 0xffffff) {
				imagePixels[i] = 0x00000000;
			}
		}

		subImage.setRGB(0, 0, width, height, imagePixels, 0, width);
	}

	public void exportTo(File file) throws IOException {
		ImageIO.write(getPreview(), "png", file);
	}
}
