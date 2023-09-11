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
    public FF downwardOffset;

    static readonly float proximityTreshold = 5f;
    static readonly float curveIncrement = 0.01f;

    float pathPos = 0f;
    private void Start()
    {
        moveDirectionX = moveDirectionY = 0f;
        shoot = true;
    }

    void EngineFollow()
    {
        Vector2 distance = Quaternion.Inverse(transform.rotation) * ((Vector2.down * downwardOffset.F()) + new Vector2(xpath.Evaluate(pathPos) * GameplayManager.gameAreaSize.x, ypath.Evaluate(pathPos) * GameplayManager.gameAreaSize.y) - new Vector2(transform.position.x, transform.position.y));
        while (distance.magnitude < proximityTreshold)
        {
            pathPos += curveIncrement;
            if (pathPos > 1f) pathPos -= 1f;
            distance = Quaternion.Inverse(transform.rotation) * ((Vector2.down * downwardOffset.value) + new Vector2(xpath.Evaluate(pathPos) * GameplayManager.gameAreaSize.x, ypath.Evaluate(pathPos) * GameplayManager.gameAreaSize.y) - new Vector2(transform.position.x, transform.position.y));
        }
        distance = distance.normalized;
        moveDirectionX = distance.x;
        moveDirectionY = distance.y;
    }

    void ConstantPath()
    {
        Vector2 distance = Quaternion.Inverse(transform.rotation) * ((Vector2.down * downwardOffset.F()) + new Vector2(xpath.Evaluate(pathPos) * GameplayManager.gameAreaSize.x, ypath.Evaluate(pathPos) * GameplayManager.gameAreaSize.y) - new Vector2(transform.position.x, transform.position.y));
        pathPos += Time.deltaTime / pathTime;
        distance = distance.normalized;
        moveDirectionX = distance.x;
        moveDirectionY = distance.y;
    }

    void PerfectPath()
    {
        transform.position = transform.rotation * ((Vector2.down * downwardOffset.F()) + new Vector2(xpath.Evaluate(pathPos) * GameplayManager.gameAreaSize.x, ypath.Evaluate(pathPos) * GameplayManager.gameAreaSize.y));
        pathPos += Time.deltaTime / pathTime / (GameplayManager.gameAreaSize.magnitude / 18.3575597507f);
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
