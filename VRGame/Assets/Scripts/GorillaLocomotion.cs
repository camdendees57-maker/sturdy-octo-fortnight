using UnityEngine;
using UnityEngine.XR;

public sealed class GorillaLocomotion : MonoBehaviour
{
    [SerializeField] private float maxArmReach = 1.35f;
    [SerializeField] private float maxPushSpeed = 7.5f;
    [SerializeField] private float gravity = -18f;
    [SerializeField] private float groundAcceleration = 8f;
    [SerializeField] private float airDrag = 0.15f;
    [SerializeField] private float handRadius = 0.09f;
    [SerializeField] private LayerMask climbableMask = ~0;

    private Rigidbody body;
    private Transform trackingSpace;
    private Transform head;
    private Transform leftHand;
    private Transform rightHand;
    private InputDevice leftDevice;
    private InputDevice rightDevice;
    private Vector3 lastLeftWorld;
    private Vector3 lastRightWorld;
    private Vector3 velocity;
    private bool initialized;

    public void Initialize(Rigidbody playerBody, Transform trackingRoot, Transform headTransform, Transform leftHandTransform, Transform rightHandTransform)
    {
        body = playerBody;
        trackingSpace = trackingRoot;
        head = headTransform;
        leftHand = leftHandTransform;
        rightHand = rightHandTransform;
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        lastLeftWorld = leftHand.position;
        lastRightWorld = rightHand.position;
        initialized = true;
    }

    private void FixedUpdate()
    {
        if (!initialized) return;

        RefreshDevices();
        head.localPosition = InputTracking.GetLocalPosition(XRNode.Head);
        head.localRotation = InputTracking.GetLocalRotation(XRNode.Head);

        Vector3 leftTarget = ClampReach(GetHandWorldPosition(leftDevice, XRNode.LeftHand));
        Vector3 rightTarget = ClampReach(GetHandWorldPosition(rightDevice, XRNode.RightHand));

        bool leftTouch = IsTouchingWorld(leftTarget);
        bool rightTouch = IsTouchingWorld(rightTarget);

        Vector3 leftDelta = leftTarget - lastLeftWorld;
        Vector3 rightDelta = rightTarget - lastRightWorld;
        Vector3 push = Vector3.zero;

        if (leftTouch) push -= leftDelta;
        if (rightTouch) push -= rightDelta;

        if (push.sqrMagnitude > 0.000001f)
        {
            float maxStep = maxPushSpeed * Time.fixedDeltaTime;
            push = Vector3.ClampMagnitude(push, maxStep);
            body.MovePosition(body.position + push);
            velocity = push / Time.fixedDeltaTime;
        }
        else
        {
            ApplyGravity();
        }

        leftHand.position = leftTarget;
        rightHand.position = rightTarget;
        lastLeftWorld = leftTarget;
        lastRightWorld = rightTarget;
        ResolveBodyContacts();
    }

    private void RefreshDevices()
    {
        if (!leftDevice.isValid) leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (!rightDevice.isValid) rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private Vector3 GetHandWorldPosition(InputDevice device, XRNode fallbackNode)
    {
        Vector3 local;
        if (!device.isValid || !device.TryGetFeatureValue(CommonUsages.devicePosition, out local))
            local = InputTracking.GetLocalPosition(fallbackNode);
        return trackingSpace.TransformPoint(local);
    }

    private Vector3 ClampReach(Vector3 target)
    {
        Vector3 fromBody = target - body.worldCenterOfMass;
        if (fromBody.magnitude > maxArmReach)
            target = body.worldCenterOfMass + fromBody.normalized * maxArmReach;
        return target;
    }

    private bool IsTouchingWorld(Vector3 center)
    {
        return Physics.CheckSphere(center, handRadius, climbableMask, QueryTriggerInteraction.Ignore);
    }

    private void ApplyGravity()
    {
        bool grounded = Physics.Raycast(body.worldCenterOfMass, Vector3.down, 0.9f, climbableMask, QueryTriggerInteraction.Ignore);

        if (grounded)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, groundAcceleration * Time.fixedDeltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, 0f, groundAcceleration * Time.fixedDeltaTime);
            if (velocity.y < 0f) velocity.y = -1f;
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
            velocity.x *= 1f - airDrag * Time.fixedDeltaTime;
            velocity.z *= 1f - airDrag * Time.fixedDeltaTime;
        }

        body.MovePosition(body.position + velocity * Time.fixedDeltaTime);
    }

    private void ResolveBodyContacts()
    {
        Collider[] hits = Physics.OverlapCapsule(
            body.position + Vector3.up * 0.15f,
            body.position + Vector3.up * 1.05f,
            0.34f,
            climbableMask,
            QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].attachedRigidbody == body) continue;

            Vector3 closest = hits[i].ClosestPoint(body.worldCenterOfMass);
            Vector3 away = body.worldCenterOfMass - closest;
            if (away.sqrMagnitude > 0.000001f)
                body.MovePosition(body.position + away.normalized * 0.01f);
        }
    }
}
