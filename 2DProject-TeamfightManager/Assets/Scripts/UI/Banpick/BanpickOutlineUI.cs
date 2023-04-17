using UnityEngine;
using UnityEngine.UI;

public class BanpickOutlineUI : UIBase
{
	[SerializeField] private Sprite _redTeamOutline;
	[SerializeField] private Sprite _blueTeamOutline;

	[SerializeField] private Image _highlightImage;
    [SerializeField] private Image _backgroundImage;

	[SerializeField] private AnimationCurve _highlightCurve;
	[SerializeField] private float _highlightOnceDuration;

	private float _elaspedTime = 0f;

	private void OnEnable()
	{
		_elaspedTime = 0f;
	}

	private void Update()
	{
		_elaspedTime += Time.deltaTime;

		float timeRatio = _elaspedTime / _highlightOnceDuration;

		float t =_highlightCurve.Evaluate(Mathf.Min(timeRatio, 1f));
		_highlightImage.fillAmount = t;

		if(_elaspedTime >= _highlightOnceDuration)
		{
			_elaspedTime -= _highlightOnceDuration;
		}
	}

	public void SetTeamKind(BattleTeamKind teamKind)
	{
		switch (teamKind)
		{
			case BattleTeamKind.RedTeam:
				_backgroundImage.sprite = _redTeamOutline;
				break;
			case BattleTeamKind.BlueTeam:
				_backgroundImage.sprite = _blueTeamOutline;
				break;
		}
	}
}
