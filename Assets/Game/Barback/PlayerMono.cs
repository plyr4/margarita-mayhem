using System.Collections;
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

    public Vector2 _collisionBoxSize = new Vector2(0.5f, 0.5f);
    private Coroutine _blinkCoroutine;
    private bool blinking;

    public void Start()
    {
        _currentGridPosition = _startingGridPosition;
        transform.localPosition = _grid64Mono.GridPositionToWorldPosition(_currentGridPosition);
        UpdateSortingOrder();
    }

    public void Update()
    {
        HandleInput();
        if (_busy) return;
        switch (GStateMachineGame.Instance.CurrentState())
        {
            case GStatePlay _:
                break;
            default:
                return;
        }

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
            bool interacted = false;
            ITile tile = _grid64Mono.GetTile(_currentGridPosition);
            if (tile != null && tile.CanInteractFrom(moveDirection))
            {
                interacted = tile.OnInteract(this, moveDirection);
            }

            tile = _grid64Mono.GetTile(_currentGridPosition + moveDirection);
            if (!interacted && tile != null && tile.CanInteractTo(moveDirection))
            {
                interacted = tile.OnInteract(this, moveDirection);
            }
        }

        CheckForCollisions();
    }

    private void CheckForCollisions()
    {
        Vector3 spritePosition = _spriteRenderer.transform.position;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(spritePosition, _collisionBoxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            Dancer dancer = collider.GetComponentInParent<Dancer>();
            if (dancer != null)
            {
                if (_carrying != null)
                {
                    DropItem();

                    if (_blinkCoroutine != null)
                    {
                        StopCoroutine(_blinkCoroutine);
                    }

                    _blinkCoroutine = StartCoroutine(blink(0.5f, 3));
                }
            }
        }
    }

    private IEnumerator blink(float duration, int blinkCount)
    {
        if (blinking) yield break;
        blinking = true;
        for (int i = 0; i < blinkCount; i++)
        {
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(duration / (blinkCount * 2));
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(duration / (blinkCount * 2));
        }

        blinking = false;
    }

    private void UpdateSortingOrder()
    {
        if (_currentGridPosition.y == _grid64Mono._rows - 1 - _yPadding.y)
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
        ITile tile = _grid64Mono.GetTile(gridPositionLastFrame);
        if (tile != null && !tile.CanMoveFrom(moveDirection))
        {
            _currentGridPosition = gridPositionLastFrame;
            return false;
        }

        tile = _grid64Mono.GetTile(_currentGridPosition);
        if (tile != null && !tile.CanMoveTo(moveDirection))
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


    private void OnDestroy()
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }
    }

    public void HandlePickup(Vector2Int moveDirection, ITile tile)
    {
        Vector3 pos = Vector3.Lerp(
            _grid64Mono.GridPositionToWorldPosition(_currentGridPosition),
            _grid64Mono.GridPositionToWorldPosition(_currentGridPosition + moveDirection),
            _upInteractDistanceFactor);
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }

        _tween = DOTween.Sequence();
        Tween tween = transform.DOLocalMove(
                pos,
                _opts._duration * _upInteractDuration)
            .OnStart(() => { _busy = true; })
            .OnComplete(() =>
            {
                _spriteRenderer.sprite = _carryingSprite;
                _carryingSpriteRenderer.gameObject.SetActive(true);
                _carrying = tile.GetCarryable();
                _carryingSpriteRenderer.sprite = _carrying.GetSprite();
            })
            .SetRelative(_opts._relative)
            .SetEase(_opts._ease);
        _tween.Append(tween);
        tween = transform.DOLocalMove(
                transform.localPosition,
                _opts._duration * _upInteractDuration)
            .OnComplete(() => { _busy = false; })
            .SetRelative(_opts._relative)
            .SetEase(_opts._ease);
        _tween.Append(tween);
    }

    public void HandleDrop(Vector2Int moveDirection, ITile tile)
    {
        Vector3 pos = Vector3.Lerp(
            _grid64Mono.GridPositionToWorldPosition(_currentGridPosition),
            _grid64Mono.GridPositionToWorldPosition(_currentGridPosition + moveDirection),
            _upInteractDistanceFactor);
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }

        _tween = DOTween.Sequence();
        Tween tween = transform.DOLocalMove(
                pos,
                _opts._duration * _upInteractDuration)
            .OnStart(() => { _busy = true; })
            .OnComplete(() => { DropItem(); })
            .SetRelative(_opts._relative)
            .SetEase(_opts._ease);
        _tween.Append(tween);
        tween = transform.DOLocalMove(
                transform.localPosition,
                _opts._duration * _upInteractDuration)
            .OnComplete(() => { _busy = false; })
            .SetRelative(_opts._relative)
            .SetEase(_opts._ease);
        _tween.Append(tween);
    }

    private void DropItem()
    {
        _spriteRenderer.sprite = _originalSprite;
        _carrying = null;
        _carryingSpriteRenderer.gameObject.SetActive(false);
    }
}