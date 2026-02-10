using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    [Header("Player Settings")]
    [SerializeField] PlayerId playerId;
    [SerializeField] InputActionReference moveAction;

    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float rotationSpeed = 0.15f;

    [Header("Collectable Settings")]
    [SerializeField] private Transform childPosition; // Where the child will be when it's collected

    private Vector2 _movementInput = Vector3.zero;
    private Rigidbody _rb;

    private Transform _collecteKid = null;

    /************** HOOKS **************/

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
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
        if (_movementInput == null) return;
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
    }

}
