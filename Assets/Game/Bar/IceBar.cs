using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IceBar : MonoBehaviour
{
    private IceRestock[] _restocks;
    public List<Sprite> _sprites;
    [SerializeField]
    [Range(0, 1f)]
    private float _value;
    public SpriteRenderer _sprite;
    public float _depletionSpeed = 0.1f;
    public Blink _outOfStockBlink;
    public int _spriteIndex;
    public CarryableSO _emptyCarryable;
    public CarryableSO _receivable;
    public CarryableSO _glasswareCarryable;

    private void Start()
    {
        _restocks = FindObjectsOfType<IceRestock>();
        foreach (IceRestock iceRestock in _restocks)
        {
            if (iceRestock != null)
            {
                iceRestock.OnInteractAction += RestockIce;
            }
        }
    }

    public void Update()
    {
        _value = BarDeplete.DepleteValue(_value, _depletionSpeed);
        _spriteIndex = BarDeplete.GetDepletionSpriteIndex(_value, _sprites);
        _sprite.sprite = _sprites[_spriteIndex];
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

    private bool RestockIce(IceRestock iceRestock, CarryableSO carryable)
    {
        if (carryable == _receivable)
        {
            SoundManager.Instance.PlayRestockItem();

            _value = 1f;
            return true;
        }

        if (carryable != null) SoundManager.Instance.PlayPlayerCollision();

        if (carryable == _glasswareCarryable)
        {
            // broken glass ruins ice
            _value = 0f;
            return true;
        }

        return false;
    }

    public bool IsStocked()
    {
        return _value > 0f;
    }
}