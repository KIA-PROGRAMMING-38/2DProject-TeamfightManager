using UnityEngine;

public static class LayerTable
{
	public static class Number
	{
		public static readonly int REDTEAM_CHAMPION = LayerMask.NameToLayer(Name.REDTEAM_CHAMPION);
		public static readonly int REDTEAM_ATKSUMMON = LayerMask.NameToLayer(Name.REDTEAM_ATKSUMMON);
		public static readonly int REDTEAM_BUFFSUMMON = LayerMask.NameToLayer(Name.REDTEAM_BUFFSUMMON);

		public static readonly int BLUETEAM_CHAMPION = LayerMask.NameToLayer(Name.BLUETEAM_CHAMPION);
		public static readonly int BLUETEAM_ATKSUMMON = LayerMask.NameToLayer(Name.BLUETEAM_ATKSUMMON);
		public static readonly int BLUETEAM_BUFFSUMMON = LayerMask.NameToLayer(Name.BLUETEAM_BUFFSUMMON);

		public static readonly int STAGE_AREALIMITLINE = LayerMask.NameToLayer(Name.STAGE_AREALIMITLINE);
		public static readonly int STAGE_OUTOFAREA = LayerMask.NameToLayer(Name.STAGE_AREALIMITLINE);

		public static int CalcOtherTeamLayer(int layer)
		{
			if (layer == REDTEAM_CHAMPION)
				return BLUETEAM_CHAMPION;

			if (layer == REDTEAM_ATKSUMMON)
				return BLUETEAM_ATKSUMMON;

			if (layer == REDTEAM_BUFFSUMMON)
				return BLUETEAM_BUFFSUMMON;

			if (layer == BLUETEAM_CHAMPION)
				return REDTEAM_CHAMPION;

			if (layer == BLUETEAM_ATKSUMMON)
				return REDTEAM_ATKSUMMON;

			if (layer == BLUETEAM_BUFFSUMMON)
				return BLUETEAM_CHAMPION;

			return -1;
		}
    }

	public static class Name
	{
		public const string REDTEAM_CHAMPION = "RedTeamChampion";
		public const string REDTEAM_ATKSUMMON = "RedTeamAtkSummon";
		public const string REDTEAM_BUFFSUMMON = "RedTeamBuffSummon";

		public const string BLUETEAM_CHAMPION = "BlueTeamChampion";
		public const string BLUETEAM_ATKSUMMON = "BlueTeamAtkSummon";
		public const string BLUETEAM_BUFFSUMMON = "BlueTeamBuffSummon";

		public const string STAGE_AREALIMITLINE = "StageAreaLimitLine";
		public const string STAGE_OUTOFAREA = "StageOutside";
    }
}