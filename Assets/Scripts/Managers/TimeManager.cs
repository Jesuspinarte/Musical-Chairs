using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour {
  private static TimeManager _instance;

  /************** SERIALIZED **************/
  [Header("Time Settings")]
  [SerializeField] private TextMeshProUGUI timerText;
  [SerializeField] private TextMeshProUGUI roundText;
  [Tooltip("In seconds")]
  [SerializeField] private float coolDownTime = 10f;
  [Tooltip("In seconds")]
  [SerializeField] private float roundTime = 45f;
  [SerializeField] private int totalRounds = 3;

  private int _currentRound = 0;
  private float _currentTime = 0f;
  private bool _isCooldownTime = false;

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
  }

  private void Update() {
    UpdateTimer();
  }

  /************** PRIVATE **************/
  private void UpdateTimer() {
    if (_isCooldownTime && _currentRound >= totalRounds) {
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

  /************** PUBLIC **************/
  public int GetCurrentRound() {
    return _currentRound;
  }

  public bool IsCooldownTime() {
    return _isCooldownTime;
  }
}
