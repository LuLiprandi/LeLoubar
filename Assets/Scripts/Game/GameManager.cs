using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const float SessionDurationSeconds = 300f; // 10 minutes
    private const int MoneyObjective = 1000;
    private const int StartingHearts = 3;
    private const int MoneyPerBeer = 50;

    public float TimeRemaining { get; private set; }
    public int Money { get; private set; }
    public int Hearts { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsVictory { get; private set; }

    public event Action<int> OnMoneyChanged;
    public event Action<int> OnHeartsChanged;
    public event Action<float> OnTimerUpdated;
    public event Action OnGameOver;
    public event Action OnVictory;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        TimeRemaining = SessionDurationSeconds;
        Money = 0;
        Hearts = StartingHearts;
        IsGameOver = false;
        IsVictory = false;
    }

    private void Update()
    {
        if (IsGameOver || IsVictory) return;

        TimeRemaining -= Time.deltaTime;
        OnTimerUpdated?.Invoke(TimeRemaining);

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            TriggerGameOver();
        }
    }

    /// <summary>Appelé quand un NPC est servi correctement.</summary>
    public void AddMoney(int amount = MoneyPerBeer)
    {
        if (IsGameOver || IsVictory) return;

        Money += amount;
        OnMoneyChanged?.Invoke(Money);

        if (Money >= MoneyObjective)
            TriggerVictory();
    }

    /// <summary>Appelé quand un criminel part sans être servi.</summary>
    public void LoseHeart()
    {
        if (IsGameOver || IsVictory) return;

        Hearts = Mathf.Max(0, Hearts - 1);
        OnHeartsChanged?.Invoke(Hearts);

        if (Hearts <= 0)
            TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log("[GameManager] Game Over.");
        OnGameOver?.Invoke();
    }

    private void TriggerVictory()
    {
        if (IsVictory) return;
        IsVictory = true;
        Debug.Log("[GameManager] Victoire !");
        OnVictory?.Invoke();
    }
}
