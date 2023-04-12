using UnityEngine;
using UnityEngine.UI;

public class TestCreateDataSaveFile : MonoBehaviour
{
	public TestAttackActionScriptableObject attackActionPrefab;
	public KeyCode attackActionCreateKey = KeyCode.Space;

	public Text text;

	public void Awake()
	{
		text.text = $"공격 행동 생성 키 : {attackActionCreateKey.ToString()}";
	}

	private void Update()
	{
		if (Input.GetKeyDown(attackActionCreateKey))
		{
			SaveLoadLogic.SaveAttackActionFile(attackActionPrefab.actionData, attackActionPrefab.impactData, 
				attackActionPrefab.performanceData, attackActionPrefab.baseFilePath, attackActionPrefab.actionName, attackActionPrefab.extension);

			Debug.Log($"{attackActionPrefab.actionName} 파일 생성 완료");
		}
	}
}