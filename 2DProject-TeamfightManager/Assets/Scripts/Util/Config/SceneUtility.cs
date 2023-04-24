public static class SceneNameTable
{
	// 최상위 씬(씬을 불러올 때 LoadSceneMode == Single를 사용)
	public const string START = "StartScene";
	public const string STADIUM = "StadiumScene";
	public const string DORMITORY = "DormitoryScene";
	public const string TITLE = "TitleScene";

	// 하위 씬(씬을 불러올 때 LoadSceneMode == Additive를 사용)
	public const string BANPICK_UI = "BanpickScene";
	public const string BATTLETEAM_INFO_UI = "BattleTeamInfoUIScene";
	public const string BATTLESTAGE = "ColoseumScene";
	public const string CHAMP_STATUSBAR_UI = "ChampStatusBarUIScene";

	public const string DORMITORY_UI = "DormitoryUIScene";

	public const string TITLE_FIGHT = "TitleFightScene";
}