using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BarTender : MonoBehaviour
{
    [SerializeField]
    private Grid64Mono _grid64Mono;
    private Sequence _tween;
    [SerializeField]
    private Vector3 _targetPosition = new Vector3(0, 0, 0);
    [SerializeField]
    private Vector2Int _startingGridPosition = new Vector2Int(0, 0);
    [SerializeField]
    private TweenOpts _opts;
    public Vector2Int _currentGridPosition = new Vector2Int(0, 0);
    private bool _busy;

    public SpriteRenderer _spriteRenderer;
    public Sprite _originalSprite;
    public Sprite _carryingSprite;
    public SpriteRenderer _carryingSpriteRenderer;
    public CarryableSO _carrying;

    private Coroutine _pacingCoroutine;
    private float _paceIntervalSeconds = 6f;
    private Vector3 _originalLocalPosition;
    private bool _left = true;

    private Coroutine _carryingCoroutine;
    private float _carryingIntervalSeconds = 4f;
    public List<CarryableSO> _stockedCarryables = new List<CarryableSO>();
    public List<CarryableSO> _unstockedCarryables = new List<CarryableSO>();
    public Margarita _margarita;

    public int _numCarryables;
    public int _numMargaritas;
    public int _numMissedMargaritas;

    public BarStock _barStock;
    public Glassware _glassware;

    public void Start()
    {
        _currentGridPosition = _startingGridPosition;
        transform.localPosition = _grid64Mono.GridPositionToWorldPosition(_currentGridPosition);
        _originalLocalPosition = transform.localPosition;
        _pacingCoroutine = StartCoroutine(pace());
        _carryingCoroutine = StartCoroutine(carry());
    }

    private IEnumerator pace()
    {
        while (true)
        {
            float randomPaceInterval = Random.Range(_paceIntervalSeconds, _paceIntervalSeconds * 1.5f);
            yield return new WaitForSeconds(randomPaceInterval);
            Vector2Int moveDirection = _left ? new Vector2Int(-1, 0) : new Vector2Int(1, 0);
            _currentGridPosition += moveDirection;

            if (_left)
            {
                _targetPosition = Vector3.Lerp(_originalLocalPosition,
                    _grid64Mono.GridPositionToWorldPosition(_currentGridPosition), 0.5f);
            }
            else
            {
                _targetPosition = _originalLocalPosition;
            }

            _left = !_left;

            HandleTween(moveDirection);
        }
    }

    private void HandleTween(Vector2Int moveDirection)
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }

        _tween = DOTween.Sequence();
        Tween tween = transform.DOLocalMove(_targetPosition, _opts._duration)
            .OnStart(() => { _busy = true; })
            .OnComplete(() => { _busy = false; })
            .SetRelative(_opts._relative)
            .SetEase(_opts._ease);
        _tween.Append(tween);
        if (moveDirection.x < 0)
        {
            _spriteRenderer.transform.localScale = new Vector3(-10, 10, 10);
        }
        else if (moveDirection.x > 0)
        {
            _spriteRenderer.transform.localScale = new Vector3(10, 10, 10);
        }
    }

    private IEnumerator carry()
    {
        while (true)
        {
            float randomCarryInterval = Random.Range(_carryingIntervalSeconds, _carryingIntervalSeconds * 1.5f);
            yield return new WaitForSeconds(randomCarryInterval);
            if (_carrying == null)
            {
                PickupRandomCarryable();
            }
            else
            {
                DropRandomCarryable();
            }
        }
    }

    private CarryableSO GetRandomCarryable(List<CarryableSO> carryables)
    {
        int randomIndex = Random.Range(0, carryables.Count);
        return carryables[randomIndex];
    }

    private void PickupRandomCarryable()
    {
        bool makeMarg = _numCarryables % 3 == 0;

        if (_barStock.IsStocked() && makeMarg)
        {
            _carrying = _margarita.GetCarryable();
            _numMargaritas++;
        }
        else if (_barStock.IsStocked())
        {
            _carrying = GetRandomCarryable(_stockedCarryables);
        }
        else if (makeMarg)
        {
            _numMissedMargaritas++;
            _carrying = GetRandomCarryable(_unstockedCarryables);
        }
        else
        {
            _carrying = GetRandomCarryable(_unstockedCarryables);
        }

        _carryingSpriteRenderer.gameObject.SetActive(true);
        _carryingSpriteRenderer.sprite = _carrying.GetSprite();
        _spriteRenderer.sprite = _carryingSprite;
        _numCarryables++;
    }

    private void DropRandomCarryable()
    {
        if (_carrying == _margarita.GetCarryable() && !_margarita._margaritaReady)
        {
            _margarita.PrepareMargarita();
        }
        if (_carrying == _glassware.GetCarryable())
        {
            _glassware.RespawnFromBarTender(this);
        }

        _carrying = null;
        _carryingSpriteRenderer.gameObject.SetActive(false);
        _carryingSpriteRenderer.sprite = _originalSprite;
        _spriteRenderer.sprite = _originalSprite;
    }
}