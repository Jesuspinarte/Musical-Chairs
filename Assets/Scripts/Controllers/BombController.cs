using UnityEngine;

public class BombController : MonoBehaviour {
  /************** SERIALIZED **************/
  [Header("Bomb Settings")]
  [SerializeField] private float explosionRadius = 1.5f;
  [SerializeField] private float explosionForce = 10f;
  [SerializeField] private float upwardsForce = 3f;
  [SerializeField] private GameObject explotionParticles;

  /************** HOOKS **************/
  private void OnCollisionEnter(Collision collision) {
    Explode();
  }

  /************** PRIVATE **************/
  private void Explode() {
    if (explotionParticles != null) {
      Instantiate(explotionParticles, transform.position, Quaternion.identity);
    }

    Collider[] collisionList = Physics.OverlapSphere(transform.position, explosionRadius);

    foreach (Collider go in collisionList) {
      Rigidbody goRb = go.GetComponent<Rigidbody>();

      Debug.Log($"Collided with: {go.name}");

      if (goRb == null) {
        if (go.transform.parent == null) continue;

        goRb = go.transform.parent.GetComponent<Rigidbody>(); // For the child
        if (goRb == null) continue;
      }
      goRb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsForce, ForceMode.Impulse);
    }

    Destroy(gameObject);
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = Color.magenta;
    Gizmos.DrawWireSphere(transform.position, explosionRadius);
  }
}
