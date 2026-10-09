using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.15f;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Apply screen shake offset if active
        if (shakeDuration > 0)
        {
            smoothedPosition += (Vector3)Random.insideUnitCircle * shakeMagnitude;
            shakeDuration -= Time.deltaTime;
        }

        transform.position = smoothedPosition;
    }

    public void TriggerShake(float duration = 0.1f, float magnitude = 0.15f)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}