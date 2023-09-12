using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBehaviour : MonoBehaviour
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
    [HideInInspector] public float moveDirectionX = 0f;
    [HideInInspector] public float moveDirectionY = 0f;
    [HideInInspector] public int attack = 0;
    [HideInInspector] public bool shoot = false;
    [HideInInspector] public int availableAttacks = 1;
}
