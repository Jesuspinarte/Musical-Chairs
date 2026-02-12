using UnityEngine;

public class BaseController : MonoBehaviour {
  [Header("Player Base Settings")]
  [SerializeField] private EnumPlayerID playerOwner;
  [SerializeField] private Transform collectionPoint;

  private void OnTriggerEnter(Collider other) {
    if (other.transform.tag == "Kid") {
      KidController kid = other.transform.GetComponent<KidController>();

      if (kid == null) return;

      kid.MarkKidAsScore();
      AddScore(kid.GetKidScore());
    }
  }


  /************** PUBLIC **************/
  public void AddScore(int score) {
    GameManager.Instance.AddScore(playerOwner, score);
  }

  public Transform GetCollectionPoint() {
    return collectionPoint;
  }
}
