using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
  private static GameManager _instance;

  [Header("Text References")]
  [SerializeField] private TextMeshProUGUI player1ScoreText;
  [SerializeField] private TextMeshProUGUI player2ScoreText;

  [Header("PowerManager")]
  [SerializeField] private float timeToGetPower = 0.5f;

  /************** HOOKS **************/

  public static GameManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<GameManager>();
        if (_instance == null) {
          GameObject go = new GameObject("GameManager");
          _instance = go.AddComponent<GameManager>();
        }
      }
      return _instance;
    }
  }

  /************** PUBLIC **************/
  public void SetPlayerScoreText(EnumPlayerID playerID, int score) {

    switch (playerID) {
      case EnumPlayerID.PLAYER1:
        if (player1ScoreText == null) return;
        player1ScoreText.text = score.ToString();

        break;

      case EnumPlayerID.PLAYER2:
        if (player2ScoreText == null) return;
        player2ScoreText.text = score.ToString();

        break;

      default:
        break;
    }
  }

  public float GetTimeToGetPower() {
    return timeToGetPower;
  }
}
