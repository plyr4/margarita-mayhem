using UnityEngine;

public class IceStock : TileMono
{
    public CarryableSO _carryable;

    public override CarryableSO GetCarryable()
    {
        return _carryable;
    }

    public override bool CanInteractTo(Vector2Int moveDirection)
    {
        if (moveDirection.y < 0)
        {
            return true;
        }

        return false;
    }

    public override bool OnInteract(PlayerMono player, Vector2Int moveDirection)
    {
        player.HandlePickup(moveDirection, this);
        // OnInteractAction?.Invoke(this);
        
        SoundManager.Instance.PlayPickupItem();
        
        return true;
    }
}