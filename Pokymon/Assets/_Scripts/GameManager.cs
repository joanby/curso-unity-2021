using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum GameState { Travel, Battle }

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Camera worldMainCamera;
    
    private GameState _gameState;

    private void Awake()
    {
        _gameState = GameState.Travel;
    }

    void Start()
    {
        playerController.OnPokemonEncountered += StartPokemonBattle;
        battleManager.OnBattleFinish += FinishPokemonBattle;
    }

    void StartPokemonBattle()
    {
        _gameState = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();  
        
        var wildPokemonCopy  = new Pokemon(wildPokemon.Base, wildPokemon.Level);
        
        battleManager.HandleStartBattle(playerParty, wildPokemonCopy);
    }

    void FinishPokemonBattle(bool playerHasWon)
    {
        _gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);
        if (!playerHasWon)
        {
            //TODO: diferenciar entre victoria y derrota
        }
    }
    
    private void Update()
    {
        if (_gameState == GameState.Travel)
        {
           playerController.HandleUpdate();
        } else if (_gameState == GameState.Battle)
        {
            battleManager.HandleUpdate();
        }
    }
}
