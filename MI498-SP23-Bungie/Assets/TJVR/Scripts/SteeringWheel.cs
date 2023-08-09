using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SteeringWheel : XRBaseInteractable
{
    public static Transform wheelTransform;
    [SerializeField] private Transform wheelTransformLocal;

    public UnityEvent<float> OnWheelRotated;

    public int maxAngle;
    public int minAngle;

    private Quaternion maxSteeringWheelRotation;
    private Quaternion minSteeringWheelRotation;

    private float currentAngle = 0.0f;

    private void Start()
    {
        maxSteeringWheelRotation = Quaternion.Euler(0, 0, maxAngle);
        minSteeringWheelRotation = Quaternion.Euler(0, 0, minAngle);
        wheelTransform = wheelTransformLocal;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        currentAngle = FindWheelAngle();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        currentAngle = FindWheelAngle();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected)
                RotateWheel();
        }
    }

    private void RotateWheel()
    {
        // Convert that direction to an angle, then rotation
        float totalAngle = FindWheelAngle();

        // Apply difference in angle to wheel
        float angleDifference = currentAngle - totalAngle;
        wheelTransformLocal.Rotate(transform.forward, -angleDifference, Space.World);
            
        // Store angle for next process
        currentAngle = totalAngle;
        OnWheelRotated?.Invoke(angleDifference);

        if (wheelTransformLocal.localRotation.z >= maxSteeringWheelRotation.z)
        {
            wheelTransformLocal.SetLocalPositionAndRotation(wheelTransformLocal.localPosition, maxSteeringWheelRotation);
        }
        if (wheelTransformLocal.localRotation.z <= minSteeringWheelRotation.z)
        {
            wheelTransformLocal.SetLocalPositionAndRotation(wheelTransformLocal.localPosition, minSteeringWheelRotation);
        }

        wheelTransform = wheelTransformLocal;
    }

    private float FindWheelAngle()
    {
        float totalAngle = 0;

        // Combine directions of current interactors
        foreach (IXRSelectInteractor interactor in interactorsSelecting)
        {
            Vector2 direction = FindLocalPoint(interactor.transform.position);
            totalAngle += ConvertToAngle(direction) * FindRotationSensitivity();
        }

        return totalAngle;
    }

    private Vector2 FindLocalPoint(Vector3 position)
    {
        // Convert the hand positions to local, so we can find the angle easier
        return transform.InverseTransformPoint(position).normalized;
    }

    private float ConvertToAngle(Vector2 direction)
    {
        // Use a consistent up direction to find the angle
        return Vector2.SignedAngle(Vector2.up, direction);
    }

    private float FindRotationSensitivity()
    {
        // Use a smaller rotation sensitivity with two hands
        return 1.0f / interactorsSelecting.Count;
    }
}