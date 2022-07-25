package io.ra6.eldora.tokencreator;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.*;

public class BorderObject {
	private final String _name;
	private final String _image;
	private final String _mask;

	private BufferedImage _tokenImage;
	private BufferedImage _maskImage;

	public BorderObject(String name, String image, String mask) throws IOException {
		_name = name;
		_image = image;
		_mask = mask;

		load();
	}

	public String getName() {
		return _name;
	}

	public String getImage() {
		return _image;
	}

	public String getMask() {
		return _mask;
	}

	public void load() throws IOException {
		_tokenImage = ImageIO.read(new File(TokenCreatorPlugin.getInstance().getBorderPath(), _image));
		_maskImage = ImageIO.read(new File(TokenCreatorPlugin.getInstance().getBorderPath(), _mask));

		var w = _maskImage.getWidth();
		var h = _maskImage.getHeight();

		_maskImage = _maskImage.getSubimage(w / 2 - _tokenImage.getWidth() / 2, h / 2 - _tokenImage.getHeight() / 2, _tokenImage.getWidth(), _tokenImage.getHeight());
	}

	public BufferedImage getTokenImage() {
		return _tokenImage;
	}

	public BufferedImage getMaskImage() {
		return _maskImage;
	}
}
