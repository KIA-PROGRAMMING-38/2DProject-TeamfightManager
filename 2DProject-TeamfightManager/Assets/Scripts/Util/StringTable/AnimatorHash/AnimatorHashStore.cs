using UnityEngine;

// 애니메이터에서 쓰는 파라미터들 이름을 미리 해시값으로 변환해서 가져갈 수 있게 저장..
public static class AnimatorHashStore
{
	public static readonly int IS_MOVE_KEY_HASH = Animator.StringToHash(AnimKeyTable.ISMOVE);
	public static readonly int ATTACK_KEY_HASH = Animator.StringToHash(AnimKeyTable.ON_ATTACK);
	public static readonly int SKILL_KEY_HASH = Animator.StringToHash(AnimKeyTable.ON_SKILL);
	public static readonly int ULT_KEY_HASH = Animator.StringToHash(AnimKeyTable.ON_ULTIMATE);
	public static readonly int DEATH_KEY_HASH = Animator.StringToHash(AnimKeyTable.ON_DEATH);
	public static readonly int REVIVAL_KEY_HASH = Animator.StringToHash(AnimKeyTable.ON_REVIVAL);
	public static readonly int ON_ANIMEND_KEY_HASH = Animator.StringToHash(AnimKeyTable.ON_ANIMEND);
	public static readonly int ANIMATION_SPEED = Animator.StringToHash(AnimKeyTable.ANIMATION_SPEED);

	public static readonly int EFFECT_KEY_HASH = Animator.StringToHash(AnimKeyTable.EFFECT);

	public static readonly int ON_HOVER_BUTTON = Animator.StringToHash(AnimKeyTable.ON_HOVER_BUTTON);
	public static readonly int ON_SELECT_BUTTON = Animator.StringToHash(AnimKeyTable.ON_SELECT_BUTTON);
}