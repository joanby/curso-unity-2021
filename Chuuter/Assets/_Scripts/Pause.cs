using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class Pause : MonoBehaviour
{

    public GameObject pauseMenu;
    public Button exitButton;

    public AudioMixerSnapshot pauseSnp, gameSnp;
    
    private void Awake()
    {
        pauseMenu.SetActive(false);
        exitButton.onClick.AddListener(ExitGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            
            pauseSnp.TransitionTo(0.1f);
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        gameSnp.TransitionTo(0.1f);
    }

    private void ExitGame()
    {
        print("EJECUCIÓN FINALIZADA");
        Application.Quit();
    }
    
}
