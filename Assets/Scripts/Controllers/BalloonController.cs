using UnityEngine;

public class BalloonController : MonoBehaviour {
  [Header("Effects")]
  [SerializeField] private GameObject collectParticles;

  /************** HOOKS **************/
  private void OnTriggerEnter(Collider other) {
    if (other.transform.tag == "Player") {
      PowerController player = other.transform.GetComponent<PowerController>();

      Instantiate(collectParticles, transform.position, Quaternion.identity);
      player.SetPowerToUse();
      Destroy(gameObject);
    }
  }

  private void Update() {
    DestroyWhenOutOfBoundaries();
  }

  /************** PRIVATE **************/
  private void DestroyWhenOutOfBoundaries() {
    Vector3 pos = transform.position;
    if (pos.x > 50 || pos.x < -50) Destroy(gameObject);
    if (pos.y > 50 || pos.y < -50) Destroy(gameObject);
    if (pos.z > 50 || pos.z < -50) Destroy(gameObject);
  }
}
