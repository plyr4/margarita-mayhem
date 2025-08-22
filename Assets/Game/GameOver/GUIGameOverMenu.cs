using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIGameOverMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewParent;
    public TMP_Text _text;
    public GameEvent _gameOverRetryEvent;
    public GameEvent _gameOverInDoneEvent;
    public float _cooldownTime = 0.5f;
    private bool _cooldown;

    private void Update()
    {
        if (GameInput.Instance._menuContinuePressed || GameInput.Instance._menuSubmitPressed)
        {
            Button button = _viewParent.GetComponentInChildren<Button>();
            ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current),
                ExecuteEvents.submitHandler);
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
                _viewParent.SetActive(true);
                _viewParent.GetComponentInChildren<UnityEngine.UI.Button>()?.Select();
                SpriteTextWriter.WriteText(_text,
                    $"{((GameOverEventOpts)playGameOverEvent._opts)._dishSink._numWashedDishes}");
                _gameOverInDoneEvent.Invoke(playGameOverEvent._opts);
                break;
            case GStateGameOverRetry _:
                _viewParent.SetActive(false);
                break;
            case GStateGameOverRetryIn _:
                ScreenTransition.Instance.Close();
                break;
            case GStatePlayLoad _:
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
        
        SoundManager.Instance.PlayButtonConfirm();
    }
}