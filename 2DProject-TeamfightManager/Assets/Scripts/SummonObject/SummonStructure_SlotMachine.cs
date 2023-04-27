using System;
using System.Collections;
using UnityEngine;

public class SummonStructure_SlotMachine : SummonStructure
{
    [SerializeField] private string _fireProjectileName;
    [SerializeField] private Vector3 _projectileOffsetPos;

    [SerializeField] private Sprite[] _numberSprites;

    private static readonly int s_OnBreakAnimHash = Animator.StringToHash("OnBreak");

    private Animator _upperSecondDigitAnimator;
    private Animator _upperThirdDigitAnimator;

    private SpriteRenderer _upperSecondDigitRenderer;
    private SpriteRenderer _upperThirdDigitRenderer;
    private SpriteRenderer _lowerSecondDigitRenderer;
    private SpriteRenderer _lowerThirdDigitRenderer;

    private WaitForSeconds _waitForDecide;
    private WaitForSeconds _waitForFireInterval;

    private Champion _ownerChampion;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();

        _waitForDecide = YieldInstructionStore.GetWaitForSec(1f);
        _waitForFireInterval = YieldInstructionStore.GetWaitForSec(_actionTickTime);

        Transform upperSlotTransform = transform.GetChild(0).GetChild(0);
        _upperSecondDigitAnimator = upperSlotTransform.GetChild(1).GetComponent<Animator>();
        _upperThirdDigitAnimator = upperSlotTransform.GetChild(2).GetComponent<Animator>();

        _upperSecondDigitRenderer = _upperSecondDigitAnimator.GetComponent<SpriteRenderer>();
        _upperThirdDigitRenderer = _upperThirdDigitAnimator.GetComponent<SpriteRenderer>();

        Transform lowerSlotTransform = transform.GetChild(0).GetChild(1);
        _lowerSecondDigitRenderer = lowerSlotTransform.GetChild(1).GetComponent<SpriteRenderer>();
        _lowerThirdDigitRenderer = lowerSlotTransform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _upperSecondDigitAnimator.enabled = true;
        _upperThirdDigitAnimator.enabled = true;

        _lowerSecondDigitRenderer.sprite = _numberSprites[0];
        _lowerThirdDigitRenderer.sprite = _numberSprites[0];
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

            projectile.transform.position = transform.position + projectileOffset;
            projectile.SetAdditionalData(LayerTable.Number.CalcOtherTeamLayer(_getLayerMask), _targetArray[0], _targetFindFunc);
        }
    }

    public override void SetAdditionalData(int layerMask, Champion target, Champion owner, Func<Vector3, Champion[], int> targetFindFunc)
    {
        base.SetAdditionalData(layerMask, target, owner, targetFindFunc);

        _ownerChampion = owner;
    }

    public void startSlotMachine()
    {
        StartCoroutine(UpdateLogic());
    }

    IEnumerator UpdateLogic()
    {
        int fireCount = 0;
        int randomNumber = 0;

        // 한 칸씩 슬롯을 정지시키면서 랜덤 숫자 계산한 뒤 발사 count에 더해준다..
        yield return _waitForDecide;
        _upperSecondDigitAnimator.enabled = false;

        randomNumber = UnityEngine.Random.Range(0, 10);
        _upperSecondDigitRenderer.sprite = _numberSprites[randomNumber];
        fireCount = randomNumber;

        yield return _waitForDecide;
        _upperThirdDigitAnimator.enabled = false;

        randomNumber = UnityEngine.Random.Range(0, 10);
        _upperThirdDigitRenderer.sprite = _numberSprites[randomNumber];
        fireCount += randomNumber;

        // 총 발사 개수에 따라 스프라이트 갱신..
        _lowerSecondDigitRenderer.sprite = _numberSprites[fireCount / 10];
        _lowerThirdDigitRenderer.sprite = _numberSprites[fireCount % 10];

        // 발사..
        for (int i = 0; i < fireCount; ++i)
        {
            this.Action();

            yield return _waitForFireInterval;
        }

        yield return YieldInstructionStore.GetWaitForSec(0.2f);

        summonObjectManager.ReleaseSummonObject(this);
        ReceiveReleaseEvent();
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
