using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReportUI : MonoBehaviour
{
    [SerializeField] private Image deadBodyImg;
    [SerializeField] private Material material;

    public void Open(EPlayerColor deadbodyColor)
    {
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMovable = false;

        Material inst = Instantiate(material);
        deadBodyImg.material = inst;
        
        gameObject.SetActive(true);
        deadBodyImg.material.SetColor("_PlayerColor",PlayerColor.GetColor(deadbodyColor));
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
