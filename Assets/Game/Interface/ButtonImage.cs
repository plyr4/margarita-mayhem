using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonImage : MonoBehaviour
{
    public Image _image;
    [Range(0,1)]
    public float _hitAlpha = 0.1f;
    
    private void Start()
    {
        _image = GetComponent<Image>();
        _image.alphaHitTestMinimumThreshold = _hitAlpha;
    }
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.alphaHitTestMinimumThreshold = _hitAlpha;
    }

    private void Update()
    {
        _image.alphaHitTestMinimumThreshold = _hitAlpha;
    }
}
