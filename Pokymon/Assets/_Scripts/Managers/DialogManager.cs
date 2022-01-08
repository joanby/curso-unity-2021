using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private Text dialogText;
    [SerializeField] private int charactersPerSecond;

    public static DialogManager SharedInstance;

    public event Action OnDialogStart, OnDialogFinish;
    
    private float timeSinceLastClick;
    [SerializeField] float timeBetweenClicks = 1.0f;

    private Dialog currentDialog;
    private int currentLine = 0;
    private bool isWriting;

    public bool IsBeingShown;
    private Action onDialogClose;
    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
    }

    public void ShowDialog(Dialog dialog, Action onDialogFinish = null)
    {
        OnDialogStart?.Invoke();
       dialogBox.SetActive(true);
       
       currentDialog = dialog;
       IsBeingShown = true;
       this.onDialogClose = onDialogFinish;
       StartCoroutine(SetDialog(currentDialog.Lines[currentLine]));
    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;
        if (Input.GetAxisRaw("Submit")!= 0 && !isWriting)
        {
            if (timeSinceLastClick >= timeBetweenClicks)
            {
                timeSinceLastClick = 0;
                currentLine++;
                if (currentLine < currentDialog.Lines.Count)
                {
                    StartCoroutine(SetDialog(currentDialog.Lines[currentLine]));
                }
                else
                {
                    currentLine = 0;
                    IsBeingShown = false;
                    dialogBox.SetActive(false);
                    onDialogClose?.Invoke();
                    OnDialogFinish?.Invoke();
                }
            }
        }
    }
    
    
    public IEnumerator SetDialog(string line)
    {
        isWriting = true;
        dialogText.text = "";
        foreach (var character in line)
        {
            if (character!=' ')
            {
                SoundManager.SharedInstance.PlayRandomCharacterSound();
            }
            dialogText.text += character;
            yield return new WaitForSeconds(1/charactersPerSecond);
        }

        isWriting = false;
    }
}
