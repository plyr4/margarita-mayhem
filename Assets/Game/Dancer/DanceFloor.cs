using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DanceFloor : MonoBehaviour
{
    public List<Dancer> _dancers = new List<Dancer>();
    public Vector2Int _danceFloorSize = new Vector2Int(6, 4);
    public Vector2Int _danceFloorOffset = new Vector2Int(1, 1);
    public List<Vector2Int> _preReservedPositions = new List<Vector2Int>();
    public Grid64Mono _grid64Mono;

    public float _moveInterval = 2f;
    private HashSet<Vector2Int> _reservedPositions = new HashSet<Vector2Int>();
    private Coroutine _moveCoroutine;
    private Coroutine _waitCoroutine;
    [SerializeField]
    public Dancer.MoveOpts _moveOpts;
    public Margarita _margarita;

    public GameObject _dancerPrefab;
    public int _startingNumDancers = 3;
    public int _maxAttempts = 2;
    public int _maxDancers = 16;
    public TextMeshPro _counterText;
    
    public void UpdateCounter()
    {
        SpriteTextWriter.WriteText(_counterText, $"{_maxDancers - _dancers.Count + 1}");
    }

    public void Start()
    {
        _reservedPositions.Clear();
        _reservedPositions.Add(_margarita._reservedPosition);
        foreach (Vector2Int pos in _preReservedPositions)
        {
            _reservedPositions.Add(pos);
        }

        _dancers.Clear();
        for (int i = 0; i < _startingNumDancers; i++)
        {
            GameObject dancerObj = Instantiate(_dancerPrefab, transform);
            Dancer dancer = dancerObj.GetComponent<Dancer>();
            if (dancer != null)
            {
                dancer.SetDancerIndex(this);
                _dancers.Add(dancer);
            }
        }

        foreach (Dancer dancer in _dancers)
        {
            dancer._gridPosition = GetRandomAvailablePosition();
            _reservedPositions.Add(dancer._gridPosition);
            dancer.transform.localPosition = _grid64Mono.GridPositionToWorldPosition(dancer._gridPosition);
        }

        UpdateDancers();

        _margarita.OnConsumeMargaritaAction += AddDancer;
        
        UpdateCounter();
    }

    public bool Full()
    {
        return _dancers.Count >= _maxDancers;
    }

    public float Fullness()
    {
        return (float)_dancers.Count / _maxDancers;
    }

    private void UpdateDancers()
    {
        if (_moveCoroutine != null) return;

        _reservedPositions.Clear();
        _reservedPositions.Add(_margarita._reservedPosition);
        foreach (Vector2Int pos in _preReservedPositions)
        {
            _reservedPositions.Add(pos);
        }

        List<Dancer> currentDancers = new List<Dancer>(_dancers);
        foreach (Dancer dancer in currentDancers)
        {
            if (dancer == null) continue;
            _reservedPositions.Add(dancer._gridPosition);
        }

        Dictionary<Dancer, Vector2Int> nextPositions = new Dictionary<Dancer, Vector2Int>();
        Dictionary<Dancer, Vector2Int> nextDirections = new Dictionary<Dancer, Vector2Int>();

        int attempt;
        for (attempt = 0; attempt <= _maxAttempts; attempt++)
        {
            nextPositions.Clear();
            nextDirections.Clear();

            HashSet<Vector2Int> testReserved = new HashSet<Vector2Int>();
            foreach (Vector2Int pos in _reservedPositions) testReserved.Add(pos);

            foreach (Dancer dancer in currentDancers)
            {
                if (dancer == null) continue;
                testReserved.Remove(dancer._gridPosition);
            }

            bool validConfig = true;
            HashSet<Vector2Int> dancerOnlyPositions = new HashSet<Vector2Int>();

            foreach (Dancer dancer in currentDancers)
            {
                if (dancer == null) continue;

                Vector2Int newPos = FindNextPosition(dancer, testReserved);
                Vector2Int nextDir = Vector2Int.zero;

                if (newPos != dancer._gridPosition)
                    nextDir = newPos - dancer._gridPosition;

                if (testReserved.Contains(newPos))
                {
                    newPos = dancer._gridPosition;
                    nextDir = Vector2Int.zero;
                }

                nextPositions[dancer] = newPos;
                nextDirections[dancer] = nextDir;
                dancerOnlyPositions.Add(newPos);
                testReserved.Add(newPos);
            }

            HashSet<Vector2Int> checkOverlap = new HashSet<Vector2Int>();
            bool overlapDetected = false;
            foreach (var pos in nextPositions.Values)
            {
                if (!checkOverlap.Add(pos))
                {
                    overlapDetected = true;
                    break;
                }
            }

            if (overlapDetected)
            {
                if (attempt == _maxAttempts - 1)
                    break;
                continue;
            }

            if (HasPathToCorners(dancerOnlyPositions))
                break;

            if (attempt == _maxAttempts - 1)
                break;
        }

        foreach (Dancer dancer in currentDancers)
        {
            if (dancer == null) continue;
            dancer.SetPointingSprite(nextDirections[dancer], _moveOpts);
        }

        foreach (Dancer dancer in currentDancers)
        {
            if (dancer == null) continue;
            dancer._isMoving = true;
        }

        _moveCoroutine = StartCoroutine(MoveDancersCoroutine(nextPositions, nextDirections));
    }

    private IEnumerator MoveDancersCoroutine(Dictionary<Dancer, Vector2Int> nextPositions,
        Dictionary<Dancer, Vector2Int> nextDirections)
    {
        yield return new WaitForSeconds(_moveInterval);

        foreach (KeyValuePair<Dancer, Vector2Int> kvp in nextPositions)
        {
            Dancer dancer = kvp.Key;
            if (dancer == null || !_dancers.Contains(dancer)) continue;

            Vector2Int newPos = kvp.Value;
            Vector2Int nextDir = nextDirections.ContainsKey(dancer) ? nextDirections[dancer] : Vector2Int.zero;

            if (newPos != dancer._gridPosition)
            {
                dancer.MoveTo(newPos, _grid64Mono, _moveOpts);
            }
            else
            {
                dancer._isMoving = false;
            }
        }

        yield return new WaitForSeconds(_moveOpts._nextMoveDelay);
        _moveCoroutine = null;
        UpdateDancers();
    }

    private Vector2Int FindNextPosition(Dancer dancer, HashSet<Vector2Int> reserved)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        Vector2Int[] directions =
        {
            Vector2Int.left, Vector2Int.left, Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right, Vector2Int.right, Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int pos = dancer._gridPosition + dir;
            if (IsValidPosition(pos) && !reserved.Contains(pos) && pos != dancer._lastGridPosition)
            {
                candidates.Add(pos);
            }
        }

        if (candidates.Count == 0)
        {
            foreach (Vector2Int dir in directions)
            {
                Vector2Int pos = dancer._gridPosition + dir;
                if (IsValidPosition(pos) && !reserved.Contains(pos))
                {
                    candidates.Add(pos);
                }
            }
        }

        if (candidates.Count == 0)
        {
            return dancer._gridPosition;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    private bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= _danceFloorOffset.x && pos.x < _danceFloorOffset.x + _danceFloorSize.x &&
               pos.y >= _danceFloorOffset.y && pos.y < _danceFloorOffset.y + _danceFloorSize.y;
    }

    private Vector2Int GetRandomAvailablePosition()
    {
        List<Vector2Int> available = new List<Vector2Int>();
        for (int x = _danceFloorOffset.x; x < _danceFloorOffset.x + _danceFloorSize.x; x++)
        {
            for (int y = _danceFloorOffset.y; y < _danceFloorOffset.y + _danceFloorSize.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!_reservedPositions.Contains(pos))
                {
                    available.Add(pos);
                }
            }
        }

        if (available.Count == 0) return _danceFloorOffset;
        return available[Random.Range(0, available.Count)];
    }

    public void AddDancer(Dancer dancer)
    {
        if (!_dancers.Contains(dancer))
        {
            _dancers.Add(dancer);
            dancer.SetPointingSprite(Vector2Int.zero, _moveOpts);
            
            UpdateCounter();
        }
    }

    private bool HasPathToCorners(HashSet<Vector2Int> dancerPositions)
    {
        Vector2Int topLeft = new Vector2Int(_danceFloorOffset.x, _danceFloorOffset.y + _danceFloorSize.y - 1);
        Vector2Int topRight = new Vector2Int(_danceFloorOffset.x + _danceFloorSize.x - 1,
            _danceFloorOffset.y + _danceFloorSize.y - 1);

        for (int x = _danceFloorOffset.x; x < _danceFloorOffset.x + _danceFloorSize.x; x++)
        {
            Vector2Int bottomStart = new Vector2Int(x, _danceFloorOffset.y);
            if (!dancerPositions.Contains(bottomStart))
            {
                if (FindPath(bottomStart, topLeft, dancerPositions) || FindPath(bottomStart, topRight, dancerPositions))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool FindPath(Vector2Int start, Vector2Int end, HashSet<Vector2Int> dancerPositions)
    {
        if (dancerPositions.Contains(start) || dancerPositions.Contains(end)) return false;

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == end) return true;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (IsValidPosition(next) && !dancerPositions.Contains(next) && !visited.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
        }

        return false;
    }
}