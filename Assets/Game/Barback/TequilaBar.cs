using System.Collections.Generic;
using UnityEngine;

public class TequilaBar : MonoBehaviour
{
    private TequilaRestock[] _restocks;
    public List<Sprite> _sprites;
    [SerializeField]
    [Range(0, 1f)]
    private float _value;
    public SpriteRenderer _sprite;
    public float _depletionSpeed = 0.1f;

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
    }

    private void RestockTequila(TequilaRestock tequilaRestock)
    {
        _value += 0.5f;
        if (_value > 1f)
        {
            _value = 1f;
        }
    }
}