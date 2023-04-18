using UnityEngine;

[CreateAssetMenu(fileName = "PilotData", menuName = "Test/PilotData")]
public class TestPilotScriptableObject : ScriptableObject
{
    public PilotData pilotData;

    public string baseFilePath = "Assets/Data";
    public string extension = ".data";
}