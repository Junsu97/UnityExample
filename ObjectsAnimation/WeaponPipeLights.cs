using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPipeLights : MonoBehaviour
{
    private Animator ani;

    private WaitForSeconds ws = new WaitForSeconds(0.15f);

    private List<WeaponPipeLights> lithgs = new List<WeaponPipeLights>();

    private void Start()
    {
        ani = GetComponent<Animator>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).GetComponent<WeaponPipeLights>();
            if (child)
            {
                lithgs.Add(child);
            }
        }
    }

    public void TurnOnLight()
    {
        ani.SetTrigger("On");
        StartCoroutine(TrunOnLightsAtChild());
    }

    private IEnumerator TrunOnLightsAtChild()
    {
        yield return ws;

        foreach (var child in lithgs)
        {
            child.TurnOnLight();
        }
    }
}
