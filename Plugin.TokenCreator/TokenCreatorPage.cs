#region

using DnDHelperV2.PluginAPI;
using Eto.Forms;

#endregion

namespace Plugin.TokenCreator;

public sealed class TokenCreatorPage : ITabComponent
{
	public string Title => "Token Creator";

	private CanvasWidget _canvasWidget;

	public Panel GetRootPanel()
	{
		var root = new StackLayout
		{
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Stretch
		};

		_canvasWidget = new CanvasWidget
		{
				Border = TokenCreatorPlugin.Instance.Borders[0]
		};

		root.Items.Add(new StackLayoutItem
		{
				Control = new GroupBox
				{
						Text = "Editor",
						Content = _canvasWidget
				},
				Expand = true
		});

		return root;
	}

	private StackLayout GetSidePanel()
	{
		var layout = new StackLayout();

		return layout;
	}
}