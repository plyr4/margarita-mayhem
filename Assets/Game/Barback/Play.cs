using DG.Tweening;
using UnityEngine;

public class Play : MonoBehaviour
{
    [SerializeField]
    private bool _active;
    [SerializeField]
    private GameEvent _playPause;
    [SerializeField]
    private GameEvent _pausePlay;
    public float _pausePlayDelay = 0.2f;

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateInit _:
                _active = false;
                break;
            case GStatePlay _:
                _active = true;
                break;
        }
    }

    public void Update()
    {
        if (!_active)
        {
            return;
        }

        if (GameInput.Instance._menuBackPressed)
        {
            HandleOnClickEscape();
        }
    }

    public void HandleOnClickEscape()
    {
        _playPause.Invoke();
        switch (GStateMachineGame.Instance.CurrentState())
        {
            case GStatePause _:
                DOVirtual.DelayedCall(_pausePlayDelay, () => _pausePlay.Invoke()).SetUpdate(true);
                break;
            case GStatePlay _:
                _playPause.Invoke();
                break;
        }
    }
}