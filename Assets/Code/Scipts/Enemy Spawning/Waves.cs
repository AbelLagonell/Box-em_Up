using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
internal struct IncreasePerWave {
    public int attack;
    public int attackSpeed;
    public int defense;
    public int speed;
    public int health;
}

public class Waves : MonoBehaviour {
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject shopKeeper;
    [SerializeField] private Transform shopKeeperSpawn;
    [SerializeField] private float radius; // Radius of which enemies will not spawn
    [SerializeField] private float withinBurstTime;
    [SerializeField] private float betweenBurstTime;
    [SerializeField] private int[] enemyPerWaveCount;
    [SerializeField] private IncreasePerWave increasePerWave;
    public int waveAmountSceneChange = 3;

    [Header("Debug")] [SerializeField] private Transform[] spawners;
    [SerializeField] private Spawner[] spawnerScript;
    [SerializeField] private int enemyCount;

    public static Waves Instance { get; private set; }

    private void Start() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
        GameStatTracker.Instance.OnWaveChange += OnWaveChange;

        OnSceneChange();
        SetRadius();
        OnWaveChange(1);
    }

    private void OnDestroy() {
        GameStatTracker.Instance.OnWaveChange -= OnWaveChange;
    }

    //We want this for available positions
    private List<Transform> GetActiveSpawners() {
        return spawners.Where((_, i) => spawnerScript[i].Active).ToList();
    }

    //This is so that the enemies don't spawn near the player.
    private void SetRadius() {
        foreach (var spawner in spawnerScript) spawner.Init(radius);
    }

    private void OnWaveChange(int wave) {
        //Wait a sec
        //Start Spawning based on array
        //  Randomize the spawning based on the active spawners
        //Make sure that does not exceed enemy count

        try {
            enemyCount = enemyPerWaveCount[wave - 1];
        } catch (IndexOutOfRangeException) {
            UnityEngine.Debug.LogError("Index out of range: Input" + (wave - 1) + "vs Max: " +
                                       enemyPerWaveCount.Length);
        }

        for (var i = 0; i < enemyCount; i++) StartCoroutine(SpawnEnemy(betweenBurstTime, enemies[0], wave));
    }

    private void OnSceneChange() {
        var spawnersGO = GameObject.FindGameObjectsWithTag("Respawn");
        spawners = new Transform[spawnersGO.Length];
        spawnerScript = new Spawner[spawnersGO.Length];
        for (var i = 0; i < spawnersGO.Length; i++) {
            spawners[i] = spawnersGO[i].GetComponent<Transform>();
            spawnerScript[i] = spawnersGO[i].GetComponent<Spawner>();
        }
    }

    public void EnemyDied() {
        enemyCount--;
        if (enemyCount <= 0) StartCoroutine(SpawnShopKeeper(0.5f));
    }

    private IEnumerator SpawnEnemy(float time, GameObject enemy, int wave) {
        yield return new WaitForSeconds(time);
        var spawnerActive = GetActiveSpawners();
        var rand = Random.Range(0, spawnerActive.Count);

        var enemyScript = Instantiate(enemy, spawnerActive[rand].position, Quaternion.identity).GetComponent<Actor>();
        enemyScript.attack += wave / increasePerWave.attack;
        enemyScript.defense += wave / increasePerWave.defense;
        enemyScript.health += wave / increasePerWave.health;
        //TODO Fix these two incrementation
        enemyScript.attackSpeed = 1 + (float)wave / increasePerWave.attackSpeed;
        enemyScript.speed = 1 + (float)wave / increasePerWave.speed;
    }

    private IEnumerator SpawnShopKeeper(float time) {
        yield return new WaitForSeconds(time);
        //Gets the audio and does the crossfade
        var audioSwitcher = GameObject.FindGameObjectWithTag("AudioSwitcher");
        audioSwitcher.GetComponent<SwitchAudio>().SwitchAudioClips();
        Instantiate(shopKeeper, shopKeeperSpawn.position, Quaternion.identity);
        MainCharacter.Instance.health += MainCharacter.Instance.health / 2;
        GameStatTracker.Instance?.HealthUpdate(MainCharacter.Instance.health);
    }
}