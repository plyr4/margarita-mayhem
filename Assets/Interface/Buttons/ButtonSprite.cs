using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(Button))]
public class ButtonSprite : Graphic
{
    public SpriteRenderer _targetSpriteRenderer;
    public Sprite _normalSprite;
    public Sprite _highlightedSprite;
    public Sprite _selectedSprite;
    public Sprite _pressedSprite;
    public Sprite _disabledSprite;

    public Button _button;
    private ColorBlock _colors;

    protected override void Start()
    {
        base.Start();
        _button = GetComponent<Button>();
        _colors = _button.colors;
    }

    public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
    {
        if (_targetSpriteRenderer == null)
            return;

        Sprite targetSprite = GetSpriteForColor(targetColor);

        if (_targetSpriteRenderer.sprite != targetSprite)
        {
            _targetSpriteRenderer.sprite = targetSprite;
        }
    }

    private Sprite GetSpriteForColor(Color color)
    {
        if (_button == null) _button = GetComponent<Button>();
        if (_button == null || _targetSpriteRenderer == null)
            return _normalSprite;
        if (!_button.interactable)
            return _disabledSprite ?? _normalSprite;

        if (color == _colors.normalColor)
            return _normalSprite;

        if (color == _colors.highlightedColor)
            return _highlightedSprite ?? _normalSprite;

        if (color == _colors.selectedColor)
            return _selectedSprite ?? _normalSprite;

        if (color == _colors.pressedColor)
            return _pressedSprite ?? _normalSprite;

        if (color == _colors.disabledColor)
            return _disabledSprite ?? _normalSprite;

        return _normalSprite;
    }

    public override Color color
    {
        get => Color.white;
        set { }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}