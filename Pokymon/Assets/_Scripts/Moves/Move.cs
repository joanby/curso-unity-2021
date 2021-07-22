using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private MoveBase _base;
    private int _pp;

    public MoveBase Base
    {
        get => _base;
        set => _base = value;
    }

    public int Pp
    {
        get => _pp;
        set => _pp = value;
    }

    public Move(MoveBase mBase)
    {
        Base = mBase;
        Pp = mBase.PP;
    }
}
