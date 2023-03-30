using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCrew : MonoBehaviour
{
    public EPlayerColor PlayerColor; // 크루원의 색을 저장할 변수
    
    private SpriteRenderer _spriteRenderer;
    private Vector3 direction; // 크루가 날아다닐 방향
    private float floatingSpeed; //날아다니는 속도
    private float rotateSpeed; // 회전속도

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetFloatingCrew(Sprite sprite, EPlayerColor playerColor, Vector3 direction, float floatingSpeed,
        float rotateSpeed, float size)
    {
        this.PlayerColor = playerColor;
        this.direction = direction;
        this.floatingSpeed = floatingSpeed;
        this.rotateSpeed = rotateSpeed;

        _spriteRenderer.sprite = sprite;
        _spriteRenderer.material.SetColor("_PlayerColor",global::PlayerColor.GetColor(playerColor));

        transform.localScale = new Vector3(size, size, size);
        _spriteRenderer.sortingOrder = (int) Mathf.Lerp(1, 32767, size);
    }

    private void Update()
    {
        transform.position += direction * floatingSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(transform.root.eulerAngles + new Vector3(0f,0f,rotateSpeed));
    }
}
