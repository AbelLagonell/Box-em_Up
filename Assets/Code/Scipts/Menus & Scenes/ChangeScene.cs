using UnityEngine;

public enum SceneIndex {
    GameStart,
    Level1,
    Level2,
    Level3,
    GameEnd,
    Credits,
    Settings,
    TestLevel
}

public class ChangeScene : MonoBehaviour {
    [SerializeField] public SceneIndex sceneIndex;

    public void SceneChange() {
        SceneController.LoadScene((int)sceneIndex);
    }
}