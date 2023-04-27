using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingDamageUI : UIBase
{
	private RectTransform _rectTransform;
	private TMP_Text _text;
    private RectTransform _wayPoint1;
    private RectTransform _wayPoint2;

    private IEnumerator _updateMoveCoroutine;
    private float _lerpT = 0f;

	// LocalPosition은 동일하기 때문에 미리 변수에 담아둔다..
    private Vector3 _startPosition;
    private Vector3 _wayPoint1Position;
    private Vector3 _wayPoint2Position;

	public float lifeTime { private get; set; }

	private void Awake()
	{
		s_dataTableManager.battleStageDataTable.OnBattleEnd -= OnBattleEnd;
		s_dataTableManager.battleStageDataTable.OnBattleEnd += OnBattleEnd;

        _text = GetComponentInChildren<TMP_Text>();

		_wayPoint1 = transform.GetChild(1).GetComponent<RectTransform>();
		_wayPoint2 = transform.GetChild(2).GetComponent<RectTransform>();

		_startPosition = _text.rectTransform.localPosition;
		_wayPoint1Position = _wayPoint1.localPosition;
		_wayPoint2Position = _wayPoint2.localPosition;
	}

	private void OnEnable()
	{
        _rectTransform = GetComponent<RectTransform>();
        _lerpT = 0f;
	}

    private void Update()
	{
		_lerpT += Time.deltaTime / lifeTime;

		if(_lerpT < 1f)
		{
			_text.transform.localPosition = MathUtility.Bezier.QuadraticBezierCurve(_startPosition, _wayPoint1Position, _wayPoint2Position, _lerpT);
		}
		else
		{
			FloatingDamageUISpawner.ReleaseDamageUI(this);
		}
	}

	// 필요한 정보를 받아오는 부분..
	public void Initialize(Vector3 startPosition, Color color, int damage)
	{
		_rectTransform.position = startPosition;
		_text.color = color;
		_text.text = StringTable.GetString(damage);
	}

	private void OnBattleEnd(BattleTeam redTeam, BattleTeam blueTeam, BattleTeamKind winTeamKind)
	{
		FloatingDamageUISpawner.ReleaseDamageUI(this);
    }
}
