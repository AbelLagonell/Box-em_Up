using UnityEngine;

[ExecuteInEditMode]
public class HitboxVisualizer : MonoBehaviour {
    private BoxCollider _boxCollider;

    private void Start() {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void OnDrawGizmos() {
#if UNITY_EDITOR
        if (_boxCollider == null) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(_boxCollider.center, _boxCollider.size);
#endif
    }
}