using DG.Tweening;
using UnityEngine;

public class DOTweenLocalMove : MonoBehaviour
{
    [SerializeField]
    private TweenOpts _opts;

    public Vector3 _targetPosition = new Vector3(0, 0, 0);

    private Tween _tween;

    void Start()
    {
        _tween = transform.DOLocalMove(
                _targetPosition,
                _opts._duration)
            .SetRelative(_opts._relative)
            .SetEase(_opts._ease)
            .SetLoops(_opts._loop ? _opts._loops : 0, LoopType.Yoyo);
    }

    void OnDestroy()
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }
    }
}