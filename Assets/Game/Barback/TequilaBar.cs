using System.Collections.Generic;
using UnityEngine;

public class TequilaBar : MonoBehaviour
{
    private TequilaRestock[] _restocks;
    public List<Sprite> _sprites;
    [SerializeField]
    [Range(0, 1f)]
    public float _value;
    public SpriteRenderer _sprite;
    public float _depletionSpeed = 0.1f;
    public Blink _outOfStockBlink;
    public CarryableSO _emptyCarryable;

    private void Start()
    {
        _restocks = FindObjectsOfType<TequilaRestock>();
        foreach (TequilaRestock tequilaRestock in _restocks)
        {
            if (tequilaRestock != null)
            {
                tequilaRestock.OnInteractAction += RestockTequila;
            }
        }
    }

    public void Update()
    {
        _value = BarDeplete.DepleteValue(_value, _depletionSpeed);
        int spriteIndex = BarDeplete.GetDepletionSpriteIndex(_value, _sprites);
        _sprite.sprite = _sprites[spriteIndex];
        HandleBlink();
    }

    private void HandleBlink()
    {
        if (!_outOfStockBlink.gameObject.activeInHierarchy && _value <= 0f)
        {
            _outOfStockBlink.gameObject.SetActive(true);
            _outOfStockBlink.StartBlinking();
        }

        if (_outOfStockBlink.gameObject.activeInHierarchy && _value > 0f)
        {
            _outOfStockBlink.gameObject.SetActive(false);
        }
    }

    private void RestockTequila(TequilaRestock tequilaRestock)
    {
        _value += 0.5f;
        if (_value > 1f)
        {
            _value = 1f;
        }
    }

    public bool IsStocked()
    {
        return _value > 0f;
    }
}