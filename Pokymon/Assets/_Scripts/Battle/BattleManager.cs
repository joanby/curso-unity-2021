using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
   StartBattle,
   ActionSelection,
   MovementSelection,
   PerformMovement,
   Busy, 
   PartySelectScreen,
   ItemSelectScreen,
   FinishBattle
}

public class BattleManager : MonoBehaviour
{
   [SerializeField] BattleUnit playerUnit;
   
   [SerializeField] BattleUnit enemyUnit;

   [SerializeField] BattleDialogBox battleDialogBox;

   [SerializeField] PartyHUD partyHUD;
   
   public BattleState state;

   public event Action<bool> OnBattleFinish;

   private PokemonParty playerParty;
   private Pokemon wildPokemon;
   
      
   private float timeSinceLastClick;
   [SerializeField] float timeBetweenClicks = 1.0f;
   

   private int currentSelectedAction;
   private int currentSelectedMovement;
   private int currentSelectedPokemon;

   public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
   {
      this.playerParty = playerParty;
      this.wildPokemon = wildPokemon;
      StartCoroutine(SetupBattle());
   }

   public IEnumerator SetupBattle()
   {
      state = BattleState.StartBattle;
      
      playerUnit.SetupPokemon(playerParty.GetFirstNonFaintedPokemon());
      
      battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
      
      enemyUnit.SetupPokemon(wildPokemon);
      
      partyHUD.InitPartyHUD();

      yield return battleDialogBox.SetDialog($"Un {enemyUnit.Pokemon.Base.Name} salvaje apareció.");

      if (enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
      {
         battleDialogBox.ToggleDialogText(true);
         battleDialogBox.ToggleActions(false);
         battleDialogBox.ToggleMovements(false);
         yield return battleDialogBox.SetDialog("El enemigo ataca primero.");
         yield return PerformEnemyMovement();
      }
      else
      {
         PlayerActionSelection();
      }
   }

   void BattleFinish(bool playerHasWon)
   {
      state = BattleState.FinishBattle;
      OnBattleFinish(playerHasWon);
   }

   void PlayerActionSelection()
   {
      state = BattleState.ActionSelection;
      StartCoroutine(battleDialogBox.SetDialog("Selecciona una acción"));
      battleDialogBox.ToggleDialogText(true);
      battleDialogBox.ToggleActions(true);
      battleDialogBox.ToggleMovements(false);
      currentSelectedAction = 0;
      battleDialogBox.SelectAction(currentSelectedAction);
   }

   void PlayerMovementSelection()
   {
      state = BattleState.MovementSelection;
      battleDialogBox.ToggleDialogText(false);
      battleDialogBox.ToggleActions(false);
      battleDialogBox.ToggleMovements(true);
      currentSelectedMovement = 0;
      battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
   }
   
   void OpenPartySelectionScreen()
   {
      state = BattleState.PartySelectScreen;
      partyHUD.SetPartyData(playerParty.Pokemons);
      partyHUD.gameObject.SetActive(true);
      currentSelectedPokemon = playerParty.GetPositionFromPokemon(playerUnit.Pokemon);
      partyHUD.UpdateSelectedPokemon(currentSelectedPokemon);

   }
   
   void OpenInventoryScreen()
   {
      print("Abrir inventario");
      
   }


   public void HandleUpdate()
   {
      timeSinceLastClick += Time.deltaTime;

      if (battleDialogBox.isWriting)
      {
         return;
      }
      
      if (state == BattleState.ActionSelection)
      {
         HandlePlayerActionSelection();
      }else if (state == BattleState.MovementSelection)
      {
         HandlePlayerMovementSelection();
      }else if (state == BattleState.PartySelectScreen)
      {
         HandlePlayerPartySelection();
      }
   }
   


   void HandlePlayerActionSelection()
   {
      if (timeSinceLastClick < timeBetweenClicks)
      {
         return;
      }
      
      if (Input.GetAxisRaw("Vertical")!=0)
      {
         timeSinceLastClick = 0;

         currentSelectedAction = (currentSelectedAction + 2) % 4;
         
         battleDialogBox.SelectAction(currentSelectedAction);
      }else if (Input.GetAxisRaw("Horizontal") != 0)
      {
         timeSinceLastClick = 0;
         currentSelectedAction = (currentSelectedAction + 1) % 2 +
                                 2 * Mathf.FloorToInt(currentSelectedAction / 2);
         battleDialogBox.SelectAction(currentSelectedAction);
      }

      if (Input.GetAxisRaw("Submit")!=0)
      {
         timeSinceLastClick = 0;
         if (currentSelectedAction == 0)
         {
            //Luchar
            PlayerMovementSelection();
         }else if (currentSelectedAction == 1)
         {
            //Cambiar Pokemon
            OpenPartySelectionScreen();
         } else if (currentSelectedAction == 2)
         {
            //Mochila
            OpenInventoryScreen();
         }else if (currentSelectedAction == 3)
         {
            //Huir
            OnBattleFinish(false);
         }
      }
   }

   
   void HandlePlayerMovementSelection()
   {
      if (timeSinceLastClick < timeBetweenClicks)
      {
         return;
      }

      /// 0  1
      /// 2  3
      if (Input.GetAxisRaw("Vertical")!=0)
      {
         timeSinceLastClick = 0;
         var oldSelectedMovement = currentSelectedMovement;
         currentSelectedMovement = (currentSelectedMovement + 2) % 4;
         if (currentSelectedMovement >= playerUnit.Pokemon.Moves.Count)
         {
            currentSelectedMovement = oldSelectedMovement;
         }
         
         battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
         
      } else if (Input.GetAxisRaw("Horizontal")!=0)
      {

         timeSinceLastClick = 0;
         var oldSelectedMovement = currentSelectedMovement;
         currentSelectedMovement = (currentSelectedMovement+1)%2 +
                                   2*Mathf.FloorToInt(currentSelectedMovement/2);

         if (currentSelectedMovement >= playerUnit.Pokemon.Moves.Count)
         {
            currentSelectedMovement = oldSelectedMovement;
         }
         battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
         
      }
      if (Input.GetAxisRaw("Submit")!=0)
      {
         timeSinceLastClick = 0;
         battleDialogBox.ToggleMovements(false);
         battleDialogBox.ToggleDialogText(true);
         StartCoroutine(PerformPlayerMovement());
      }

      if (Input.GetAxisRaw("Cancel")!=0)
      {
         PlayerActionSelection();
      }
   }

   void HandlePlayerPartySelection()
   {
      if (timeSinceLastClick < timeBetweenClicks)
      {
         return;
      }

      /// 0  1
      /// 2  3
      /// 4  5
      if (Input.GetAxisRaw("Vertical")!=0)
      {
         timeSinceLastClick = 0;
         currentSelectedPokemon -= (int)Input.GetAxisRaw("Vertical")*2;

      } else if (Input.GetAxisRaw("Horizontal")!=0)
      {
         timeSinceLastClick = 0;
         currentSelectedPokemon += (int)Input.GetAxisRaw("Horizontal");
      }

      currentSelectedPokemon = Mathf.Clamp(currentSelectedPokemon,
         0, playerParty.Pokemons.Count - 1);
      partyHUD.UpdateSelectedPokemon(currentSelectedPokemon);

      if (Input.GetAxisRaw("Submit")!=0)
      {
         timeSinceLastClick = 0;
         var selectedPokemon = playerParty.Pokemons[currentSelectedPokemon];
         if (selectedPokemon.HP <= 0)
         {
            partyHUD.SetMessage("No puedes enviar un pokemon debilitado");
            return;
         }
         else if (selectedPokemon == playerUnit.Pokemon)
         {
            partyHUD.SetMessage("No puedes seleccionar el pokemon en batalla");
            return;
         }

         partyHUD.gameObject.SetActive(false);
         state = BattleState.Busy;
         StartCoroutine(SwitchPokemon(selectedPokemon));
      }

      if (Input.GetAxisRaw("Cancel")!=0)
      {
         partyHUD.gameObject.SetActive(false);
         PlayerActionSelection();
      }

   }

   IEnumerator PerformPlayerMovement()
   {
      state = BattleState.PerformMovement;
      
      Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];
      if (move.Pp<=0){
         PlayerMovementSelection();
         yield break;
      }
      yield return RunMovement(playerUnit, enemyUnit, move);

      if (state == BattleState.PerformMovement)
      {
         StartCoroutine(PerformEnemyMovement());
      }
      
   }

   IEnumerator PerformEnemyMovement()
   {
      state = BattleState.PerformMovement;

      Move move = enemyUnit.Pokemon.RandomMove();

      yield return RunMovement(enemyUnit, playerUnit, move);

      if (state == BattleState.PerformMovement)
      {
         PlayerActionSelection();
      }
      
      
   }


   IEnumerator RunMovement(BattleUnit attacker, BattleUnit target, Move move)
   {
      move.Pp--;
      yield return battleDialogBox.SetDialog($"{attacker.Pokemon.Base.Name} ha usado {move.Base.Name}");

      var oldHPVal = target.Pokemon.HP;
      
      attacker.PlayAttackAnimation();
      yield return new WaitForSeconds(1f);
      target.PlayReceiveAttackAnimation();

      var damageDesc = target.Pokemon.ReceiveDamage(attacker.Pokemon, move);
      yield return target.Hud.UpdatePokemonData(oldHPVal); 
      yield return ShowDamageDescription(damageDesc);
      
      if (damageDesc.Fainted)
      {
         yield return battleDialogBox.SetDialog($"{target.Pokemon.Base.Name} se ha debilitado");
         target.PlayFaintAnimation();
         yield return new WaitForSeconds(1.5f);
         
         CheckForBattleFinish(target);
      }
   }

   void CheckForBattleFinish(BattleUnit faintedUnit)
   {
      if (faintedUnit.IsPlayer)
      {
         var nextPokemon = playerParty.GetFirstNonFaintedPokemon();
         if (nextPokemon != null)
         {
            OpenPartySelectionScreen();
         }
         else
         {
            BattleFinish(false);
         }
      }
      else
      {
         BattleFinish(true);
      }
   }

   IEnumerator ShowDamageDescription(DamageDescription desc)
   {
      if (desc.Critical > 1f)
      {
         yield return battleDialogBox.SetDialog("¡Un golpe crítico!");
      }

      if (desc.Type > 1)
      {
         yield return battleDialogBox.SetDialog("¡Es super efectivo!");
      }else if (desc.Type < 1)
      {
         yield return battleDialogBox.SetDialog("No es muy efectivo...");
      }
      
   }

   IEnumerator SwitchPokemon(Pokemon newPokemon)
   {

      if (playerUnit.Pokemon.HP > 0)
      {
         yield return battleDialogBox.SetDialog($"¡Vuelve {playerUnit.Pokemon.Base.Name}!");
         playerUnit.PlayFaintAnimation();
         yield return new WaitForSeconds(1.5f);
      }
      
      
      playerUnit.SetupPokemon(newPokemon);
      battleDialogBox.SetPokemonMovements(newPokemon.Moves);
      
      yield return battleDialogBox.SetDialog($"¡Ve {newPokemon.Base.Name}!");
      StartCoroutine(PerformEnemyMovement());
   }
   
}
