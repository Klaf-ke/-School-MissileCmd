using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
  [Header("Game objects")]
  [SerializeField] private Transform character;
  [SerializeField] private Transform characterModel;
  [SerializeField] private Transform terrainHolder;
  
  [Header("UI Elements")]
  [SerializeField] private TMPro.TextMeshProUGUI scoreText; // Gameplay score
  [SerializeField] private TMPro.TextMeshProUGUI finalScoreText; // End game score
  [SerializeField] private TMPro.TextMeshProUGUI highScoreText; // End game high score
  public GameObject GameOverTxt;

  [Header("Terrain objects")]
  [SerializeField] private Grass grassPrefab;
  [SerializeField] private Road roadPrefab;

  [Header("Game parameters")]
  [SerializeField] private float moveDuration = 0.2f;
  [SerializeField] private int spawnDistance = 20;
  
  [Header("Juice / Effects")]
  [SerializeField] private float punchScaleAmount = 1.3f; // Scale multiplier during pop
  [SerializeField] private float popDuration = 0.15f;    // How long the pop takes

  enum GameState {
    Ready,
    Moving,
    Dead
  }
  private GameState gameState;
  private Vector2Int characterPos;
  private int spawnLocation;
  private List<(float terrainHeight, HashSet<int> locations, GameObject obj)> obstacles = new();
  
  private int score = 0;
  private int highScore = 0;
  private Vector3 originalScoreScale; // Stores the starting scale of the score text
  private Coroutine scorePopCoroutine;

  void Awake() {
    highScore = PlayerPrefs.GetInt("HighScore", 0);
    
    // Remember the baseline scale of the score UI text
    if (scoreText != null) {
      originalScoreScale = scoreText.transform.localScale;
    }

    NewLevel();
    GameOverTxt.SetActive(false);
  }

  private void NewLevel() {
    gameState = GameState.Ready;
    
    if (AudioManager.Instance != null) {
        AudioManager.Instance.PlayMusic();
    }

   finalScoreText.text = "";
    highScoreText.text = "";

    characterPos = new Vector2Int(0, -1);
    character.position = new Vector3(0, 0.2f, -1);
    character.GetComponent<Character>().Reset();

    score = 0;
    scoreText.text = "0";
    scoreText.transform.localScale = originalScoreScale; // Reset scale just in case
    scoreText.gameObject.SetActive(true);

    obstacles.Clear();
    foreach (Transform child in terrainHolder) {
      Destroy(child.gameObject);
    }

    spawnLocation = 0;
    for (int i = 0; i < spawnDistance; i++) {
      SpawnObstacle();
    }
  }

  private void SpawnObstacle() {
    float roadProbability = Mathf.Lerp(0.5f, 0.9f, spawnLocation / 250f);

    if (Random.value < roadProbability) {
      Road road = Instantiate(roadPrefab, terrainHolder);
      obstacles.Add((0.1f, road.Init(spawnLocation), road.gameObject));
      road.gameObject.name = $"{spawnLocation} - Road";
    } else {
      Grass grass = Instantiate(grassPrefab, terrainHolder);
      obstacles.Add((0.2f, grass.Init(spawnLocation), grass.gameObject));
      grass.gameObject.name = $"{spawnLocation} - Grass";
    }

    spawnLocation++;
  }

  private bool InStartArea(Vector2Int location) {
    if ((location.y > -5) && (location.y < 0) && (location.x > -6) && (location.x < 6)) {
      return true;
    }
    return false;
  }

  void Update() {
    if (gameState == GameState.Ready) {
      Vector2Int moveDirection = Vector2Int.zero;
      if (Keyboard.current.upArrowKey.wasPressedThisFrame) {
        character.localRotation = Quaternion.identity;
        moveDirection.y = 1;
      } else if (Keyboard.current.downArrowKey.wasPressedThisFrame) {
        character.localRotation = Quaternion.Euler(0, 180, 0);
        moveDirection.y = -1;
      } else if (Keyboard.current.leftArrowKey.wasPressedThisFrame) {
        character.localRotation = Quaternion.Euler(0, -90, 0);
        moveDirection.x = -1;
      } else if (Keyboard.current.rightArrowKey.wasPressedThisFrame) {
        character.localRotation = Quaternion.Euler(0, 90, 0);
        moveDirection.x = 1;
      }

      if (moveDirection != Vector2Int.zero) {
        Vector2Int destination = characterPos + moveDirection;
        if (InStartArea(destination) || ((destination.y >= 0) && !obstacles[destination.y].locations.Contains(destination.x))) {
          characterPos = destination;
          StartCoroutine(MoveCharacter());

          if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayJump();
          }

          // Score increase check
          if ((destination.y + 1) > score) {
            score = destination.y + 1;
            scoreText.text = $"{score}";
            
            // Trigger the UI Pop effect
            if (scorePopCoroutine != null) StopCoroutine(scorePopCoroutine);
            scorePopCoroutine = StartCoroutine(PopScoreText());
          }
        }

        while (obstacles.Count < (characterPos.y + spawnDistance)) {
          SpawnObstacle();

          int oldIndex = characterPos.y - spawnDistance;
          if ((oldIndex >= 0) && (obstacles[oldIndex].obj != null)) {
            Destroy(obstacles[oldIndex].obj);
          }
        }

        if (characterPos.y < (score - 10)) {
          character.GetComponent<Character>().Kill(character.transform.position + new Vector3(0, 0.2f, 0.5f));
        }
      }
    }

    if (gameState == GameState.Dead && Keyboard.current.rKey.wasPressedThisFrame) {
      GameOverTxt.SetActive(false);
      NewLevel();
    }

    Vector3 cameraPosition = new(character.position.x + 2, 4, character.position.z - 3);
    cameraPosition.x = Mathf.Clamp(cameraPosition.x, -1, 5);
    Camera.main.transform.position = cameraPosition;
  }

  private IEnumerator MoveCharacter() {
    gameState = GameState.Moving;
    float elapsedTime = 0f;

    float yHeight = 0.2f;
    if (characterPos.y >= 0) {
      yHeight = obstacles[characterPos.y].terrainHeight;
    }

    Vector3 startPos = character.position;
    Vector3 endPos = new(characterPos.x, yHeight, characterPos.y);

    Quaternion startRotation = characterModel.localRotation;

    while (elapsedTime < moveDuration) {
      float percent = elapsedTime / moveDuration;
      Vector3 newPos = Vector3.Lerp(startPos, endPos, percent);
      newPos.y = yHeight + (0.5f * Mathf.Sin(Mathf.PI * percent));
      character.position = newPos;

      Vector3 rotation = characterModel.localRotation.eulerAngles;
      characterModel.localRotation = Quaternion.Euler(-5f * Mathf.PI * Mathf.Cos(Mathf.PI * percent), rotation.y, rotation.z);

      elapsedTime += Time.deltaTime;
      yield return null;
    }

    character.position = endPos;
    characterModel.localRotation = startRotation;

    if (gameState == GameState.Moving) {
      gameState = GameState.Ready;
    }
  }

  // Coroutine to animate the score text size
  private IEnumerator PopScoreText() {
    float halfDuration = popDuration / 2f;
    Vector3 targetScale = originalScoreScale * punchScaleAmount;

    // Scale Up
    float elapsed = 0f;
    while (elapsed < halfDuration) {
      elapsed += Time.deltaTime;
      scoreText.transform.localScale = Vector3.Lerp(originalScoreScale, targetScale, elapsed / halfDuration);
      yield return null;
    }

    // Scale Down back to normal
    elapsed = 0f;
    while (elapsed < halfDuration) {
      elapsed += Time.deltaTime;
      scoreText.transform.localScale = Vector3.Lerp(targetScale, originalScoreScale, elapsed / halfDuration);
      yield return null;
    }

    scoreText.transform.localScale = originalScoreScale;
  }

  public void PlayerCollision() {
    gameState = GameState.Dead;

    if (AudioManager.Instance != null) {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayGameOver();
    }

    if (score > highScore) {
      highScore = score;
      PlayerPrefs.SetInt("HighScore", highScore);
      PlayerPrefs.Save();
    }

    // Stop any ongoing pop coroutines before hiding
    if (scorePopCoroutine != null) StopCoroutine(scorePopCoroutine);

    scoreText.gameObject.SetActive(false);

    finalScoreText.text = $"Score: {score}";
    highScoreText.text = $"High Score: {highScore}";

    GameOverTxt.SetActive(true);
  }
}