using UnityEngine;

public static class UIUtility
{
	// Sprite의 Pivot에 따라 중간 위치를 조정해주는 함수..
	public static void CalcSpriteCenterPos(RectTransform transform, Sprite sprite, Vector2 startLocalPos)
	{
        // Champion Icon Image 가 가운데 올 수 있도록 위치 조정..
        float imageWidth = transform.rect.width;
		float imageHeight = transform.rect.height;

        float ratio = sprite.pivot.x / sprite.texture.width - 0.5f;
		startLocalPos.x = startLocalPos.x - ratio * imageWidth;

		ratio = sprite.pivot.y / sprite.texture.height - 0.5f;
		startLocalPos.y = startLocalPos.y - ratio * imageHeight;

		transform.localPosition = startLocalPos;
	}
}