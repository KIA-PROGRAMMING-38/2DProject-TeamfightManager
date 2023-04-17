public static class ChampionUtility
{
	public static string CalcClassTypeName(ChampionClassType classType)
	{
		switch (classType)
		{
			case ChampionClassType.Warrior:
				return "전사";
			case ChampionClassType.ADCarry:
				return "원거리";
			case ChampionClassType.Magician:
				return "마법사";
			case ChampionClassType.Assistant:
				return "전투 보조";
			case ChampionClassType.Assassin:
				return "암살자";
		}

		return "";
	}
}