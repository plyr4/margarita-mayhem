using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Margarita : TileMono
{
    private Coroutine _attractDancerCoroutine;
    public bool _margaritaReady;
    public SpriteRenderer _sprite;
    public float _attractDelay = 3f;
    public GameObject _dancerPrefab;
    public Vector2Int _reservedPosition;
    [SerializeField]
    private Dancer.MoveOpts _moveOpts;
    public Glassware _glassware;
    public DanceFloor _danceFloor;
    public CarryableSO _carryable;
    public Action<Dancer> OnConsumeMargaritaAction;

    public override CarryableSO GetCarryable()
    {
        return _carryable;
    }

    private void Reset()
    {
        TileMono[] tileMonos = GetComponentsInChildren<TileMono>();
        foreach (TileMono tileMono in tileMonos)
        {
            if (tileMono != this)
            {
                _grid64Mono = tileMono._grid64Mono;
                _gridPosition = tileMono._gridPosition;
                DestroyImmediate(tileMono);
                break;
            }
        }
    }

    private void Start()
    {
        _margaritaReady = false;
        _sprite.gameObject.SetActive(false);
        _reservedPosition = new Vector2Int(_gridPosition.x, _gridPosition.y);
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

    public void PrepareMargarita()
    {
        if (_margaritaReady) return;
        _margaritaReady = true;
        _sprite.gameObject.SetActive(true);
        if (_danceFloor.Full())
        {
            // todo: end day
            return;
        }

        AttractDancer();
    }

    private void ConsumeMargarita()
    {
        if (_attractDancerCoroutine != null)
        {
            StopCoroutine(_attractDancerCoroutine);
        }

        _margaritaReady = false;
        _sprite.gameObject.SetActive(false);
        _glassware.RespawnFromMargarita(this);
    }

    private void AttractDancer()
    {
        if (_attractDancerCoroutine != null)
        {
            StopCoroutine(_attractDancerCoroutine);
        }

        _attractDancerCoroutine = StartCoroutine(attractDancer());
    }

    private IEnumerator attractDancer()
    {
        Vector3 worldPos = _grid64Mono.GridPositionToWorldPosition(_reservedPosition + Vector2Int.right * 3);
        GameObject dancerGO = Instantiate(_dancerPrefab,
            worldPos, Quaternion.identity, _danceFloor.transform);
        Dancer dancer = dancerGO.GetComponent<Dancer>();
        dancer.SetDancerIndex(_danceFloor);
        if (dancer != null)
        {
            dancer._gridPosition = _reservedPosition;
            dancer.SetPointingSprite(Vector2Int.left, _moveOpts);
        }

        _moveOpts._tweenOpts.OnComplete = () => { dancer.SetDrinkingSprite(); };

        dancer.MoveTo(_reservedPosition, _grid64Mono, _moveOpts);

        float delay = _attractDelay + Random.Range(_attractDelay, _attractDelay * 1.5f);
        yield return new WaitForSeconds(delay);
        if (_margaritaReady)
        {
            ConsumeMargarita();

            OnConsumeMargaritaAction?.Invoke(dancer);
        }
    }
}