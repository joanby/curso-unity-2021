using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Text dialogText;

    [SerializeField] GameObject actionSelect;
    [SerializeField] GameObject movementSelect;
    [SerializeField] GameObject movementDesc;
    
    public float charactersPerSecond = 10.0f;

    public IEnumerator SetDialog(string message)
    {
        dialogText.text = "";
        foreach (var character in message)
        {
            dialogText.text += character;
            yield return new WaitForSeconds(1/charactersPerSecond);
        }
    }
}
