using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFollowCurve : AiBehaviour
{
    public AnimationCurve xpath;
    public AnimationCurve ypath;
    public enum MoveMode { ConstantPath, EngineFollow, PerfectPath } // ConstantPath recommended
    public MoveMode moveMode;
    public enum CurvePlacement { Global, Local};
    public CurvePlacement curvePlacement;
    public bool cyclic = true;
    public float pathTime = 20f;
    public Vector2FF variableOffset;

    static readonly float proximityTreshold = 5f;
    static readonly float curveIncrement = 0.01f;

    float pathPos = 0f;
    private void Start()
    {
        shoot = true;
        attack = -1;
    }

    Vector2 GetTargetPosition()
    {
        Vector2 global = variableOffset.F() + new Vector2((xpath.Evaluate(pathPos) + levelOffset.x) * GameplayManager.gameAreaSize.x, (ypath.Evaluate(pathPos) + levelOffset.y) * GameplayManager.gameAreaSize.y);
        if (curvePlacement == CurvePlacement.Global) return global;
        else return transform.rotation * global;
    }

    void EngineFollow()
    {
        Vector2 distance = Quaternion.Inverse(transform.rotation) * (GetTargetPosition() - new Vector2(transform.position.x, transform.position.y));
        while (distance.magnitude < proximityTreshold)
        {
            pathPos += curveIncrement;
            if (pathPos > 1f) pathPos -= 1f;
            distance = Quaternion.Inverse(transform.rotation) * (GetTargetPosition() - new Vector2(transform.position.x, transform.position.y));
        }
        distance = distance.normalized;
        moveDirectionX = distance.x;
        moveDirectionY = distance.y;
    }

    void ConstantPath()
    {
        Vector2 distance = Quaternion.Inverse(transform.rotation) * (GetTargetPosition() - new Vector2(transform.position.x, transform.position.y));
        pathPos += Time.deltaTime / pathTime;
        distance = distance.normalized;
        moveDirectionX = distance.x;
        moveDirectionY = distance.y;
    }

    void PerfectPath()
    {
        transform.position = GetTargetPosition();
        pathPos += Time.deltaTime / pathTime / (GameplayManager.gameAreaSize.magnitude / 18.3575597507f);
    }

    public void Update()
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
            else StartCoroutine(GetComponent<EnemyController>().FlyAway());
        }
    }

}
