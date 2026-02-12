using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MagnetAbility : MonoBehaviour {
  [Header("Magnet References")]
  [SerializeField] private Transform magnetPoint;
  [SerializeField] private LayerMask kidsLayer;

  [Header("Magnet Settings")]
  [SerializeField] private float magnetRadius = 10f;
  [SerializeField] private float attractionForce = 20f;
  [SerializeField] private float stickyDistance = 1.5f;

  private bool isActive = false;
  private int maxCapacity = 10;
  private List<GameObject> kidsList;

  private Rigidbody _rb;

  /************** HOOKS **************/
  private void Awake() {
    _rb = GetComponent<Rigidbody>();
    kidsList = new List<GameObject>();
  }

  private void FixedUpdate() {
    GrabKids();
    DropKids();
  }

  /************** PRIVATE **************/

  private void GrabKids() {
    if (!isActive) return;
    // Clean list of scored childs first

    List<GameObject> tempKidList = new List<GameObject>();

    foreach (GameObject kid in kidsList) {
      if (kid == null) continue; // If kid was scored
      if (kid.GetComponent<FixedJoint>() == null) continue; // If kid was scored
      if (kid.GetComponent<KidController>().HasScored()) continue; // If kid was scored
      tempKidList.Add(kid);
    }

    kidsList = tempKidList;

    if (kidsList.Count >= maxCapacity) return;

    Collider[] kidsColliders = Physics.OverlapSphere(transform.position, magnetRadius, kidsLayer);

    foreach (Collider kidCollider in kidsColliders) {
      GameObject kid = kidCollider.transform.parent.gameObject;

      if (kidsList.Contains(kid)) continue;
      if (kidsList.Count >= maxCapacity) break;

      float distance = Vector3.Distance(transform.position, kid.transform.position);

      if (distance > stickyDistance) {
        Vector3 direction = (transform.position - kid.transform.position).normalized;
        Rigidbody rbKid = kid.GetComponent<Rigidbody>(); // Should be the parent

        rbKid.AddForce(direction * attractionForce, ForceMode.Acceleration);
      }
      else {
        StickKid(kid);
      }
    }
  }

  private void StickKid(GameObject kid) {
    Rigidbody rbKid = kid.GetComponent<Rigidbody>();

    FixedJoint joint = kid.AddComponent<FixedJoint>();
    joint.connectedBody = _rb;
    rbKid.linearDamping = 10f; // TEST
    kidsList.Add(kid);
  }

  private void DropKids() {
    if (isActive || kidsList.Count <= 0) return;

    foreach (GameObject kid in kidsList) {
      if (kid == null) continue; // If kid was scored

      FixedJoint joint = kid.GetComponent<FixedJoint>();
      Destroy(joint);
    }

    kidsList.Clear();
  }

  /************** PUBLIC **************/
  public void SetMaxCapacity(int newMaxCapacity) {
    maxCapacity = newMaxCapacity;
  }

  public void EnableMagnet() {
    isActive = true;
  }

  public void DisableMagnet() {
    isActive = false;
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = Color.magenta;
    Gizmos.DrawWireSphere(transform.position, magnetRadius);
  }
}
