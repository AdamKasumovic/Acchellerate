using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;

public class SimpleFollowRecenter : CinemachineExtension
{
    public bool Recenter;
    public float RecenterTime = 0.5f;
    public bool doOnce = false;
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Body)
            return;

        Transform target = vcam != null ? vcam.Follow : null;
        if (target == null)
            return;

        if (Recenter && CarManager.Instance != null && CarManager.Instance.carController != null && CarManager.Instance.carController.isGrounded)
        {
            // How far away from centered are we?
            Vector3 up = vcam.State.ReferenceUp;
            Vector3 back = vcam.transform.position - target.position;
            float angle = UnityVectorExtensions.SignedAngle(
                back.ProjectOntoPlane(up), -target.forward.ProjectOntoPlane(up), up);
            if (Mathf.Abs(angle) < 0.1f)
                Recenter = doOnce ? false : true; // done!
            else
            {
                angle = Damper.Damp(angle, RecenterTime, deltaTime);
                Vector3 pos = state.RawPosition - target.position;
                pos = Quaternion.AngleAxis(angle, up) * pos;
                state.RawPosition = pos + target.position;
            }
        }
    }
}
