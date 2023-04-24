using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public Sprite sprite
    {
        set
        {
			_spriteRenderer.sprite = value;
        }
    }

    private SpriteRenderer _spriteRenderer;
	public float moveSpeed { get; set; }
	private static Vector3 s_moveDirection = Vector3.left;

	public CloudSpawner cloudSpawner { private get; set; }

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		transform.Translate(moveSpeed * Time.deltaTime * s_moveDirection);

		if(transform.position.x < -10f)
		{
			cloudSpawner.ReleaseCloud(this);
		}
	}
}
