using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackToBehaviour : AiBehaviour
{
    public List<AiBehaviour> behaviours;
    [SerializeField] List<float> attackDelays;
    Spaceship ship;
    bool coroutineRunnin;
    private void Awake()
    {
        ship = GetComponent<Spaceship>();
        coroutineRunnin = false;
    }

    void Start()
    {
        attack = Random.Range(0, availableAttacks);
        shoot = false;
    }

    IEnumerator ShootWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        shoot = true;
        coroutineRunnin = false;
    }


    public override void Behave()
    {
        behaviours[attack].Behave();

        // shootin'
        if (ship.canShoot && !coroutineRunnin && !shoot)
        {
            attack = Random.Range(0, availableAttacks);
            behaviours[attack].levelOffset = levelOffset;
            if (behaviours[attack] is AiFollowCurve) (behaviours[attack] as AiFollowCurve).ResetPathPos();
            coroutineRunnin = true;
            StartCoroutine(ShootWithDelay(attackDelays[attack]));
        }
        if (!ship.canShoot || coroutineRunnin) shoot = false;

        //movin'
        moveDirectionX = behaviours[attack].moveDirectionX;
        moveDirectionY = behaviours[attack].moveDirectionY;
    }
}
