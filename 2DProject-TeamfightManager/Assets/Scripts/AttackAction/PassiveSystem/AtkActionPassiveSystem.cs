public class AtkActionPassiveSystem
{
	private AttackAction _attackAction;
	private AtkActionPassiveData _passiveData;

	private Champion _ownerChampion;
	private ChampionAnimation _ownerChampAnim;

	public Champion ownerChampion
	{
		private get => _ownerChampion;
		set
		{
			_ownerChampion = value;
			_ownerChampAnim = _ownerChampion.GetComponentInChildren<ChampionAnimation>();

			switch (_passiveData.impactTimeKind)
			{
				case PassiveImpactTimeKind.OnMyTeamHit:
					break;
				case PassiveImpactTimeKind.OnKillTarget:
					_ownerChampion.OnKill -= OnOwnerChampionKill;
					_ownerChampion.OnKill += OnOwnerChampionKill;
					break;
			}
		}
	}

	public AtkActionPassiveSystem(AttackAction attackAction, AtkActionPassiveData passiveData)
	{
		_attackAction = attackAction;
		_passiveData = passiveData;
	}

	public void Release()
	{
		_ownerChampion.OnKill -= OnOwnerChampionKill;
	}

	private void OnOwnerChampionKill(Champion killedChampion)
	{
		_ownerChampion.Attack(ActionKeyTable.ULTIMATE);
		_ownerChampAnim.ChangeState(ChampionAnimation.AnimState.Ultimate, true);
	}
}