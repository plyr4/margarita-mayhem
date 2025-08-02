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
        int spriteIndex = BarDeplete.GetDepletionSpriteIndex(_value, _sprites);
        _sprite.sprite = _sprites[spriteIndex];
    }

    private void RestockIce(IceRestock iceRestock)
    {
        _value = 1f;
    }
}