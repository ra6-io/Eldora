#region

using System.ComponentModel;
using System.Runtime.CompilerServices;

#endregion

namespace Plugin.PointBuy;

public sealed class PointBuyData : INotifyPropertyChanged
{
	private int _strengthScore;
	private int _dexterityScore;
	private int _constitutionScore;
	private int _intelligenceScore;
	private int _wisdomScore;
	private int _charismaScore;

	public int StrengthScore
	{
		get => _strengthScore;
		set
		{
			_strengthScore = value;

			OnPropertyChanged(nameof(StrengthScore));
		}
	}

	public int DexterityScore
	{
		get => _dexterityScore;
		set
		{
			_dexterityScore = value;

			OnPropertyChanged(nameof(DexterityScore));
		}
	}

	public int ConstitutionScore
	{
		get => _constitutionScore;
		set
		{
			_constitutionScore = value;

			OnPropertyChanged(nameof(ConstitutionScore));
		}
	}

	public int IntelligenceScore
	{
		get => _intelligenceScore;
		set
		{
			_intelligenceScore = value;

			OnPropertyChanged(nameof(IntelligenceScore));
		}
	}

	public int WisdomScore
	{
		get => _wisdomScore;
		set
		{
			_wisdomScore = value;

			OnPropertyChanged(nameof(WisdomScore));
		}
	}

	public int CharismaScore
	{
		get => _charismaScore;
		set
		{
			_charismaScore = value;

			OnPropertyChanged(nameof(CharismaScore));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}