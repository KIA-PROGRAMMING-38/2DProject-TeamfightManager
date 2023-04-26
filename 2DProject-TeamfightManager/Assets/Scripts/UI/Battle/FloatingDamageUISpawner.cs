using System;
using UnityEngine;
using Util.Pool;

/// <summary>
/// Floating Damage UI를 생성 및 필드에 보여주는 기능들을 제공한다..
/// </summary>
public static class FloatingDamageUISpawner
{
    public enum DamageKind
    {
        Damage,
        Heal,
    }

    public static Transform uiParentTransform { private get; set; }
    private static ObjectPooler<FloatingDamageUI> _uiPooler;

    // Monobehaviour를 상속받지 않기 때문에 외부에서 생성하는 함수를 받아온다..
    public static void Initialize(Func<FloatingDamageUI> createFunc)
    {
        _uiPooler = new ObjectPooler<FloatingDamageUI>(createFunc, null, null, null, 20, 20, 100);
	}

    // 화면에 Floating Damage UI를 보여주고 싶을 때 호출하는 함수..
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

        damageUI.Initialize(uiPosition, uiColor, damage);
        damageUI.lifeTime = 0.7f;
		damageUI.gameObject.SetActive(true);

        damageUI.transform.SetParent(uiParentTransform, true);

        Debug.Log("데미지UI 생성");
	}

    // Floating Damage UI가 할 일을 끝나고 pool에 들어갈 때 호출된다..
	public static void ReleaseDamageUI(FloatingDamageUI damageUI)
    {
#if UNITY_EDITOR
		Debug.Assert(null != _uiPooler);
#endif
		if (null == damageUI)
            return;

        damageUI.gameObject.SetActive(false);

        _uiPooler.Release(damageUI);

        damageUI.transform.parent = null;

		Debug.Log("데미지UI release");
	}
}
