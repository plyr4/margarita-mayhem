using System;
using UnityEngine;

public class IceRestock : TileMono
{
    public Func<IceRestock, CarryableSO, bool> OnInteractAction;
    public CarryableSO _receivable;

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
        return OnInteractAction.Invoke(this, player._carrying);
    }
}