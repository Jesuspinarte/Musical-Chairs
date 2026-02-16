using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class PowerController : MonoBehaviour {
  [Header("Power Settings")]
  [SerializeField] InputActionReference powerAction;

  [Header("Effects")]
  [SerializeField] private GameObject speedParticles;
  [SerializeField] private GameObject forceParticles;
  [SerializeField] private GameObject magnetParticles;

  private EnumPower powerToUse;
  private EnumPower currentPower;
  private bool isSearchingPower;

  private float _powerTimer = 0f;
  private PlayerController _playerController;

  /************** HOOKS **************/

  private void OnEnable() {
    if (powerAction == null) return;

    powerAction.action.Enable();
    powerAction.action.started += OnUsePower;
  }

  private void OnDisable() {
    if (powerAction == null) return;

    powerAction.action.Disable();
    powerAction.action.started -= OnUsePower;
  }

  private void Awake() {
    _playerController = GetComponent<PlayerController>();
    PowerManager.Instance.SetPlayerPowerText(_playerController.GetPlayerID(), powerToUse.ToString());
  }

  private void Start() {
    UpdatePowerText();
  }

  private void Update() {
    if (isSearchingPower) {
      powerToUse = (EnumPower)Random.Range(1, PowerManager.Instance.GetTotalPowersSize());
      UpdatePowerText();
    }
  }

  /************** PRIVATE **************/
  private IEnumerator GetRandmPowerToUse() {
    isSearchingPower = true;
    yield return new WaitForSeconds(PowerManager.Instance.GetTimeToGetPower());
    isSearchingPower = false;
  }

  private IEnumerator OnPowerFinish() {
    yield return new WaitForSeconds(_powerTimer);

    powerToUse = EnumPower.NONE;
    ResetPlayerStats();
    UpdatePowerText();
    StopPowerAnimationText();

    if (currentPower == EnumPower.MAGNET) _playerController.StopBeingGreedy();
  }

  private void StartPowerAnimationText() {
    PowerManager.Instance.AnimatePowerText(_playerController.GetPlayerID());
  }

  private void StopPowerAnimationText() {
    PowerManager.Instance.ResetPowerTextAnimation(_playerController.GetPlayerID());
  }

  private void UpdatePowerText() {
    PowerManager.Instance.SetPlayerPowerText(_playerController.GetPlayerID(), PowerManager.Instance.GetPowerName(powerToUse));
  }

  private void ResetPlayerStats() {
    if (_playerController == null) return;
    _playerController.ResetPowerProperties();
  }

  private void OnUsePower(CallbackContext ctx) {
    if (powerToUse == EnumPower.NONE) return;
    if (isSearchingPower == true) return;

    currentPower = powerToUse;

    OnPowerActivated();
  }

  private void OnPowerActivated() {
    if (_playerController == null) return;

    bool isPowerValid = true;

    switch (currentPower) {
      case EnumPower.NONE:
        isPowerValid = false;
        break;

      case EnumPower.FORCE:
        (float forceMultiplier, float forceTimer) = PowerManager.Instance.GetForcePowerValue();
        _powerTimer = forceTimer;
        Instantiate(forceParticles, transform.position, Quaternion.identity);
        _playerController.BeStronger(forceMultiplier);
        break;

      case EnumPower.MAGNET:
        (int newMaxCapacity, float magnetTimer) = PowerManager.Instance.GetMaxMagnetCapacity();
        _powerTimer = magnetTimer;
        Instantiate(magnetParticles, transform.position, Quaternion.identity);
        _playerController.BeGreedy(newMaxCapacity);
        break;

      case EnumPower.SPEED:
        (float maxMovementSpeedValue, float accelerationValue, float speedTimer) = PowerManager.Instance.GetSpeedPowerValue();
        _powerTimer = speedTimer;
        Instantiate(speedParticles, transform.position, Quaternion.identity);
        _playerController.BeFaster(maxMovementSpeedValue, accelerationValue);
        break;

      case EnumPower.BOMB:
        (GameObject bombPrefab, float bombTimer) = PowerManager.Instance.GetBombPrefab();
        _powerTimer = bombTimer;
        _playerController.BeMad(bombPrefab);
        break;

      default:
        isPowerValid = false;
        break;
    }

    if (isPowerValid) {
      StartPowerAnimationText();
      StartCoroutine(OnPowerFinish());
    }
  }

  /************** PUBLIC **************/

  public void SetPowerToUse() {
    if (powerToUse != EnumPower.NONE) return;

    StartCoroutine(GetRandmPowerToUse());
  }

  /************** DEBUG **************/
  private void GreenLog(string logText) {
    Debug.Log($"<color=#a6e69a>{logText}</color>");
  }

  private void BlueLog(string logText) {
    Debug.Log($"<color=#52b8cc>{logText}</color>");
  }

  private void OrangeLog(string logText) {
    Debug.Log($"<color=#d18547>{logText}</color>");
  }
}
