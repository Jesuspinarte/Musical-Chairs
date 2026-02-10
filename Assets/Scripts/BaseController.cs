using UnityEngine;

public class BaseController : MonoBehaviour
{
    [Header("Debug Settings")]
    public int score = 0;

    [Header("Player Base Settings")]
    [SerializeField] private PlayerId playerOwner;
    [SerializeField] private Transform collectionPoint;

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Player") {
            PlayerController player = other.transform.GetComponent<PlayerController>();

            if (player == null || player.GetPlayerID() != playerOwner) return;

            if (player.DropKid(collectionPoint)) ++score;
        }
    }
}
