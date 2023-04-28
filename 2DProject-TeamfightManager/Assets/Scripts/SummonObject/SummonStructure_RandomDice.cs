using System;
using System.Collections;
using UnityEngine;

public class SummonStructure_RandomDice : SummonStructure
{
    [SerializeField] private string _fireProjectileName;
    [SerializeField] private Vector3 _projectileOffsetPos;

    [SerializeField] private Sprite[] _diceSprites;

    private Animator _diceAnimator;
    private SpriteRenderer _diceSpriteRenderer;
    private Animator _outLineAnimator;

    private static readonly int s_OnBreakAnimHash = Animator.StringToHash("OnBreak");

    private WaitForSeconds _waitForDecideDiceScale;
    private WaitForSeconds _waitForFireInterval;

    private Champion _ownerChampion;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();

        // Yield Instruction 미리 캐싱..
        _waitForDecideDiceScale = YieldInstructionStore.GetWaitForSec(2f);
        _waitForFireInterval = YieldInstructionStore.GetWaitForSec(_actionTickTime);

        // 애니메이션 및 스프라이트에 관련된 컴포넌트들 가져오기..
        _diceAnimator = transform.Find("Dice").GetComponent<Animator>();
        _diceSpriteRenderer = _diceAnimator.GetComponent<SpriteRenderer>();
        _outLineAnimator = transform.Find("Outline").GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _diceAnimator.gameObject.SetActive(true);
        _diceAnimator.enabled = true;

        _outLineAnimator.gameObject.SetActive(false);

        StartCoroutine(UpdateLogic());
    }

    new private void Update()
    {
        base.Update();

        if( null != _ownerChampion && _ownerChampion.isDead)
        {
            summonObjectManager.ReleaseSummonObject(this);
            ReceiveReleaseEvent();
        }
    }

    protected override void Action()
    {
        int targetCount = _targetFindFunc.Invoke(transform.position, _targetArray);
        if (0 < targetCount)
        {
            if (null == _targetArray[0])
                return;

            float directionX = (_targetArray[0].transform.position - transform.position).x;
            bool flipX = directionX < 0f;

            Projectile projectile = summonObjectManager.GetSummonObject<Projectile>(_fireProjectileName);

            projectile.OnExecuteImpact -= OnProjectileImpact;
            projectile.OnExecuteImpact += OnProjectileImpact;

            projectile.OnRelease -= OnProjectileRelease;
            projectile.OnRelease += OnProjectileRelease;

            projectile.gameObject.SetActive(true);

            Vector3 projectileOffset = _projectileOffsetPos;
            if (flipX)
                projectileOffset.x *= -1f;

            projectile.transform.position = transform.position + projectileOffset;
            projectile.SetAdditionalData(LayerTable.Number.CalcOtherTeamLayer(_getLayerMask), _targetArray[0], _targetFindFunc);
        }
    }

    public override void SetAdditionalData(int layerMask, Champion target, Champion owner, Func<Vector3, Champion[], int> targetFindFunc)
    {
        base.SetAdditionalData(layerMask, target, owner, targetFindFunc);

        _ownerChampion = owner;
    }

    public void OnDiceBreakAnimEnd()
    {
        gameObject.SetActive(false);

        summonObjectManager.ReleaseSummonObject(this);
        ReceiveReleaseEvent();
    }

    IEnumerator UpdateLogic()
    {
        yield return _waitForDecideDiceScale;

        // 랜덤한 숫자를 뽑은 뒤 그 숫자에 맞춰 스프라이트 갱신 및 발사 개수 갱신..
        int randomValue = UnityEngine.Random.Range(1, 7);
        int randomIndex = randomValue - 1;

        _outLineAnimator.gameObject.SetActive(true);
        _diceAnimator.enabled = false;
        _diceSpriteRenderer.sprite = _diceSprites[randomIndex];

        _audioSource.PlayOneShot(SoundStore.GetAudioClip("RandomDice_Decide"));

        for (int i = 0; i < randomValue; ++i)
        {
            this.Action();

            yield return _waitForFireInterval;
        }

        _diceAnimator.gameObject.SetActive(false);
        _outLineAnimator.SetTrigger(s_OnBreakAnimHash);

        yield return null;
    }

    private void OnProjectileImpact(SummonObject summonObject, Champion[] _championArray, int targetCount)
    {
        _targetArray = _championArray;

        ReceiveImpactExecuteEvent(targetCount);
    }

    private void OnProjectileRelease(SummonObject summonObject)
    {
        summonObject.OnExecuteImpact -= OnProjectileImpact;
        summonObject.OnRelease -= OnProjectileRelease;
    }
}
