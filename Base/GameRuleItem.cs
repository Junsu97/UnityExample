using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuleItem : MonoBehaviour
{
    [SerializeField] private GameObject inActiveObject;

    void Start()
    {
        if(!AmongUsRoomPlayer.MyRoomPlayer.isServer)
        {
            inActiveObject.SetActive(false);
        }
    }
}
