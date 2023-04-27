using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampBuffUI : UIBase
{
    private const int BUFFINDEX_ATKSTAT = 0;
    private const int BUFFINDEX_ATKSPEED = 1;
    private const int BUFFINDEX_DEFENCE = 2;
    private const int BUFFINDEX_MOVESPEED = 3;

    private Champion _ownerChampion;
    public Champion ownerChampion
    {
        set
        {
            if(null != _ownerChampion)
            {
                _ownerChampion.OnChangeBuffStatus -= ChangeChampionStatus;
			}

            _ownerChampion = value;

			_ownerChampion.OnChangeBuffStatus -= ChangeChampionStatus;
			_ownerChampion.OnChangeBuffStatus += ChangeChampionStatus;
        }
    }
    private static Dictionary<string, Sprite> s_spriteContainer = null;

    private Image[] _allImages;
    private int _imageCount = 0;

    private void Awake()
    {
        _allImages = GetComponentsInChildren<Image>();
        _imageCount = _allImages.Length;

        for(int i = 0; i < _imageCount; ++i)
        {
            _allImages[i].gameObject.SetActive( false );
        }

        if ( null == s_spriteContainer )
        {
            SetupSprite();
        }
    }

    private void OnDisable()
    {
        if (null != _ownerChampion)
            _ownerChampion.OnChangeBuffStatus -= ChangeChampionStatus;
    }

    private static void SetupSprite()
    {
        s_spriteContainer = new Dictionary<string, Sprite>();

        // 텍스처 로드..
        s_spriteContainer.Add( "Debuff_AtkStat", Resources.Load<Sprite>( "Sprites/UI/Champion/Data/status/atk_debuff" ) );
        s_spriteContainer.Add( "Debuff_AtkSpeed", Resources.Load<Sprite>( "Sprites/UI/Champion/Data/status/as_debuff" ) );
        s_spriteContainer.Add( "Debuff_Defence", Resources.Load<Sprite>( "Sprites/UI/Champion/Data/status/armor_break_debuff_icon" ) );
        s_spriteContainer.Add( "Debuff_MoveSpeed", Resources.Load<Sprite>( "Sprites/UI/Champion/Data/status/movement_speed_debuff" ) );

        s_spriteContainer.Add( "Buff_AtkStat", Resources.Load<Sprite>( "Sprites/UI/Champion/Data/status/attack_buff" ) );
        s_spriteContainer.Add( "Buff_AtkSpeed", Resources.Load<Sprite>( "Sprites/UI/Champion/Data/status/as_buff" ) );
        s_spriteContainer.Add( "Buff_Defence", Resources.Load<Sprite>( "Sprites/UI/Champion/Data/status/armor_buff_icon" ) );
        s_spriteContainer.Add( "Buff_MoveSpeed", Resources.Load<Sprite>( "Sprites/UI/Champion/Data/status/movement_speed_buff" ) );
    }

    // 챔피언 Status가 갱신될 때마다 호출되는 콜백 함수(버프, 디버프가 추가 or 제거될 때마다 호출)..
    private void ChangeChampionStatus(ChampionStatus status)
    {
        for( int i = 0; i < _imageCount; ++i )
        {
            _allImages[i].gameObject.SetActive( false );
        }

        if( 0 != status.atkStat )
            ChangeSprite( BUFFINDEX_ATKSTAT, status.atkStat > 0, "Buff_AtkStat", "Debuff_AtkStat" );
        if ( 0f != status.atkSpeed )
            ChangeSprite( BUFFINDEX_ATKSPEED, status.atkSpeed > 0f, "Buff_AtkSpeed", "Debuff_AtkSpeed" );
        if ( 0 != status.defence )
            ChangeSprite( BUFFINDEX_DEFENCE, status.defence > 0, "Buff_Defence", "Debuff_Defence" );
        if ( 0f != status.moveSpeed )
            ChangeSprite( BUFFINDEX_MOVESPEED, status.moveSpeed > 0f, "Buff_MoveSpeed", "Debuff_MoveSpeed" );
    }

    private void ChangeSprite(int index, bool isBuff, string buffName, string debuffName)
    {
        string key = (isBuff) ? buffName : debuffName;
        _allImages[index].sprite = s_spriteContainer[key];
        _allImages[index].gameObject.SetActive( true );
    }
}
