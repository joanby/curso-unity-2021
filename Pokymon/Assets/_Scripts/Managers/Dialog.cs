using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialog 
{
  [SerializeField] private List<string> lines;

  public List<string> Lines => lines;
}
