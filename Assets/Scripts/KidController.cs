using UnityEngine;

public class KidController : MonoBehaviour {

    [Header("Collision Settings")]
    [Tooltip("Trigger to know if the Player collected the kid")]
    [SerializeField] private SphereCollider triggerCollider;
    [SerializeField] private SphereCollider collisionCollider;

    private PlayerController _chairOwner = null; // The chair that collected the kid

    /************** HOOKS **************/

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

}
