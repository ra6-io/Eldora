#region

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Serilog;

#endregion

namespace Plugin.PointBuy;

[JsonObject(MemberSerialization.OptIn)]
public sealed class PointBuyConfigObject
{
	/// <summary>
	/// The number of points available to spend.
	/// </summary>
	[JsonProperty("available_points")]
	public int AvailablePoints { get; set; }

	/// <summary>
	/// The maximum points to spend on a single attribute.
	/// </summary>
	[JsonProperty("maximum_points")]
	public int MaximumPurchaseable { get; set; }

	/// <summary>
	/// The minimum points to spend on a single attribute.
	/// </summary>
	[JsonProperty("minimum_points")]
	public int MinimumPurchaseable { get; set; }

	/// <summary>
	/// The cost of a points.
	/// <br/>
	/// Ranges between 3 and 18.
	/// </summary>
	[JsonProperty("point_costs")]
	public int[] PointCosts { get; set; } = DefaultPointCosts;

	private static readonly int[] DefaultPointCosts = { -9, -6, -4, -2, -1, 0, 1, 2, 3, 4, 5, 7, 9, 12, 15, 19 };


	[OnDeserialized]
	public void OnDeserialized(StreamingContext context)
	{
		if (PointCosts.Length == 16) return;

		Log.Warning(
				"PointBuyConfig: PointCosts length does not match 16, Reverting to default");
		PointCosts = DefaultPointCosts;

		if (MinimumPurchaseable < 3)
		{
			Log.Warning(
					"PointBuyConfig: MinimumPurchaseable is less than 3, Reverting to 3");
			MinimumPurchaseable = 3;
		}

		if (MaximumPurchaseable > 18)
		{
			Log.Warning(
					"PointBuyConfig: MaximumPurchaseable is greater than 18, Reverting to 18");
			MaximumPurchaseable = 18;
		}
	}

	public override string ToString()
	{
		return
				$"AvailablePoints: {AvailablePoints}, " +
				$"MaximumPurchaseable: {MaximumPurchaseable}, " +
				$"MinimumPurchaseable: {MinimumPurchaseable}, " +
				$"PointCosts: {string.Join(",", PointCosts)}, ";
	}

	/// <summary>
	/// Returns the cost of a point.
	/// </summary>
	/// <param name="totalScore"></param>
	/// <returns></returns>
	public int GetPointCostFor(int totalScore)
	{
		return totalScore switch
		{
				< 3 => PointCosts[0],
				> 18 => PointCosts[15],
				_ => PointCosts[totalScore - 3]
		};
	}
}