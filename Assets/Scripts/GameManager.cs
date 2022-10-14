using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Abstract;
using Sirenix.OdinInspector;
using TMPro;
using Types;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static List<Vector3Int> pathNodes;
    public static Dictionary<Vector3Int, ATower> towerPositions;

    [Title("Settings - WorldGen")] public Tilemap tilemap;
    [Space(16)] public int maxNodeCount;
    public int maxDistance;
    [Header("Tiles")] public TileBase land;
    public TileBase path;
    public TileBase castle;
    public TileBase portal;

    [Title("Settings - Castle")] public int maxHealth;

    [Title("Settings - Portal")] public Transform enemyContainer;

    [Title("Settings - Economy")] [SerializeField]
    private int startingMoney;

    [SerializeField] private int waveCompletedReward;

    [Title("Settings - UI")] [SerializeField]
    private TextMeshProUGUI moneyText;

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI waveTimerText;
    [SerializeField] private TextMeshProUGUI towerCostText;


    [Title("Prefabs")] public GameObject fireTowerPrefab;
    public GameObject iceTowerPrefab;
    public GameObject mortarTowerPrefab;
    public GameObject goldMinePrefab;
    [Space(10)] public GameObject coinDisplayPrefab;

    [Title("References")] [SerializeField] private RectTransform towerSelection;
    [SerializeField] private Camera mainCamera;

    [Title("Wave Settings")] public float waveTimer;
    public Wave[] waves;
    [Space(16)]
    [SerializeField] private GameObject[] enemyPrefabs;

    private Vector3Int _selectedTile;
    private int _builtTowers = 0;
    private int _currentMoney;
    private float _currentWaveTimer;
    public int currentWave = 0;

    private bool _gameStarted = false;
    private int _remainingEnemies = 0;


    private int GetTowerCost() => (int)MathF.Ceiling(Mathf.Pow(_builtTowers + 1, 1.25f) * 50);

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        pathNodes = new List<Vector3Int>();
        towerPositions = new Dictionary<Vector3Int, ATower>();
        Instance = this;
        _currentMoney = startingMoney;
        moneyText.text = _currentMoney.ToString("N0", new CultureInfo("en-US"));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (tilemap.GetTile(
                    tilemap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(.5f, .5f))) !=
                land) return;

            ShowTowerSelection(
                tilemap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(.5f, .5f)));
        }

        if (_gameStarted && _currentWaveTimer <= 0)
        {
            if (waves.Length > 0)
            {
                StartCoroutine(SpawnWave(waves[0]));
                waves = waves[1..];
            }
            else
            {
                Wave wave = new Wave
                {
                    subWaves = new List<SubWave>()
                };
                int subWaves = Random.Range(1, 4);
                for (int i = 0; i < subWaves; i++)
                {
                    SubWave subWave = new SubWave
                    {
                        enemy = enemyPrefabs[Random.Range(0, 2)],
                        count = Random.Range(1, 10)
                    };
                    wave.subWaves.Add(subWave);
                }
                if (currentWave % 5 == 0) // Add boss to every fifth wave
                {
                    SubWave subWave = new SubWave
                    {
                        enemy = enemyPrefabs[3],
                        count = currentWave / 5
                    };
                    wave.subWaves.Add(subWave);
                }
                StartCoroutine(SpawnWave(wave));
            }

            _currentWaveTimer = waveTimer;
        }
        else if (_gameStarted)
        {
            _currentWaveTimer -= Time.deltaTime;
            waveTimerText.text = _currentWaveTimer.ToString("N1", new CultureInfo("en-US"));
        }
    }

    private void ShowTowerSelection(Vector3Int position)
    {
        if (towerPositions.ContainsKey(position))
            return; // TODO: If there is time, maybe implement selling and upgrading?

        int towerCost = GetTowerCost();

        towerCostText.text = towerCost > _currentMoney
            ? "<color=\"red\">" + towerCost.ToString("N0", new CultureInfo("en-US")) + "</color>"
            : towerCost.ToString("N0", new CultureInfo("en-US"));

        _selectedTile = position;
        towerSelection.position = Input.mousePosition;
        towerSelection.gameObject.SetActive(true);
    }

    private void CancelTowerSelection()
    {
        towerSelection.gameObject.SetActive(false);
    }

    public void BuildFireTower()
    {
        if (!AttemptBuy()) return;
        ATower tower = GameObject.Instantiate(fireTowerPrefab, _selectedTile, Quaternion.identity)
            .GetComponent<ATower>();
        towerPositions.Add(_selectedTile, tower);
        OnTowerBuilt();
    }

    public void BuildMortarTower()
    {
        if (!AttemptBuy()) return;
        ATower tower = GameObject.Instantiate(mortarTowerPrefab, _selectedTile, Quaternion.identity)
            .GetComponent<ATower>();
        towerPositions.Add(_selectedTile, tower);
        OnTowerBuilt();
    }

    public void BuildIceTower()
    {
        if (!AttemptBuy()) return;
        ATower tower = GameObject.Instantiate(iceTowerPrefab, _selectedTile, Quaternion.identity)
            .GetComponent<ATower>();
        towerPositions.Add(_selectedTile, tower);
        OnTowerBuilt();
    }

    public void BuildGoldMine()
    {
        if (!AttemptBuy()) return;
        ATower tower = GameObject.Instantiate(goldMinePrefab, _selectedTile, Quaternion.identity)
            .GetComponent<ATower>();
        towerPositions.Add(_selectedTile, tower);
        OnTowerBuilt();
    }

    private bool AttemptBuy()
    {
        if (_currentMoney < GetTowerCost()) return false;
        _currentMoney -= GetTowerCost();
        moneyText.text = _currentMoney.ToString("N0", new CultureInfo("en-US"));
        return true;
    }

    private void OnTowerBuilt()
    {
        CancelTowerSelection();
        _builtTowers++;
    }

    public void AddMoney(int money, Vector3 position)
    {
        _currentMoney += money;
        moneyText.text = _currentMoney.ToString("N0", new CultureInfo("en-US"));
        if (position == Vector3.zero) return;
        GameObject coinDisplay = GameObject.Instantiate(coinDisplayPrefab, position, Quaternion.identity);
        coinDisplay.GetComponent<CoinDisplay>().Init(money);
    }

    public void UpdateHealth(int currentHealth)
    {
        healthText.text = currentHealth.ToString();
    }

    public void OnEnemyKilled()
    {
        _remainingEnemies--;
        if (_remainingEnemies == 0)
        {
            AddMoney(waveCompletedReward, Vector3.zero);
        }
    }

    public void SummonNextWave()
    {
        _currentWaveTimer = 0;
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        currentWave++;
        _remainingEnemies = wave.subWaves.Sum(x => x.count);

        foreach (SubWave subWave in wave.subWaves)
        {
            for (int i = 0; i < subWave.count; i++)
            {
                GameObject enemy = GameObject.Instantiate(subWave.enemy, enemyContainer);
                enemy.GetComponent<AEnemy>().Spawn();
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSecondsRealtime(5f);
        }
    }

    public void StartGame()
    {
        _gameStarted = true;
    }
}