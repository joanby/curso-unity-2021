using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameModeWaves : MonoBehaviour
{

    [SerializeField]
    private Life playerLife;

    [SerializeField]
    private Life baseLife;
    
    private void Start()
    {
        playerLife.onDeath.AddListener(CheckLoseCondition);
        baseLife.onDeath.AddListener(CheckLoseCondition);
        
        EnemyManager.SharedInstance.onEnemyChanged.AddListener(CheckWinCondition);
        WaveManager.SharedInstance.onWaveChanged.AddListener(CheckWinCondition);
    }

    void CheckLoseCondition()
    {
        SceneManager.LoadScene("LoseScene", LoadSceneMode.Single);
    }

    void CheckWinCondition()
    {
        //GANAR
        if (EnemyManager.SharedInstance.EnemyCount <= 0 && 
            WaveManager.SharedInstance.WavesCount <= 0)
        {
            SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
        }
    }

}
