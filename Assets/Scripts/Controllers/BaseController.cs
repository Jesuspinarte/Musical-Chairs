using UnityEngine;

public class BaseController : MonoBehaviour {
  [Header("Debug Settings")]
  public int score = 0;

  [Header("Player Base Settings")]
  [SerializeField] private EnumPlayerID playerOwner;
  [SerializeField] private Transform collectionPoint;

  private void OnTriggerEnter(Collider other) {
    if (other.transform.tag == "Player") {
      PlayerController player = other.transform.GetComponent<PlayerController>();

      if (player == null || player.GetPlayerID() != playerOwner) return;

      KidController kid = player.DropKid(collectionPoint);

      if (kid != null) {
        score += (int)Mathf.Ceil(kid.GetKidMass());
        GameManager.Instance.SetPlayerScoreText(playerOwner, score);
      }
    }
  }
}
