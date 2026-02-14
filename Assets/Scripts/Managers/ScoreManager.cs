using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
  private static ScoreManager _instance;

  [Header("Player 1")]
  [SerializeField] private TextMeshProUGUI player1ScoreText;

  [Header("Player 2")]
  [SerializeField] private TextMeshProUGUI player2ScoreText;

  private int _scoreP1 = 0;
  private int _scoreP2 = 0;

  /************** HOOKS **************/
  public static ScoreManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<ScoreManager>();
        if (_instance == null) {
          GameObject go = new GameObject("ScoreManager");
          _instance = go.AddComponent<ScoreManager>();
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

  public int AddScore(EnumPlayerID playerID, int score) {
    switch (playerID) {
      case EnumPlayerID.PLAYER1:
        _scoreP1 += score;
        SetPlayerScoreText(playerID, _scoreP1);

        return _scoreP2;
      case EnumPlayerID.PLAYER2:
        _scoreP2 += score;
        SetPlayerScoreText(playerID, _scoreP2);

        return _scoreP2;
      default:
        return 0;
    }
  }

  public int GetPlayer1Score() {
    return _scoreP1;
  }

  public int GetPlayer2Score() {
    return _scoreP1;
  }

  public EnumPlayerID? GetWinningPlayer() {
    if (_scoreP1 == _scoreP2) return null;
    if (_scoreP1 > _scoreP2) return EnumPlayerID.PLAYER1;
    return EnumPlayerID.PLAYER2;
  }
}
