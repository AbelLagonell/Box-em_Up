using UnityEngine;
using UnityEngine.Serialization;

public class SpawnPersistent : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject mainCharacter;
    public GameObject waveTracking;
    public GameObject gameTracking;
    [FormerlySerializedAs("hud")] public GameObject statTracking;


    private void Start() {
        if (MainCharacter.Instance == null) Instantiate(mainCharacter, Vector3.up, Quaternion.identity);
        if (StatDisplay.Instance == null) Instantiate(statTracking);
        if (Waves.Instance == null) Instantiate(waveTracking);
        if (GameStatTracker.Instance == null) Instantiate(gameTracking);
    }
}