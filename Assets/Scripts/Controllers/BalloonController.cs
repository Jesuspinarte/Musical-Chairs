using UnityEngine;

public class BalloonController : MonoBehaviour {
  private void OnTriggerEnter(Collider other) {
    if (other.transform.tag == "Player") {
      PowerController player = other.transform.GetComponent<PowerController>();

      player.SetPowerToUse();
      Destroy(gameObject);
    }
  }
}
