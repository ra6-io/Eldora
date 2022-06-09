using System.Collections.Generic;
using Eto.Forms;

namespace DnDHelperV2.InternalComponents.PluginPage;

public sealed class PluginPage : IInternalTabComponent
{
	public string Title => "Plugins";

	public bool IsOnlyDebugVisible => false;

	public Panel GetRootPanel()
	{
		var pluginPath = DnDHelper.PluginPath;

		var pluginLabel = new Label
		{
				Text = $"Plugin path: {pluginPath}"
		};

		var layout = new StackLayout
		{
				Orientation = Orientation.Vertical,
				HorizontalContentAlignment = HorizontalAlignment.Stretch
		};

		layout.Items.Add(pluginLabel);

		var installedPlugins = new ListBox {};

		DnDHelper.Instance.PluginLoader.ForEachLoadedPlugin(tuple =>
		{
			installedPlugins.Items.Add(
					$"Installed: {tuple.metadata.PluginName} - {tuple.metadata.PluginVersion} by {tuple.metadata.Author}");
		});

		var availablePlugins = new ListBox {};

		layout.Items.Add(new StackLayoutItem(
				new StackLayout
				{
						Orientation = Orientation.Horizontal,
						VerticalContentAlignment = VerticalAlignment.Stretch,
						Items =
						{
								new StackLayoutItem(new Scrollable
										{
												Content = availablePlugins
										},
										true),
								new StackLayoutItem(new Scrollable
										{
												Content = installedPlugins
										},
										true)
						}
				},
				true
		));

		return new Panel
		{
				Content = layout
		};
	}
}