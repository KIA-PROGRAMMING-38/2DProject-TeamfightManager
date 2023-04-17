using UnityEngine;

/// <summary>
/// è�Ǿ��� HP, MP, �ñر� ���� ������ ȭ�鿡 �����ִ� UI..
/// è�Ǿ��� ����ٴѴ�..
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

	[SerializeField] private GaugeBarUI _hpBarUI;
	[SerializeField] private GaugeBarUI _barrierBarUI;
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
    }

	private void Start()
	{
		_hpBarUI.gaugeBarColor = (teamKind == BattleTeamKind.RedTeam) ? redTeamHPBarColor : blueTeamHPBarColor;
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
		_hpBarUI.SetFillAmount(ratio);
	}

	public void SetMPRatio(float ratio)
	{
		_mpBarUI.SetFillAmount(ratio);
	}

	public void SetBarrierRatio(float ratio)
	{
		_barrierBarUI.SetFillAmount(ratio);
	}

	public void SetUltimateActive(bool active)
	{
		_ultimateIconUI.SetIconActive(false);
	}

	public void SetUltimateIconSprite(Sprite iconSprite)
	{
		_ultimateIconUI.SetUltimateIconSprite(iconSprite);
	}
}
