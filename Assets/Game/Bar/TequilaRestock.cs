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
        if (player._carrying != _receivable)
        {
            if (player._carrying != null) SoundManager.Instance.PlayPlayerCollision();
            return false;
        }

        SoundManager.Instance.PlayRestockItem();
        OnInteractAction?.Invoke(this);
        return true;
    }
}