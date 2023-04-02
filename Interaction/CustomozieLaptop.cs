using UnityEngine;

public class CustomozieLaptop : MonoBehaviour
{
    [SerializeField] private Sprite useButtonSprite;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        var inst = Instantiate(_spriteRenderer.material);
        _spriteRenderer.material = inst;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var character = col.GetComponent<CharacterMover>();
        if (character != null && character.hasAuthority)
        {
            _spriteRenderer.material.SetFloat("_Hightlighted",1f);
            LobbyUIManager.Instance.SetUseButton(useButtonSprite,OnClickeUse);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var character = other.GetComponent<CharacterMover>();
        if (character != null && character.hasAuthority)
        {
            _spriteRenderer.material.SetFloat("_Hightlighted",0f);
            LobbyUIManager.Instance.UnSetUseButton();
        }
    }
    public void OnClickeUse()
    {
        LobbyUIManager.Instance.CustomizeUI.Open();
    }
}
