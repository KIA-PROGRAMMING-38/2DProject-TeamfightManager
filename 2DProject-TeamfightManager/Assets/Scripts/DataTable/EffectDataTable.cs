using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ����Ʈ�� ���õ� ������ �����ϴ� ������ ���̺�..
/// </summary>
public class EffectDataTable
{
	public readonly string DEFAULT_EFFECT_PATH = "Assets/Animations/Effect/Champion";
	public readonly string DEFAULT_EFFECT_PREFAB_PATH = "Assets/Prefabs/Effect.prefab";

	// ����Ʈ�� ������ ����Ʈ�� ������ �ִϸ��̼��� ����Ʈ �̸����� ������ �����̳� ����..
	// ���⿡�� ó�� ������ �ҷ��� ��� ����Ʈ�� ������ ������ ����..
	private Dictionary<string, EffectInfo> _effectsInfo = new Dictionary<string, EffectInfo>();
	private Dictionary<string, AnimationClip> _effectsAnimClip = new Dictionary<string, AnimationClip>();

	public void AddEffectInfo(string effectName, EffectInfo effectInfo)
	{
		_effectsInfo.Add(effectName, effectInfo);

		AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(DEFAULT_EFFECT_PATH, effectName + ".anim"));
		_effectsAnimClip.Add(effectName, clip);
	}

	public EffectInfo GetEffectInfo(string effectName)
	{
		return _effectsInfo[effectName];
	}

	public AnimationClip GetEffectAnimClip(string effectName)
	{
		return _effectsAnimClip[effectName];
	}
}
