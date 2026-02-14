using UnityEngine;

public class GameManager : MonoBehaviour {
  private static GameManager _instance;

  /************** SERIALIZED **************/
  [Header("GLOBAL SETTINGS")]
  [Header("Player Settings")]
  [SerializeField] private float timeToRespawn = 3f;

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

  /************** PUBLIC **************/

  public float GetTimeToRespawn() {
    return timeToRespawn;
  }
}
