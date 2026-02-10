using UnityEngine;
using UnityEngine.AI;

public class ChildController : MonoBehaviour {
    [Header("Dectection Settings")]
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] float runDistance = 0.5f;

    private NavMeshAgent _agent;
    private Transform[] _chairList;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();

        GameObject[] chairGoList = GameObject.FindGameObjectsWithTag("Player");
        _chairList = new Transform[chairGoList.Length];

        for (int i = 0; i < chairGoList.Length; ++i)
            _chairList[i] = chairGoList[i].transform;
    }

    void Start() {

    }

    void Update() {
        ShouldRun();
    }

    private void ShouldRun() {
        Transform closestChair = GetClosestChair();

        if (closestChair == null) return;

        float distance = Vector3.Distance(transform.position, closestChair.position);

        if (distance < runDistance) {
            Runaway(closestChair);
        }

    }

    private void Runaway(Transform chair) {
        if (chair == null) return;

        Vector3 newDirection = transform.position - chair.position;
        Vector3 finalPosition = transform.position + newDirection.normalized * runDistance;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(finalPosition, out hit, 2.0f, NavMesh.AllAreas)) {
            _agent.SetDestination(hit.position);
        }
    }

    private Transform GetClosestChair() {
        Transform closestChair = null;
        float minDistance = Mathf.Infinity;
        Vector3 childPosition = transform.position;

        foreach (Transform chair in _chairList) {
            if (chair == null) continue;
            float distance = (chair.position - childPosition).sqrMagnitude;

            if (distance < minDistance) {
                minDistance = distance;
                closestChair = chair;
            }
        }

        return closestChair;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
