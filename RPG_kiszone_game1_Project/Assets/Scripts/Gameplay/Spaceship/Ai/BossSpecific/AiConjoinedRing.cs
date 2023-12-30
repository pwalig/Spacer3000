using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiConjoinedRing : AiBehaviour
{
    static float globalAngle = 0f;
    public float distanceToKeep = 100f;
    public float rotationSpeed = 3f;
    float startAngle;
    static bool angleLock = false;
    static int shooters = 0;
    bool shootLockRunning = false;

    Vector3 distance;

    IEnumerator ShootLock(float time)
    {
        shootLockRunning = true;
        yield return new WaitForSeconds(time);
        --shooters;
        shootLockRunning = false;
    }

    private void Start()
    {
        startAngle = transform.rotation.eulerAngles.z;
        attack = -1;
    }
    public override void Behave()
    {
        if (!angleLock)
        {
            globalAngle += Time.deltaTime * rotationSpeed;
            angleLock = true;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, startAngle + globalAngle);
        distance = Quaternion.Inverse(transform.rotation) * (Quaternion.Euler(0f, 0f, startAngle + globalAngle - 180f) * Vector3.up * distanceToKeep - transform.position);
        moveDirectionX = distance.normalized.x;
        moveDirectionY = distance.normalized.y;
        if (distance.magnitude <= 5f)
        {
            moveDirectionX = 0f;
            moveDirectionY = 0f;
        }
        if (shooters < 3) { 
            shoot = true;
            ++shooters;
            StartCoroutine(ShootLock(3f * GameData.GetDifficultyMulitplier(0.3f, true)));
        }
    }
    private void LateUpdate()
    {
        angleLock = false;
        while (GameplayManager.GetPlayerPosition().magnitude > transform.position.magnitude - 15f) GameplayManager.playerTransform.position -= GameplayManager.playerTransform.position.normalized * 0.5f;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        if (shootLockRunning) shooters = Mathf.Clamp(shooters - 1, 0, 6);
    }
}
