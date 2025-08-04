using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOverEventOpts : IGameEventOpts
{
    public Margarita _margarita;
    public DanceFloor _danceFloor;
    public Bartender _bartender;
    public DishSink _dishSink;
}

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
    public DishSink _dishSink;
    public CarryableSO _carryable;
    public Action<Dancer> OnConsumeMargaritaAction;

    public GameEvent _playGameOverEvent;

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

    public void PrepareMargarita(Bartender bartender)
    {
        if (_margaritaReady) return;
        _margaritaReady = true;
        _sprite.gameObject.SetActive(true);

        AttractDancer(bartender);
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

    private void AttractDancer(Bartender bartender)
    {
        if (_attractDancerCoroutine != null)
        {
            StopCoroutine(_attractDancerCoroutine);
        }

        _attractDancerCoroutine = StartCoroutine(attractDancer(bartender));
    }

    private IEnumerator attractDancer(Bartender bartender)
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

            if (_danceFloor.Full())
            {
                GameOverEventOpts gameOverOpts = new GameOverEventOpts();
                gameOverOpts._margarita = this;
                gameOverOpts._danceFloor = _danceFloor;
                gameOverOpts._bartender = bartender;
                gameOverOpts._dishSink = _dishSink;
                _playGameOverEvent?.Invoke(gameOverOpts);
            }
            else
            {
                OnConsumeMargaritaAction?.Invoke(dancer);
            }
        }
    }
}