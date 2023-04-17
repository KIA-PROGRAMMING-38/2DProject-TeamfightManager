using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBanEffectUI : UIBase
{
	[SerializeField] private Image _highlightImage;

	private void OnEnable()
	{
		StartCoroutine(UpdateHighlight());
	}

	IEnumerator UpdateHighlight()
	{
		float fillAmount = 0f;

		while(true)
		{
			fillAmount = Mathf.Min(1f, fillAmount + Time.deltaTime * 2.3f);

			_highlightImage.fillAmount = fillAmount;

			if (fillAmount >= 1f)
			{
				break;
			}

			yield return null;
		}
	}
}
