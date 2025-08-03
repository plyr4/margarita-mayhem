using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class Dancers
{
    public List<Sprite> _sprites;
}

public class Dancer : MonoBehaviour
{
    public Vector2Int _gridPosition = new Vector2Int(0, 0);
    public Vector2Int _moveDirection = new Vector2Int(0, 0);
    public Coroutine _danceCoroutine;
    public Tween _moveTween;

    public List<Dancers> _dancerSprites;
    public Vector2Int _lastGridPosition = new Vector2Int(0, 0);
    public SpriteRenderer _spriteRenderer;
    public int _directionIndex;
    public int _dancerIndex = 0;
    public bool _isMoving;
    private int _drinkingIndex = 5;

    [Serializable]
    public class MoveOpts
    {
        public float _nextMoveDelay;
        public float _danceInterval = 0.3f;
        public TweenOpts _tweenOpts;
    }

    public void SetDancerIndex(DanceFloor danceFloor)
    {
        int dancerIndex = danceFloor._dancers.Count % _dancerSprites.Count;
        _dancerIndex = dancerIndex;
        _spriteRenderer.sprite = GetSprite(_directionIndex);
    }

    public void MoveTo(Vector2Int newGridPosition, Grid64Mono grid, MoveOpts opts)
    {
        if (_moveTween != null && _moveTween.IsActive())
        {
            _moveTween.Kill();
        }

        _lastGridPosition = _gridPosition;
        _gridPosition = newGridPosition;
        Vector3 worldPos = grid.GridPositionToWorldPosition(_gridPosition);
        _isMoving = true;
        if (_danceCoroutine != null)
        {
            StopCoroutine(_danceCoroutine);
        }

        _moveTween = transform
            .DOLocalMove(worldPos, opts._tweenOpts._duration)
            .SetEase(opts._tweenOpts._ease)
            .OnComplete(() =>
            {
                _isMoving = false; 
                opts._tweenOpts.OnComplete?.Invoke();
            });
    }

    public void SetPointingSprite(Vector2Int direction, MoveOpts moveOpts)
    {
        if (direction == Vector2Int.right) _directionIndex = 1;
        else if (direction == Vector2Int.left) _directionIndex = 2;
        else if (direction == Vector2Int.up) _directionIndex = 3;
        else if (direction == Vector2Int.down) _directionIndex = 4;
        else _directionIndex = 0;
        _spriteRenderer.sprite = GetSprite(_directionIndex);

        if (_danceCoroutine != null)
        {
            StopCoroutine(_danceCoroutine);
        }

        _danceCoroutine = StartCoroutine(dance(moveOpts));
    }

    private IEnumerator dance(MoveOpts moveOpts)
    {
        while (true)
        {
            SetDancingSprite();
            yield return new WaitForSeconds(moveOpts._danceInterval / 2);
            SetIdleSprite();
            yield return new WaitForSeconds(moveOpts._danceInterval);
        }
    }

    public void SetIdleSprite()
    {
        _spriteRenderer.sprite = GetSprite(_directionIndex);
    }

    public void SetDrinkingSprite()
    {
        _spriteRenderer.sprite = GetSprite(_drinkingIndex);
    }

    private void SetDancingSprite()
    {
        _spriteRenderer.sprite = GetSprite(_directionIndex + _dancerSprites[0]._sprites.Count / 2);
    }

    private Sprite GetSprite(int index)
    {
        if (_dancerSprites[_dancerIndex] != null && _dancerSprites[_dancerIndex]._sprites.Count > 0)
        {
            if (index >= 0 && index < _dancerSprites[_dancerIndex]._sprites.Count)
                return _dancerSprites[_dancerIndex]._sprites[index];
        }

        return null;
    }
}