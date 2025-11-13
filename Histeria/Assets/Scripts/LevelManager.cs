using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    DungeonPopulator populator;

    private void Awake()
    {
        populator = new DungeonPopulator();
    }

    public void Update()
    {
        if (populator.enemyNumber < 1)
        {
            SceneManager.LoadScene("Level2");
        }
    }
}
