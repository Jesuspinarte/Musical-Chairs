using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class PowerController : MonoBehaviour {
  [Header("Power Settings")]
  [SerializeField] InputActionReference powerAction;

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
    GameManager.Instance.SetPlayerPowerText(_playerController.GetPlayerID(), powerToUse.ToString());
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
    yield return new WaitForSeconds(GameManager.Instance.GetTimeToGetPower());
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
    GameManager.Instance.AnimatePowerText(_playerController.GetPlayerID());
  }

  private void StopPowerAnimationText() {
    GameManager.Instance.ResetPowerTextAnimation(_playerController.GetPlayerID());
  }

  private void UpdatePowerText() {
    GameManager.Instance.SetPlayerPowerText(_playerController.GetPlayerID(), PowerManager.Instance.GetPowerName(powerToUse));
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
        _playerController.BeStronger(forceMultiplier);
        break;

      case EnumPower.MAGNET:
        (int newMaxCapacity, float magnetTimer) = PowerManager.Instance.GetMaxMagnetCapacity();
        _powerTimer = magnetTimer;
        _playerController.BeGreedy(newMaxCapacity);
        break;

      case EnumPower.SPEED:
        (float maxMovementSpeedValue, float accelerationValue, float speedTimer) = PowerManager.Instance.GetSpeedPowerValue();
        _powerTimer = speedTimer;
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
