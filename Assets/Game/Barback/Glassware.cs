using UnityEngine;

public class Glassware : TileMono
{
    public override bool CanInteractTo(Vector2Int moveDirection)
    {
        if (moveDirection.y < 0)
        {
            return true;
        }

        return false;
    }

    public override bool CanInteractFrom(Vector2Int moveDirection)
    {
        if (moveDirection.y > 0)
        {
            return true;
        }

        return false;
    }

    public override bool CanMoveTo(Vector2Int moveDirection)
    {
        if (moveDirection.y != 0)
        {
            return false;
        }

        return true;
    }

    public override bool CanMoveFrom(Vector2Int moveDirection)
    {
        if (moveDirection.y != 0)
        {
            return false;
        }

        return true;
    }
}