#region

using Eto.Drawing;
using Eto.Forms;

#endregion

namespace Plugin.TokenCreator;

public sealed class CanvasWidget : Drawable
{
	/// <summary>
	///     The border to be displayed
	/// </summary>
	public BorderObject Border { get; set; }


	private bool _dragging;
	private PointF _dragStart;
	private PointF _dragPosition;

	public void Export()
	{

	}

	protected override void OnPaint(PaintEventArgs e)
	{
		var g = e.Graphics;

		g.FillRectangle(Brushes.Red, _dragPosition.X, _dragPosition.Y, 20, 20);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);

		_dragging = false;
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

		Invalidate(false);
	}

}