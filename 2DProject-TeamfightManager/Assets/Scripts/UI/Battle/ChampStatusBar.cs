using UnityEngine;

/// <summary>
/// 챔피언의 HP, MP, 궁극기 관련 정보를 화면에 보여주는 UI..
/// 챔피언을 따라다닌다..
/// </summary>
public class ChampStatusBar : UIBase
{
	private Transform _target;
    public Transform target
	{
		private get => _target;
		set
		{
			_target = value;

			_buffUI.ownerChampion = _target.GetComponent<Champion>();
		}
	}
	public BattleTeamKind teamKind { private get; set; }
	public Vector3 offsetPosition;

	private Camera _mainCamera;
	private RectTransform _transform;
	
	private HPBarUI _hpBarUI;
	[SerializeField] private GaugeBarUI _mpBarUI;
	private UltimateIconUI _ultimateIconUI;
	private ChampBuffUI _buffUI;

	private readonly Color redTeamHPBarColor = Color.red;
	private readonly Color blueTeamHPBarColor = Color.green;

	private void Awake()
	{
		_mainCamera = Camera.main;
		_transform = GetComponent<RectTransform>();

		_ultimateIconUI = GetComponentInChildren<UltimateIconUI>();
		_buffUI = GetComponentInChildren<ChampBuffUI>();
		_hpBarUI = GetComponentInChildren<HPBarUI>();
    }

	private void Start()
	{
		_hpBarUI.SetHPBarColor((teamKind == BattleTeamKind.RedTeam) ? redTeamHPBarColor : blueTeamHPBarColor);
	}

	private void LateUpdate()
	{
		if(null != target)
		{
			_transform.position = _mainCamera.WorldToScreenPoint(target.transform.position + offsetPosition);
		}
	}

	public void SetHPRatio(float ratio)
	{
		_hpBarUI.SetHPBarGauge(ratio);
	}

	public void SetMPRatio(float ratio)
	{
		_mpBarUI.fillAmount = ratio;
	}

	public void SetBarrierRatio(float ratio)
	{
		_hpBarUI.SetBarrierBarGauge(ratio);
	}

	public void SetUltimateActive(bool active)
	{
		_ultimateIconUI.SetIconActive(false);
	}

	public void SetUltimateIconSprite(Sprite iconSprite)
	{
		_ultimateIconUI.SetUltimateIconSprite(iconSprite);
	}

	public void SetUltimateIconHide(bool isHide)
	{
		_ultimateIconUI.gameObject.SetActive(!isHide);
	}
}
