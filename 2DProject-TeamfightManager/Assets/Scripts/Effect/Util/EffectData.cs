using UnityEngine;

public class EffectData
{
	public string name;				// 이펙트의 이름(얘는 이펙트 매니저에서 키로 사용되어 이펙트 매니저에서 찾을 때 사용된다)..
	public string animClipPath;		// 애니메이션 클립 경로(처음 딱 한번 이 경로에서 애니메이션 클립을 불러온다)..
	public Vector3 offsetPos;       // 이펙트가 중심지점에서 얼마만큼 떨어져있는가(주로 챔피언이 중심지점이 된다)..
	public bool isAutoDestroy;      // false인 경우 직접 제거해줘야 한다..
	public bool isUseLifeTime;		// lifeTime을 사용하는지 안하는지(사용하지 않는 애는 애니메이션이 끝났을 때 삭제되도록한다)..
	public float lifeTime;			// 생존 시간..

	public EffectData(string name, string animClipPath, Vector3 offsetPos, bool isAutoDestroy = true, bool isUseLifeTime = false, float lifeTime = 0f)
	{
		this.name = name;
		this.animClipPath = animClipPath;
		this.offsetPos = offsetPos;
		this.isUseLifeTime = isUseLifeTime;
		this.isAutoDestroy = isAutoDestroy;
		this.lifeTime = lifeTime;
	}

	public EffectData()
	{

	}
}