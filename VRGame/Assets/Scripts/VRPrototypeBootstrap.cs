using UnityEngine;
using UnityEngine.XR;

public sealed class VRPrototypeBootstrap : MonoBehaviour
{
    [SerializeField] private Color worldColor = new Color(0.035f, 0.045f, 0.07f);
    [SerializeField] private Color platformColor = new Color(0.12f, 0.55f, 0.85f);

    private void Awake()
    {
        Application.targetFrameRate = 90;
        QualitySettings.vSyncCount = 0;
        XRSettings.enabled = true;
        BuildWorld();
        BuildPlayer();
    }

    private void BuildWorld()
    {
        RenderSettings.ambientLight = worldColor;

        CreateCube("Ground", new Vector3(0f, -0.15f, 0f), new Vector3(18f, 0.3f, 18f), worldColor);
        CreateCube("WallNorth", new Vector3(0f, 2f, 9f), new Vector3(18f, 4f, 0.3f), worldColor);
        CreateCube("WallSouth", new Vector3(0f, 2f, -9f), new Vector3(18f, 4f, 0.3f), worldColor);
        CreateCube("WallEast", new Vector3(9f, 2f, 0f), new Vector3(0.3f, 4f, 18f), worldColor);
        CreateCube("WallWest", new Vector3(-9f, 2f, 0f), new Vector3(0.3f, 4f, 18f), worldColor);

        CreateCube("PlatformLow", new Vector3(0f, 1.1f, 3.5f), new Vector3(4f, 0.4f, 4f), platformColor);
        CreateCube("PlatformLeft", new Vector3(-4.5f, 2.4f, 0.5f), new Vector3(3f, 0.4f, 3f), platformColor);
        CreateCube("PlatformRight", new Vector3(4.5f, 3.4f, -1.5f), new Vector3(3f, 0.4f, 3f), platformColor);
        CreateCube("PlatformHigh", new Vector3(0f, 4.8f, -4.5f), new Vector3(4f, 0.4f, 3f), platformColor);

        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8f;
            Vector3 p = new Vector3(Mathf.Cos(angle) * 6f, 1.2f, Mathf.Sin(angle) * 6f);
            CreateCylinder("ClimbPole_" + i, p, 0.35f, 2.4f, platformColor);
        }
    }

    private void BuildPlayer()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0f, 1.2f, 0f);

        Rigidbody body = player.AddComponent<Rigidbody>();
        body.mass = 1f;
        body.drag = 0.15f;
        body.angularDrag = 0.05f;
        body.constraints = RigidbodyConstraints.FreezeRotation;
        body.interpolation = RigidbodyInterpolation.Interpolate;

        CapsuleCollider capsule = player.AddComponent<CapsuleCollider>();
        capsule.height = 1.25f;
        capsule.radius = 0.35f;
        capsule.center = new Vector3(0f, 0.625f, 0f);

        GameObject trackingSpace = new GameObject("TrackingSpace");
        trackingSpace.transform.SetParent(player.transform, false);

        GameObject cameraObject = new GameObject("MainCamera");
        cameraObject.transform.SetParent(trackingSpace.transform, false);
        Camera cam = cameraObject.AddComponent<Camera>();
        cam.nearClipPlane = 0.03f;
        cam.farClipPlane = 80f;
        cam.stereoTargetEye = StereoTargetEyeMask.Both;
        cameraObject.tag = "MainCamera";

        GameObject left = CreateHand("LeftHand", trackingSpace.transform);
        GameObject right = CreateHand("RightHand", trackingSpace.transform);

        GorillaLocomotion locomotion = player.AddComponent<GorillaLocomotion>();
        locomotion.Initialize(body, trackingSpace.transform, cameraObject.transform, left.transform, right.transform);
    }

    private GameObject CreateHand(string name, Transform parent)
    {
        GameObject hand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hand.name = name;
        hand.transform.SetParent(parent, false);
        hand.transform.localScale = Vector3.one * 0.18f;

        Rigidbody rb = hand.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        return hand;
    }

    private GameObject CreateCube(string name, Vector3 position, Vector3 scale, Color color)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().material.color = color;
        return obj;
    }

    private GameObject CreateCylinder(string name, Vector3 position, float radius, float height, Color color)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = new Vector3(radius, height * 0.5f, radius);
        obj.GetComponent<Renderer>().material.color = color;
        return obj;
    }
}
