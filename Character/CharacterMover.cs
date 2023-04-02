using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;


public class CharacterMover : NetworkBehaviour
{
    protected Animator ani;
    private bool isMovable; // 캐릭터 이동 가능 상태 확인

    public bool IsMovable
    {
        get { return isMovable; }
        set
        {
            if (!value)
            {
                ani.SetBool("IsMove",false);
            }

            isMovable = value;
        }
    }
    [SyncVar] // 네트워크 동기화
    public float speed = 2f;

    protected SpriteRenderer _spriteRenderer;
    
    // hook = SyncVar로 동기화 된 변수가 서버에서 변경되었을때,
    // hook으로 등록한 함수가 클라이언트에서 호출되도록 만들어주는 기능
    [SyncVar (hook = nameof(SetPlayerColor_Hook))]
    public EPlayerColor playerColor;

    [SerializeField]
    private float characterSize = 0.5f;

    [SerializeField] private float camSize = 2.5f;
    public void SetPlayerColor_Hook(EPlayerColor oldColor, EPlayerColor newColor)
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        _spriteRenderer.material.SetColor("_PlayerColor",PlayerColor.GetColor(newColor));
    }
    [SyncVar(hook = nameof(SetNickname_Hook))]
    public string nickname;

    [SerializeField]
    protected Text nicknameText;

    public void SetNickname_Hook(string _, string value)
    {
        nicknameText.text = value;
    }
    public virtual void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.material.SetColor("_PlayerColor",PlayerColor.GetColor(playerColor));
        
        ani = GetComponent<Animator>();
        if (hasAuthority)
        {
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            cam.orthographicSize = camSize;
            // orthographicSize 은 orthographic 모드일때 카메라 크기에 절반
            // orthographicSize는 카메라 수직크기의 절반 수평 화면 크기는 뷰포트의 화면 비율(aspect ratio)에 따라 달라집니다
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        //hasAuthority == 클라이언트가 이 오브젝트에 대한 권하을 가지고 있다면
        if (hasAuthority && isMovable)
        {
            bool isMove = false;
            if (PlayerSettings.ControlType == EControlType.KeyboardMouse)
            {
                Vector3 dir = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 
                    Input.GetAxis("Vertical"), 0f),1f);
                if (dir.x < 0f) transform.localScale = new Vector3(-characterSize, characterSize, 1f);
                else if (dir.x > 0f) transform.localScale = new Vector3(characterSize, characterSize, 1f);
                transform.position += dir * speed * Time.deltaTime;
                isMove = dir.magnitude != 0;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 dir = (Input.mousePosition - new Vector3(Screen.width * 0.5f,
                        Screen.height * 0.5f, 0)).normalized;
                    if (dir.x < 0f) transform.localScale = new Vector3(-characterSize, characterSize, 1f);
                    else if (dir.x > 0f) transform.localScale = new Vector3(characterSize, characterSize, 1f);
                    transform.position += dir * speed * Time.deltaTime;
                    isMove = dir.magnitude != 0;
                }
            }
            ani.SetBool("IsMove",isMove);
        }

        if (transform.localScale.x < 0)
        {
            nicknameText.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (transform.localScale.x > 0)
        {
            nicknameText.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
