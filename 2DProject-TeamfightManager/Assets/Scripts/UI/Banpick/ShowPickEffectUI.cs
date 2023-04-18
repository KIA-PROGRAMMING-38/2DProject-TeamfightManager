using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowPickEffectUI : MonoBehaviour
{
    [SerializeField] private Image _highlightImage;
    [SerializeField] private Image _numberTextBackgroundImage;
    [SerializeField] private TMP_Text _pickNumberText;

	[SerializeField] private ShowChampAnimUI _sharedMaterialOwner;

	private void OnEnable()
	{
        _highlightImage.gameObject.SetActive(false);
        _pickNumberText.gameObject.SetActive(false);

        StartCoroutine(UpdateHighlight());
	}

	public void SetPickData(BattleTeamKind teamKind, int index)
    {
        _highlightImage.material = _sharedMaterialOwner.material;
		_numberTextBackgroundImage.material = _sharedMaterialOwner.material;

        _pickNumberText.text = StringTable.GetString(index);
    }

    IEnumerator UpdateHighlight()
    {
        float fillAmount = 0f;
        _highlightImage.gameObject.SetActive(true);

		while (true)
        {
            fillAmount = Mathf.Min(1f, fillAmount + Time.deltaTime * 2.3f);

            _highlightImage.fillAmount = fillAmount;

            if (fillAmount >= 1f)
                break;

            yield return null;
		}

        Color color = _pickNumberText.color;
		_pickNumberText.gameObject.SetActive(true);

		while (true)
        {
			color.a = Mathf.Min(1f, color.a + Time.deltaTime * 2.3f);

			_pickNumberText.color = color;

			if (color.a >= 1f)
				break;

			yield return null;
		}
	}
}
