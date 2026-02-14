using UnityEngine;

public class BoundaryController : MonoBehaviour {
  /************** HOOKS **************/
  void OnTriggerEnter(Collider other) {
    if (other.gameObject.tag == "Player") {
      PlayerController player = other.transform.GetComponent<PlayerController>();
      player.RespawnPlayer();
    } else if (other.transform.tag == "Balloon") {
      Destroy(other.gameObject);
    } else if (other.transform.tag == "Kid") {
      if (other.transform.parent == null) Destroy(other.gameObject);
      else Destroy(other.transform.parent.gameObject);
    }
  }
}
