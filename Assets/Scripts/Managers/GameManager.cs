using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {
  private static GameManager _instance;

  [Header("TEXT REFERENCES")]

  [Header("Player 1")]
  [SerializeField] private TextMeshProUGUI player1ScoreText;
  [SerializeField] private TextMeshProUGUI player1PowerText;
  private BlinkText _player1PowerAnimation;

  [Header("Player 2")]
  [SerializeField] private TextMeshProUGUI player2ScoreText;
  [SerializeField] private TextMeshProUGUI player2PowerText;
  private BlinkText _player2PowerAnimation;

  [Header("PowerManager")]
  [SerializeField] private float timeToGetPower = 0.5f;
  [SerializeField] private float textAnimationSpeed = 10f;

  [Header("GLOBAL SETTINGS")]

  [Header("Time Settings")]
  [SerializeField] private TextMeshProUGUI timerText;
  [SerializeField] private TextMeshProUGUI roundText;
  [Tooltip("In seconds")]
  [SerializeField] private float coolDownTime = 10f;
  [Tooltip("In seconds")]
  [SerializeField] private float roundTime = 45f;
  [SerializeField] private int totalRounds = 3;

  [Header("Player Settings")]
  [SerializeField] private float timeToRespawn = 3f;

  private int _scoreP1 = 0;
  private int _scoreP2 = 0;
  private int _currentRound = 0;
  private float _currentTime = 0f;
  private bool _isCooldownTime = false;

  /************** HOOKS **************/

  public static GameManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<GameManager>();
        if (_instance == null) {
          GameObject go = new GameObject("GameManager");
          _instance = go.AddComponent<GameManager>();
        }
      }
      return _instance;
    }
  }

  private void Awake() {
    _player1PowerAnimation = player1PowerText.GetComponent<BlinkText>();
    _player2PowerAnimation = player2PowerText.GetComponent<BlinkText>();
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
  public void SetPlayerScoreText(EnumPlayerID playerID, int score) {

    switch (playerID) {
      case EnumPlayerID.PLAYER1:
        if (player1ScoreText == null) return;
        player1ScoreText.text = score.ToString();

        break;

      case EnumPlayerID.PLAYER2:
        if (player2ScoreText == null) return;
        player2ScoreText.text = score.ToString();

        break;

      default:
        break;
    }
  }

  public void SetPlayerPowerText(EnumPlayerID playerID, string powerName) {
    switch (playerID) {
      case EnumPlayerID.PLAYER1:
        if (player1PowerText == null) return;
        player1PowerText.text = powerName;

        break;

      case EnumPlayerID.PLAYER2:
        if (player2PowerText == null) return;
        player2PowerText.text = powerName;

        break;

      default:
        break;
    }
  }

  public void AnimatePowerText(EnumPlayerID playerID) {
    switch (playerID) {
      case EnumPlayerID.PLAYER1:
        _player1PowerAnimation.minAlpha = .2f;
        _player1PowerAnimation.speed = textAnimationSpeed;
        break;

      case EnumPlayerID.PLAYER2:
        _player2PowerAnimation.minAlpha = .2f;
        _player2PowerAnimation.speed = textAnimationSpeed;
        break;

      default:
        break;
    }
  }

  public void ResetPowerTextAnimation(EnumPlayerID playerID) {
    switch (playerID) {
      case EnumPlayerID.PLAYER1:
        _player1PowerAnimation.minAlpha = 1f;
        _player1PowerAnimation.speed = 1f;
        break;

      case EnumPlayerID.PLAYER2:
        _player2PowerAnimation.minAlpha = 1f;
        _player2PowerAnimation.speed = 1f;
        break;

      default:
        break;
    }
  }

  public float GetTimeToGetPower() {
    return timeToGetPower;
  }

  public int AddScore(EnumPlayerID playerID, int score) {
    switch (playerID) {
      case EnumPlayerID.PLAYER1:
        _scoreP1 += score;
        SetPlayerScoreText(playerID, _scoreP1);

        return _scoreP2;
      case EnumPlayerID.PLAYER2:
        _scoreP2 += score;
        SetPlayerScoreText(playerID, _scoreP2);

        return _scoreP2;
      default:
        return 0;
    }
  }

  public float GetTimeToRespawn() {
    return timeToRespawn;
  }
}
