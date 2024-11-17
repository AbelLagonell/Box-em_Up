using UnityEngine;

public class Swing : MonoBehaviour {
    public float totalArcAngle = 90f; // The total angle of the arc in degrees
    public float arcDuration = 1f;    // The duration of the arc movement in seconds

    private Vector3 _centerPosition;
    private Quaternion _characterRotation;
    private float _elapsedTime;
    private float _radius = 2f; // The radius of the circle
    private float _startAngle;

    private void Start() {
        _centerPosition = transform.position;
        _startAngle = -totalArcAngle / 2f;
    }

    private void Update() {
        _elapsedTime += Time.deltaTime;
        var normalizedTime = _elapsedTime / arcDuration;
        var currentAngle = Mathf.Lerp(_startAngle, _startAngle + totalArcAngle, normalizedTime);

        var currentPosition = CalculateArcPosition(currentAngle);
        var currentRotation = CalculateArcRotation(currentAngle);

        transform.position = currentPosition;
        transform.rotation = currentRotation;

        if (_elapsedTime >= arcDuration)
            Destroy(gameObject);
    }

    private Vector3 CalculateArcPosition(float angle) {
        // Calculate local offset
        var localOffset = new Vector3(
                                      _radius * Mathf.Sin(angle * Mathf.Deg2Rad),
                                      0f,
                                      _radius * Mathf.Cos(angle * Mathf.Deg2Rad)
                                     );

        // Transform the offset by character rotation
        var rotatedOffset = _characterRotation * localOffset;
        return _centerPosition + rotatedOffset;
    }

    private Quaternion CalculateArcRotation(float angle) {
        // Rotate the object to face the tangent of the circle at the current position
        return _characterRotation * Quaternion.Euler(-90f, angle, 0f);
    }

    public void NewAngle(float angle, float radius, Quaternion charRot) {
        _startAngle = -angle / 2;
        _radius = radius;
        _characterRotation = charRot;
    }
}