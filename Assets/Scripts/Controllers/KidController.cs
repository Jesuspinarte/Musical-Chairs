using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidController : MonoBehaviour {

  [Header("Collision Settings")]
  [Tooltip("Trigger to know if the Player collected the kid")]
  [SerializeField] private SphereCollider triggerCollider;
  [SerializeField] private SphereCollider collisionCollider;

  [Header("Kid Settings")]
  [SerializeField] private List<SkinnedMeshRenderer> rendererList;

  [Header("Cooldown Settings")]
  [SerializeField] private float explosionForce = 150f;
  [SerializeField] private float explosionRadius = 2f;
  [SerializeField] private float upwardsForce = 3f;

  [Header("Effects")]
  [SerializeField] private GameObject scoreParticles;
  [SerializeField] private GameObject explosionParticles;

  private Rigidbody _rb;
  private bool _hasScored = false; // To prevent scoring twice or more
  private PlayerController _chairOwner = null; // The chair that collected the kid

  /************** HOOKS **************/

  private void OnEnable() {
    TimeManager.OnCooldownChange += OnCooldownChange;
  }

  private void OnDisable() {
    TimeManager.OnCooldownChange -= OnCooldownChange;
  }

  private void Awake() {
    _rb = GetComponent<Rigidbody>();
  }

  private void Update() {
    SitOnChair();
    DestroyWhenOutOfBoundaries();
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
      ScoreKid(baseController.GetCollectionPoint(), baseController.GetBasePlayerOwner());
    }
  }

  /************** PRIVATE **************/
  private void DestroyWhenOutOfBoundaries() {
    Vector3 pos = transform.position;
    if (pos.x > 50 || pos.x < -50) Destroy(gameObject);
    if (pos.y > 50 || pos.y < -50) Destroy(gameObject);
    if (pos.z > 50 || pos.z < -50) Destroy(gameObject);
  }

  private void OnCooldownChange() {
    if (!TimeManager.Instance.IsCooldownTime()) return;

    Instantiate(explosionParticles, transform.position, Quaternion.identity);

    float randomX = Random.Range(-1f, 1f);
    float randomZ = Random.Range(-1f, 1f);

    Vector3 randomDirection = new Vector3(randomX, 1f, randomZ).normalized;
    _rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsForce, ForceMode.Impulse);
    _rb.AddForce(randomDirection * explosionForce * _rb.mass, ForceMode.Impulse); // Shoot
    _rb.AddTorque(Random.insideUnitSphere * explosionForce, ForceMode.Impulse); // Rotate
  }

  private void SitOnChair() {
    if (GetComponent<FixedJoint>()) return; // if it's affected by magnet, do not sit
    if (_chairOwner == null) return;

    Transform followPoint = _chairOwner.GetSittingPoint();
    transform.position = followPoint.position;
    transform.rotation = followPoint.rotation;
  }

  /************** PUBLIC **************/
  public KidController ScoreKid(Transform collectionPoint, EnumPlayerID playerID) {
    DetachFromPlayer();
    Vector3 particlesPos = transform.position;
    particlesPos.y -= 1;
    Instantiate(scoreParticles, particlesPos, Quaternion.identity);

    TextManager.Instance.DisplayKidScore(transform.position, GetKidScore().ToString(), playerID);

    triggerCollider.enabled = false;
    transform.position = collectionPoint.position;
    // TODO: Maybe play some particles here
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
