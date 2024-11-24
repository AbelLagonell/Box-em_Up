using TMPro;
using UnityEngine;

public class NextWaveButton : MonoBehaviour {
    public TextMeshProUGUI textMeshProUGUI;

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        GameStatTracker.Instance.IncrementWaveCount();
        if (GameStatTracker.Instance.GetWaveCount() % (Waves.Instance.waveAmountSceneChange + 1) != 0) return;
        //We need to transistion to next level
        textMeshProUGUI.text = "Next Level";
        var scene = GetComponent<ChangeScene>();
        var changeScene =
            (SceneIndex)((int)scene.sceneIndex +
                         GameStatTracker.Instance.GetWaveCount() / Waves.Instance.waveAmountSceneChange);
        Waves.Instance.OnSceneChange();
        scene.sceneIndex = changeScene;
        scene.SceneChange();
    }
}