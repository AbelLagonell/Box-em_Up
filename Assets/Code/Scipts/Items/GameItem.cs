using UnityEngine;

public class GameItem : MonoBehaviour {
    [SerializeField] protected Texture2D[] textures;
    private MeshRenderer _meshRenderer;

    private void FixedUpdate() {
        transform.position =
            new Vector3(transform.position.x, 0.33f * Mathf.Sin(Time.time * 1.5f) + 1.5f, transform.position.z);
    }

    protected void Init() {
        var rb = GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(0f, 1f, 0f);
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    protected void UpdateTexture(int index) {
        if (textures.Length > 0 && _meshRenderer != null)
            _meshRenderer.material.mainTexture = textures[index];
    }
}