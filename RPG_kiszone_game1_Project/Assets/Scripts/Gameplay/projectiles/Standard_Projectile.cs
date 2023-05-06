using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Standard_Projectile : Projectile
{
    // Update is called once per frame
    void Update()
    {
        //transform.position + transform.up * speed * Time.deltaTime;
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
