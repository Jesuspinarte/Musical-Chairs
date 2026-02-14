using TMPro;
using UnityEngine;

public class PowerManager : MonoBehaviour {
  private static PowerManager _instance;

  /************** SERIALIZED **************/
  [Header("TEXT REFERENCES")]

  [Header("Player 1")]
  [SerializeField] private TextMeshProUGUI player1PowerText;
  private BlinkText _player1PowerAnimation;

  [Header("Player 2")]
  [SerializeField] private TextMeshProUGUI player2PowerText;
  private BlinkText _player2PowerAnimation;

  [Header("Power Constraints")]
  [SerializeField] private float timeToGetPower = 0.5f;
  [SerializeField] private float textAnimationSpeed = 10f;

  [Header("Speed Power")]
  [SerializeField] private float speedPowerValue = 20f;
  [SerializeField] private float accelerationPowerValue = 20f;
  [SerializeField] private float speedTimer = 3f;

  [Header("Force Power")]
  [Range(0.1f, 1f)]
  [Tooltip("Force percentage. CANNOT BE ZERO (0)")]
  [SerializeField] private float forcePowerValue = 0.2f;
  [SerializeField] private float forceTimer = 5f;

  [Header("Magnet Power")]
  [SerializeField] private int maxMagnetCapacity = 10;
  [SerializeField] private float magnetTimer = 7f;

  [Header("Bomb Power")]
  [SerializeField] private GameObject bombPrefab;
  [SerializeField] private float bombTimer = .5f;
  [SerializeField] private float launchForce = 15f; // Fuerza del brazo
  [SerializeField] private float launchAngle = 1f;

  private int _totalPowersSize = 0;

  /************** HOOKS **************/
  public static PowerManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<PowerManager>();
        if (_instance == null) {
          GameObject go = new GameObject("PowerManager");
          _instance = go.AddComponent<PowerManager>();
        }
      }
      return _instance;
    }
  }

  private void Awake() {
    _totalPowersSize = System.Enum.GetValues(typeof(EnumPower)).Length;
    _player1PowerAnimation = player1PowerText.GetComponent<BlinkText>();
    _player2PowerAnimation = player2PowerText.GetComponent<BlinkText>();
  }

  /************** PRIVATE **************/

  /************** PUBLIC **************/
  public int GetTotalPowersSize() {
    return _totalPowersSize;
  }

  public (float speedValue, float accelerationValue, float timer) GetSpeedPowerValue() {
    return (speedValue: speedPowerValue, accelerationValue: accelerationPowerValue, timer: speedTimer);
  }

  public (float value, float timer) GetForcePowerValue() {
    return (value: forcePowerValue, timer: forceTimer);
  }

  public (int value, float timer) GetMaxMagnetCapacity() {
    return (value: maxMagnetCapacity, timer: magnetTimer);
  }

  public (GameObject bombPrefab, float timer) GetBombPrefab() {
    return (bombPrefab, timer: bombTimer);
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

  public string GetPowerName(EnumPower power) {
    switch (power) {
      case EnumPower.FORCE:
        return "BE STRONGER!";

      case EnumPower.MAGNET:
        return "BE GREEDY!";

      case EnumPower.SPEED:
        return "BE FASTER!";

      case EnumPower.BOMB:
        return "BE MAD!!!";

      case EnumPower.NONE:
      default:
        return "-";
    }
  }

  public float GetLaunchForce() {
    return launchForce;
  }

  public float GetLaunchAngle() {
    return launchAngle;
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
