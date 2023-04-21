using System.Collections;
using UnityEngine;
using Util.Pool;

public class TestLogic : MonoBehaviour
{
	public Transform t1;
	public Transform t2;

    public float moveSpeed;

	private IEnumerator test;

	public int atk = 0;
	public int def = 0;

    private void Start()
	{
		
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Z))
		{
            atk += 5;
			int damage = Formula.Damage(atk, def, 1);
            Debug.Log($"공격력 : {atk}, 방어력 : {def}, 데미지 : {damage}");
		}
        if (Input.GetKeyDown(KeyCode.X))
        {
            def += 1;
            int damage = Formula.Damage(atk, def, 1);
            Debug.Log($"공격력 : {atk}, 방어력 : {def}, 데미지 : {damage}");
        }
    }
}
