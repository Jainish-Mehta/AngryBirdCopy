using UnityEngine;
using UnityEngine.SceneManagement;

public  class LevelController : MonoBehaviour
{
    private static int _levelIndex = 1;
    private Enemy[] _enemies;
    private int count = 1;
    private void OnEnable()
    {
        _enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

    }
    void Update()
    {
        foreach (Enemy enemy in _enemies)
        {
            if (enemy != null)
            {
                return;
            }

            if (count == 1)
            {
                Debug.Log("All enemies have been killed!");
                count = 0; // Reset count to prevent multiple logs
                _levelIndex++;
                string nextSceneName = "Level" + _levelIndex.ToString();
                if (Application.CanStreamedLevelBeLoaded(nextSceneName))
                {
                    SceneManager.LoadScene(nextSceneName);
                }
                else
                {
                    Debug.Log("No more levels available. Returning to main menu.");
                    SceneManager.LoadScene("SampleScene"); // Load main menu or a different scene
                }
            }
        }
        
        // Add logic for what happens when all enemies are killed, e.g., load next level, show victory screen, etc.
    }
}
