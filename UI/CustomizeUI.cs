using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
public class CustomizeUI : MonoBehaviour
{
    [SerializeField] private Button colorButton;
    [SerializeField] private GameObject colorPanel;

    [SerializeField] private Button gameRuleButton;
    [SerializeField] private GameObject gameRulePanel;
    
    [SerializeField] private Image chracterPreview;

    [SerializeField] private List<ColorSelectButton> _colorSelectButtons;
    void Start()
    {
        var inst = Instantiate(chracterPreview.material);
        chracterPreview.material = inst;
    }

    private void OnEnable()
    {
        UpdateColorButton();

        var roomSlots = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;
        foreach (var player in roomSlots)
        {
            var aPlayer = player as AmongUsRoomPlayer;
            if (aPlayer.isLocalPlayer)
            {
                UpdatePreviewColor(aPlayer.playerColor);
                break;
            }
        }
    }

    public void ActiveColorPanel()
    {
        colorButton.image.color = new Color(0f, 0f, 0f, 0.75f);
        gameRuleButton.image.color = new Color(0f, 0f, 0f, 0.25f);
        
        colorPanel.SetActive(true);
        gameRulePanel.SetActive(false);
    }

    public void ActiveGameRulePanel()
    {
        colorButton.image.color = new Color(0f, 0f, 0f, 0.25f);
        gameRuleButton.image.color = new Color(0f, 0f, 0f, 0.75f);
        
        colorPanel.SetActive(false);
        gameRulePanel.SetActive(true);
    }

    public void UpdateColorButton()
    {
        // 커스터마이즈UI가 활성화했을 때 컬러선택패널이 자동으로 뜨도록 함. 
        ActiveColorPanel();
        
        var roomSloats = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;

        for (int i = 0; i < _colorSelectButtons.Count; i++)
        {
            _colorSelectButtons[i].SetInteractable(true);
        }

        foreach (var player in roomSloats)
        {
            var aPlayer = player as AmongUsRoomPlayer;
            _colorSelectButtons[(int)aPlayer.playerColor].SetInteractable(false);
        }
    }

    public void UpdateSelectColorButton(EPlayerColor color)
    {
        _colorSelectButtons[(int)color].SetInteractable(false);
    }

    public void UpdateUnSelectColorButton(EPlayerColor color)
    {
        _colorSelectButtons[(int)color].SetInteractable(true);
    }

    public void UpdatePreviewColor(EPlayerColor color)
    {
        chracterPreview.material.SetColor("_PlayerColor",PlayerColor.GetColor(color));
    }

    public void OnClickColorButton(int index)
    {
        if (_colorSelectButtons[index].isInteractable)
        {
            AmongUsRoomPlayer.MyRoomPlayer.CmdSetPlayerColor((EPlayerColor)index);
            UpdatePreviewColor((EPlayerColor)index);
        }
    }

    public void Open()
    {
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMovable = false;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMovable = true;
        gameObject.SetActive(false);
    }
}
