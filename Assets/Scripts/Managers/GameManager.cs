using TMPro;
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
}
