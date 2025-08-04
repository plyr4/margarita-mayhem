using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DishSink : TileMono
{
    public Action<DishSink> OnInteractAction;
    public CarryableSO _receivable;
    public TextMeshPro _numWashedDishesText;
    public GameObject _plusCounterAnimationParent;
    public TextMeshPro _plusCounterText;
    public int _numWashedDishes = 0;
    public float _plusCounterAnimationHeight = 1f;
    public float _plusCounterAnimationDuration = 1f;
    public Ease _plusCounterAnimationEase = Ease.OutCubic;

    public void Start()
    {
        _plusCounterAnimationParent.transform.localPosition = Vector3.zero;
        _plusCounterText.gameObject.SetActive(false);
    }

    public override CarryableSO GetReceivable()
    {
        return _receivable;
    }

    public override bool CanInteractFrom(Vector2Int moveDirection)
    {
        if (moveDirection.y > 0)
        {
            return true;
        }

        return false;
    }

    public override bool OnInteract(PlayerMono player, Vector2Int moveDirection)
    {
        player.HandleDrop(moveDirection, this);
        if (player._carrying != _receivable) return false;
        OnInteractAction?.Invoke(this);
        _numWashedDishes++;

        SpriteTextWriter.WriteText(_numWashedDishesText, $"{_numWashedDishes}");
        SpriteTextWriter.WriteText(_plusCounterText, $"+1");
        _plusCounterAnimationParent.transform.localPosition = Vector3.zero;

        _plusCounterAnimationParent.transform
            .DOLocalMoveY(_plusCounterAnimationHeight, _plusCounterAnimationDuration)
            .SetEase(_plusCounterAnimationEase)
            .OnStart(() => { _plusCounterText.gameObject.SetActive(true); })
            .OnComplete(() => { _plusCounterText.gameObject.SetActive(false); })
            ;

        return true;
    }
}