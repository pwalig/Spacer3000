using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for all Ai behaviours
/// </summary>
public abstract class AiBehaviour : MonoBehaviour
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
    [System.Serializable]
    public class Vector2FF
    {
        public FF x;
        public FF y;
        public Vector2 F()
        {
            return new Vector2(x.F(), y.F());
        }
    }
    [System.Serializable]
    public class Vector3FF
    {
        public FF x;
        public FF y;
        public FF z;
        public Vector3 F()
        {
            return new Vector3(x.F(), y.F(), z.F());
        }
    }
    [HideInInspector] public float moveDirectionX = 0f;
    [HideInInspector] public float moveDirectionY = 0f;
    [HideInInspector] public Vector2 levelOffset = Vector2.zero;
    [HideInInspector] public int attack = -1;
    [HideInInspector] public bool shoot = false;
    [HideInInspector] public int availableAttacksCount = 1;

    /// <summary>
    /// AiBehaviour's equivalent of Update function, but not called automatically.
    /// </summary>
    public abstract void Behave();
}
