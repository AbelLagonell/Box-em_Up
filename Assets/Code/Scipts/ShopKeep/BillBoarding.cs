using UnityEngine;

public class BillBoarding : MonoBehaviour {
    private void FixedUpdate() {
        transform.rotation = Camera.main.transform.rotation;
    }
}
