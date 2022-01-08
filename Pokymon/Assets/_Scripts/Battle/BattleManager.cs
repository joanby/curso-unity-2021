using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState
{
   StartBattle,
   ActionSelection,
   MovementSelection,
   Busy, 
   YesNoChoice,
   PartySelectScreen,
   ItemSelectScreen,
   ForgetMovement,
   RunTurn,
   FinishBattle
}

public enum BattleAction
{
   Move, SwitchPokemon, UseItem, Run
}

public enum BattleType
{
   WildPokemon,
   Trainer,
   Leader
}

public class BattleManager : MonoBehaviour
{
   [SerializeField] BattleUnit playerUnit;
   
   [SerializeField] BattleUnit enemyUnit;

   [SerializeField] BattleDialogBox battleDialogBox;

   [SerializeField] PartyHUD partyHUD;

   [SerializeField] SelectionMovementUI selectMoveUI;

   [SerializeField] GameObject pokeball;

   [SerializeField] private Image playerImage, trainerImage;
   
   public BattleState state;
   public BattleState? previousState;

   public BattleType type;
   
   public event Action<bool> OnBattleFinish;

   private PokemonParty playerParty;
   private PokemonParty trainerParty;
   private Pokemon wildPokemon;
   
   private float timeSinceLastClick;
   [SerializeField] float timeBetweenClicks = 1.0f;
   

   private int currentSelectedAction;
   private int currentSelectedMovement;
   private int currentSelectedPokemon;
   private bool currentSelectedChoice = true;

   private int escapeAttempts;
   private MoveBase moveToLearn;

   public AudioClip attackClip, damageClip, levelUpClip, pokeballClip, faintedClip, endBattleClip;

   private PlayerController player;
   private TrainerController trainer;
   
   public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
   {
      type = BattleType.WildPokemon;
      escapeAttempts = 0;
      this.playerParty = playerParty;
      this.wildPokemon = wildPokemon;
      StartCoroutine(SetupBattle());
   }

   public void HandleStartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty, bool isLeader = false)
   {
      type = (isLeader? BattleType.Leader: BattleType.Trainer);
      this.playerParty = playerParty;
      this.trainerParty = trainerParty;

      player = playerParty.GetComponent<PlayerController>();
      trainer = trainerParty.GetComponent<TrainerController>();
      
      StartCoroutine(SetupBattle());
   }

   public IEnumerator SetupBattle()
   {
      state = BattleState.StartBattle;
      playerUnit.ClearHUD();
      enemyUnit.ClearHUD();
      
      if (type == BattleType.WildPokemon)
      {
         enemyUnit.SetupPokemon(wildPokemon);
         playerUnit.SetupPokemon(playerParty.GetFirstNonFaintedPokemon());
         
         battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
         yield return battleDialogBox.SetDialog($"Un {enemyUnit.Pokemon.Base.Name} salvaje apareció.");
      }
      else //Entrenador y lider
      {
         playerUnit.gameObject.SetActive(false);
         enemyUnit.gameObject.SetActive(false);

         var playerInitialPosition = playerImage.transform.localPosition;
         playerImage.transform.localPosition = playerInitialPosition - new Vector3(400f, 0, 0);
         playerImage.transform.DOLocalMoveX(playerInitialPosition.x, 0.5f);
         
         var trainerInitialPosition = trainerImage.transform.localPosition;
         trainerImage.transform.localPosition = trainerInitialPosition + new Vector3(400f, 0, 0);
         trainerImage.transform.DOLocalMoveX(trainerInitialPosition.x, 0.5f);
         
         playerImage.gameObject.SetActive(true);
         trainerImage.gameObject.SetActive(true);
         playerImage.sprite = player.TrainerSprite;
         trainerImage.sprite = trainer.TrainerSprite;

         yield return battleDialogBox.SetDialog($"¡{trainer.TrainerName} quiere luchar!");
         
         //Enviar el primer pokemon del rival entrenador
         yield return trainerImage.transform.DOLocalMoveX(trainerImage.transform.localPosition.x+400, 0.5f).WaitForCompletion();
         trainerImage.gameObject.SetActive(false);
         enemyUnit.gameObject.SetActive(true);
         var enemyPokemon = trainerParty.GetFirstNonFaintedPokemon();
         enemyUnit.SetupPokemon(enemyPokemon);
         yield return battleDialogBox.SetDialog($"{trainer.TrainerName} ha enviado a {enemyPokemon.Base.Name}");
         trainerImage.transform.localPosition = trainerInitialPosition;
         
         //Enviar el primer pokemon del jugador
         yield return playerImage.transform.DOLocalMoveX(playerImage.transform.localPosition.x-400, 0.5f).WaitForCompletion();
         playerImage.gameObject.SetActive(false);
         playerUnit.gameObject.SetActive(true);
         var playerPokemon = playerParty.GetFirstNonFaintedPokemon();
         playerUnit.SetupPokemon(playerPokemon);
         battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
         yield return battleDialogBox.SetDialog($"Ve {playerPokemon.Base.Name}");
         playerImage.transform.localPosition = playerInitialPosition;
      }

      partyHUD.InitPartyHUD();
      
      PlayerActionSelection();
   }

   void BattleFinish(bool playerHasWon)
   {
      SoundManager.SharedInstance.PlaySound(endBattleClip);
      state = BattleState.FinishBattle;
      playerParty.Pokemons.ForEach(p => p.OnBattleFinish());
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

   IEnumerator YesNoChoice(Pokemon newTrainerPokemon)
   {
      state = BattleState.Busy;
      yield return battleDialogBox.SetDialog(
         $"{trainer.TrainerName} va a sacar a {newTrainerPokemon.Base.Name}. ¿Quieres cambiar tu Pokemon?");
      state = BattleState.YesNoChoice;
      battleDialogBox.ToggleYesNoBox(true);
   }

   public void HandleUpdate()
   {
      timeSinceLastClick += Time.deltaTime;
      
      if (timeSinceLastClick < timeBetweenClicks || battleDialogBox.isWriting)
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
      }else if (state == BattleState.YesNoChoice)
      {
         HandleYesNoChoice();
      }else if (state == BattleState.ForgetMovement)
      {
         selectMoveUI.HandleForgetMoveSelection(
            (moveIndex) =>
            {
               if (moveIndex < 0)
               {
                  timeSinceLastClick = 0;
                  return;
               }

               StartCoroutine(ForgetOldMove(moveIndex));
            });
      }
   }


   IEnumerator ForgetOldMove(int moveIndex)
   {
      selectMoveUI.gameObject.SetActive(false);
      if (moveIndex == PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
      {
        yield return 
            battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} no ha aprendido {moveToLearn.Name}");
      }
      else
      {
         var selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
         yield return 
            battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} olvidó {selectedMove.Name} y aprendió {moveToLearn.Name}");
         playerUnit.Pokemon.Moves[moveIndex] = new Move(moveToLearn);
                  
      }
               
      moveToLearn = null;
      //TODO: revisar más adelante cuando haya entrenadores 
      state = BattleState.FinishBattle;

   }

   void HandlePlayerActionSelection()
   {

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
            previousState = state;
            OpenPartySelectionScreen();
         } else if (currentSelectedAction == 2)
         {
            //Mochila
            StartCoroutine(RunTurns(BattleAction.UseItem));
         }else if (currentSelectedAction == 3)
         {
            //Huir
            StartCoroutine(RunTurns(BattleAction.Run));
         }
      }
   }

   
   void HandlePlayerMovementSelection()
   {

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
         StartCoroutine(RunTurns(BattleAction.Move));
      }

      if (Input.GetAxisRaw("Cancel")!=0)
      {
         PlayerActionSelection();
      }
   }

   void HandlePlayerPartySelection()
   {
      
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
         battleDialogBox.ToggleActions(false);

         if (previousState == BattleState.ActionSelection)
         {
            previousState = null;
            StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
         }
         else
         {
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedPokemon));
         }
         
      }

      if (Input.GetAxisRaw("Cancel")!=0)
      {
         if (playerUnit.Pokemon.HP <= 0)
         {
            partyHUD.SetMessage("Tienes que seleccionar un Pokemon para continuar...");
            return;
         }
         
         partyHUD.gameObject.SetActive(false);
         if (previousState == BattleState.YesNoChoice)
         {
            previousState = null;
            StartCoroutine(SendNextTrainerPokemonToBattle());
         }
         else
         {
            PlayerActionSelection();
         }
      }

   }

   void HandleYesNoChoice()
   {
      if (Input.GetAxisRaw("Vertical")!=0)
      {
         timeSinceLastClick = 0;
         currentSelectedChoice = !currentSelectedChoice;
      }
      battleDialogBox.SelectYesNoAction(currentSelectedChoice);

      if (Input.GetAxisRaw("Submit") != 0)
      {
         timeSinceLastClick = 0;
         battleDialogBox.ToggleYesNoBox(false);
         if (currentSelectedChoice)
         {
            previousState = BattleState.YesNoChoice;
            OpenPartySelectionScreen();
         }
         else
         {
            StartCoroutine(SendNextTrainerPokemonToBattle());
         }
      }

      if (Input.GetAxisRaw("Cancel") != 0)
      {
         timeSinceLastClick = 0;
         battleDialogBox.ToggleYesNoBox(false);
         StartCoroutine(SendNextTrainerPokemonToBattle());
      }
   }

   IEnumerator RunTurns(BattleAction playerAction)
   {
      state = BattleState.RunTurn;
      
      if (playerAction == BattleAction.Move)
      {
         playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentSelectedMovement];
         enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.RandomMove();

         bool playerGoesFirst = true;
         int enemyPriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;
         int playerPriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
         if (enemyPriority > playerPriority)
         {
            playerGoesFirst = false;
         }
         else if (enemyPriority == playerPriority)
         {
            playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
         }

         var firstUnit = (playerGoesFirst ? playerUnit : enemyUnit);
         var secondUnit = (playerGoesFirst ? enemyUnit : playerUnit);

         var secondPokemon = secondUnit.Pokemon;
         
         //Primer Turno
         yield return RunMovement(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
         yield return RunAfterTurn(firstUnit);
         if (state == BattleState.FinishBattle)
         {
            yield break;
         }

         if (secondPokemon.HP > 0)
         {
            //Segundo Turno
            yield return RunMovement(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(secondUnit);
            if (state == BattleState.FinishBattle)
            {
               yield break;
            }
         }
         
      }
      else
      {
         if (playerAction == BattleAction.SwitchPokemon)
         {
            var selectedPokemon = playerParty.Pokemons[currentSelectedPokemon];
            state = BattleState.Busy;
            yield return SwitchPokemon(selectedPokemon);
         }else if (playerAction == BattleAction.UseItem)
         {
            battleDialogBox.ToggleActions(false);
            yield return ThrowPokeball();
         }else if (playerAction == BattleAction.Run)
         {
            yield return TryToEscapeFromBattle();
         }
         
         //Turno del Enemigo
         var enemyMove = enemyUnit.Pokemon.RandomMove();
         yield return RunMovement(enemyUnit, playerUnit, enemyMove);

         yield return RunAfterTurn(enemyUnit);

         if (state == BattleState.FinishBattle)
         {
            yield break;
         }
      }

      if (state != BattleState.FinishBattle)
      {
         PlayerActionSelection();
      }
   }

   IEnumerator RunMovement(BattleUnit attacker, BattleUnit target, Move move)
   {
      //Comprobar el estado alterado que me impida atacar en este turno (paralisis, congelado o dormido)
      bool canRunMovement = attacker.Pokemon.OnStartTurn();
      if (!canRunMovement)
      {
         yield return ShowStatsMessages(attacker.Pokemon);
         yield return attacker.Hud.UpdatePokemonData();
         yield break;
      }
      yield return ShowStatsMessages(attacker.Pokemon);
      
      move.Pp--;
      yield return battleDialogBox.SetDialog($"{attacker.Pokemon.Base.Name} ha usado {move.Base.Name}");

      if (MoveHits(move, attacker.Pokemon, target.Pokemon))
      {
         yield return RunMoveAnims(attacker, target);

         if (move.Base.MoveType == MoveType.Stats)
         {
            yield return RunMoveStats(attacker.Pokemon, target.Pokemon, move.Base.Effects, move.Base.Target);
         }
         else
         {
            var damageDesc = target.Pokemon.ReceiveDamage(attacker.Pokemon, move);
            yield return target.Hud.UpdatePokemonData();
            yield return ShowDamageDescription(damageDesc);
         }
         
         //Chequear posibles estados secundarios
         if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0)
         {
            foreach (var sec in move.Base.SecondaryEffects)
            {
               if ((sec.Target == MoveTarget.Other && target.Pokemon.HP > 0) || 
                   (sec.Target == MoveTarget.Self && attacker.Pokemon.HP > 0))
               {
                  var rnd = Random.Range(0, 100);
                  if (rnd < sec.Chance)
                  {
                     yield return RunMoveStats(attacker.Pokemon, target.Pokemon, sec, sec.Target);
                  }
               }
               
            }
         }

         if (target.Pokemon.HP <= 0)
         {
            yield return HandlePokemonFainted(target);
         }
      }
      else
      {
         yield return battleDialogBox.SetDialog($"El ataque de {attacker.Pokemon.Base.Name} ha fallado");
      }

   }

   IEnumerator RunMoveAnims(BattleUnit attacker, BattleUnit target)
   {
      attacker.PlayAttackAnimation();
      SoundManager.SharedInstance.PlaySound(attackClip);
      yield return new WaitForSeconds(1f);
      
      target.PlayReceiveAttackAnimation();
      SoundManager.SharedInstance.PlaySound(damageClip);
      yield return new WaitForSeconds(1f);
   }
   
   IEnumerator RunMoveStats(Pokemon attacker, Pokemon target, MoveStatEffect effect, MoveTarget moveTarget)
   {
      
      //Stats Boosting
      foreach (var boost in effect.Boostings)
      {
         if (boost.target == MoveTarget.Self)
         {
            attacker.ApplyBoost(boost);
         }
         else
         {
            target.ApplyBoost(boost);
         }
      }
      //Status Condition
      if (effect.Status != StatusConditionID.none)
      {
         if (moveTarget == MoveTarget.Other)
         {
            target.SetConditionStatus(effect.Status);
         }
         else
         {
            attacker.SetConditionStatus(effect.Status);
         }
      }     
      
      //Volatile Status Condition
      if (effect.VolatileStatus != StatusConditionID.none)
      {
         if (moveTarget == MoveTarget.Other)
         {
            target.SetVolatileConditionStatus(effect.VolatileStatus);
         }
         else
         {
            attacker.SetVolatileConditionStatus(effect.VolatileStatus);
         }
      }

      yield return ShowStatsMessages(attacker);
      yield return ShowStatsMessages(target);
   }

   bool MoveHits(Move move, Pokemon attacker, Pokemon target)
   {
      if (move.Base.AlwaysHit)
      {
         return true;
      }
      
      float rnd = Random.Range(0, 100);
      float moveAcc = move.Base.Accuracy;

      float accuracy = attacker.StatsBoosted[Stat.Accuracy];
      float evasion = target.StatsBoosted[Stat.Evasion];
      
      float multiplierAcc = 1.0f + Mathf.Abs(accuracy) / 3.0f; //+-33%
      float multiplierEvs = 1.0f + Mathf.Abs(evasion) / 3.0f; //+-33%

      if (accuracy > 0)
      {
         moveAcc *= multiplierAcc;
      }
      else
      {
         moveAcc /= multiplierAcc;
      }

      if (evasion > 0)
      {
         moveAcc /= multiplierEvs;
      }
      else
      {
         moveAcc *= multiplierEvs;
      }
      
      return rnd < moveAcc;
   }

   IEnumerator ShowStatsMessages(Pokemon pokemon)
   {
      while (pokemon.StatusChangeMessages.Count > 0)
      {
         var message = pokemon.StatusChangeMessages.Dequeue();
         yield return battleDialogBox.SetDialog(message);
      }
   }

   IEnumerator RunAfterTurn(BattleUnit attacker)
   {
      if (state == BattleState.FinishBattle)
      {
         yield break;
      }
      yield return new WaitUntil(() => state == BattleState.RunTurn);
      //Comprobar estados alterados como quemadura o envenenamiento a final de turno.
      attacker.Pokemon.OnFinishTurn();
      yield return ShowStatsMessages(attacker.Pokemon);
      yield return attacker.Hud.UpdatePokemonData();
      if (attacker.Pokemon.HP<=0)
      {
         yield return HandlePokemonFainted(attacker);
      }
      
      yield return new WaitUntil(() => state == BattleState.RunTurn);
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
         if (type == BattleType.WildPokemon)
         {
            BattleFinish(true);
         }
         else //Batalla contra un entrenador Pokemon
         {
            var nextPokemon = trainerParty.GetFirstNonFaintedPokemon();
            if (nextPokemon!=null)
            {
               //Enviar el siguiente Pokemon a batalla
               StartCoroutine(YesNoChoice(nextPokemon));
            }
            else
            {
               BattleFinish(true);
            }
         }
         
      }
   }

   IEnumerator SendNextTrainerPokemonToBattle()
   {
      state = BattleState.Busy;
      var nextPokemon = trainerParty.GetFirstNonFaintedPokemon();
      enemyUnit.SetupPokemon(nextPokemon);
      yield return battleDialogBox.SetDialog($"{trainer.TrainerName} ha enviado a {nextPokemon.Base.Name}");
      state = BattleState.RunTurn;
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
      yield return new WaitForSeconds(1.0f);
      if (previousState == null)
      {
         state = BattleState.RunTurn;
      } else if (previousState == BattleState.YesNoChoice)
      {
         yield return SendNextTrainerPokemonToBattle();
      }
   }


   IEnumerator ThrowPokeball()
   {
      state = BattleState.Busy;

      if (type != BattleType.WildPokemon)
      {
         yield return battleDialogBox.SetDialog("No puedes robar los pokemon de otros entrenadores");
         state = BattleState.RunTurn;
         yield break;
      }

      yield return battleDialogBox.SetDialog($"Has lanzado una {pokeball.name}!");

      SoundManager.SharedInstance.PlaySound(pokeballClip);
      var pokeballInst = Instantiate(pokeball, playerUnit.transform.position +
                                               new Vector3(-2,0),
                                                Quaternion.identity);

      var pokeballSpt = pokeballInst.GetComponent<SpriteRenderer>();

      yield return pokeballSpt.transform.DOLocalJump(enemyUnit.transform.position +
                                        new Vector3(0, 1.5f), 2f, 
                                       1, 1f).WaitForCompletion();
      yield return enemyUnit.PlayCapturedAnimation();
      yield return pokeballSpt.transform.DOLocalMoveY(enemyUnit.transform.position.y - 2f, 0.3f).WaitForCompletion();

      var numberOfShakes = TryToCatchPokemon(enemyUnit.Pokemon);
      for (int i = 0; i < Mathf.Min(numberOfShakes, 3); i++)
      {
         yield return new WaitForSeconds(0.5f);
         yield return pokeballSpt.transform.DOPunchRotation(new Vector3(0, 0, 15f), 0.6f).WaitForCompletion();
      }

      if (numberOfShakes == 4)
      {
         yield return battleDialogBox.SetDialog($"¡{enemyUnit.Pokemon.Base.Name} capturado!");
         yield return pokeballSpt.DOFade(0, 1.5f).WaitForCompletion();

         if (playerParty.AddPokemonToParty(enemyUnit.Pokemon))
         {
            yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} se ha añadido a tu equipo.");
         }
         else
         {
            yield return battleDialogBox.SetDialog("En algun momento, lo mandaremos al PC de Bill...");
         }

         Destroy(pokeballInst);
         BattleFinish(true);
      }
      else
      {
         yield return new WaitForSeconds(0.5f);
         pokeballSpt.DOFade(0, 0.2f);
         yield return enemyUnit.PlayBreakOutAnimation();

         if (numberOfShakes <2)
         {
            yield return battleDialogBox.SetDialog($"¡{enemyUnit.Pokemon.Base.Name} ha escapado!");
         }
         else
         {
            yield return battleDialogBox.SetDialog("¡Casi lo has atrapado!");
         }
         Destroy(pokeballInst);
         state = BattleState.RunTurn;
      }
      
   }


   int TryToCatchPokemon(Pokemon pokemon)
   {
      float bonusPokeball = 1; // TODO: clase pokeball con su multiplicador
      float bonusStat = 1;     //TODO: stats para chequear condición de modificación
      float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * bonusPokeball * bonusStat/(3*pokemon.MaxHP);

      if (a >= 255)
      {
         return 4;
      }


      float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680/a));

      int shakeCount = 4;
      while (shakeCount<4)
      {
         if (Random.Range(0, 65535) >=b)
         {
            break;
         }
         else
         {
            shakeCount++;
         }
      }

      return shakeCount;
   }


   
   IEnumerator TryToEscapeFromBattle()
   {
      state = BattleState.Busy;

      if (type != BattleType.WildPokemon)
      {
         yield return battleDialogBox.SetDialog("No puedes huir de combates contra entrenadores Pokemon");
         state = BattleState.RunTurn;
         yield break;
      }

      escapeAttempts++;
      
      //Es contra un Pokemon salvaje
      int playerSpeed = playerUnit.Pokemon.Speed;
      int enemySpeed = enemyUnit.Pokemon.Speed;

      if (playerSpeed >= enemySpeed)
      {
         yield return battleDialogBox.SetDialog("Has escapado con éxito");
         yield return new WaitForSeconds(0.3f);
         OnBattleFinish(true);
      }
      else
      {
         int oddsScape = (Mathf.FloorToInt(playerSpeed * 128 / enemySpeed) + 30 * escapeAttempts) % 256;
         if (Random.Range(0, 256) < oddsScape)
         {
            yield return battleDialogBox.SetDialog("Has escapado con éxito");
            yield return new WaitForSeconds(1);
            OnBattleFinish(true);
         }
         else
         {
            yield return battleDialogBox.SetDialog("No puedes escapar");
            state = BattleState.RunTurn;
         }
      }

   }

   IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
   {
      yield return battleDialogBox.SetDialog($"{faintedUnit.Pokemon.Base.Name} se ha debilitado");
      SoundManager.SharedInstance.PlaySound(faintedClip);
      faintedUnit.PlayFaintAnimation();
      yield return new WaitForSeconds(1.5f);

      if (!faintedUnit.IsPlayer)
      {
         //EXP ++
         int expBase = faintedUnit.Pokemon.Base.ExpBase;
         int level = faintedUnit.Pokemon.Level;
         float multiplier = (type == BattleType.WildPokemon ? 1 : 1.5f);
         int wonExp = Mathf.FloorToInt(expBase * level * multiplier / 7);
         playerUnit.Pokemon.Experience += wonExp;
         yield return battleDialogBox.SetDialog(
            $"{playerUnit.Pokemon.Base.Name} ha ganado {wonExp} puntos de experiencia");
         yield return playerUnit.Hud.SetExpSmooth();
         yield return new WaitForSeconds(0.5f);
         
         //Chequear New Level
         while (playerUnit.Pokemon.NeedsToLevelUp())
         {
            SoundManager.SharedInstance.PlaySound(levelUpClip);
            playerUnit.Hud.SetLevelText();
            playerUnit.Pokemon.HasHPChanged = true;
            yield return playerUnit.Hud.UpdatePokemonData();
            yield return new WaitForSeconds(1);
            yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} sube de nivel!");
            

            //INTENTAR APRENDER UN NUEVO MOVIMIENTO
            var newLearnableMove = playerUnit.Pokemon.GetLearnableMoveAtCurrentLevel();
            if (newLearnableMove!=null)
            {
               if (playerUnit.Pokemon.Moves.Count < PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
               {
                  playerUnit.Pokemon.LearnMove(newLearnableMove);
                  yield return battleDialogBox.SetDialog(
                     $"{playerUnit.Pokemon.Base.Name} ha aprendido {newLearnableMove.Move.Name}");
                  battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
               }
               else
               {
                  yield return battleDialogBox.SetDialog(
                     $"{playerUnit.Pokemon.Base.Name} intenta aprender {newLearnableMove.Move.Name}");
                  yield return battleDialogBox.SetDialog(
                     $"Pero no puedo aprener más de {PokemonBase.NUMBER_OF_LEARNABLE_MOVES} movimientos");
                  yield return ChooseMovementToForget(playerUnit.Pokemon, newLearnableMove.Move);
                  yield return new WaitUntil(() => state!=BattleState.ForgetMovement);
               }
            }
            
            yield return playerUnit.Hud.SetExpSmooth(true);
         }
      }
      
      CheckForBattleFinish(faintedUnit);
   }


   IEnumerator ChooseMovementToForget(Pokemon learner, MoveBase newMove)
   {
      state = BattleState.Busy;
      yield return battleDialogBox.SetDialog("Selecciona el movimiento que quieres olvidar");
      selectMoveUI.gameObject.SetActive(true);
      selectMoveUI.SetMovements(learner.Moves.Select(mv => mv.Base).ToList(), newMove);
      moveToLearn = newMove;
      state = BattleState.ForgetMovement;

   }
   
}
