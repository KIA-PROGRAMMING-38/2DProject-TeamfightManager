using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowCurBanpickStageUI : UIBase
{
    private BattleStageDataTable _battleStageDataTable;

    [SerializeField] private Color _redTeamTurnColor;
    [SerializeField] private Color _blueTeamTurnColor;

    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TMP_Text _orderText;
    [SerializeField] private RectTransform _curStageTextTransform;
    [SerializeField] private TMP_Text _curStageDescText;
    [SerializeField] private TMP_Text _curStageLevelText;
    [SerializeField] private Animator _arrowAnimator;

    private static readonly int s_teamKindAnimParam = Animator.StringToHash("teamKind");

    [SerializeField] private float _blueTeamTurnCurStageInfoPosX;
    [SerializeField] private float _redTeamTurnCurStageInfoPosX;
    private Vector3 _curStageTextPos;

    // Start is called before the first frame update
    void Awake()
    {
        _battleStageDataTable = s_dataTableManager.battleStageDataTable;

        _battleStageDataTable.OnBanpickUpdate -= UpdateBanpickData;
        _battleStageDataTable.OnBanpickUpdate += UpdateBanpickData;

        _battleStageDataTable.OnBanpickOneStageStart -= OnBanpickOneStageStart;
        _battleStageDataTable.OnBanpickOneStageStart += OnBanpickOneStageStart;

        _battleStageDataTable.OnBanpickEnd -= OnBanpickEnd;
        _battleStageDataTable.OnBanpickEnd += OnBanpickEnd;

        _curStageTextPos = _curStageTextTransform.localPosition;
    }

    private void Start()
    {
        ModifyBanpickData( _battleStageDataTable.curBanpickStageInfo.teamKind, _battleStageDataTable.curBanpickLevel );
    }

    private void OnDisable()
    {
        _battleStageDataTable.OnBanpickUpdate -= UpdateBanpickData;
        _battleStageDataTable.OnBanpickOneStageStart -= OnBanpickOneStageStart;
        _battleStageDataTable.OnBanpickEnd -= OnBanpickEnd;
    }

    private void UpdateBanpickData( string championName, BanpickStageKind stageKind, BattleTeamKind teamKind, int index )
    {
        teamKind = _battleStageDataTable.curBanpickStageInfo.teamKind;
        int stage = _battleStageDataTable.curBanpickStageInfo.progressStageNumber;

		ModifyBanpickData( teamKind, stage );
    }

    private void ModifyBanpickData(BattleTeamKind teamKind, int curPickNumber )
    {
        switch ( teamKind )
        {
            case BattleTeamKind.RedTeam:
                _backgroundImage.color = _redTeamTurnColor;
                _curStageTextPos.x = _redTeamTurnCurStageInfoPosX;
                break;
            case BattleTeamKind.BlueTeam:
                _backgroundImage.color = _blueTeamTurnColor;
                _curStageTextPos.x = _blueTeamTurnCurStageInfoPosX;
                break;
        }

        _curStageLevelText.text = StringTable.GetString( curPickNumber ) + "/" + StringTable.GetString( _battleStageDataTable.maxBanpickLevel );

        _curStageTextTransform.localPosition = _curStageTextPos;
        _arrowAnimator.SetInteger( s_teamKindAnimParam, (int)teamKind );
    }

    private void OnBanpickOneStageStart(BanpickStageKind stageKind)
    {
        switch ( stageKind )
        {
            case BanpickStageKind.Ban:
                _orderText.text = "금지할 챔피언을 선택하세요";
                _curStageDescText.text = "금지 단계";
                break;
            case BanpickStageKind.Pick:
                _orderText.text = "사용할 챔피언을 선택하세요";
                _curStageDescText.text = "선택 단계";
                break;
        }
    }

    private void OnBanpickEnd()
    {
        _curStageDescText.gameObject.SetActive( false );
        _curStageLevelText.gameObject.SetActive( false );
        _arrowAnimator.gameObject.SetActive( false );

        if ( _backgroundImage.color == _redTeamTurnColor )
            _backgroundImage.color = _blueTeamTurnColor;
        else if(_backgroundImage.color == _blueTeamTurnColor)
            _backgroundImage.color = _redTeamTurnColor;
    }
}
