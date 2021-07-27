using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
   StartBattle,
   PlayerSelectAction,
   PlayerMove,
   EnemyMove,
   Busy
}

public class BattleManager : MonoBehaviour
{
   [SerializeField] BattleUnit playerUnit;
   [SerializeField] BattleHUD playerHUD;
   
   [SerializeField] BattleUnit enemyUnit;
   [SerializeField] BattleHUD enemyHUD;

   [SerializeField] BattleDialogBox battleDialogBox;

   public BattleState state;

   public event Action<bool> OnBattleFinish;

   private PokemonParty playerParty;
   private Pokemon wildPokemon;
   
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
      playerHUD.SetPokemonData(playerUnit.Pokemon); 
      
      battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
      
      enemyUnit.SetupPokemon(wildPokemon);
      enemyHUD.SetPokemonData(enemyUnit.Pokemon);
      
      yield return battleDialogBox.SetDialog($"Un {enemyUnit.Pokemon.Base.Name} salvaje apareció.");

      if (enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
      {
         StartCoroutine(battleDialogBox.SetDialog("El enemigo ataca primero."));
         EnemyAction();
      }
      else
      {
         PlayerAction();
      }
   }

   void PlayerAction()
   {
      state = BattleState.PlayerSelectAction;
      StartCoroutine(battleDialogBox.SetDialog("Selecciona una acción"));
      battleDialogBox.ToggleDialogText(true);
      battleDialogBox.ToggleActions(true);
      battleDialogBox.ToggleMovements(false);
      currentSelectedAction = 0;
      battleDialogBox.SelectAction(currentSelectedAction);
   }

   void PlayerMovement()
   {
      state = BattleState.PlayerMove;
      battleDialogBox.ToggleDialogText(false);
      battleDialogBox.ToggleActions(false);
      battleDialogBox.ToggleMovements(true);
      currentSelectedMovement = 0;
      battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
   }


   public void HandleUpdate()
   {
      timeSinceLastClick += Time.deltaTime;

      if (battleDialogBox.isWriting)
      {
         return;
      }
      
      if (state == BattleState.PlayerSelectAction)
      {
         HandlePlayerActionSelection();
      }else if (state == BattleState.PlayerMove)
      {
         HandlePlayerMovementSelection();
      }
   }
   
   
   private float timeSinceLastClick;
   public float timeBetweenClicks = 1.0f;
   

   private int currentSelectedAction;

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
            PlayerMovement();
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

   private int currentSelectedMovement;
   
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
         var oldSelectedMovement = (currentSelectedMovement+1)%2 +
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
         PlayerAction();
      }
      
      
      
   }


   IEnumerator PerformPlayerMovement()
   {
      Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];
      move.Pp--;
      yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} ha usado {move.Base.Name}");

      var oldHPVal = enemyUnit.Pokemon.HP;
      
      playerUnit.PlayAttackAnimation();
      yield return new WaitForSeconds(1f);
      enemyUnit.PlayReceiveAttackAnimation();

      var damageDesc = enemyUnit.Pokemon.ReceiveDamage(playerUnit.Pokemon, move);
      enemyHUD.UpdatePokemonData(oldHPVal);
      yield return ShowDamageDescription(damageDesc);
      
      if (damageDesc.Fainted)
      {
         yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} se ha debilitado");
         enemyUnit.PlayFaintAnimation();
         yield return new WaitForSeconds(1.5f);
         
         OnBattleFinish(true);
      }
      else
      {
         StartCoroutine(EnemyAction());
      }

   }
   
   
   
   IEnumerator EnemyAction()
   {
      state = BattleState.EnemyMove;

      Move move = enemyUnit.Pokemon.RandomMove();
      move.Pp--;
      yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} ha usado {move.Base.Name}.");

      var oldHPVal = playerUnit.Pokemon.HP;
      
      enemyUnit.PlayAttackAnimation();
      yield return new WaitForSeconds(1f);
      playerUnit.PlayReceiveAttackAnimation();

      var damageDesc = playerUnit.Pokemon.ReceiveDamage(enemyUnit.Pokemon, move);
      playerHUD.UpdatePokemonData(oldHPVal);
      yield return ShowDamageDescription(damageDesc);
      
      if (damageDesc.Fainted)
      {
         yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} ha sido debilitado.");
         playerUnit.PlayFaintAnimation();
         
         yield return new WaitForSeconds(1.5f);

         var nextPokemon = playerParty.GetFirstNonFaintedPokemon();
         if (nextPokemon == null)//No quedan pokemons con vida
         {
            OnBattleFinish(false);
         }
         else//Tengo que sacar a otro pokemon
         {
            playerUnit.SetupPokemon(nextPokemon);
            playerHUD.SetPokemonData(nextPokemon);
            
            battleDialogBox.SetPokemonMovements(nextPokemon.Moves);

            yield return battleDialogBox.SetDialog($"¡Adelante {nextPokemon.Base.Name}!");
               
            PlayerAction();   
         }
         
      }
      else
      {
         PlayerAction();
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
   
   void OpenInventoryScreen()
   {
      print("Abrir inventario");


      if (Input.GetAxisRaw("Cancel")!=0)
      {
         PlayerAction();
      }
   }

   void OpenPartySelectionScreen()
   {
      print("Abrir la pantalla para seleccionar Pokemons");
      
      if (Input.GetAxisRaw("Cancel")!=0)
      {
         PlayerAction();
      }
   }
}
