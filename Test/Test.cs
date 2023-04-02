using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Test : MonoBehaviour
{
    public int x;
    public int y;

    protected virtual void Testab(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
