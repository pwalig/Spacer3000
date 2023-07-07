using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class Following_Projectile : Projectile
{
    public float TurnSpeed = 5;
    // Update is called once per frame
    void Update()
    {
        Vector3 target;
        if(CompareTag("Player_Projectile"))
        {
            target = GameplayManager.GetPlayerPosition(transform.position);
        }
        else
        {
            target = GameplayManager.GetPlayerPosition(transform.position);
        }
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward ,target - transform.position), TurnSpeed * Time.deltaTime * ); Problem - turnspeed nie określa prędkości w radianach czy stopniach, tylko w procencie stopnia, o który ma się obrócić. to znaczy, że im więcej jest do obrócenia, tym szybciej się obraca.
        Vector3 direction = Vector3.RotateTowards(transform.up, target - transform.position, TurnSpeed * Time.deltaTime, 0.0f); // tworzy nowy wektor, do którego ma się patrzeć
        transform.rotation = Quaternion.LookRotation(transform.forward, direction); //patrzy w stronę wektora. przód ma być zawsze ten sam, gra jest w 2d, czyli przód to oś z.
        transform.Translate(speed * Time.deltaTime * Vector3.up);
    }
}
