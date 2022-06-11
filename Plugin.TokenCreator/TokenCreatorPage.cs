using DnDHelperV2.PluginAPI;
using Eto.Drawing;
using Eto.Forms;
using Serilog;

namespace Plugin.TokenCreator;

public sealed class TokenCreatorPage : ITabComponent
{
	public string Title => "Token Creator";

	private readonly EditorView _activeEditorView = new();

	public event EventHandler<EventArgs>? UpdateCanvas;

	public Panel GetRootPanel()
	{
		var editorView = new ImageView();
		var previewView = new ImageView();

		previewView.Image = _activeEditorView.GetClippedImage();

		var layout = new StackLayout
		{
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Stretch,
				Items =
				{
						new StackLayoutItem(new GroupBox
								{
										Text = "Editor",
										Content = editorView
								},
								true),
						new StackLayout
						{
								Orientation = Orientation.Vertical,
								HorizontalContentAlignment = HorizontalAlignment.Stretch,
								Items =
								{
										new StackLayoutItem(new GroupBox
										{
												Text = "Preview",
												Content = previewView
										}),
										new Label
										{
												Text = "Note: The exported Image will only be inside the borders!!",
												Font = new Font("Arial", 10.0f, FontStyle.Bold),
												TextColor = Colors.Red
										},
										new StackLayoutItem(CreateSidePanel(), true),
								}
						},
				}
		};

		UpdateCanvas += (_, _) =>
		{
			_activeEditorView.UpdateCanvas();

			editorView.Image = _activeEditorView.Canvas;
			previewView.Image = _activeEditorView.GetClippedImage();
		};

		editorView.SizeChanged += (_, _) =>
		{
			_activeEditorView.SetCanvasSize(editorView.Size.Width, editorView.Size.Height);
			OnUpdateCanvas();
		};

		editorView.MouseWheel += (_, args) =>
		{
			_activeEditorView.Scale(args.Delta.Height);
			OnUpdateCanvas();
		};

		editorView.MouseDown += (_, args) =>
		{
			_activeEditorView.MouseDown();
			_activeEditorView.SetPosition(args.Location.X, args.Location.Y);
			OnUpdateCanvas();
		};

		editorView.MouseUp += (_, args) =>
		{
			_activeEditorView.MouseUp();
			_activeEditorView.SetPosition(args.Location.X, args.Location.Y);
			OnUpdateCanvas();
		};

		editorView.MouseMove += (_, args) =>
		{
			if (!_activeEditorView.IsMouseDown()) return;

			_activeEditorView.SetPosition(args.Location.X, args.Location.Y);
			OnUpdateCanvas();
		};

		_activeEditorView.SetBorder(TokenCreatorPlugin.Instance.Borders[0]);

		return new Panel
		{
				Content = layout
		};
	}

	private Panel CreateSidePanel()
	{
		var groupBox = new GroupBox
		{
				Text = "Tools",
				Width = 300
		};

		var layout = new StackLayout
		{
				Orientation = Orientation.Vertical,
				HorizontalContentAlignment = HorizontalAlignment.Stretch
		};

		var bordersCombobox = new ComboBox
		{
				ReadOnly = true,
				SelectedIndex = 0
		};
		TokenCreatorPlugin.Instance.Borders.ForEach(border => bordersCombobox.Items.Add(border.BorderName));

		bordersCombobox.SelectedValueChanged += (sender, args) =>
		{
			foreach (var border in TokenCreatorPlugin.Instance.Borders)
			{
				if (border.BorderName != bordersCombobox.SelectedValue.ToString()) continue;

				_activeEditorView.SetBorder(border);
				OnUpdateCanvas();
				break;
			}

			Log.Information("Selected border: {Border}", bordersCombobox.SelectedValue);
		};

		var backgroundColorPicker = new ColorPicker
		{
				Value = Colors.White
		};

		var overlayColorPicker = new ColorPicker
		{
				Value = Colors.Transparent,
				AllowAlpha = true
		};

		backgroundColorPicker.ValueChanged += (_, _) =>
		{
			_activeEditorView.SetBackgroundColor(backgroundColorPicker.Value);

			OnUpdateCanvas();
			//Log.Debug("Color changed to {@Color}", backgroundColorPicker.Value);
		};

		overlayColorPicker.ValueChanged += (_, _) =>
		{
			_activeEditorView.SetOverlayColor(overlayColorPicker.Value);

			OnUpdateCanvas();
			//Log.Debug("OverlayColor changed to {@Color}", overlayColorPicker.Value);
		};

		layout.Items.Add(new Label { Text = "Select Border:" });
		layout.Items.Add(bordersCombobox);

		layout.Items.Add(new Label { Text = "Background Color:" });
		layout.Items.Add(backgroundColorPicker);

		layout.Items.Add(new Label { Text = "Overlay Color:" });
		layout.Items.Add(overlayColorPicker);

		var tokenRadioButton = new RadioButton
		{
				Text = "Token"
		};

		var backgroundRadioButton = new RadioButton(tokenRadioButton)
		{
				Text = "Background",
				Checked = true
		};

		tokenRadioButton.CheckedChanged += (_, _) =>
		{
			_activeEditorView.SetEditMode(tokenRadioButton.Checked);

			OnUpdateCanvas();
		};

		var scaleFactorNumber = new NumericStepper
		{
				MaxValue = EditorView.MAX_SCALE_FACTOR,
				MinValue = EditorView.MIN_SCALE_FACTOR,
				Value = 0.1f,
				FormatString = "0.00",
				Increment = 0.01f
		};

		scaleFactorNumber.ValueChanged += (_, _) => { _activeEditorView.SetScaleFactor((float)scaleFactorNumber.Value); };

		layout.Items.Add(new GroupBox
		{
				Text = "Editor movement",
				Content = new StackLayout
				{
						Orientation = Orientation.Vertical,
						Items =
						{
								tokenRadioButton,
								backgroundRadioButton
						}
				}
		});

		layout.Items.Add(new StackLayout
		{
				Orientation = Orientation.Horizontal,
				Items =
				{
						new Label
						{
								Text = "Image scale factor",
								VerticalAlignment = VerticalAlignment.Center
						},
						scaleFactorNumber
				}
		});

		var selectedImageLabel = new Label();

		layout.Items.Add(new Button((_, _) =>
		{
			var fileSelector = new OpenFileDialog
			{
					Filters =
					{
							new FileFilter("Images", ".png", ".jpg", ".jpeg", ".bmp")
					}
			};

			if (fileSelector.ShowDialog(TokenCreatorPlugin.Instance.MainWindow) == DialogResult.Ok)
			{
				var fileName = fileSelector.FileName;

				Log.Information("Selected file: {FileName}", fileName);
				_activeEditorView.SetImage(new Bitmap(fileName));
				selectedImageLabel.Text = fileName;
			}
			else
			{
				Log.Information("Aborted file selection");
			}
		})
		{
				Text = "Select Image",
				ToolTip = "Selects the Image of the token"
		});
		layout.Items.Add(new Label { Text = "Selected Image: " });
		layout.Items.Add(selectedImageLabel);

		layout.Items.Add(null);

		layout.Items.Add(new Button((_, _) =>
		{
			if (!_activeEditorView.HasImage())
			{
				MessageBox.Show(TokenCreatorPlugin.Instance.MainWindow,
						"No image selected",
						"Error",
						MessageBoxButtons.OK,
						MessageBoxType.Error);
				return;
			}
		})
		{
				Text = "Save to json",
				ToolTip = "Saves as json to be able to edit afterwards"
		});

		layout.Items.Add(new Button((_, _) => {})
		{
				Text = "Load from json",
				ToolTip = "Loads a saved json"
		});

		layout.Items.Add(new Button((_, _) =>
		{
			if (!_activeEditorView.HasImage())
			{
				MessageBox.Show(TokenCreatorPlugin.Instance.MainWindow,
						"No image selected",
						"Error",
						MessageBoxButtons.OK,
						MessageBoxType.Error);
				return;
			}

			var fileSelector = new SaveFileDialog()
			{
					Filters =
					{
							new FileFilter("Images", ".png")
					}
			};

			if (fileSelector.ShowDialog(TokenCreatorPlugin.Instance.MainWindow) == DialogResult.Ok)
			{
				var fileName = fileSelector.FileName!;
				_activeEditorView.ExportTo(fileName);

				Log.Information("Selected file: {FileName}", fileName);
			}
			else
			{
				Log.Information("Aborted file saving");
			}
		})
		{
				Text = "Export to png",
				ToolTip = "Exports to png with transparency"
		});

		groupBox.Content = layout;
		return groupBox;
	}

	private void OnUpdateCanvas()
	{
		UpdateCanvas?.Invoke(this, EventArgs.Empty);
	}
}