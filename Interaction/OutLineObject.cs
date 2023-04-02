using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLineObject : MonoBehaviour
{
   private SpriteRenderer _spriteRenderer;
   [SerializeField] private Color OutlineColor;

   private void Start()
   {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      var inst = Instantiate(_spriteRenderer.material);
      _spriteRenderer.material = inst;
      _spriteRenderer.material.SetColor("_OutlineColor",OutlineColor);
   }

   private void OnTriggerEnter2D(Collider2D col)
   {
      var character = col.GetComponent<CharacterMover>();
      if (character != null && character.hasAuthority)
      {
         _spriteRenderer.enabled = true;
      }
   }

   private void OnTriggerExit2D(Collider2D other)
   {
      var character = other.GetComponent<CharacterMover>();
      if (character != null && character.hasAuthority)
      {
         _spriteRenderer.enabled = false;
      }
   }
}
