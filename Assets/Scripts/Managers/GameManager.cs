using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using static UnityEngine.InputSystem.InputAction;

public class GameManager : MonoBehaviour {
  private static GameManager _instance;

  /************** SERIALIZED **************/
  [Header("GLOBAL SETTINGS")]
  [SerializeField] private float timeToRespawn = 3f;
  [SerializeField] InputActionReference restartButton;

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

  private void OnEnable() {
    if (restartButton != null) restartButton.action.Enable();
    restartButton.action.performed += RestartGame;
  }

  private void OnDisable() {
    if (restartButton != null) restartButton.action.Disable();
    restartButton.action.performed -= RestartGame;
  }

  /************** PRIVATE **************/
  private void RestartGame(CallbackContext ctx) {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }


  /************** PUBLIC **************/
  public float GetTimeToRespawn() {
    return timeToRespawn;
  }
}
