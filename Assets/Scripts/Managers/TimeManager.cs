using System;
using FMODUnity;
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
  [SerializeField] private float statusStartGameDuration = 3f;

  [Header("Effects")]
  [SerializeField] private GameObject gameOverParticles;
  [SerializeField] private GameObject cooldownParticles;
  [SerializeField] private GameObject playParticles;

  [Header("Audio SFX")]
  public EventReference sfxExplosionRound;

  private int _currentRound = 0;
  private float _currentTime = 0f;
  private bool _isCooldownTime = false;
  private bool _lastIsCooldownTime = false;

  private bool _isWinTextDisplayed = false;
  private bool _isCooldownTextDisplayed = false;
  private bool _isPlayTextDisplayed = false;

  private GameObject _currentParticles = null;

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

    if (_isCooldownTime)
      RuntimeManager.PlayOneShot(sfxExplosionRound, Vector3.zero);
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
    if (_isWinTextDisplayed) return;


    if (IsGameOver() && !_isWinTextDisplayed) {
      EnumPlayerID? winner = ScoreManager.Instance.GetWinningPlayer();
      _isWinTextDisplayed = true;

      if (_currentParticles != null) Destroy(_currentParticles);
      _currentParticles = Instantiate(gameOverParticles, transform.position, Quaternion.identity);
      MusicManager.Instance.ChangeMusic("GameOver");

      switch (winner) {
        case null:
          TextManager.Instance.DisplayWinText("YOU TIED! BE SAAAAAAAAD :(");
          return;
        case EnumPlayerID.PLAYER1:
          TextManager.Instance.DisplayWinText("PLAYER 1: BEEE CHAAAAAAAD!");
          return;
        case EnumPlayerID.PLAYER2:
          TextManager.Instance.DisplayWinText("PLAYER 2: BEEE CHAAAAAAAD x2!");
          return;
        default:
          TextManager.Instance.DisplayWinText("BE CONFUSED???");
          return;
      }
    }

    if (_isCooldownTime && !_isCooldownTextDisplayed) {
      _isCooldownTextDisplayed = true;
      _isPlayTextDisplayed = false;

      if (_currentParticles != null) Destroy(_currentParticles);

      Vector3 particlesPos = transform.position;
      particlesPos.y += 3;
      _currentParticles = Instantiate(cooldownParticles, particlesPos, Quaternion.identity);

      MusicManager.Instance.ChangeMusic("Cooldown");
      TextManager.Instance.DisplayCooldownText("BE COOL! CHILL OUT!");
      return;
    }

    if (_currentTime > roundTime - statusStartGameDuration && !_isPlayTextDisplayed && !_isCooldownTime) {
      _isCooldownTextDisplayed = false;
      _isPlayTextDisplayed = true;

      if (_currentParticles != null) Destroy(_currentParticles);
      _currentParticles = Instantiate(playParticles, transform.position, Quaternion.identity);

      switch (_currentRound) {
        case 1:
          MusicManager.Instance.ChangeMusic("Round1");
          break;

        case 2:
          MusicManager.Instance.ChangeMusic("Round2");
          break;

        case 3:
          MusicManager.Instance.ChangeMusic("Round3");
          break;

        default:
          break;
      }

      TextManager.Instance.DisplayPlayText("BE CRAAAAAAAZYYYYY!");
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
