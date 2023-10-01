using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedExpire : MonoBehaviour
{
    public float lifeTime = 2f;
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    private void Start()
    {
        StartCoroutine(Expire());
    }
}
