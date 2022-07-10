#region

using Eldora.PluginAPI;
using Eto.Drawing;
using Eto.Forms;
using Serilog;

#endregion

namespace Plugin.TokenCreator;

public sealed class TokenCreatorPage : ITabComponent
{
	private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp" };

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

		root.Items.Add(GetSidePanel());

		return root;
	}

	private StackLayout GetSidePanel()
	{
		var layout = new StackLayout
		{
				Width = 300,
				Orientation = Orientation.Vertical,
				HorizontalContentAlignment = HorizontalAlignment.Stretch
		};

		var preview = new ImageView
		{
				BackgroundColor = Colors.LightGrey
		};
		_canvasWidget.PreviewChanged += (_, args) => { preview.Image = args.Image; };

		layout.Items.Add(new StackLayoutItem
		{
				Control = new GroupBox
				{
						Text = "Preview",
						Content = preview
				}
		});

		var backgroundColorSelector = new ColorPicker
		{
				Value = Colors.White,
				AllowAlpha = false
		};

		backgroundColorSelector.ValueChanged += (_, _) =>
		{
			_canvasWidget.TokenBackgroundColor = backgroundColorSelector.Value;
			_canvasWidget.RecalculatePreviewImage();
		};

		var tokenSelector = new ComboBox
		{
				ReadOnly = true
		};
		TokenCreatorPlugin.Instance.Borders.ForEach(border => { tokenSelector.Items.Add(border.BorderName); });

		tokenSelector.SelectedIndexChanged += (_, _) =>
		{
			_canvasWidget.Border = TokenCreatorPlugin.Instance.Borders[tokenSelector.SelectedIndex];
			_canvasWidget.RecalculatePreviewImage();
		};
		tokenSelector.SelectedIndex = 0;

		var selectedImageLabel = new Label { Text = "Selected Image:" };

		var imageSelectionButton = new Button
		{
				Text = "Select Image"
		};

		imageSelectionButton.Click += (_, _) =>
		{
			var selector = new OpenFileDialog
			{
					Filters =
					{
							new FileFilter("Image", ImageExtensions)
					},
					MultiSelect = false
			};

			if (selector.ShowDialog(TokenCreatorPlugin.Instance.MainWindow) != DialogResult.Ok) return;

			var file = selector.FileName;
			if (file == null) return;

			Log.Information("Selected image: {File}", file);
			selectedImageLabel.Text = $"Selected: {file}";
			_canvasWidget.TokenImage = new Bitmap(file);
		};

		layout.Items.Add(new StackLayoutItem
		{
				Control = new GroupBox
				{
						Text = "Tools",
						Content = new StackLayout
						{
								Orientation = Orientation.Vertical,
								HorizontalContentAlignment = HorizontalAlignment.Stretch,
								Items =
								{
										tokenSelector,
										backgroundColorSelector,
										imageSelectionButton,
										selectedImageLabel
								}
						}
				},
				Expand = true
		});

		var exportImageButton = new Button
		{
				Text = "Export Token"
		};

		exportImageButton.Click += (sender, args) =>
		{

			var selector = new SaveFileDialog()
			{
					Filters =
					{
							new FileFilter("Image", ImageExtensions)
					}
			};

			if (selector.ShowDialog(TokenCreatorPlugin.Instance.MainWindow) != DialogResult.Ok) return;

			var file = selector.FileName;
			if (file == null) return;

			Log.Information("Selected image: {File}", file);

			_canvasWidget.Export(file);
			TokenCreatorPlugin.Instance.PlatformHandler.OpenFolder(file, true);
		};

		var exportJsonButton = new Button
		{
				Text = "Export JSON"
		};
		exportImageButton.Click += (sender, args) => {};

		var importJsonButton = new Button
		{
				Text = "Import JSON"
		};
		importJsonButton.Click += (sender, args) => {};

		layout.Items.Add(new StackLayoutItem
		{
				Control = new GroupBox
				{
						Text = "IO",
						Content = new StackLayout
						{
								Orientation = Orientation.Vertical,
								HorizontalContentAlignment = HorizontalAlignment.Stretch,
								Items =
								{
										exportImageButton,
										exportJsonButton,
										importJsonButton
								}
						}
				}
		});

		return layout;
	}
}