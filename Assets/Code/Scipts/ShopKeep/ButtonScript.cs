using System;
using UnityEngine;

public class ButtonScript : MonoBehaviour {
    [SerializeField] private int id;

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        OnButtonPressed?.Invoke(id);
    }

    public void SetId(int i) {
        id = i;
    }

    public event Action<int> OnButtonPressed;
}