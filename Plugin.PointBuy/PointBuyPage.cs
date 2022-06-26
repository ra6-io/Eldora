#region

using DnDHelperV2.PluginAPI;
using Eto.Drawing;
using Eto.Forms;

#endregion

namespace Plugin.PointBuy;

// TODO: Update UI

public sealed class PointBuyPage : ITabComponent
{
	public string Title => "Point Buy";

	private static readonly Font TitleFont = new("Arial", 18.0f);
	private static readonly Font StandardFont = new("Arial", 14.0f);

	private enum Attribute
	{
		Strength,
		Dexterity,
		Constitution,
		Intelligence,
		Wisdom,
		Charisma
	}

	private delegate void UpdateEventHandler(object sender);

	private event UpdateEventHandler? UpdateEvent;

	private readonly Dictionary<Attribute, AttributeValues> _attributeData = new()
	{
			{ Attribute.Strength, new AttributeValues() },
			{ Attribute.Dexterity, new AttributeValues() },
			{ Attribute.Constitution, new AttributeValues() },
			{ Attribute.Intelligence, new AttributeValues() },
			{ Attribute.Wisdom, new AttributeValues() },
			{ Attribute.Charisma, new AttributeValues() }
	};

	public Panel GetRootPanel()
	{
		var table = new TableLayout
		{
				Rows =
				{
						new TableRow { ScaleHeight = true },

						BuildTitleRow(),

						BuildAttributePanel(Attribute.Strength),
						BuildAttributePanel(Attribute.Dexterity),
						BuildAttributePanel(Attribute.Constitution),
						BuildAttributePanel(Attribute.Intelligence),
						BuildAttributePanel(Attribute.Wisdom),
						BuildAttributePanel(Attribute.Charisma),

						BuildBottomRow(),

						new TableRow { ScaleHeight = true }
				}
		};
		ResetValues();

		var layout = new StackLayout
		{
				//BackgroundColor = Colors.Red,
				Orientation = Orientation.Vertical,
				HorizontalContentAlignment = HorizontalAlignment.Stretch
		};

		layout.Items.Add(table);

		layout.Items.Add(new GroupBox
		{
				Text = "Config",
		});

		return new Panel
		{
				Content = layout,
				// Content = table
		};
	}

	private void ResetValues()
	{
		foreach (var (_, value) in _attributeData)
		{
			value.AbilityScore = PointBuyPlugin.Instance.SelectedConfig().MinimumPurchaseable;
		}

		UpdateEvent?.Invoke(this);
	}

	private TableRow BuildBottomRow()
	{
		var totalPointsLabel = new Label
		{
				Text = "0/27",
				TextAlignment = TextAlignment.Center,
				Font = TitleFont,
		};

		UpdateEvent += _ =>
		{
			var spentPoints = _attributeData.Select(pair => pair.Value.PointCost()).Sum();

			var availablePoints = PointBuyPlugin.Instance.SelectedConfig().AvailablePoints;
			totalPointsLabel.Text = $"{spentPoints}/{availablePoints}";

			totalPointsLabel.TextColor = spentPoints > availablePoints ? Colors.Red : Colors.Black;
		};

		var tableRow = new TableRow
		{
				Cells =
				{
						new TableCell(null),
						new TableCell(null),
						new TableCell(new Button((sender, args) => ResetValues())
						{
								Text = "Reset"
						}),
						new TableCell(null),
						new TableCell(null),
						new TableCell(null),
						new TableCell(null),
						new TableCell(new Label
						{
								Text = "Total Points",
								Font = TitleFont,
								TextAlignment = TextAlignment.Center
						}),
						new TableCell(totalPointsLabel),
						new TableCell(null),
				}
		};
		return tableRow;
	}

	private TableRow BuildTitleRow()
	{
		var tableRow = new TableRow
		{
				Cells =
				{
						new TableCell(null, true),
						new TableCell(new Label
						{
								Text = "Attribute",
								Font = TitleFont,
								Width = 150,
								TextAlignment = TextAlignment.Center
						}),
						new TableCell(new Label
								{
										Text = "Ability Score",
										Font = TitleFont,
										TextAlignment = TextAlignment.Center
								},
								true),
						new TableCell(new Label
								{
										Text = "+",
										Font = TitleFont,
										TextAlignment = TextAlignment.Center
								},
								true),
						new TableCell(new Label
								{
										Text = "Racial Bonus",
										Font = TitleFont,
										TextAlignment = TextAlignment.Center
								},
								true),
						new TableCell(new Label
								{
										Text = "=",
										Font = TitleFont,
										TextAlignment = TextAlignment.Center
								},
								true),
						new TableCell(new Label
								{
										Text = "Total Score",
										Font = TitleFont,
										TextAlignment = TextAlignment.Center
								},
								true),
						new TableCell(new Label
								{
										Text = "Ability Modifier",
										Font = TitleFont,
										TextAlignment = TextAlignment.Center
								},
								true),
						new TableCell(new Label
								{
										Text = "Point Cost",
										Font = TitleFont,
										TextAlignment = TextAlignment.Center
								},
								true),
						new TableCell(null, true),
				},
		};
		return tableRow;
	}

	private TableRow BuildAttributePanel(Attribute attribute)
	{
		var abilityScoreStepper = new NumericStepper
		{
				MinValue = PointBuyPlugin.Instance.SelectedConfig().MinimumPurchaseable,
				MaxValue = PointBuyPlugin.Instance.SelectedConfig().MaximumPurchaseable,
				Value = PointBuyPlugin.Instance.SelectedConfig().MinimumPurchaseable
		};
		_attributeData[attribute].AbilityScore = (int)abilityScoreStepper.Value;

		var racialBonus = new Label
		{
				Text = "0",
				Font = TitleFont,
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
		};

		var totalScoreLabel = new Label
		{
				Text = "0",
				Font = TitleFont,
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
		};

		var abilityModifierLabel = new Label
		{
				Text = "0",
				Font = TitleFont,
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
		};

		var pointCostLabel = new Label
		{
				Text = "0",
				Font = TitleFont,
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
		};

		abilityScoreStepper.ValueChanged += (_, _) =>
		{
			//Log.Information("{Attribute} changed to {Value}", attribute, abilityScoreStepper.Value);
			var attr = _attributeData[attribute];
			attr.AbilityScore = (int)abilityScoreStepper.Value;

			UpdateEvent?.Invoke(this);
		};

		UpdateEvent += _ =>
		{
			var attr = _attributeData[attribute];
			abilityScoreStepper.Value = attr.AbilityScore;

			pointCostLabel.Text = attr.PointCost().ToString();

			totalScoreLabel.Text = attr.TotalScore().ToString();
			abilityModifierLabel.Text = attr.AbilityModifier().ToString();
		};

		var tableRow = new TableRow
		{
				Cells =
				{
						new TableCell(null),
						new TableCell(new Label
						{
								Text = attribute.ToString(),
								Font = TitleFont,
								TextAlignment = TextAlignment.Left,
								VerticalAlignment = VerticalAlignment.Center
						}),
						new TableCell(abilityScoreStepper),
						new TableCell(new Label
						{
								Text = "+",
								Font = TitleFont,
								TextAlignment = TextAlignment.Center,
								VerticalAlignment = VerticalAlignment.Center
						}),
						new TableCell(racialBonus),
						new TableCell(new Label
						{
								Text = "=",
								Font = TitleFont,
								TextAlignment = TextAlignment.Center,
								VerticalAlignment = VerticalAlignment.Center
						}),
						new TableCell(totalScoreLabel),
						new TableCell(abilityModifierLabel),
						new TableCell(pointCostLabel),
						new TableCell(null)
				},
		};
		return tableRow;
	}

	private sealed class AttributeValues
	{
		public int AbilityScore { get; set; }
		public int RacialBonus { get; set; }

		public int TotalScore()
		{
			return AbilityScore + RacialBonus;
		}

		public int PointCost()
		{
			return PointBuyPlugin.Instance.SelectedConfig().GetPointCostFor(AbilityScore);
		}

		/// <summary>
		/// subtract 10 from the TotalScore and then divide the total by 2
		/// </summary>
		/// <returns></returns>
		public int AbilityModifier()
		{
			return (int)Math.Floor((TotalScore() - 10.0) / 2.0);
		}
	}
}