using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMovementUI : MonoBehaviour
{
    [SerializeField] private Text[] movementTexts;
    private int currentSelectedMovement = 0;
    
    /*private void Start()
    {
        movementTexts = GetComponentsInChildren<Text>(true);
    }*/

    public void SetMovements(List<MoveBase> pokemonMoves, MoveBase newMove)
    {
        currentSelectedMovement = 0;

        for (int i = 0; i < pokemonMoves.Count; i++)
        {
            movementTexts[i].text = pokemonMoves[i].Name;
        }

        movementTexts[pokemonMoves.Count].text = newMove.Name;
    }

    public void HandleForgetMoveSelection(Action<int> onSelected)
    {

        if (Input.GetAxisRaw("Vertical")!=0)
        {
            int direction = Mathf.FloorToInt(Input.GetAxisRaw("Vertical"));
            currentSelectedMovement -= direction;
            onSelected?.Invoke(-1);
        }
        currentSelectedMovement = Mathf.Clamp(currentSelectedMovement, 
            0, PokemonBase.NUMBER_OF_LEARNABLE_MOVES);
        UpdateColorForgetMoveSelection(currentSelectedMovement); 

        if (Input.GetAxisRaw("Submit")!=0)
        {
            onSelected?.Invoke(currentSelectedMovement);
        }
    }

    public void UpdateColorForgetMoveSelection(int selectedMove)
    {
        for (int i = 0; i <= PokemonBase.NUMBER_OF_LEARNABLE_MOVES; i++)
        {
            movementTexts[i].color = (i == selectedMove ?  ColorManager.SharedInstance.selectedColor : Color.black);
        }
    }
}
