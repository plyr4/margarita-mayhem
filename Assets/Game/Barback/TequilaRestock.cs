using System;
using UnityEngine;

public class TequilaRestock : TileMono
{
    public Action<TequilaRestock> OnInteractAction;
    public CarryableSO _receivable;

    public override CarryableSO GetReceivable()
    {
        return _receivable;
    }

    public override bool CanInteractTo(Vector2Int moveDirection)
    {
        // restock is at the bottom of screen so the player is above it when you interact with it "from above"
        if (moveDirection.y < 0)
        {
            return true;
        }

        return false;
    }
    
    public override bool OnInteract(PlayerMono player, Vector2Int moveDirection)
    {
        if (player._carrying != _receivable) return false;
        OnInteractAction?.Invoke(this);
        return true;
    }
}