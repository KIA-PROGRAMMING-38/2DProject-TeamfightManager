using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ����Ʈ�� ���õ� ������ �����ϴ� ������ ���̺�..
/// </summary>
public class EffectDataTable
{
	public readonly string DEFAULT_EFFECT_PATH = "Animations/Effect/Champion";
	public readonly string DEFAULT_EFFECT_PREFAB_PATH = "Prefabs/Effect";

	// ����Ʈ�� ������ ����Ʈ�� ������ �ִϸ��̼��� ����Ʈ �̸����� ������ �����̳� ����..
	// ���⿡�� ó�� ������ �ҷ��� ��� ����Ʈ�� ������ ������ ����..
	private Dictionary<string, EffectData> _effectsDataNameKey = new Dictionary<string, EffectData>();
	private Dictionary<string, AnimationClip> _effectsAnimClipNameKey = new Dictionary<string, AnimationClip>();

	public void AddEffectInfo(string effectName, EffectData effectData)
	{
		_effectsDataNameKey.Add(effectName, effectData);

		AnimationClip clip = Resources.Load<AnimationClip>(Path.Combine(DEFAULT_EFFECT_PATH, effectName));
		_effectsAnimClipNameKey.Add(effectName, clip);
	}

	public EffectData GetEffectInfo(string effectName)
	{
		return _effectsDataNameKey[effectName];
	}

	public AnimationClip GetEffectAnimClip(string effectName)
	{
		return _effectsAnimClipNameKey[effectName];
	}
}