using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
    public Material material;
    public Material changeMaterial;
	public float EfectTime;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
	}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(GGamBBak(EfectTime));
        }
    }

    IEnumerator GGamBBak(float time)
    {
        float cur = 0f;

        GetComponent<SpriteRenderer>().material = changeMaterial;

        while (true)
        {
            cur += Time.deltaTime;
            float t = Mathf.Min(cur / time, 1f);
            GetComponent<SpriteRenderer>().material.SetFloat("HitEffectBlend", t);

            yield return null;

            if (t >= 0.9999f)
                break;
        }

        while (true)
        {
            cur -= Time.deltaTime;
            float t = Mathf.Min(cur / time, 1f);
            GetComponent<SpriteRenderer>().material.SetFloat("HitEffectBlend", t);

            yield return null;

            if (t <= 0.00001f)
                break;
        }

        GetComponent<SpriteRenderer>().material = material;
    }
}
