using UnityEngine;

public class ChangeMenu : MonoBehaviour {
    [SerializeField] private GameObject[] screens;

    private void Start() {
        foreach (var screen in screens) screen.SetActive(false);
        screens[0].SetActive(true);
    }

    public void ChangeScreen(int index) {
        if (index > screens.Length) return;
        foreach (var screen in screens) screen.SetActive(false);
        screens[index].SetActive(true);
    }
}