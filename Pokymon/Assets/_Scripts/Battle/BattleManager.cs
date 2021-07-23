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
   
   private void Start()
   {
      StartCoroutine(SetupBattle());
   }

   public IEnumerator SetupBattle()
   {
      state = BattleState.StartBattle;
      
      playerUnit.SetupPokemon();
      playerHUD.SetPokemonData(playerUnit.Pokemon); 
      
      battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
      
      enemyUnit.SetupPokemon();
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

   IEnumerator EnemyAction()
   {
      state = BattleState.EnemyMove;

      Move move = enemyUnit.Pokemon.RandomMove();
      yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} ha usado {move.Base.Name}.");

      bool pokemonFainted = playerUnit.Pokemon.ReceiveDamage(enemyUnit.Pokemon, move);
      playerHUD.UpdatePokemonData();
      
      if (pokemonFainted)
      {
         yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} ha sido debilitado.");
      }
      else
      {
         PlayerAction();
      }

   }

   private void Update()
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

         currentSelectedAction = (currentSelectedAction + 1) % 2;
         
         battleDialogBox.SelectAction(currentSelectedAction);
      }

      if (Input.GetAxisRaw("Submit")!=0)
      {
         timeSinceLastClick = 0;
         if (currentSelectedAction == 0)
         {
            PlayerMovement();
         }else if (currentSelectedAction == 1)
         {
            //TODO: implementar la huida
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
         var oldSelectedMovement = currentSelectedMovement;
         if (currentSelectedMovement<=1)
         {
            currentSelectedMovement = (currentSelectedMovement + 1) % 2;
         }
         else //currentSelectedMovement >= 2
         {
            currentSelectedMovement = (currentSelectedMovement + 1) % 2 + 2;
         }

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
   }


   IEnumerator PerformPlayerMovement()
   {
      Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];
      yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} ha usado {move.Base.Name}");

      bool pokemonFainted = enemyUnit.Pokemon.ReceiveDamage(playerUnit.Pokemon, move);
      enemyHUD.UpdatePokemonData();
      
      if (pokemonFainted)
      {
         yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} se ha debilitado");
      }
      else
      {
         StartCoroutine(EnemyAction());
      }

   }
}
