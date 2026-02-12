using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidController : MonoBehaviour {

  [Header("Collision Settings")]
  [Tooltip("Trigger to know if the Player collected the kid")]
  [SerializeField] private SphereCollider triggerCollider;
  [SerializeField] private SphereCollider collisionCollider;

  [Header("Kid Settings")]
  [SerializeField] private float destroyTimer = 5f;
  [SerializeField] private List<SkinnedMeshRenderer> rendererList;

  private Rigidbody _rb;
  private bool _hasScored = false; // To prevent scoring twice or more
  private PlayerController _chairOwner = null; // The chair that collected the kid

  /************** HOOKS **************/

  private void Awake() {
    _rb = GetComponent<Rigidbody>();
  }

  private void Update() {
    SitOnChair();
  }

  private void OnTriggerEnter(Collider other) {
    if (triggerCollider == null) return;

    // Sit the kid on the chair
    if (other.transform.tag == "Player" && _chairOwner == null) {
      PlayerController player = other.transform.GetComponent<PlayerController>();

      if (player.HasSittingKid()) return;

      collisionCollider.enabled = false;

      _chairOwner = player;
      player.SitKid(transform);
    }

    if (other.transform.tag == "Base" && !_hasScored) {
      MarkKidAsScore();
      BaseController baseController = other.transform.GetComponent<BaseController>();

      baseController.AddScore(GetKidScore());
      ScoreKid(baseController.GetCollectionPoint());
    }
  }

  /************** PRIVATE **************/
  private void SitOnChair() {
    if (GetComponent<FixedJoint>()) return; // if it's affected by magnet, do not sit
    if (_chairOwner == null) return;

    Transform followPoint = _chairOwner.GetSittingPoint();
    transform.position = followPoint.position;
    transform.rotation = followPoint.rotation;
  }

  // TODO: Maybe play some particles here
  private IEnumerator RemoveKidFromGame() {
    yield return new WaitForSeconds(destroyTimer);

    Destroy(gameObject);
  }

  /************** PUBLIC **************/
  public KidController ScoreKid(Transform collectionPoint) {
    DetachFromPlayer();

    triggerCollider.enabled = false;
    transform.position = collectionPoint.position;

    // TODO: Maybe play some particles here
    // StartCoroutine(RemoveKidFromGame());
    Destroy(gameObject);

    return GetComponent<KidController>();
  }

  public KidController DetachFromPlayer() {
    if (_chairOwner != null) _chairOwner.DetachKid();

    _chairOwner = null;
    collisionCollider.enabled = true;
    _rb.linearVelocity = Vector3.zero;
    transform.position += Vector3.up;

    return GetComponent<KidController>();
  }

  public void SetKidMass(float mass) {
    if (_rb == null) return;
    _rb.mass = mass;
  }

  public int GetKidScore() {
    return (int)Mathf.Ceil(GetKidMass());
  }

  public float GetKidMass() {
    return _rb.mass;
  }

  public void MarkKidAsScore() {
    _hasScored = true;
    FixedJoint joint = GetComponent<FixedJoint>();
    if (joint != null) Destroy(joint);
  }

  public void UpdateKidColor(Color newColor) {
    if (rendererList == null || rendererList.Count == 0) return;

    foreach (Renderer renderer in rendererList) {
      renderer.material.color = newColor;
    }
  }

  public bool HasScored() {
    return _hasScored;
  }
}
