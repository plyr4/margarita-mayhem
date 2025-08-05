using TMPro;
using UnityEngine;

public class GUIGameOverMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewParent;
    public TextMeshPro _text;
    public GameEvent _gameOverRetryEvent;
    public float _cooldownTime = 0.5f;
    private bool _cooldown;

    private void Update()
    {
        if (GameInput.Instance._menuContinuePressed || GameInput.Instance._menuSubmitPressed)
        {
            GameOverRetryHandleOnClick();
        }
    }

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateInit _:
                _viewParent.SetActive(false);
                break;
            case GStateGameOverIn playGameOverEvent:
                ScreenTransition.Instance.Close();
                break;
            case GStateGameOver playGameOverEvent:
                _viewParent.SetActive(true);
                _viewParent.GetComponentInChildren<UnityEngine.UI.Button>()?.Select();
                SpriteTextWriter.WriteText(_text,
                    $"{((GameOverEventOpts)playGameOverEvent._opts)._dishSink._numWashedDishes}");
                break;
            case GStateGameOverRetryIn _:
                _viewParent.SetActive(false);
                break;
            case GStatePlay _:
                _viewParent.SetActive(false);
                break;
            case GStatePauseRetry _:
                _viewParent.SetActive(false);
                break;
        }
    }

    private void resetCooldown()
    {
        _cooldown = false;
    }

    public void GameOverRetryHandleOnClick()
    {
        if (_cooldown)
            return;

        _cooldown = true;
        Invoke(nameof(resetCooldown), _cooldownTime);

        _gameOverRetryEvent.Invoke();
    }
}