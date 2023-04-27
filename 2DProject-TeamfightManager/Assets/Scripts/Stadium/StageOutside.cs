using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageOutside : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( true == collision.CompareTag("SummonObject") )
        {
            SummonObject summonObject = collision.GetComponent<SummonObject>();
            if (null != summonObject)
            {
                summonObject.Release();
            }
        }
    }
}
