using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPipeLightStarter : MonoBehaviour
{
   private WaitForSeconds ws = new WaitForSeconds(1f);
   private List<WeaponPipeLights> lights = new List<WeaponPipeLights>();

   private void Start()
   {
      for (int i = 0; i < transform.childCount; i++)
      {
         var child = transform.GetChild(i).GetComponent<WeaponPipeLights>();
         if (child)
         {
            lights.Add(child);
         }
      }

      StartCoroutine(TurnOnPipeLight());
   }

   private IEnumerator TurnOnPipeLight()
   {
      while (true)
      {
         yield return ws;
         foreach (var child in lights)
         {
            child.TurnOnLight();
         }
      }
   }
}
