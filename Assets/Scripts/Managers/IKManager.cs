using UnityEngine;

public class IKManager : MonoBehaviour {
  private static IKManager _instance;

  public static IKManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<IKManager>();

        if (_instance == null) {
          GameObject go = new GameObject("IKManager");
          _instance = go.AddComponent<IKManager>();
        }
      }
      return _instance;
    }
  }

  /************** PUBLIC **************/
  public Transform GetIKContainer() {
    return transform;
  }
}
