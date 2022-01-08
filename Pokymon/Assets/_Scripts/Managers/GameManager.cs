using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum GameState { Travel, Battle , Dialog, Cutscene}

[RequireComponent(typeof(ColorManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Camera worldMainCamera;
    [SerializeField] private Image transitionPanel;
    
    private GameState _gameState;

    public AudioClip worldClip, battleClip;

    public static GameManager SharedInstance;

    private TrainerController trainer;
    
    private void Awake()
    {
        if (SharedInstance!= null)
        {
            Destroy(this);
        }
        SharedInstance = this;
        _gameState = GameState.Travel;
    }

    void Start()
    {
        StatusConditionFactory.InitFactory();
        SoundManager.SharedInstance.PlayMusic(worldClip);
        playerController.OnPokemonEncountered += StartPokemonBattle;
        playerController.OnEnterTrainersFov += (Collider2D trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                _gameState = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
        };
        battleManager.OnBattleFinish += FinishPokemonBattle;
        DialogManager.SharedInstance.OnDialogStart += () =>
        {
            _gameState = GameState.Dialog;
        };

        DialogManager.SharedInstance.OnDialogFinish += () =>
        {
            if(_gameState == GameState.Dialog)
                _gameState = GameState.Travel;
            //TODO: si el diálogo es con un entrenador pokemon, no vamos a travel, si no a battle
        };
    }

    void StartPokemonBattle()
    {
        StartCoroutine(FadeInBattle());
    }
    
    public void StartTrainerBattle(TrainerController trainer)
    {
        this.trainer = trainer;
        StartCoroutine(FadeInTrainerBattle(trainer));
    }

    IEnumerator FadeInBattle()
    {
        SoundManager.SharedInstance.PlayMusic(battleClip);
        _gameState = GameState.Battle;

        yield return transitionPanel.DOFade(1.0f, 1.0f).WaitForCompletion();
        yield return new WaitForSeconds(0.2f);
        
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();  
        
        var wildPokemonCopy  = new Pokemon(wildPokemon.Base, wildPokemon.Level);
        
        battleManager.HandleStartBattle(playerParty, wildPokemonCopy);
        yield return transitionPanel.DOFade(0.0f, 1.0f).WaitForCompletion();
        
    }    
    
    
    IEnumerator FadeInTrainerBattle(TrainerController trainerController)
    {
        SoundManager.SharedInstance.PlayMusic(battleClip);
        _gameState = GameState.Battle;

        yield return transitionPanel.DOFade(1.0f, 1.0f).WaitForCompletion();
        yield return new WaitForSeconds(0.2f);
        
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainerController.GetComponent<PokemonParty>();
        
        battleManager.HandleStartTrainerBattle(playerParty, trainerParty);
        yield return transitionPanel.DOFade(0.0f, 1.0f).WaitForCompletion();
        
    }

    void FinishPokemonBattle(bool playerHasWon)
    {
        if (trainer != null && playerHasWon)
        {
            trainer.AfterTrainerLostBattle();
            trainer = null;
        }
        StartCoroutine(FadeOutBattle());
    }

    IEnumerator FadeOutBattle()
    {
        
        yield return transitionPanel.DOFade(1.0f, 1.0f).WaitForCompletion();
        yield return new WaitForSeconds(0.2f);

        SoundManager.SharedInstance.PlayMusic(worldClip);
        
        
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);
        
        yield return transitionPanel.DOFade(0.0f, 1.0f).WaitForCompletion();
        _gameState = GameState.Travel;
    }


    private void Update()
    {
        if (_gameState == GameState.Travel)
        {
           playerController.HandleUpdate();
        } else if (_gameState == GameState.Battle)
        {
            battleManager.HandleUpdate();
        }else if (_gameState == GameState.Dialog)
        {
            DialogManager.SharedInstance.HandleUpdate();
        }
    }
}
