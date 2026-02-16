using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class IntroController : MonoBehaviour {
  [SerializeField] private string nextScene = "MusicalPeople";
  [SerializeField] InputActionReference skipAction;

  private void OnEnable() {
    if (skipAction != null) skipAction.action.Enable();
  }

  private void OnDisable() {
    if (skipAction != null) skipAction.action.Disable();
  }

  private void Update() {
    if (skipAction.action.ReadValue<float>() != 0f)
      OnSkip();
  }

  private void OnSkip() {
    SceneManager.LoadScene(nextScene);
  }
}

