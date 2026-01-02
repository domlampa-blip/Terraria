using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For future UI implementation

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isGameOver = false;
    public float survivalTime = 0f;

    [Header("Game Stats")]
    public int killCount = 0;
    public int activeEnemies = 0;
    public int maxEnemies = 5;

    [Header("UI References")]
    public Text killCountText;
    public GameObject gameOverPanel;
    public Text finalStatsText; // Optional: To show stats on game over

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!isGameOver)
        {
            survivalTime += Time.deltaTime;
            UpdateUI();
        }

        // Restart logic (Simple R key for now)
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    void UpdateUI()
    {
        if (killCountText != null)
        {
            killCountText.text = "Kills: " + killCount;
        }
    }

    public void RegisterSpawn()
    {
        activeEnemies++;
    }

    public void RegisterKill()
    {
        killCount++;
        activeEnemies--; 
        // Safety clamp
        if (activeEnemies < 0) activeEnemies = 0;
    }

    public void EndGame()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("Game Over! Survived: " + survivalTime.ToString("F2") + "s | Kills: " + killCount);
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalStatsText != null)
            {
                finalStatsText.text = $"Survived: {survivalTime:F2}s\nKills: {killCount}";
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Temporary Debug UI if no Canvas is set up
    void OnGUI()
    {
        if (killCountText == null && !isGameOver)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(10, 10, 200, 50), "Kills: " + killCount, style);
            GUI.Label(new Rect(10, 40, 200, 50), "Enemies: " + activeEnemies + "/" + maxEnemies, style);
        }

        if (isGameOver && gameOverPanel == null)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 40;
            style.normal.textColor = Color.red;
            style.alignment = TextAnchor.MiddleCenter;
            
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), ""); // Dim background slightly
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100), "GAME OVER", style);
            
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 10, 300, 50), $"Survived: {survivalTime:F2}s | Kills: {killCount}", style);
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50, 300, 50), "Press 'R' to Restart", style);
        }
    }
}
