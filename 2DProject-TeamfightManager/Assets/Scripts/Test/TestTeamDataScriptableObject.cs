using UnityEngine;

[CreateAssetMenu(fileName = "TeamData", menuName = "Test/TeamData")]
public class TestTeamDataScriptableObject : ScriptableObject
{
    public TeamData teamData;
    public TeamBelongPilotData belongPilotData;
    public TeamResourceData resourceData;

    public string baseFilePath = "Assets/Data";
    public string extension = ".data";
}