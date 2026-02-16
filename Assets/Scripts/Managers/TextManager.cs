using PixelBattleText;
using UnityEngine;

public class TextManager : MonoBehaviour {
  private static TextManager _instance;

  public static TextManager Instance {
    get {
      if (_instance == null) {
        _instance = FindFirstObjectByType<TextManager>();

        if (_instance == null) {
          GameObject go = new GameObject("TextManager");
          _instance = go.AddComponent<TextManager>();
        }
      }
      return _instance;
    }
  }

  /************** SERIALIZED **************/
  [Header("Animation References")]
  [SerializeField] private TextAnimation scoreAnimation;
  [SerializeField] private TextAnimation winAnination;
  [SerializeField] private TextAnimation cooldownAnimation;
  [SerializeField] private TextAnimation playAnimation;
  [SerializeField] private TextAnimation respawnAnimation;

  [Header("Player Score Settings")]
  [SerializeField] private Vector3 heightOffsetP1 = new Vector3(0, 2f, 0);
  [SerializeField] private Vector3 heightOffsetP2 = new Vector3(0, 2f, 0);

  [Header("Player Respawn Settings")]
  [SerializeField] private Vector2 respawnTextPointP1 = Vector2.zero;
  [SerializeField] private Vector2 respawnTextPointP2 = Vector2.zero;

  /************** PUBLIC **************/
  public void DisplayKidScore(Vector3 worldPosition, string score, EnumPlayerID playerID) {
    Vector3 heightOffset = playerID == EnumPlayerID.PLAYER1 ? heightOffsetP1 : heightOffsetP2;

    Vector3 finalWorldPos = worldPosition + heightOffset;
    Vector3 viewportPoint = Camera.main.WorldToViewportPoint(finalWorldPos);
    Vector2 canvasPos = new Vector2(viewportPoint.x, viewportPoint.y);

    if (viewportPoint.z > 0 && canvasPos.x > 0 && canvasPos.x < 1 && canvasPos.y > 0 && canvasPos.y < 1) {
      PixelBattleTextController.DisplayText(score, scoreAnimation, canvasPos);
    }
  }

  public void DisplayCooldownText(string cooldownText) {
    PixelBattleTextController.DisplayText(cooldownText, cooldownAnimation, new Vector2(0.5f, 0.7f));
  }

  public void DisplayPlayText(string playText) {
    PixelBattleTextController.DisplayText(playText, playAnimation, new Vector2(0.5f, 0.7f));
  }

  public void DisplayWinText(string winText) {
    PixelBattleTextController.DisplayText(winText, winAnination, new Vector2(0.5f, 0.7f));
  }

  public void DisplayRespanwText(EnumPlayerID playerID) {
    if (playerID == EnumPlayerID.PLAYER1)
      PixelBattleTextController.DisplayText("PLAYER 1 RESPAWNING", respawnAnimation, respawnTextPointP1);
    else
      PixelBattleTextController.DisplayText("PLAYER 2 RESPAWNING", respawnAnimation, respawnTextPointP2);
  }
}
