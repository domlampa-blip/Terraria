using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For future UI implementation

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isGameOver = false;
    public float survivalTime = 0f;
    
    // UI References (assign in Inspector later)
    // public Text timeText; 
    // public GameObject gameOverPanel;

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
            // UpdateUI(); // Implement later
        }

        // Restart logic (Simple R key for now)
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void EndGame()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("Game Over! Survived: " + survivalTime.ToString("F2") + "s");
        // Show GameOver UI
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
