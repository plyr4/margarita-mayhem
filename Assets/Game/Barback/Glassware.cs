using System.Collections;
using UnityEngine;

public class Glassware : TileMono
{
    public CarryableSO _carryable;
    public float _respawnDelay = 20f;
    public bool _autoRespawn = true;
    public bool _disabledOnStart;

    public void Start()
    {
        if (_disabledOnStart)
        {
            gameObject.SetActive(false);
        }
    }

    public override CarryableSO GetCarryable()
    {
        return _carryable;
    }

    public override bool OnInteract(PlayerMono player, Vector2Int moveDirection)
    {
        if (!gameObject.activeInHierarchy) return false;

        player.HandlePickup(moveDirection, this);
        // OnInteractAction?.Invoke(this);
        gameObject.SetActive(false);

        if (_autoRespawn) player.StartCoroutine(enableAfterDelay(_respawnDelay));
        return true;
    }

    public void RespawnFromMargarita(Margarita margarita)
    {
        if (gameObject.activeInHierarchy) return;

        gameObject.SetActive(true);
    }

    public void RespawnFromBarTender(Bartender bartender)
    {
        if (gameObject.activeInHierarchy) return;

        gameObject.SetActive(true);
    }

    private IEnumerator enableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(true);
    }

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
        if (moveDirection.y < 0)
        {
            return false;
        }

        return true;
    }

    public override bool CanMoveFrom(Vector2Int moveDirection)
    {
        if (moveDirection.y > 0)
        {
            return false;
        }

        return true;
    }
}