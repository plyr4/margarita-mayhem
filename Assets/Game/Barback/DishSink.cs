using System;
using TMPro;
using UnityEngine;

public class DishSink : TileMono
{
    public Action<DishSink> OnInteractAction;
    public CarryableSO _receivable;
    public TextMeshPro _text;
    public int _numWashedDishes = 0;

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

        SpriteTextWriter.WriteText(_text, $"{_numWashedDishes}");
        return true;
    }
}