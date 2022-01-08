using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator
{
  private SpriteRenderer renderer;
  private List<Sprite> animFrames;
  private float frameRate;
  
  private int currentFrame;
  private float timer;

  public List<Sprite> AnimFrames => animFrames;
  public CustomAnimator(SpriteRenderer renderer, List<Sprite> animFrames, float frameRate = 0.25f)
  {
      this.renderer = renderer;
      this.animFrames = animFrames;
      this.frameRate = frameRate;
  }

  public void Start()
  {
      currentFrame = 0;
      timer = 0f;
      renderer.sprite = animFrames[currentFrame];
  }

  public void HandleUpdate()
  {
      timer += Time.deltaTime;
      if (timer > frameRate)
      {
          currentFrame = (currentFrame + 1) % animFrames.Count;
          renderer.sprite = animFrames[currentFrame];
          timer -= frameRate;
      }
  }
  
}
