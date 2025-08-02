using DG.Tweening;
using UnityEngine;

public class PlayerMono : MonoBehaviour
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

    public Vector2Int _yPadding = new Vector2Int(1, 0);
    public Vector2Int _xPadding = new Vector2Int(0, 0);

    public bool _needToResetX = false;
    public bool _needToResetY;

    public SpriteRenderer _spriteRenderer;

     public Sprite _originalSprite;
    public Sprite _carryingSprite;

    public SpriteRenderer _carryingSpriteRenderer;

    public float _upInteractDuration = 1f;
    public float _upInteractDistanceFactor = 0.5f;
    public float _downInteractDuration = 0.66f;
    public float _downInteractDistanceFactor = 0.3f;

    public int _originalSortingOrder = 2;
    public int _behindBarSortingOrderOffset = 1;

    public CarryableSO _carrying;

    public void Start()
    {
        _currentGridPosition = _startingGridPosition;
        transform.localPosition = _grid64Mono.GridPositionToWorldPosition(_currentGridPosition);
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        if (_currentGridPosition.y == _yPadding.x)
        {
            _spriteRenderer.sortingOrder = _originalSortingOrder + _behindBarSortingOrderOffset;
        }
        else
        {
            _spriteRenderer.sortingOrder = _originalSortingOrder;
        }
    }

    public bool CanMoveTo(Vector2Int moveDirection, Vector2Int gridPositionLastFrame)
    {
        ITile tile = _grid64Mono.GetTile(_currentGridPosition);
        if (tile != null && !tile.CanMoveTo(moveDirection))
        {
            _currentGridPosition = gridPositionLastFrame;
            return false;
        }

        tile = _grid64Mono.GetTile(gridPositionLastFrame);
        if (tile != null && !tile.CanMoveFrom(moveDirection))
        {
            _currentGridPosition = gridPositionLastFrame;
            return false;
        }

        return true;
    }

    private void HandleInput()
    {
        if (GameInput.Instance._horizontalMovement.x == 0)
        {
            _needToResetX = false;
        }

        if (GameInput.Instance._horizontalMovement.y == 0)
        {
            _needToResetY = false;
        }
    }

    private void HandleMovement(Vector2Int gridPositionLastFrame)
    {
        if (_busy) return;
        if (!_needToResetX)
        {
            if (GameInput.Instance._horizontalMovement.x > 0.1f)
            {
                _currentGridPosition.x++;
                _needToResetX = true;
            }

            if (GameInput.Instance._horizontalMovement.x < -0.1f)
            {
                _currentGridPosition.x--;
                _needToResetX = true;
            }
        }

        if (!_needToResetY && !_needToResetX)
        {
            if (GameInput.Instance._horizontalMovement.y > 0.1f)
            {
                _currentGridPosition.y++;
                _needToResetY = true;
            }

            if (GameInput.Instance._horizontalMovement.y < -0.1f)
            {
                _currentGridPosition.y--;
                _needToResetY = true;
            }
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
            .SetEase(_opts._ease)
            .SetLoops(_opts._loop ? _opts._loops : 0, LoopType.Yoyo);
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

    private void HandleInteract(ITile tile, Vector2Int moveDirection, bool fromCurrent)
    {
        bool interacted = tile.OnInteract(this, moveDirection);
        
        Vector3 pos = Vector3.Lerp(
            _grid64Mono.GridPositionToWorldPosition(_currentGridPosition),
            _grid64Mono.GridPositionToWorldPosition(_currentGridPosition + moveDirection),
            fromCurrent ? _upInteractDistanceFactor : _downInteractDistanceFactor);
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }

        _tween = DOTween.Sequence();
        Tween tween = transform.DOLocalMove(
                pos,
                _opts._duration * (fromCurrent ? _upInteractDuration : _downInteractDuration))
            .OnStart(() => { _busy = true; })
            .OnComplete(() =>
            {
                // this wont work for glassware etc
                // we need this to check if the thing we just interacted with is a stock or a restock
                // then also if were already carrying something
                if (fromCurrent && tile.GetCarryable() != null)
                {
                    _spriteRenderer.sprite = _carryingSprite;
                    _carryingSpriteRenderer.gameObject.SetActive(true);
                     _carrying = tile.GetCarryable();
                    _carryingSpriteRenderer.sprite = _carrying.GetSprite();
                }
                else if (!fromCurrent)
                {
                    _spriteRenderer.sprite = _originalSprite;
                     _carrying = null;
                    _carryingSpriteRenderer.gameObject.SetActive(false);
                }
            })
            .SetRelative(_opts._relative)
            .SetEase(_opts._ease);
        _tween.Append(tween);
        tween = transform.DOLocalMove(
                transform.localPosition,
                _opts._duration * (fromCurrent ? _upInteractDuration : _downInteractDuration))
            .OnComplete(() => { _busy = false; })
            .SetRelative(_opts._relative)
            .SetEase(_opts._ease);
        _tween.Append(tween);
    }

    public void Update()
    {
        HandleInput();
        if (_busy) return;
        Vector2Int gridPositionLastFrame = _currentGridPosition;
        HandleMovement(gridPositionLastFrame);
        Vector2Int moveDirection = _currentGridPosition - gridPositionLastFrame;
        _currentGridPosition.x =
            Mathf.Clamp(_currentGridPosition.x, _xPadding.x, _grid64Mono._columns - 1 - _xPadding.y);
        _currentGridPosition.y = Mathf.Clamp(_currentGridPosition.y, _yPadding.x, _grid64Mono._rows - 1 - _yPadding.y);
        if (gridPositionLastFrame != _currentGridPosition && CanMoveTo(moveDirection, gridPositionLastFrame))
        {
            _targetPosition = _grid64Mono.GridPositionToWorldPosition(_currentGridPosition);
            HandleTween(moveDirection);
            UpdateSortingOrder();
        }
        else if (moveDirection != Vector2Int.zero)
        {
            ITile tile = _grid64Mono.GetTile(_currentGridPosition);
            if (tile != null && tile.CanInteractFrom(moveDirection))
            {
                HandleInteract(tile, moveDirection, true);
                return;
            }

            tile = _grid64Mono.GetTile(_currentGridPosition + moveDirection);
            if (tile != null && tile.CanInteractTo(moveDirection))
            {
                HandleInteract(tile, moveDirection, false);
                return;
            }
        }
    }

    private void OnDestroy()
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }
    }
}