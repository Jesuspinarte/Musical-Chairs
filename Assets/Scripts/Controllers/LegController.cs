using System.Collections;
using UnityEngine;

public class LegController : MonoBehaviour {
  /************** SERIALIZED **************/
  [Header("References")]
  [SerializeField] private Transform ikTarget;
  [SerializeField] private Transform homePos;
  [SerializeField] private LayerMask groundLayer;

  [Header("Step Settings")]
  [SerializeField] private float stepDistance = 0.5f;
  [SerializeField] private float stepHeight = 0.4f;
  [SerializeField] private float stepDuration = 0.3f;
  [SerializeField] private float overshoot = 0.2f;
  [Tooltip("To make the leg move differently than the other legs")]
  [SerializeField] private bool isDelayed = false;

  private bool _isMoving = false;

  private Vector3 _groundPoint = Vector3.zero;
  private Vector3 _footPosition = Vector3.zero;

  // /************** HOOKS **************/
  private void Awake() {
    _footPosition = ikTarget.position;
  }

  private void Start() {
    // ikTarget.SetParent(IKManager.Instance.GetIKContainer());
    GroundLeg();
  }

  private void Update() {
    if (!_isMoving) {
      ikTarget.position = _footPosition;
    }
    // if (_isMoving) return;

    float distance = Vector3.Distance(ikTarget.position, homePos.position);

    if (!_isMoving && distance > stepDistance)
      StartCoroutine(Walk());
  }

  /************** PRIVATE **************/
  private void CalculateDestinationPoint() {
    RaycastHit hit;
    bool checkGround = Physics.Raycast(homePos.position + Vector3.up * 2f, Vector3.down, out hit, 10f, groundLayer);

    if (checkGround) {
      Vector3 movementDirection = (homePos.position + ikTarget.position).normalized;
      _groundPoint = hit.point + (movementDirection * overshoot);
      _groundPoint.y += .15f; // Foot size
    }
  }

  private IEnumerator Walk() {
    CalculateDestinationPoint();
    _isMoving = true;

    // Vector3 initPoint = ikTarget.position;
    Vector3 initPoint = _footPosition;
    Vector3 finalPoint = _groundPoint;
    float time = 0;

    if (isDelayed)
      yield return new WaitForSeconds(0f);
    else
      yield return new WaitForSeconds(.15f);

    while (time < 1) {
      time += Time.deltaTime / stepDuration;

      Vector3 newPos = Vector3.Lerp(initPoint, finalPoint, time);
      newPos.y += Mathf.Sin(time * Mathf.PI) * stepHeight;
      ikTarget.position = newPos;

      yield return null;
    }

    _footPosition = finalPoint;
    ikTarget.position = finalPoint;

    _isMoving = false;
  }

  private void GroundLeg() {
    CalculateDestinationPoint();
    _footPosition = _groundPoint;
    ikTarget.position = _groundPoint;
  }

  private void OnDrawGizmos() {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(_groundPoint, .1f);

    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(_footPosition, .1f);
  }
}
