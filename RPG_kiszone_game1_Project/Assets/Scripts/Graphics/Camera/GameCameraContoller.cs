using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraContoller : MonoBehaviour
{
    [SerializeField] List<SecondOrderDynamics> systems = new List<SecondOrderDynamics>(); //requires 4 systems
    static Camera cam;
    static float desiredDistance;
    static float desiredAngle;
    static float desiredFieldOfView;
    static float grasp;
    [SerializeField] bool autoDistance = false;
    public AnimationCurve _graspCorrection;
    public static AnimationCurve graspCorrection;

    void Awake()
    {
        graspCorrection = _graspCorrection;
        cam = gameObject.GetComponentInChildren<Camera>();
        desiredAngle = 180f;
        SetFieldOfView(13f);
        systems[0].Initialise(desiredDistance);
        systems[1].Initialise(desiredAngle);
        systems[2].Initialise(desiredFieldOfView);
        systems[3].Initialise(TargetGrasp());
    }

    public static void ChangeDistance(float delta, bool adjustFieldOfView = true)
    {
        desiredDistance = Mathf.Clamp(desiredDistance + delta, 50f, 1000000f);
        if (adjustFieldOfView)
        {
            desiredFieldOfView = Mathf.Atan(TargetGrasp() / desiredDistance) * Mathf.Rad2Deg;
        }
    }
    public static void SetDistance(float val, bool adjustFieldOfView = true)
    {
        desiredDistance = Mathf.Clamp(val, 50f, 1000000f);
        if (adjustFieldOfView)
        {
            desiredFieldOfView = Mathf.Atan(TargetGrasp() / desiredDistance) * Mathf.Rad2Deg;
        }
    }
    
    public static void ChangeAngle(float delta)
    {
        desiredAngle = Mathf.Clamp(desiredAngle + delta, 0f, 360f);
    }
    public static void SetAngle(float val)
    {
        desiredAngle = Mathf.Clamp(val, 0f, 360f);
    }

    public static void ChangeFieldOfView(float delta, bool adjustDistance = true)
    {
        desiredFieldOfView = Mathf.Clamp(desiredFieldOfView + delta, 1f, 90f);
        if (adjustDistance)
        {
            desiredDistance = TargetGrasp() / Mathf.Tan(desiredFieldOfView * Mathf.Deg2Rad);
        }
    }
    public static void SetFieldOfView(float val, bool adjustDistance = true)
    {
        desiredFieldOfView = Mathf.Clamp(val, 1f, 90f);
        if (adjustDistance)
        {
            desiredDistance = TargetGrasp() / Mathf.Tan(desiredFieldOfView * Mathf.Deg2Rad);
        }
    }

    static float TargetGrasp()
    {
        return GameplayManager.gameAreaSize.x * (1 + graspCorrection.Evaluate(desiredFieldOfView));
    }
    float CalculateDistance()
    {
        return grasp / Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad);
    }

    void Update()
    {
        grasp = systems[3].Update(TargetGrasp());

        transform.parent.eulerAngles = Vector3.right * (systems[1].Update(desiredAngle) - 180f);

        cam.fieldOfView = systems[2].Update(desiredFieldOfView);

        transform.localPosition = -Vector3.forward * systems[0].Update(desiredDistance);
        if (autoDistance) transform.localPosition = -Vector3.forward * CalculateDistance();

        cam.nearClipPlane = Mathf.Clamp(-transform.localPosition.z - 100f, 1f, 1000000f);
        cam.farClipPlane = Mathf.Clamp(-transform.localPosition.z + 1000f, 1f, 1000000f);

#if (UNITY_EDITOR)
        if (Input.GetKey(KeyCode.Z)) { ChangeAngle(-Input.mouseScrollDelta.y); Debug.Log("camera angle: " + desiredAngle); }
        if (Input.GetKey(KeyCode.X)) { ChangeFieldOfView(Input.mouseScrollDelta.y); Debug.Log("camera field of view: " + desiredFieldOfView); }
#endif
    }
}
