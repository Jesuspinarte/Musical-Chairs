using UnityEngine;

public class PowerManager : MonoBehaviour {
  private static PowerManager _instance;

  [Header("POWER SETTINGS")]
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
}
