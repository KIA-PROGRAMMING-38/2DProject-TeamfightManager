using UnityEngine;

public class ChampStatusBar : UIBase
{
    public Transform target { private get; set; }
	public BattleTeamKind teamKind { private get; set; }
	public Vector3 offsetPosition;

	private Camera _mainCamera;
	private RectTransform _transform;

	private HPBarUI _hpBarUI;
	private MPBarUI _mpBarUI;
	private UltimateIconUI _ultimateIconUI;

	private readonly Color redTeamHPBarColor = Color.red;
	private readonly Color blueTeamHPBarColor = Color.green;

	private void Awake()
	{
		_mainCamera = Camera.main;
		_transform = GetComponent<RectTransform>();

		_hpBarUI = GetComponentInChildren<HPBarUI>();
		_mpBarUI = GetComponentInChildren<MPBarUI>();
		_ultimateIconUI = GetComponentInChildren<UltimateIconUI>();
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
		_hpBarUI.SetHPRatio(ratio);
	}

	public void SetMPRatio(float ratio)
	{
		_mpBarUI.SetMPRatio(ratio);
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
