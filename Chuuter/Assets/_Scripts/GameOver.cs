using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text actualScore, actualTime, bestScore, bestTime;

    public bool playerHasWon;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerHasWon)
        {
            actualScore.text = "Score: " + PlayerPrefs.GetInt("Last Score");
            actualTime.text = "Time: " + PlayerPrefs.GetFloat("Last Time");
            bestScore.text = "Best: " + PlayerPrefs.GetInt("High Score");
            bestTime.text = "Best: " + PlayerPrefs.GetFloat("Low Time"); 
        }
        

    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene("Level 1");
    }


}
