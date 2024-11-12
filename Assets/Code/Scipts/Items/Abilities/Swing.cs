using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TerrainUtils;

public class Swing : MonoBehaviour {
    private float _radius = 2f;       // The radius of the circle
    public float totalArcAngle = 90f; // The total angle of the arc in degrees
    public float arcDuration = 1f;    // The duration of the arc movement in seconds

    private Vector3 _centerPosition;
    private float _startAngle;
    private float _elapsedTime;

    private void Start() {
        // Calculate the center position of the circle
        _centerPosition = transform.position;

        // Calculate the starting angle of the object
        _startAngle = -totalArcAngle / 2f;
    }

    private void Update() {
        // Calculate the elapsed time since the start of the arc movement
        _elapsedTime += Time.deltaTime;

        // Normalize the elapsed time to a value between 0 and 1
        var normalizedTime = _elapsedTime / arcDuration;

        // Calculate the current angle along the arc
        var currentAngle = Mathf.Lerp(_startAngle, _startAngle + totalArcAngle, normalizedTime);

        // Calculate the current position and rotation along the arc
        var currentPosition = CalculateArcPosition(currentAngle);
        var currentRotation = CalculateArcRotation(currentAngle);

        // Update the object's position and rotation
        transform.position = currentPosition;
        transform.rotation = currentRotation;

        // Check if the arc movement is complete
        if (_elapsedTime >= arcDuration)
            Destroy(gameObject);
    }

    private Vector3 CalculateArcPosition(float angle) {
        // Calculate the position along the arc using the circle formula
        var z = _centerPosition.x + _radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        var x = _centerPosition.y + _radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        var y = _centerPosition.z;
        return new Vector3(x, y, z);
    }

    private Quaternion CalculateArcRotation(float angle) {
        // Rotate the object to face the tangent of the circle at the current position
        var tangentAngle = angle;
        return Quaternion.Euler(-90f, tangentAngle, 0f);
    }

    public void newAngle(float angle, float radius) {
        _startAngle = -angle / 2;
        _radius     = radius;
    }
}