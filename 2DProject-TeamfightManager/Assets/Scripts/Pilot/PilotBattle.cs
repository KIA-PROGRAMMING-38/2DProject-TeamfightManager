using UnityEngine;

public class PilotBattle : MonoBehaviour
{
    public Champion controlChampion
    {
        get => _controlChampion;
		set
        {
            _controlChampion = value;
		}
    }
    private Champion _controlChampion;
    public BattleTeam myTeam { private get; set; }
}
