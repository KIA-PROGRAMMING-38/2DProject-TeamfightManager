using System;
using UnityEngine;
using Util.Pool;

/// <summary>
/// Floating Damage UI�� ���� �� �ʵ忡 �����ִ� ��ɵ��� �����Ѵ�..
/// </summary>
public static class FloatingDamageUISpawner
{
    public enum DamageKind
    {
        Damage,
        Heal,
    }

    public static GameManager gameManager { private get; set; }
    public static Transform uiParentTransform { private get; set; }
    private static ObjectPooler<FloatingDamageUI> _uiPooler;

    // Monobehaviour�� ��ӹ��� �ʱ� ������ �ܺο��� �����ϴ� �Լ��� �޾ƿ´�..
    public static void Initialize(Func<FloatingDamageUI> createFunc)
    {
        _uiPooler = new ObjectPooler<FloatingDamageUI>(createFunc, null, null, null, 20, 20, 100);
	}

    // ȭ�鿡 Floating Damage UI�� �����ְ� ���� �� ȣ���ϴ� �Լ�..
    public static void ShowFloatingDamageUI(Vector3 spawnPosition, int damage, DamageKind damageKind)
    {
#if UNITY_EDITOR
        Debug.Assert(null != _uiPooler);
#endif
        FloatingDamageUI damageUI = _uiPooler.Get();
        Vector3 uiPosition = Camera.main.WorldToScreenPoint(spawnPosition);
        Color uiColor = default(Color);

		switch (damageKind)
		{
			case DamageKind.Damage:
				uiColor = Color.red;
				break;
			case DamageKind.Heal:
				uiColor = Color.green;
				break;
		}

        damageUI.lifeTime = 0.7f;
        damageUI.gameObject.SetActive(true);
        damageUI.Initialize(uiPosition, uiColor, damage);

        damageUI.transform.SetParent(uiParentTransform, true);

        Debug.Log("������UI ����");
	}

    // Floating Damage UI�� �� ���� ������ pool�� �� �� ȣ��ȴ�..
	public static void ReleaseDamageUI(FloatingDamageUI damageUI)
    {
#if UNITY_EDITOR
		Debug.Assert(null != _uiPooler);
#endif
		if (null == damageUI)
            return;

        damageUI.gameObject.SetActive(false);

        _uiPooler.Release(damageUI);

        damageUI.transform.parent = gameManager.transform;

		Debug.Log("������UI release");
	}
}
