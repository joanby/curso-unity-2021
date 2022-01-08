using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FacingDirection { Down, Up, Left, Right}

public class CharacterAnimator : MonoBehaviour
{

    public float MoveX, MoveY;
    public bool IsMoving;

    [SerializeField] private List<Sprite> walkDownSprites, walkUpSprites, walkLeftSprites, walkRightSprites;
    [SerializeField] private FacingDirection defaultDirection = FacingDirection.Down;
    
    public FacingDirection DefaultDirection => defaultDirection;
    
    private CustomAnimator walkDownAnim, walkUpAnim, walkLeftAnim, walkRightAnim;
    private CustomAnimator currentAnimator;
    private SpriteRenderer renderer;

    private bool wasPreviouslyMoving = false;
    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new CustomAnimator(renderer, walkDownSprites);
        walkUpAnim = new CustomAnimator(renderer, walkUpSprites);
        walkLeftAnim = new CustomAnimator(renderer, walkLeftSprites);
        walkRightAnim = new CustomAnimator(renderer, walkRightSprites);
        SetFacingDirection(defaultDirection);
        
        currentAnimator = walkDownAnim;
    }

    private void Update()
    {
        var previousAnimator = currentAnimator;
        if (MoveX == 1)
        {
            currentAnimator = walkRightAnim;
        }
        else if (MoveX == -1)
        {
            currentAnimator = walkLeftAnim;
        }
        else if (MoveY == 1)
        {
            currentAnimator = walkUpAnim;
        }
        else if (MoveY == -1)
        {
            currentAnimator = walkDownAnim;
        }

        if (previousAnimator != currentAnimator || IsMoving != wasPreviouslyMoving)
        {
            currentAnimator.Start();
        }

        if (IsMoving){
            currentAnimator.HandleUpdate();
        }else
        {
            renderer.sprite = currentAnimator.AnimFrames[0];//Truco de Idle
        }

        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection direction)
    {
        if (direction == FacingDirection.Down)
        {
            MoveY = -1;
        } else if (direction == FacingDirection.Up)
        {
            MoveY = 1;
        }else if (direction == FacingDirection.Left)
        {
            MoveX = -1;
        }else if (direction == FacingDirection.Right)
        {
            MoveX = 1;
        }
    }
}
