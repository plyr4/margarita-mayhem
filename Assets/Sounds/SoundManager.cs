using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            // attempt to locate the singleton
            if (_instance == null)
            {
                _instance = (SoundManager)FindObjectOfType(typeof(SoundManager));
            }

            // create a new singleton
            if (_instance == null)
            {
                _instance = (new GameObject("SoundManager")).AddComponent<SoundManager>();
            }

            // return singleton
            return _instance;
        }
    }

    public AudioSource _theme;
    public float _themeVolume = 0.8f;
    public AudioSource _buttonConfirm;
    public AudioSource _footStep;
    public AudioSource _pickupItem;
    public AudioSource _washDish;
    public AudioSource _restockItem;
    public AudioSource _outOfStockBlink;
    public AudioSource _endOfLevel;
    public AudioSource _newDancer;
    public AudioSource _playerCollision;
    public float _sfxVolume = 0.6f;
    public bool _mute = false;
    public bool _initialized = false;
    public bool _playThemeOnStart = true;

    public IEnumerator Start()
    {
        UpdateVolume(_sfxVolume);
        Mute(_mute);

        yield return new WaitForSecondsRealtime(0.5f);
        // play theme in loop
        _theme.loop = true;
        _theme.volume = _themeVolume;
        if (_playThemeOnStart) PlayTheme();
        // InvokeRepeating(nameof(PlayTheme), 1f, _theme.clip.length + 0.5f);
        _initialized = true;
    }

    public void HandleMuteEvent(IGameEventOpts opts)
    {
        Mute(!_mute);
    }

    public void PlayTheme()
    {
        _theme.Play();
    }

    public void UpdateVolume(float volume)
    {
        _sfxVolume = volume;
        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
        {
            audioSource.volume = volume;
        }

        _theme.volume = _themeVolume;
    }

    public void PlaySoundUnscaled(AudioSource audioSource)
    {
        float t = Time.timeScale;
        Time.timeScale = 1f;
        AudioSource.PlayClipAtPoint(audioSource.clip, Camera.main.transform.position);
        Time.timeScale = t;
    }

    public void PlayButtonConfirm()
    {
        PlaySoundUnscaled(_buttonConfirm);
    }

    public void PlayFootStep()
    {
        PlaySoundUnscaled(_footStep);
    }

    public void PlayPickupItem()
    {
        PlaySoundUnscaled(_pickupItem);
    }

    public void PlayRestockItem()
    {
        PlaySoundUnscaled(_restockItem);
    }
    
    public void PlayWashDish()
    {
        PlaySoundUnscaled(_washDish);
    }
    
    public void PlayOutOfStockBlink()
    {
        PlaySoundUnscaled(_outOfStockBlink);
    }

    public void PlayEndOfLevel()
    {
        PlaySoundUnscaled(_endOfLevel);
    }

    public void PlayNewDancer()
    {
        PlaySoundUnscaled(_newDancer);
    }

    public void PlayPlayerCollision()
    {
        PlaySoundUnscaled(_playerCollision);
    }

    public void Mute(bool m)
    {
        _theme.mute = m;
        if (_buttonConfirm != null) _buttonConfirm.mute = m;
        if (_footStep != null) _footStep.mute = m;
        if (_pickupItem != null) _pickupItem.mute = m;
        if (_washDish != null) _washDish.mute = m;
        if (_restockItem != null) _restockItem.mute = m;
        if (_outOfStockBlink != null) _outOfStockBlink.mute = m;
        if (_endOfLevel != null) _endOfLevel.mute = m;
        if (_newDancer != null) _newDancer.mute = m;
        if (_playerCollision != null) _playerCollision.mute = m;
        _mute = m;
    }

    public bool ToggleMute()
    {
        Mute(!_mute);
        return _mute;
    }
}