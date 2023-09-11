using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiEscape : SpaceshipController
{
    [System.Serializable]
    public class FF
    {
        public float value;
        public float speed;
        public float acceleration;
        public float F()
        {
            value += speed * Time.deltaTime;
            speed += acceleration * Time.deltaTime;
            return value;
        }
    }

    [HideInInspector] public bool escape = false; // wrap child classes update code in if(!escape){ }

    public IEnumerator FlyAway(float delay=0f)
    {
        yield return new WaitForSeconds(delay);
        if (this != null)
        {
            escape = true;
            // correct course
            while (this != null && Mathf.Abs(transform.position.x) < GameplayManager.gameAreaSize.x + 40f && Mathf.Abs(transform.position.y) < GameplayManager.gameAreaSize.y + 40f)
            {
                Vector3 dir = (Quaternion.Inverse(transform.rotation) * transform.position.normalized);
                moveDirectionX = dir.x;
                moveDirectionY = dir.y;
                yield return 0;
            }
            //delete
            if (this != null)
            {
                LevelManager.enemies.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        StopCoroutine(FlyAway());
    }
}
