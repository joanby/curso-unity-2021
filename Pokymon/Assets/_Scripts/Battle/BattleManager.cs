using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
   [SerializeField] BattleUnit playerUnit;
   [SerializeField] BattleHUD playerHUD;
   
   [SerializeField] BattleUnit enemyUnit;
   [SerializeField] BattleHUD enemyHUD;

   [SerializeField] BattleDialogBox battleDialogBox;
   private void Start()
   {
      SetupBattle();
   }

   public void SetupBattle()
   {
      playerUnit.SetupPokemon();
      playerHUD.SetPokemonData(playerUnit.Pokemon); 
      
      enemyUnit.SetupPokemon();
      enemyHUD.SetPokemonData(enemyUnit.Pokemon);
      
      StartCoroutine(battleDialogBox.SetDialog($"Un {enemyUnit.Pokemon.Base.Name} salvaje apareció."));
   }


}
