using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum NpcState { Idle, Walking, Talking }

public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog dialog;
    private NpcState state;
    [SerializeField] private float idleTime = 3f;
    private float idleTimer = 0f;
    [SerializeField] private List<Vector2> moveDirections;
    private int currentDirection;
    
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }


    public void Interact(Vector3 source)
    {
        if (state == NpcState.Idle)
        {
            state = NpcState.Talking;
            character.LookTowards(source);
            DialogManager.SharedInstance.ShowDialog(dialog, () =>
            {
                idleTimer = 0f;
                state = NpcState.Idle;
            });
        }
    }


    private void Update()
    {
        
        if (state == NpcState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleTime)
            {
                idleTimer = 0f;
                StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }


    IEnumerator Walk()
    {
        state = NpcState.Walking;
        var oldPos = transform.position;
        var direction = Vector2.zero;
        if (moveDirections.Count > 0 )
        {
            direction = moveDirections[currentDirection];
        }
        else
        {
            direction = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        }

        yield return character.MoveTowards(direction);
        if (moveDirections.Count > 0 && transform.position != oldPos)
        {
            currentDirection = (currentDirection + 1) % moveDirections.Count; 
        }
        state = NpcState.Idle;
    }
}
