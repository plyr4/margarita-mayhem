using System.Collections;
using UnityEngine;

public class Glassware : TileMono
{
    public CarryableSO _carryable;
    public float _respawnDelay = 20f;
    public bool _autoRespawn = true;
    public bool _disabledOnStart;
    public DanceFloor _danceFloor;

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
        
        SoundManager.Instance.PlayPickupItem();

        gameObject.SetActive(false);

        float respawnTime = _respawnDelay;
        if (_danceFloor != null)
        {
            float fullness = _danceFloor.Fullness();
            respawnTime = _respawnDelay * (1f - fullness * 0.25f);
        }

        respawnTime += Random.Range(-1f, 1f);

        if (_autoRespawn) player.StartCoroutine(enableAfterDelay(respawnTime));
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