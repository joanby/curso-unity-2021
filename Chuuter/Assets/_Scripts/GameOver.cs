using System.Collections;
using System.Collections.Generic;
using System.Text;
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
            int score = PlayerPrefs.GetInt("Last Score");
            float time = PlayerPrefs.GetFloat("Last Time");
            
            actualScore.text = string.Format("Score: {0}", score);
            actualTime.text = string.Format("Time: {0}", time);
            
            StringBuilder builder = new StringBuilder();
            builder.Append("Best: ");
            builder.Append(PlayerPrefs.GetInt("High Score"));
            bestScore.text = builder.ToString();
            
            //string s = "Mi" + "nombre" + "es" + "Juan" + "Gabriel";

            builder.Clear();
            builder.Append("Best: ");
            builder.Append(PlayerPrefs.GetFloat("Low Time"));
            bestTime.text = builder.ToString(); 
            
            print($"La partida ha terminado con {score} puntos en {time} segundos");

            string msg = $"Esto es un mensaje de {score} puntos";

        }
        

    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene("Level 1");
    }


}
