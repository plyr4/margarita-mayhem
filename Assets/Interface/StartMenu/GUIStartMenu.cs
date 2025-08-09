using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class GUIStartMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewParent;
    [SerializeField]
    private GameObject _buttonsViewParent;
    [SerializeField]
    private GameObject _tutorialViewParent;
    [SerializeField]
    private GameObject _tutorialButtonsViewParent;
    public GameEvent _startPlayEvent;
    public bool _tutorialEnabled;
    public float _cooldownTime = 0.5f;
    private bool _cooldown;

    public List<Sprite> _tutorialSprites;
    public float _tutorialSpritesDelay = 0.5f;
    public float _tutorialSpritesDuration = 1f;

    private SpriteRenderer _tutorialImage;
    private Sequence _spriteSequence;

    private void Update()
    {
        if (GameInput.Instance._menuContinuePressed || GameInput.Instance._menuSubmitPressed)
        {
            if (_tutorialEnabled)
            {
                Button button = _tutorialButtonsViewParent.GetComponentInChildren<Button>();
                ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current),
                    ExecuteEvents.submitHandler);
            }
            else
            {
                Button button = _buttonsViewParent.GetComponentInChildren<Button>();
                ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current),
                    ExecuteEvents.submitHandler);
            }
        }
    }

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateInit _:
                _tutorialViewParent.SetActive(false);
                _tutorialButtonsViewParent.SetActive(false);
                if (_spriteSequence != null) _spriteSequence.Kill();
                break;
            case GStateStartIn _:
                _viewParent.SetActive(true);
                _buttonsViewParent.SetActive(true);
                if (_spriteSequence != null) _spriteSequence.Kill();
                break;
            case GStateStart _:
                _viewParent.SetActive(true);
                _buttonsViewParent.SetActive(true);
                _buttonsViewParent.GetComponentInChildren<UnityEngine.UI.Button>().Select();
                if (_spriteSequence != null) _spriteSequence.Kill();
                break;
            case GStatePlayLoad _:
                _viewParent.SetActive(false);
                _buttonsViewParent.SetActive(false);
                _tutorialViewParent.SetActive(false);
                _tutorialButtonsViewParent.SetActive(false);
                if (_spriteSequence != null) _spriteSequence.Kill();
                break;
        }
    }

    public void StartMenuHandleOnClick()
    {
        if (_cooldown)
            return;

        _cooldown = true;
        Invoke(nameof(resetCooldown), _cooldownTime);
        OpenTutorial();
        SoundManager.Instance.PlayButtonConfirm();
    }

    private void OpenTutorial()
    {
        _tutorialViewParent.SetActive(true);
        if (_tutorialImage == null)
            _tutorialImage = _tutorialViewParent.GetComponentInChildren<SpriteRenderer>();

        _tutorialImage.sprite = _tutorialSprites[0];

        if (_tutorialSprites != null && _tutorialSprites.Count > 0 && _tutorialImage != null)
        {
            DOVirtual.DelayedCall(_tutorialSpritesDelay, () =>
            {
                _spriteSequence = DOTween.Sequence();
                for (int i = 0; i < _tutorialSprites.Count; i++)
                {
                    int index = i;
                    _spriteSequence.AppendCallback(() => _tutorialImage.sprite = _tutorialSprites[index]);
                    _spriteSequence.AppendInterval(_tutorialSpritesDuration);
                }

                _spriteSequence.OnComplete(() =>
                {
                    _tutorialEnabled = true;
                    _tutorialButtonsViewParent.SetActive(true);
                    _buttonsViewParent.SetActive(false);
                    _tutorialViewParent.GetComponentInChildren<UnityEngine.UI.Button>().Select();
                });
                _spriteSequence.Play();
            });
        }
    }

    private void resetCooldown()
    {
        _cooldown = false;
    }

    public void TutorialHandleOnClick()
    {
        if (!_tutorialEnabled)
            return;
        if (_cooldown)
            return;

        _cooldown = true;
        Invoke(nameof(resetCooldown), _cooldownTime);
        CloseTutorial();
        SoundManager.Instance.PlayButtonConfirm();
    }

    private void CloseTutorial()
    {
        if (_tutorialImage == null)
            _tutorialImage = _tutorialViewParent.GetComponentInChildren<SpriteRenderer>();

        if (_tutorialSprites != null && _tutorialSprites.Count > 0 && _tutorialImage != null)
        {
            DOVirtual.DelayedCall(_tutorialSpritesDelay, () =>
            {
                _spriteSequence = DOTween.Sequence();
                for (int i = _tutorialSprites.Count - 1; i >= 0; i--)
                {
                    int index = i;
                    _spriteSequence.AppendCallback(() => _tutorialImage.sprite = _tutorialSprites[index]);
                    _spriteSequence.AppendInterval(_tutorialSpritesDuration);
                }

                _spriteSequence.OnComplete(() => { _startPlayEvent.Invoke(); });
                _spriteSequence.Play();
            });
        }
    }
}