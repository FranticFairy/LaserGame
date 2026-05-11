using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("References")]
    public LaserTarget[] targets;       // Assign all targets in Inspector
    public Crystal[] crystals;          // Assign all crystals in Inspector
    public GameObject levelClearedUI;   // Assign your Level Cleared panel

    private bool levelComplete = false;

    private void Awake()
    {
        Instance = this;
        levelClearedUI.SetActive(false);
    }

    public void CheckWinCondition()
    {
        if (levelComplete) return;

        // All targets must be activated
        foreach (LaserTarget target in targets)
        {
            if (!target.isActivated) return;
        }

        // At least one crystal must be active (i.e. hit this frame)
        bool anyCrystalHit = false;
        foreach (Crystal crystal in crystals)
        {
            if (crystal.IsHitThisFrame)
            {
                anyCrystalHit = true;
                break;
            }
        }

        if (!anyCrystalHit) return;

        levelComplete = true;
        levelClearedUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}