using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

  [Header("Player Settings")]
  [SerializeField] EnumPlayerID playerId;
  [SerializeField] InputActionReference moveAction;

  [Header("Movement Settings")]
  [SerializeField] private float acceleration = 50f;
  [SerializeField] private float maxMovementSpeed = 10f;
  [SerializeField] private float rotationSpeed = 0.15f;
  [SerializeField] private float dampingModifier = .65f;

  [Header("Collectable Settings")]
  [SerializeField] private Transform childPosition; // Where the child will be when it's collected

  private Vector2 _movementInput = Vector3.zero;
  private Transform _collecteKid = null;
  private Rigidbody _rb;
  private MagnetAbility _magnetAbility;

  // Initial values
  private int _maxCapacity = 1;
  private int _initialMaxCapacity = 1;
  private float _initialMass = 1f;
  private float _initialAcceleration;
  private float _forceMultiplier = 1f;
  private float _initialMaxMovementSpeed;
  private float _initialLinearDamping = 0f;
  private Vector3 _initialPosition;
  private bool _isDead = false;

  /************** HOOKS **************/

  private void Awake() {
    _rb = GetComponent<Rigidbody>();
    _magnetAbility = GetComponent<MagnetAbility>();

    _initialMaxCapacity = 1;
    _initialAcceleration = acceleration;
    _initialLinearDamping = _rb.linearDamping;
    _forceMultiplier = 1f;
    _initialMaxMovementSpeed = maxMovementSpeed;
    _initialPosition = transform.position;
  }

  private void OnEnable() {
    if (moveAction != null) moveAction.action.Enable();
  }

  private void OnDisable() {
    if (moveAction != null) moveAction.action.Disable();
  }

  private void Update() {
    GetMovement();
  }

  private void FixedUpdate() {
    if (_isDead) return;

    Vector3 flatVelocity;
    Vector3 newDirection = new Vector3(_movementInput.x, 0f, _movementInput.y).normalized;

    if (newDirection.magnitude > .1f) {
      _rb.AddForce(newDirection * acceleration, ForceMode.Acceleration);
    }

    flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

    if (flatVelocity.magnitude > maxMovementSpeed) {
      Vector3 limitedVel = flatVelocity.normalized * maxMovementSpeed;
      _rb.linearVelocity = new Vector3(limitedVel.x, _rb.linearVelocity.y, limitedVel.z);
    }

    if (flatVelocity.magnitude > .5f) {
      transform.forward = Vector3.Slerp(transform.forward, flatVelocity.normalized, rotationSpeed);
    }
  }

  /************** PRIVATE **************/

  private void GetMovement() {
    if (_movementInput == null || _isDead) return;
    _movementInput = moveAction.action.ReadValue<Vector2>();
  }

  /************** PUBLIC **************/
  public Transform GetSittingPoint() {
    if (childPosition == null) return null;
    return childPosition;
  }

  public bool HasSittingKid() {
    return _collecteKid != null;
  }

  public void SitKid(Transform kid) {
    _collecteKid = kid;
    _rb.linearDamping = kid.GetComponent<KidController>().GetKidMass() * dampingModifier * _forceMultiplier;
  }

  public EnumPlayerID GetPlayerID() {
    return playerId;
  }

  public KidController DropKid() {
    if (_collecteKid == null) return null;
    KidController kid = _collecteKid.GetComponent<KidController>();
    KidController kidController = kid.DetachFromPlayer();

    DetachKid();

    return kidController;
  }

  public void DetachKid() {
    _collecteKid = null;
    _rb.linearDamping = _initialLinearDamping;
  }

  public void ResetPowerProperties() {
    _maxCapacity = _initialMaxCapacity;
    _rb.linearDamping = _rb.linearDamping / _forceMultiplier; // Resets force

    acceleration = _initialAcceleration;
    maxMovementSpeed = _initialMaxMovementSpeed;
    _forceMultiplier = 1f;
  }

  // Strength ability
  public void BeStronger(float newForceMultiplier) {
    if (newForceMultiplier == 0) {
      Debug.LogError("ForceMultiplier CANNOT be ZERO (0)");
      return;
    }

    _forceMultiplier = newForceMultiplier;
    _rb.linearDamping = _rb.linearDamping * _forceMultiplier; // Changes force
  }

  // Speed ability
  public void BeFaster(float newMaxMovementSpeed, float newAccelerationValue) {
    maxMovementSpeed = newMaxMovementSpeed;
    acceleration = newAccelerationValue;
  }

  // Start Magnet ability
  public void BeGreedy(int newMaxCapacity) {
    _maxCapacity = newMaxCapacity;

    _initialMass = _rb.mass;
    _rb.mass = 500f;
    _magnetAbility.SetMaxCapacity(_maxCapacity);
    _magnetAbility.EnableMagnet();
  }

  // Stop Magnet ability
  public void StopBeingGreedy() {
    _rb.mass = _initialMass;
    _maxCapacity = 1;
    _magnetAbility.DisableMagnet();
  }

  // Launch Bomb
  public void BeMad(GameObject bombPrefab) {
    Vector3 spawnPosition = transform.position + transform.forward * 2f + Vector3.up * 1f;

    GameObject bomb = Instantiate(bombPrefab, spawnPosition, transform.rotation);
    Rigidbody rbBomb = bomb.GetComponent<Rigidbody>();

    if (rbBomb == null) return;

    Vector3 launchDirection = (transform.forward + Vector3.up * PowerManager.Instance.GetLaunchAngle()).normalized;
    rbBomb.AddForce(launchDirection * PowerManager.Instance.GetLaunchForce(), ForceMode.VelocityChange);
  }

  public void RespawnPlayer() {
    if (!_isDead)
      StartCoroutine(StartRespawnProcess());
  }

  /************** COROUTINES **************/
  private IEnumerator StartRespawnProcess() {
    if (_isDead) yield return null;

    _isDead = true;
    ResetPowerProperties();
    DropKid();

    yield return new WaitForSeconds(GameManager.Instance.GetTimeToRespawn());

    _rb.linearVelocity = Vector3.zero;
    transform.position = _initialPosition + Vector3.up;
    transform.rotation = Quaternion.identity;
    _isDead = false;
  }
}
