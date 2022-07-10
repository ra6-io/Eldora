#region

using Eto.Drawing;
using Eto.Forms;

#endregion

namespace Plugin.TokenCreator;

public sealed class PreviewChangedEventArgs : EventArgs
{
	public Bitmap Image { get; }

	public PreviewChangedEventArgs(Bitmap image)
	{
		Image = image;
	}
}

public sealed class CanvasWidget : Drawable
{
	private BorderObject? _border;

	/// <summary>
	///     The border to be displayed
	/// </summary>
	public BorderObject? Border
	{
		get => _border;
		set
		{
			_border = value;
			//if (_border != null) _previewBitmap = new RawBitmap(_border.ImageBitmap.Width, _border.ImageBitmap.Height);
			Invalidate(false);
		}
	}

	private Image? _tokenImage;

	/// <summary>
	///     The image to be tokenized
	/// </summary>
	public Image? TokenImage
	{
		get => _tokenImage;
		set
		{
			_tokenImage = value;
			Invalidate(false);
		}
	}

	private Color _tokenBackgroundColor = Colors.White;

	/// <summary>
	///     The background color of the token
	/// </summary>
	public Color TokenBackgroundColor
	{
		get => _tokenBackgroundColor;
		set
		{
			_tokenBackgroundColor = value;
			Invalidate(false);
		}
	}

#region Scale

	/// <summary>
	///     The maximum scale values of the token image
	/// </summary>
	private const float MinScale = 0.1f;

	private const float MaxScale = 10.0f;

	/// <summary>
	///     The current scale of the token image
	/// </summary>
	private float _scale = 1.0f;

	private float _scaleFactor = 0.1f;

	/// <summary>
	///     The factor for scaling
	/// </summary>
	public float ScaleFactor
	{
		get => _scaleFactor;
		set
		{
			if (value > 3.0f) value = 3.0f;
			if (value < 0.1f) value = 0.1f;
			_scaleFactor = value;
		}
	}

#endregion

	public event EventHandler<PreviewChangedEventArgs>? PreviewChanged;

#region Dragging

	private bool _dragging;
	private PointF _dragStart;
	private PointF _dragPosition;

#endregion

	private PointF _canvasCenter;

	/// <summary>
	///     Exports the preview to the path
	/// </summary>
	/// <param name="path"></param>
	public void Export(string path)
	{
		var masked = GetMaskedValue();
		masked?.Save(path, ImageFormat.Png);
	}

	/// <summary>
	///     Recalculates the preview
	///     Raises the <see cref="PreviewChanged" /> event
	/// </summary>
	public void RecalculatePreviewImage()
	{
		var masked = GetMaskedValue();
		if (masked == null) return;
		RaisePreviewChangedEvent(masked);
	}

	/// <summary>
	///     Gets the masked value
	/// </summary>
	/// <returns></returns>
	private Bitmap? GetMaskedValue()
	{
		if (Border == null) return null;
		if (Width <= 0 || Height <= 0) return null;

		// Create the preview bitmap
		var image = new Bitmap(Width, Height, PixelFormat.Format32bppRgba);

		using (var g = new Graphics(image))
		{
			g.FillRectangle(TokenBackgroundColor, 0, 0, Width, Height);

			if (TokenImage != null)
			{
				// Paint token
				g.SaveTransform();
				g.TranslateTransform(_dragPosition);

				g.ScaleTransform(_scale);
				g.TranslateTransform(-TokenImage.Center());

				g.DrawImage(TokenImage, PointF.Empty);
				g.RestoreTransform();
			}
		}

		// Rectangle for the token center
		var rect = new Rectangle(new Point(_canvasCenter - Border.ImageBitmap.Size / 2), Border.ImageBitmap.Size);

		image = image.CloneAndExtract(PixelFormat.Format32bppRgba, rect);
		// mask image
		var masked = new RawBitmap(image).Mask(Border.MaskBitmap).Bitmap;

		// Paints the overlay border
		using (var g = new Graphics(masked))
		{
			g.DrawImage(Border.ImageBitmap, PointF.Empty);
		}

		return masked;
	}

	/// <summary>
	///     Raises the <see cref="PreviewChanged" /> event
	/// </summary>
	/// <param name="image"></param>
	private void RaisePreviewChangedEvent(Bitmap image)
	{
		PreviewChanged?.Invoke(this, new PreviewChangedEventArgs(image));
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		var g = e.Graphics;

		g.FillRectangle(TokenBackgroundColor, 0, 0, Width, Height);

		if (TokenImage != null)
		{
			// Paint token
			g.SaveTransform();
			g.TranslateTransform(_dragPosition);

			g.ScaleTransform(_scale);
			g.TranslateTransform(-TokenImage.Center());

			g.DrawImage(TokenImage, PointF.Empty);
			g.RestoreTransform();
		}

		// Draws the border
		if (Border != null) g.DrawImage(Border.ImageBitmap, _canvasCenter - Border.ImageBitmap.Center());
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);

		_canvasCenter = new PointF(Width / 2f, Height / 2f);

		// Repaints
		Invalidate(false);
		RecalculatePreviewImage();
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);

		_dragging = false;
		RecalculatePreviewImage();
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);

		_dragging = true;
		_dragStart = e.Location;
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);

		if (!_dragging) return;

		var dif = e.Location - _dragStart;
		_dragPosition += dif;
		_dragStart = e.Location;

		// Repaints
		Invalidate(false);
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		base.OnMouseWheel(e);

		_scale += e.Delta.Height * ScaleFactor;
		_scale = Math.Clamp(_scale, MinScale, MaxScale);

		Invalidate(false);
		RecalculatePreviewImage();
	}
}