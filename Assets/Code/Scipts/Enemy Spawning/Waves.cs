using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private float radius; // Radius of which enemies will not spawn
    [SerializeField] private float withinBurstTime;
    [SerializeField] private float betweenBurstTime;
    [SerializeField] private int[] enemyPerWaveCount;
    [SerializeField] private int[] enemyBurstCount;
    [SerializeField] private IncreasePerWave increasePerWave;
    public int waveAmountSceneChange = 3;

    [Header("Debug")] [SerializeField] private Transform[] spawners;
    [SerializeField] private Spawner[] spawnerScript;
    [SerializeField] private int enemyCount;
    [SerializeField] private Transform shopKeeperSpawn;

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

    private void FixedUpdate() {
        if (SceneManager.GetActiveScene().buildIndex == 4) Destroy(gameObject);
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
        if (wave != 1) SwitchTracks();
        try {
            enemyCount = enemyPerWaveCount[wave - 1];
        } catch (IndexOutOfRangeException) {
            UnityEngine.Debug.LogError("Index out of range: Input" + (wave - 1) + " vs Max: " +
                                       enemyPerWaveCount.Length);
            SceneManager.LoadScene((int)SceneIndex.GameEnd);
        }

        var cEnemyBurst = enemyBurstCount[wave - 1];
        var cEnemiesRemaining = enemyPerWaveCount[wave - 1];
        do {
            var cEnemiesSpawn = 0;
            if (cEnemiesRemaining - cEnemyBurst <= 0) cEnemiesSpawn = cEnemiesRemaining;
            else cEnemiesSpawn = cEnemiesRemaining - cEnemyBurst;
            StartCoroutine(SpawnEnemies(enemies[Random.Range(0, enemies.Length)], wave,
                                        cEnemiesSpawn));
            cEnemiesRemaining -= cEnemiesSpawn;
        } while (cEnemiesRemaining > 0);
    }

    public void OnSceneChange() {
        //Spawners
        var spawnersGO = GameObject.FindGameObjectsWithTag("Respawn");
        spawners = new Transform[spawnersGO.Length];
        spawnerScript = new Spawner[spawnersGO.Length];
        for (var i = 0; i < spawnersGO.Length; i++) {
            spawners[i] = spawnersGO[i].GetComponent<Transform>();
            spawnerScript[i] = spawnersGO[i].GetComponent<Spawner>();
        }

        shopKeeperSpawn = GameObject.FindGameObjectWithTag("Shop").transform;
    }

    public void EnemyDied() {
        enemyCount--;
        if (enemyCount <= 0) StartCoroutine(SpawnShopKeeper(0.5f));
    }

    private IEnumerator SpawnEnemies(GameObject enemy, int wave, int amount) {
        yield return new WaitForSeconds(betweenBurstTime);
        for (var i = 0; i < amount; i++) StartCoroutine(SpawnEnemy(withinBurstTime, enemy, wave));
    }

    private IEnumerator SpawnEnemy(float time, GameObject enemy, int wave) {
        yield return new WaitForSeconds(time);
        var spawnerActive = GetActiveSpawners();
        try {
            //Test if its working
            var transform1 = spawnerActive[0].transform;
        } catch {
            OnSceneChange();
            spawnerActive = GetActiveSpawners();
        }

        var rand = Random.Range(0, spawnerActive.Count);
        var spawner = spawnerActive[rand].position;
        var enemyScript = Instantiate(enemy, spawner, Quaternion.identity).GetComponent<Actor>();
        enemyScript.attack += wave / increasePerWave.attack;
        enemyScript.defense += wave / increasePerWave.defense;
        enemyScript.health += wave / increasePerWave.health;
        enemyScript.attackSpeed = 1 + (float)wave / increasePerWave.attackSpeed;
        enemyScript.speed += (enemyScript.speed + wave) / increasePerWave.speed;
    }

    private IEnumerator SpawnShopKeeper(float time) {
        yield return new WaitForSeconds(time);
        //Gets the audio and does the crossfade
        SwitchTracks();
        Instantiate(shopKeeper, shopKeeperSpawn.position, Quaternion.identity);
        MainCharacter.Instance.AddHealth();
    }

    private void SwitchTracks() {
        var audioSwitcher = GameObject.FindGameObjectWithTag("AudioSwitcher");
        audioSwitcher?.GetComponent<SwitchAudio>().SwitchAudioClips();
    }
}