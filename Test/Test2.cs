using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : Test
{
    protected void SetInt(int x, int y)
    {
        base.Testab(x,y);
        Debug.Log(x*y);
    }

    private void Start()
    {
        SetInt(1,5);
    }
}
