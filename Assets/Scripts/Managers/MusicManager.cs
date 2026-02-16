using UnityEngine;

using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour {
  private static MusicManager _instance;

  public static MusicManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<MusicManager>();
        if (_instance == null) {
          GameObject go = new GameObject("MusicManager");
          _instance = go.AddComponent<MusicManager>();
        }
      }
      return _instance;
    }
  }

  /************** SERIALIZED **************/
  [Header("FMOD Settings")]
  public EventReference musicEvent;
  private EventInstance musicInstance;

  /************** HOOKS **************/
  private void Start() {
    musicInstance = RuntimeManager.CreateInstance(musicEvent);
    musicInstance.start();
    ChangeMusic("Cooldown");
  }

  private void OnDestroy() {
    musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    musicInstance.release();
  }

  /************** PUBLIC **************/
  public void ChangeMusic(string newState) {
    // musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    musicInstance.setParameterByNameWithLabel("Game_State", newState);
    // musicInstance.start();
    Debug.Log($"MusicManager: Changing to: {newState}");
  }
}
