using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 이펙트와 관련된 정보를 저장하는 데이터 테이블..
/// </summary>
public class EffectDataTable
{
	public readonly string DEFAULT_EFFECT_PATH = "Animations/Effect/Champion";
	public readonly string DEFAULT_EFFECT_PREFAB_PATH = "Prefabs/Effect";

	// 이펙트의 정보와 이펙트가 실행할 애니메이션을 이펙트 이름마다 저장할 컨테이너 생성..
	// 여기에는 처음 파일을 불러와 모든 이펙트의 정보를 저장할 예정..
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
#if UNITY_EDITOR
		Debug.Assert(true == _effectsDataNameKey.ContainsKey(effectName), $"{effectName} invalid key");
#endif

		return _effectsDataNameKey[effectName];
	}

	public AnimationClip GetEffectAnimClip(string effectName)
	{
#if UNITY_EDITOR
		Debug.Assert(true == _effectsAnimClipNameKey.ContainsKey(effectName), $"{effectName} invalid key");
#endif

		return _effectsAnimClipNameKey[effectName];
	}
}
