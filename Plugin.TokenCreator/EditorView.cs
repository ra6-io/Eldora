using System.Runtime.InteropServices;
using Eto.Drawing;
using Serilog;

namespace Plugin.TokenCreator;

public sealed class EditorView
{
	private const bool TokenMode = true;
	private const bool BackgroundMode = false;

	private static readonly Bitmap Empty = new(1, 1, PixelFormat.Format32bppRgba);

	/// <summary>
	/// Overlay
	/// </summary>
	private Bitmap _overlay = Empty;

	/// <summary>
	/// Image data of the canvas
	/// </summary>
	private Bitmap _canvas = Empty;

	/// <summary>
	/// The canvas link
	/// </summary>
	public Image Canvas => _canvas;

	/// <summary>
	/// The plain image of the Background
	/// </summary>
	private Bitmap? _image;

	/// <summary>
	/// The scaled image of the Background
	/// </summary>
	private Bitmap? _scaledImage;

	/// <summary>
	/// Mouse Location
	/// </summary>
	private float _mouseLocationX = 0.0f;

	private float _mouseLocationY = 0.0f;

	/// <summary>
	/// Image scale
	/// </summary>
	private float _scale = 1.0f;

	/// <summary>
	/// Background color
	/// </summary>
	private Color _backgroundColor = Colors.White;

	/// <summary>
	/// Overlay color
	/// </summary>
	private Color _overlayColor = Colors.Transparent;

	/// <summary>
	/// Canvas center
	/// </summary>
	private float _canvasCenterX = 0.0f;

	private float _canvasCenterY = 0.0f;

	/// <summary>
	/// Scale factor
	/// </summary>
	private float _scaleFactor = 0.1f;

	/// <summary>
	/// Max scale factor
	/// </summary>
	public const float MAX_SCALE_FACTOR = 0.3f;

	/// <summary>
	/// Min scale factor
	/// </summary>
	public const float MIN_SCALE_FACTOR = 0.01f;


	/// <summary>
	/// Is the mouse down
	/// </summary>
	private bool _mouseDown = false;

	/// <summary>
	/// The edit mode of this editor view
	/// </summary>
	private bool _editMode = BackgroundMode;

	/// <summary>
	/// The selected border
	/// </summary>
	private Border? _selectedBorder;

	public void UpdateCanvas()
	{
		using var g = new Graphics(_canvas);
		DrawEditorView(g);
	}

	/// <summary>
	/// Sets the image for the token
	/// </summary>
	/// <param name="bitmap"></param>
	public void SetImage(Bitmap bitmap)
	{
		_image = bitmap;
	}

	/// <summary>
	/// Sets the background color
	/// </summary>
	/// <param name="color"></param>
	public void SetBackgroundColor(Color color)
	{
		_backgroundColor = color;
	}

	/// <summary>
	/// Draws the editor view
	/// </summary>
	/// <param name="graphics"></param>
	private void DrawEditorView(Graphics graphics)
	{
		graphics.FillRectangle(_backgroundColor, 0, 0, _canvas.Width, _canvas.Height);

		switch (_editMode)
		{
			case TokenMode:
			{
				var overlayCenter = _overlay.Center();

				if (_image != null)
				{
					var newWidth = (int)(_image.Width * _scale);
					var newHeight = (int)(_image.Height * _scale);

					if (_scaledImage == null ||
					    (_scaledImage != null && (_scaledImage.Width != newWidth || _scaledImage.Height != newHeight)))
					{
						var scaled = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppRgba);

						using var scaledGraphics = new Graphics(scaled);
						scaledGraphics.DrawImage(_image, 0, 0, newWidth, newHeight);

						_scaledImage = scaled;
					}

					var imageCenter = _scaledImage?.Center() ?? _image.Center();
					var img = _scaledImage ?? _image;

					graphics.DrawImage(img,
							_canvasCenterX - imageCenter.X,
							_canvasCenterY - imageCenter.Y);

					// draw rect over img
					graphics.FillRectangle(_overlayColor,
							_canvasCenterX - imageCenter.X,
							_canvasCenterY - imageCenter.Y,
							img.Width,
							img.Height);
				}

				graphics.DrawImage(_overlay,
						_mouseLocationX - overlayCenter.X,
						_mouseLocationY - overlayCenter.Y);
				break;
			}
			case BackgroundMode:
			{
				var overlayCenter = _overlay.Center();

				if (_image != null)
				{
					var newWidth = (int)(_image.Width * _scale);
					var newHeight = (int)(_image.Height * _scale);

					if (_scaledImage == null ||
					    (_scaledImage != null && (_scaledImage.Width != newWidth || _scaledImage.Height != newHeight)))
					{
						var scaled = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppRgba);

						using var scaledGraphics = new Graphics(scaled);
						scaledGraphics.DrawImage(_image, 0, 0, newWidth, newHeight);

						_scaledImage = scaled;
					}

					var imageCenter = _scaledImage?.Center() ?? _image.Center();

					var img = _scaledImage ?? _image;

					graphics.DrawImage(img,
							_mouseLocationX - imageCenter.X,
							_mouseLocationY - imageCenter.Y);

					// draw rect over img
					graphics.FillRectangle(_overlayColor,
							_canvasCenterX - imageCenter.X,
							_canvasCenterY - imageCenter.Y,
							img.Width,
							img.Height);
				}

				graphics.DrawImage(_overlay, _canvasCenterX - overlayCenter.X, _canvasCenterY - overlayCenter.Y);
				break;
			}
		}
	}


	/// <summary>
	/// Sets the canvas size
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public void SetCanvasSize(int width, int height)
	{
		_canvas = new Bitmap(width, height, PixelFormat.Format32bppRgba);

		Log.Debug("Updated Canvas Width {Width} Height {Height}", width, height);

		_canvasCenterX = width / 2.0f;
		_canvasCenterY = height / 2.0f;
	}

	/// <summary>
	/// Sets the position of the mouse
	/// </summary>
	/// <param name="locationX"></param>
	/// <param name="locationY"></param>
	public void SetPosition(float locationX, float locationY)
	{
		_mouseLocationX = locationX;
		_mouseLocationY = locationY;

		//Log.Debug("Clicked {LocX} {LocY}", locationX, locationY);
	}

	/// <summary>
	/// Sets the scale of the token image
	/// </summary>
	/// <param name="deltaHeight"></param>
	public void Scale(float deltaHeight)
	{
		_scale = deltaHeight * _scaleFactor;
		_scale = Math.Clamp(_scale, 0.1f, 3.0f);
		//Log.Debug("Token Creator Scale: {Scale}", _scale);
	}

	/// <summary>
	/// Gets the clipped image
	/// </summary>
	/// <returns></returns>
	public Bitmap GetClippedImage()
	{
		var clippedBmp = GetClipped();
		return clippedBmp;
	}

	/// <summary>
	/// Gets the clipped bitmap from the canvas
	/// </summary>
	/// <returns></returns>
	private Bitmap GetClipped()
	{
		var dst = new Bitmap(_overlay.Width, _overlay.Height, PixelFormat.Format32bppRgba);

		using var g = new Graphics(dst);

		switch (_editMode)
		{
			case TokenMode:
			{
				g.DrawImage(Canvas,
						new RectangleF(_mouseLocationX - _overlay.Width / 2.0f,
								_mouseLocationY - _overlay.Height / 2.0f,
								_canvas.Width,
								_canvas.Height),
						new PointF(0, 0));
				break;
			}
			case BackgroundMode:
			{
				g.DrawImage(Canvas,
						new RectangleF(_canvasCenterX - _overlay.Width / 2.0f,
								_canvasCenterY - _overlay.Height / 2.0f,
								_canvas.Width,
								_canvas.Height),
						new PointF(0, 0));
				break;
			}
		}

		return dst;
	}

	/// <summary>
	/// Sets the mouse to down
	/// </summary>
	public void MouseDown()
	{
		_mouseDown = true;
	}

	/// <summary>
	/// Sets the mouse to up 
	/// </summary>
	public void MouseUp()
	{
		_mouseDown = false;
	}

	/// <summary>
	/// Returns true if the mouse is down
	/// </summary>
	/// <returns></returns>
	public bool IsMouseDown()
	{
		return _mouseDown;
	}

	/// <summary>
	/// Sets the overlay color
	/// </summary>
	/// <param name="value"></param>
	public void SetOverlayColor(Color value)
	{
		_overlayColor = value;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="editMode">True is token mode, False is background mode</param>
	public void SetEditMode(bool editMode)
	{
		_editMode = editMode;

		Log.Information("Changed edit mode to {EditMode} mode", editMode ? "Token" : "Background");
	}

	/// <summary>
	/// Sets the scale factor
	/// <br/>
	/// If no parameter is given, will reset to default
	/// </summary>
	/// <param name="factor"></param>
	public void SetScaleFactor(float factor = 0.1f)
	{
		_scaleFactor = Math.Clamp(factor, MIN_SCALE_FACTOR, MAX_SCALE_FACTOR);
	}

	/// <summary>
	/// Sets the border
	/// </summary>
	/// <param name="border"></param>
	public void SetBorder(Border? border)
	{
		if (border == null) return;

		_selectedBorder = border;
		_overlay = _selectedBorder.GetBitmap();
	}

	/// <summary>
	/// Exports the current preview to file
	/// </summary>
	/// <param name="fileName"></param>
	public void ExportTo(string fileName)
	{
		if (!HasImage() || _selectedBorder == null) return;

		CalculateMaskedBitmap().Save(fileName, ImageFormat.Png);
		Log.Information("Exported image to {FileName}", fileName);

		TokenCreatorPlugin.Instance.PlatformHandler.OpenFolder(fileName, true);
	}

	/// <summary>
	/// Returns true if the current image is not null
	/// </summary>
	/// <returns></returns>
	public bool HasImage()
	{
		return _image != null;
	}

	/// <summary>
	/// Calculates the masked image
	/// <br/>
	/// </summary>
	/// <returns></returns>
	private Bitmap CalculateMaskedBitmap()
	{
		if (_selectedBorder == null) return Empty;

		var maskedBitmap = GetClipped();
		using var clippedData = maskedBitmap.Lock();

		var mask = _selectedBorder.GetMask();

		// calculate offset of bmp from center of clipped
		var offsetX = (int)(mask.Width / 2.0f - maskedBitmap.Width / 2.0f);
		var offsetY = (int)(mask.Height / 2.0f - maskedBitmap.Height / 2.0f);

		mask = mask.Extract(offsetX, offsetY, maskedBitmap.Width, maskedBitmap.Height);

		using var maskData = mask.Lock();

		var resultBytes = Math.Abs(clippedData.ScanWidth) * maskedBitmap.Height;

		var result = new byte[resultBytes];
		var maskBytesArray = new byte[resultBytes];

		Marshal.Copy(clippedData.Data, result, 0, resultBytes);
		Marshal.Copy(maskData.Data, maskBytesArray, 0, resultBytes);

		var pixels = Enumerable.Range(0, maskBytesArray.Length / 4).Select(x => new
		{
				mB = maskBytesArray[x * 4],
				mG = maskBytesArray[(x * 4) + 1],
				mR = maskBytesArray[(x * 4) + 2],
				mA = maskBytesArray[(x * 4) + 3],

				rB = result[x * 4],
				rG = result[(x * 4) + 1],
				rR = result[(x * 4) + 2],
				rA = result[(x * 4) + 3],

				MakeTransparent = new Action(() => result[(x * 4) + 3] = 0)
		});

		pixels.AsParallel().ForAll(p =>
		{
			// if black make transparent
			if (p.mR == 0 && p.mG == 0 && p.mB == 0)
			{
				p.MakeTransparent();
			}
		});

		Marshal.Copy(result, 0, clippedData.Data, resultBytes);

		return maskedBitmap;
	}
}