using System.Collections;
using UnityEngine;

public class KidController : MonoBehaviour {

    [Header("Collision Settings")]
    [Tooltip("Trigger to know if the Player collected the kid")]
    [SerializeField] private SphereCollider triggerCollider;
    [SerializeField] private SphereCollider collisionCollider;

    [Header("Kid Settings")]
    [SerializeField] private float destroyTimer = 5f;

    private Rigidbody _rb;
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

        if (other.transform.tag == "Player") {
            PlayerController player = other.transform.GetComponent<PlayerController>();

            if (player.HasSittingKid()) return;

            triggerCollider.enabled = false;
            collisionCollider.enabled = false;

            _chairOwner = player;
            player.SitKid(transform);
        }
    }

    /************** PRIVATE **************/
    private void SitOnChair() {
        if (_chairOwner == null) return;

        Transform followPoint = _chairOwner.GetSittingPoint();
        transform.position = followPoint.position;
        transform.rotation = followPoint.rotation;
    }

    private IEnumerator RemoveKidFromGame() {
        yield return new WaitForSeconds(destroyTimer);

        Destroy(gameObject);
    }

    /************** PUBLIC **************/
    public bool DetatchFromPlayer(Transform collectionPoint) {
        if (_chairOwner == null) return false;

        _chairOwner = null;
        collisionCollider.enabled = true;
        _rb.linearVelocity = Vector3.zero;
        transform.position = collectionPoint.position;

        StartCoroutine(RemoveKidFromGame());

        return true;
    }

    public void SetKidMass(float mass) {
        if (_rb ==  null) return;
        _rb.mass = mass;
    }

    public float GetKidMas() {
        return _rb.mass;
    }
}
