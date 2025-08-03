using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenTransitionSpriteAnimator : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    private float _value = 0f;
    private SpriteRenderer _spriteRenderer;
    public List<Sprite> _sprites;

    private void Update()
    {
        if (Application.isPlaying) return;

        UpdateSprite();
    }

    public void SetProgress(float value)
    {
        _value = Mathf.Clamp01(value);
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (_sprites.Count > 0)
        {
            int spriteIndex = Mathf.Clamp(Mathf.FloorToInt(_value * _sprites.Count), 0, _sprites.Count - 1);
            _spriteRenderer.sprite = _sprites[spriteIndex];
        }
    }
}