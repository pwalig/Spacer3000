using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFollowCurve : AiEscape
{
    public AnimationCurve xpath;
    public AnimationCurve ypath;
    public enum MoveMode { ConstantPath, EngineFollow, PerfectPath } // ConstantPath recommended
    public MoveMode moveMode;
    public bool cyclic = true;
    public float pathTime = 20f;

    static readonly float proximityTreshold = 5f;
    static readonly float curveIncrement = 0.01f;

    float pathPos = 0f;
    Vector2 offset = Vector2.zero;
    private void Start()
    {
        moveDirectionX = moveDirectionY = 0f;
        shoot = true;
    }

    void EngineFollow()
    {
        Vector2 distance = offset + new Vector2(xpath.Evaluate(pathPos), ypath.Evaluate(pathPos)) - new Vector2(transform.position.x, transform.position.y);
        while (distance.magnitude < proximityTreshold)
        {
            pathPos += curveIncrement;
            if (pathPos > 1f) pathPos -= 1f;
            distance = offset + new Vector2(xpath.Evaluate(pathPos), ypath.Evaluate(pathPos)) - new Vector2(transform.position.x, transform.position.y);
        }
        moveDirectionX = Mathf.Clamp(distance.x, -1f, 1f);
        moveDirectionY = Mathf.Clamp(distance.y, -1f, 1f);
    }

    void ConstantPath()
    {
        Vector2 distance = offset + new Vector2(xpath.Evaluate(pathPos), ypath.Evaluate(pathPos)) - new Vector2(transform.position.x, transform.position.y);
        pathPos += Time.deltaTime / pathTime;
        moveDirectionX = Mathf.Clamp(distance.x, -1f, 1f);
        moveDirectionY = Mathf.Clamp(distance.y, -1f, 1f);
    }

    void PerfectPath()
    {
        transform.position = offset + new Vector2(xpath.Evaluate(pathPos), ypath.Evaluate(pathPos));
        pathPos += Time.deltaTime / pathTime;
    }

    public void Update()
    {
        if (!escape)
        {
            switch (moveMode)
            {
                case MoveMode.EngineFollow:
                    EngineFollow();
                    break;
                case MoveMode.ConstantPath:
                    ConstantPath();
                    break;
                case MoveMode.PerfectPath:
                    PerfectPath();
                    break;
                default:
                    break;
            }

            if (pathPos > 1f)
            {
                if (cyclic) pathPos -= 1f;
                else StartCoroutine(FlyAway());
            }
        }
    }

}
