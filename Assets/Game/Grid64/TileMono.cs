using UnityEngine;

public class TileMono : MonoBehaviour, ITile
{
    public Vector2Int _gridPosition = new Vector2Int(0, 0);
    public Grid64Mono _grid64Mono;

    public virtual void DestroyTile(Grid64Mono grid)
    {
    }

    public virtual bool CanMoveTo(Vector2Int moveDirection)
    {
        return true;
    }

    public virtual bool CanMoveFrom(Vector2Int moveDirection)
    {
        return true;
    }

    public virtual bool CanInteractTo(Vector2Int moveDirection)
    {
        return false;
    }

    public virtual bool CanInteractFrom(Vector2Int moveDirection)
    {
        return false;
    }

    public virtual CarryableSO GetCarryable()
    {
        return null;
    }

    public virtual CarryableSO GetReceivable()
    {
        return null;
    }

    public virtual bool OnInteract(PlayerMono player, Vector2Int moveDirection)
    {
        return false;
    }
}