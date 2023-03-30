using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EffectDataTable
{
	public readonly string DEFAULT_EFFECT_PATH = "Assets/Animations/Effect/Champion";
	public readonly string DEFAULT_EFFECT_PREFAB_PATH = "Assets/Prefabs/Effect.prefab";

	// 이펙트의 정보와 이펙트가 실행할 애니메이션을 이펙트 이름마다 저장할 컨테이너 생성..
	// 여기에는 처음 파일을 불러와 모든 이펙트의 정보를 저장할 예정..
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
