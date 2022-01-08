using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Text dialogText;

    [SerializeField] GameObject actionSelect;
    [SerializeField] GameObject movementSelect;
    [SerializeField] GameObject movementDesc;
    [SerializeField] private GameObject yesNoBox;
    
    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> movementTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;
    [SerializeField] Text yesText, noText;
    
    public float timeToWaitAfterText = 1f;
    public float charactersPerSecond = 10.0f;

    public bool isWriting = false;

   public IEnumerator SetDialog(string message)
    {
        isWriting = true;
        
        dialogText.text = "";
        foreach (var character in message)
        {
            if (character!=' ')
            {
                SoundManager.SharedInstance.PlayRandomCharacterSound();
            }
            dialogText.text += character;
            yield return new WaitForSeconds(1/charactersPerSecond);
        }
        
        yield return new WaitForSeconds(timeToWaitAfterText);
        isWriting = false;
    }

    public void ToggleDialogText(bool activated)
    {
        dialogText.enabled = activated;
    }

    public void ToggleActions(bool activated)
    {
        actionSelect.SetActive(activated);
    }

    public void ToggleMovements(bool activated)
    {
        movementSelect.SetActive(activated);
        movementDesc.SetActive(activated);
    }


    public void ToggleYesNoBox(bool activated)
    {
        yesNoBox.SetActive(activated);
    }
    
    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].color = (i == selectedAction ? ColorManager.SharedInstance.selectedColor : ColorManager.SharedInstance.defaultColor);
        }
    }

    public void SetPokemonMovements(List<Move> moves)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                movementTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                movementTexts[i].text = "---";
            }
        }
    }
    
    public void SelectMovement(int selectedMovement, Move move)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].color = (i == selectedMovement ?  ColorManager.SharedInstance.selectedColor : ColorManager.SharedInstance.defaultColor);
        }

        ppText.text = $"PP {move.Pp}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString().ToUpper();

        ppText.color = ColorManager.SharedInstance.PPColor((float)move.Pp/move.Base.PP);
        movementDesc.GetComponent<Image>().color = ColorManager.TypeColor.GetColorFromType(move.Base.Type);
    }

    
    public void SelectYesNoAction(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = ColorManager.SharedInstance.selectedColor;
            noText.color = ColorManager.SharedInstance.defaultColor;
            
        }
        else
        {
            yesText.color = ColorManager.SharedInstance.defaultColor;
            noText.color = ColorManager.SharedInstance.selectedColor;
        }
    }
    
}
