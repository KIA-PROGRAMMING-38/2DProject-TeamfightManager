using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowAtkActionDataUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _cooltimeText;
	[SerializeField] private TMP_Text _descriptionText;

    public void ChangeData(Sprite iconSprite, float cooltime, bool isUltimate, bool isPassive, string description)
    {
        _iconImage.sprite = iconSprite;
		_descriptionText.text = description;

		if (isPassive)
		{
			_cooltimeText.text = "����ȿ��";
		}
		else
		{
			if (isUltimate)
			{
				_cooltimeText.text = "1ȸ/SET";
			}
			else
			{
				_cooltimeText.text = StringTable.GetString(cooltime) + "��";
			}
		}
    }
}
