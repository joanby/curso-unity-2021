using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterAnimator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private string trainerName;
    [SerializeField] private Sprite trainerSprite;
    
    public string TrainerName => trainerName;

    public Sprite TrainerSprite => trainerSprite;
    
    private Vector2 input;

    private Character _character;
    

    public event Action OnPokemonEncountered;

    public event Action<Collider2D> OnEnterTrainersFov;
    
    private float timeSinceLastClick;
    [SerializeField] float timeBetweenClicks = 1.0f;


    void Awake()
    {
        _character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;
        
        if (!_character.IsMoving)
        {
            /*input = new Vector2(Input.GetAxisRaw("Horizontal"), 
                Input.GetAxisRaw("Vertical"));*/
            
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input != Vector2.zero)
            {
                StartCoroutine(_character.MoveTowards(input, OnMoveFinish));
            }
        }
        
        _character.HandleUpdate();

        if (Input.GetAxisRaw("Submit")!=0)
        {
            if(timeSinceLastClick >= timeBetweenClicks)
                Interact();
        }
    }

    void OnMoveFinish()
    {
        CheckForPokemon();
        CheckForInTrainersFoV();
    }
    private void Interact()
    {
        timeSinceLastClick = 0;
        
        var facingDirection = new Vector3(_character.Animator.MoveX, _character.Animator.MoveY);
        var interactPosition = transform.position + facingDirection;
        
        Debug.DrawLine(transform.position, interactPosition, Color.magenta, 1.0f);
        var collider = Physics2D.OverlapCircle(interactPosition, 0.2f, GameLayers.SharedInstance.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform.position);
        }
    }


[SerializeField] float verticalOffset = 0.2f;

    private void CheckForPokemon()
    {
        if (Physics2D.OverlapCircle(transform.position-new Vector3(0, verticalOffset), 0.2f, GameLayers.SharedInstance.PokemonLayer)!=null)
        {
            if (Random.Range(0, 100) < 15)
            {
                _character.Animator.IsMoving = false;
                OnPokemonEncountered();
            }
        }
    }
    
    private void CheckForInTrainersFoV()
    {
        var collider = Physics2D.OverlapCircle(transform.position - new Vector3(0, verticalOffset),
            0.2f, GameLayers.SharedInstance.FovLayer);
        if (collider!=null)
        {
            _character.Animator.IsMoving = false;
            OnEnterTrainersFov?.Invoke(collider);
        }
    }

}
