using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUI : UIBase
{
    public UIMove _moveComponent;

    private Image _blueTeamLogoImage;
    private Image _redTeamLogoImage;

    private TMP_Text _blueTeamNameText;
    private TMP_Text _redTeamNameText;
    private TMP_Text _scoreText;
    private TMP_Text _winloseText;

    private void Awake()
    {
        _moveComponent = GetComponent<UIMove>();

        s_dataTableManager.battleStageDataTable.OnBattleEnd -= OnBattleEnd;
        s_dataTableManager.battleStageDataTable.OnBattleEnd += OnBattleEnd;

        Transform battleUIParent = transform.GetChild(2);
        _blueTeamLogoImage = battleUIParent.Find("BlueTeamLogoImage").GetComponent<Image>();
        _redTeamLogoImage = battleUIParent.Find("RedTeamLogoImage").GetComponent<Image>();

        _blueTeamNameText = battleUIParent.Find("BlueTeamNameText").GetComponent<TMP_Text>();
        _redTeamNameText = battleUIParent.Find("RedTeamNameText").GetComponent<TMP_Text>();
        _scoreText = battleUIParent.Find("GameScoreText").GetComponent<TMP_Text>();
        _winloseText = battleUIParent.Find("WinLoseText").GetComponent<TMP_Text>();
    }

    private void OnBattleEnd(BattleTeam redTeam, BattleTeam blueTeam, BattleTeamKind winTeamKind)
    {
        s_dataTableManager.battleStageDataTable.OnBattleEnd -= OnBattleEnd;

        _moveComponent.StartMove(false);

        string redTeamName = redTeam.teamName;
        string blueTeamName = blueTeam.teamName;

        _redTeamLogoImage.sprite = s_dataTableManager.teamDataTable.GetLogoSprite(redTeamName);
        _blueTeamLogoImage.sprite = s_dataTableManager.teamDataTable.GetLogoSprite(redTeamName);

        _redTeamNameText.text = redTeamName;
        _blueTeamNameText.text = blueTeamName;

        switch (winTeamKind)
        {
            case BattleTeamKind.RedTeam:
                _scoreText.text = "0 : 1";
                if (redTeamName == s_gameManager.gameGlobalData.playerTeamName)
                    _winloseText.text = "½Â¸®";
                else
                    _winloseText.text = "ÆÐ¹è";
                break;
            case BattleTeamKind.BlueTeam:
                _scoreText.text = "1 : 0";
                if (blueTeamName == s_gameManager.gameGlobalData.playerTeamName)
                    _winloseText.text = "½Â¸®";
                else
                    _winloseText.text = "ÆÐ¹è";
                break;
            default:
                _scoreText.text = "0 : 0";
                _winloseText.text = "¹«½ÂºÎ";
                break;
        }
    }

    public void OnClickProgressButton()
    {
        s_gameManager.ChangeScene(SceneNameTable.DORMITORY);
    }
}
