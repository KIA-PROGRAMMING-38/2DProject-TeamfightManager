using System;

class BattleStageChampionDataTable
{
	public event Action<BattleInfoData> OnChangedBattleInfoData;
	public event Action<float> OnChangedHPRatio;
	public event Action<float> OnChangedMana;
	public event Action<bool> OnChangedUseUltimate;

	private BattleInfoData battleInfoData;

	private Champion _champion;
	public Champion champion
	{
		get => _champion;
		set
		{
			_champion = value;

			_champion.OnChangedHPRatio -= OnChangedChampionHPRatio;
			_champion.OnChangedHPRatio += OnChangedChampionHPRatio;

			_champion.OnChangedMana -= OnChangedChampionMana;
			_champion.OnChangedMana += OnChangedChampionMana;

			_champion.OnChangedUseUltimate -= OnChangedChampionUseUltiate;
			_champion.OnChangedUseUltimate += OnChangedChampionUseUltiate;
		}
	}

	public Pilot pilot { get; set; }

	public BattleStageChampionDataTable()
	{
		battleInfoData = new BattleInfoData();
	}

	public int takeDamage
	{
		set
		{
			battleInfoData.totalHit += value;
			OnChangedBattleInfoData?.Invoke(battleInfoData);
		}
	}

	public int attackDamage
	{
		set
		{
			battleInfoData.totalDamage += value;
			OnChangedBattleInfoData?.Invoke(battleInfoData);
		}
	}

	public int hill
	{
		set
		{
			battleInfoData.totalHill += value;
		}
	}

	private void OnChangedChampionHPRatio(float hpRatio)
	{
		OnChangedHPRatio?.Invoke(hpRatio);
	}

	private void OnChangedChampionMana(float manaRatio)
	{
		OnChangedMana?.Invoke(manaRatio);
	}

	private void OnChangedChampionUseUltiate(bool isUseUltimate)
	{
		OnChangedUseUltimate?.Invoke(isUseUltimate);
	}
}