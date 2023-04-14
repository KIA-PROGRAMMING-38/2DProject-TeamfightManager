using UnityEngine;

// 애니메이터에서 쓰는 파라미터들 이름을 미리 해시값으로 변환해서 가져갈 수 있게 저장..
public static class AnimatorHashStore
{
	public static readonly int isMoveKeyHash = Animator.StringToHash(AnimKeyTable.isMove);
	public static readonly int attackKeyHash = Animator.StringToHash(AnimKeyTable.onAttack);
	public static readonly int skillKeyHash = Animator.StringToHash(AnimKeyTable.onSkill);
	public static readonly int ultKeyHash = Animator.StringToHash(AnimKeyTable.onUltimate);
	public static readonly int deathKeyHash = Animator.StringToHash(AnimKeyTable.onDeath);
	public static readonly int revivalKeyHash = Animator.StringToHash(AnimKeyTable.onRevival);
	public static readonly int onAnimEndKeyHash = Animator.StringToHash(AnimKeyTable.onAnimEnd);

	public static readonly int effectKeyHash = Animator.StringToHash(AnimKeyTable.effect);
}