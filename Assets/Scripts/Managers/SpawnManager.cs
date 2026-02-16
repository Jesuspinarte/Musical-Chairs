using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
  private static SpawnManager _instance;

  [Header("Spawn Zone")]
  [SerializeField] private Vector2 startingPoint = Vector2.zero;
  [SerializeField] private Vector2 endingPoint = Vector2.zero;

  [Header("Spawn Settings")]
  [SerializeField] private Vector2 kidMassRange = Vector2.one;
  [SerializeField] private float spawnKidTime = 3f;
  [SerializeField] private float spawnBalloonTime = 2f;
  [SerializeField] private float initialAltitude = 10f;

  [Header("Objects Containers")]
  [SerializeField] private GameObject kidsContainer;
  [SerializeField] private GameObject balloonsContainer;

  [Header("Spawn Objects")]
  [Tooltip("Collectable objects")]
  [SerializeField] private GameObject kidPrefab;
  [Tooltip("Item boxes")]
  [SerializeField] private GameObject balloonPrefab;

  private bool _canSpawnKids = false;
  private bool _canSpawnBalloons = true;

  /************** HOOKS **************/

  private void OnEnable() {
    TimeManager.OnCooldownChange += OnCooldownChange;
  }

  private void OnDisable() {
    TimeManager.OnCooldownChange += OnCooldownChange;
  }

  public static SpawnManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<SpawnManager>();
        if (_instance == null) {
          GameObject go = new GameObject("SpawnManager");
          _instance = go.AddComponent<SpawnManager>();
        }
      }
      return _instance;
    }
  }

  private void Awake() {
    StartCoroutine(SpawnBalloons());
  }

  /************** PRIVATE COROUTINES **************/

  private IEnumerator SpawnKids() {
    if (kidPrefab == null) yield return null;
    if (kidsContainer == null) yield return null;
    if (!_canSpawnKids) yield return null;

    Vector3 spawnPoint = Vector3.zero;

    spawnPoint.x = Random.Range(startingPoint.x, endingPoint.x);
    spawnPoint.z = Random.Range(startingPoint.y, endingPoint.y);
    spawnPoint.y = initialAltitude;

    float spawnMass = Random.Range(kidMassRange.x, kidMassRange.y);
    float percent = 1f - ((spawnMass - 1f) / (kidMassRange.y - 1f));
    float colorValue = Mathf.Lerp(0.9f, 0.3f, percent);
    Color newColor = new Color(colorValue, colorValue, colorValue);

    KidController kidController = Instantiate(kidPrefab, spawnPoint, Quaternion.identity, kidsContainer.transform).GetComponent<KidController>();

    kidController.SetKidMass(spawnMass);
    kidController.UpdateKidColor(newColor);

    yield return new WaitForSeconds(spawnKidTime);

    StartCoroutine(SpawnKids());
  }

  /**
   * Item boxes spawner
   */
  private IEnumerator SpawnBalloons() {
    if (balloonPrefab == null) yield return null;
    if (balloonsContainer == null) yield return null;
    if (!_canSpawnBalloons) yield return null;

    Vector3 spawnPoint = Vector3.zero;

    spawnPoint.x = Random.Range(startingPoint.x, endingPoint.x);
    spawnPoint.z = Random.Range(startingPoint.y, endingPoint.y);
    spawnPoint.y = initialAltitude;

    Instantiate(balloonPrefab, spawnPoint, Quaternion.identity, balloonsContainer.transform);

    yield return new WaitForSeconds(spawnBalloonTime);

    StartCoroutine(SpawnBalloons());
  }

  /************** PRIVATE **************/
  private void OnCooldownChange() {
    if (TimeManager.Instance.IsCooldownTime()) {
      _canSpawnKids = false;
      StopAllCoroutines();
      StartCoroutine(SpawnBalloons());
    }
    else {
      if (TimeManager.Instance.GetCurrentRound() > 1) {
        spawnKidTime /= 2;
      }

      _canSpawnKids = true;
      StartCoroutine(SpawnKids());
    }
  }
}
