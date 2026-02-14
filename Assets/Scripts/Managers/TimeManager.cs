using System;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour {
  private static TimeManager _instance;

  public static Action OnCooldownChange;

  /************** SERIALIZED **************/
  [Header("Time Settings")]
  [SerializeField] private TextMeshProUGUI timerText;
  [SerializeField] private TextMeshProUGUI roundText;
  [Tooltip("In seconds")]
  [SerializeField] private float coolDownTime = 10f;
  [Tooltip("In seconds")]
  [SerializeField] private float roundTime = 45f;
  [SerializeField] private int totalRounds = 3;

  [Header("Text Settings")]
  [SerializeField] private TextMeshProUGUI statusText;
  [SerializeField] private float statusStartGameDuration = 3f;

  private int _currentRound = 0;
  private float _currentTime = 0f;
  private bool _isCooldownTime = false;
  private bool _lastIsCooldownTime = false;
  private BlinkText _statusTextSettings;

  /************** HOOKS **************/

  public static TimeManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<TimeManager>();
        if (_instance == null) {
          GameObject go = new GameObject("TimeManager");
          _instance = go.AddComponent<TimeManager>();
        }
      }
      return _instance;
    }
  }

  private void Awake() {
    _statusTextSettings = statusText.GetComponent<BlinkText>();
  }

  private void Start() {
    _currentTime = coolDownTime;
    _isCooldownTime = true;
    _lastIsCooldownTime = true;
  }

  private void Update() {
    UpdateTimer();
    UpdateStatusText();
    HasCooldownChanged();
  }

  /************** PRIVATE **************/
  private void HasCooldownChanged() {
    if (_lastIsCooldownTime == _isCooldownTime) return;
    _lastIsCooldownTime = _isCooldownTime;
    OnCooldownChange?.Invoke();
  }

  private void UpdateTimer() {
    if (IsGameOver()) {
      _currentTime = 0;
      roundText.text = "GAME";
      timerText.text = "OVER!";
      return;
    }
    _currentTime -= Time.deltaTime;

    if (_currentTime <= 0f) {
      _currentTime = _isCooldownTime ? roundTime : coolDownTime;
      _currentRound += _isCooldownTime ? 1 : 0;
      _isCooldownTime = !_isCooldownTime;
    }

    timerText.text = $"{(int)Mathf.Ceil(_currentTime)}s";
    roundText.text = _isCooldownTime ? "COOLDOWN" : $"ROUND {_currentRound}";
  }

  private void UpdateStatusText() {
    if (IsGameOver()) {
      EnumPlayerID? winner = ScoreManager.Instance.GetWinningPlayer();
      statusText.enabled = true;
      _statusTextSettings.speed = 20f;

      switch (winner) {
        case null:
          statusText.text = "YOU TIED! BE SAAAAAAAAD :(";
          return;
        case EnumPlayerID.PLAYER1:
          statusText.text = "PLAYER 1: BEEE CHAAAAAAMP!";
          return;
        case EnumPlayerID.PLAYER2:
          statusText.text = "PLAYER 2: BEEE CHAAAAAAMP!";
          return;
        default:
          statusText.text = "BE CONFUSED???";
          return;
      }
    }

    if (_isCooldownTime) {
      statusText.enabled = true;
      statusText.text = "BE COOL! CHILL OUT!";
      _statusTextSettings.speed = 1f;
      return;
    }

    if (_currentTime > roundTime - statusStartGameDuration) {
      statusText.enabled = true;
      statusText.text = "BE CRAAAAAAAZYYYYY!";
      _statusTextSettings.speed = 7f;
      return;
    }
    else {
      statusText.enabled = false;
      _statusTextSettings.speed = 1f;
      return;
    }
  }

  /************** PUBLIC **************/
  public int GetCurrentRound() {
    return _currentRound;
  }

  public bool IsCooldownTime() {
    return _isCooldownTime;
  }

  public bool IsGameOver() {
    return _isCooldownTime && _currentRound >= totalRounds;
  }
}
