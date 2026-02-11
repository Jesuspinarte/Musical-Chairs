using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class PowerController : MonoBehaviour {
    [Header("Power Settings")]
    [SerializeField] InputActionReference powerAction;

    [Header("Debug Variables")]
    [SerializeField] private EnumPowers powerToUse;
    [SerializeField] private EnumPowers currentPower;
    [SerializeField] private bool isSearchingPower;

    private int totalPowersSize = 0;

    /************** HOOKS **************/

    private void OnEnable() {
        if (powerAction == null) return;

        powerAction.action.Enable();
        powerAction.action.started += UsePower;
    }

    private void OnDisable() {
        if (powerAction == null) return;

        powerAction.action.Disable();
        powerAction.action.started -= UsePower;
    }

    private void Awake() {
        totalPowersSize = System.Enum.GetValues(typeof(EnumPowers)).Length;
    }

    private void Update() {
        if (isSearchingPower) {
            powerToUse = (EnumPowers)Random.Range(1, totalPowersSize);
        }
    }

    /************** PRIVATE **************/
    private IEnumerator GetRandmPowerToUse() {
        isSearchingPower = true;
        yield return new WaitForSeconds(GameManager.Instance.GetTimeToGetPower());
        isSearchingPower = false;
    }

    private void UsePower(CallbackContext ctx) {
        if (powerToUse == EnumPowers.NONE) return;
        if (isSearchingPower == true) return;

        currentPower = powerToUse;
        powerToUse = EnumPowers.NONE;
    }

    /************** PUBLIC **************/

    public void SetPowerToUse() {
        if (powerToUse != EnumPowers.NONE) return;

        StartCoroutine(GetRandmPowerToUse());
    }
}
