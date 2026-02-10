using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    private static SpawnManager _instance;

    [Header("Spawn Zone")]
    [SerializeField] private Vector2 startingPoint = Vector2.zero;
    [SerializeField] private Vector2 endingPoint = Vector2.zero;

    [Header("Spawn Settings")]
    [SerializeField] private Vector2 childMassRange = Vector2.one;
    [SerializeField] private float spawnChildTime = 2f;
    [SerializeField] private float spawnBalloonTime = 2f;
    [SerializeField] private float initialAltitude = 10f;

    [Header("Objects Containers")]
    [SerializeField] private GameObject childsContainer;
    [SerializeField] private GameObject balloonsContainer;

    [Header("Spawn Objects")]
    [Tooltip("Collectable objects")]
    [SerializeField] private GameObject childPrefab;
    [Tooltip("Item boxes")]
    [SerializeField] private GameObject balloonPrefab;

    /************** HOOKS **************/

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
        StartCoroutine(SpawnChilds());
        StartCoroutine(SpawnBalloons());
    }

    /************** PRIVATE COROUTINES **************/

    private IEnumerator SpawnChilds() {
        if (childPrefab == null) yield return null;
        if (childsContainer == null) yield return null;

        Vector3 spawnPoint = Vector3.zero;

        spawnPoint.x = Random.Range(startingPoint.x, endingPoint.x);
        spawnPoint.z = Random.Range(startingPoint.y, endingPoint.y);
        spawnPoint.y = initialAltitude;

        float spawnMass = Random.Range(childMassRange.x, childMassRange.y);
        float colorValue = 1f - ((spawnMass - 1f) / (childMassRange.y - 1f));
        Color newColor = new Color(colorValue, colorValue, colorValue, 1f);

        KidController kidController = Instantiate(childPrefab, spawnPoint, Quaternion.identity, childsContainer.transform).GetComponent<KidController>();

        kidController.SetKidMass(spawnMass);
        kidController.UpdateKidColor(newColor);

        yield return new WaitForSeconds(spawnChildTime);

        StartCoroutine(SpawnChilds());
    }

    /**
     * Item boxes spawner
     */
    private IEnumerator SpawnBalloons() {
        if (balloonPrefab == null) yield return null;
        if (balloonsContainer == null) yield return null;

        yield return new WaitForSeconds(spawnBalloonTime);
    }
}
