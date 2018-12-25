﻿using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Collections;

#region Base Structure
/// <summary>
/// Generic event with all reference than you need to play a simple sound
/// and mantain the reference
/// </summary>
[System.Serializable]
public class GenericEvent
{
    [EventRef]
    public string eventPath;
    public EventInstance fmodEvent;
}
/// <summary>
/// Generic event with reference at parameter of the event
/// </summary>
[System.Serializable]
public class GenericEventMonoParameter : GenericEvent
{
    public ParameterInstance eventParameter;
    public string parameterName;
}
/// <summary>
/// Generic event with reference at parameter[] of the event
/// </summary>
[System.Serializable]
public class GenericEventMultipleParameter : GenericEvent
{
    public ParameterInstance[] eventParameter;
    public string[] parameterName;
}
#endregion

public class FmodManager : StudioBankLoader
{
    /// <summary>
    /// singleton
    /// </summary>
    public static FmodManager instance;

    private string m_busPrefix = "bus:/";

    #region Unity Method
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        if (instance == this)
        {
            Unload();
            Debug.LogWarning("[FMOD Manager] All bank was Unloaded");
        }
    }
    #endregion

    #region SoundMethod
    /// <summary>
    /// Use to instance Generic event in game
    /// </summary>
    /// <param name="_genericEvent"></param>
    public void CreateGenericEnventInstance(ref GenericEvent _genericEvent)
    {
        _genericEvent.fmodEvent = RuntimeManager.CreateInstance(_genericEvent.eventPath);
    }
    /// <summary>
    /// Use to instance Generic event with parameter
    /// </summary>
    /// <param name="_genericEventInstance"></param>
    public void CreateGenericMonoEventParameterInstance(ref GenericEventMonoParameter _genericEventInstance)
    {
        if(_genericEventInstance.eventPath == "")
        {
            Debug.LogWarning("event path null");
            return;
        }

        _genericEventInstance.fmodEvent = RuntimeManager.CreateInstance(_genericEventInstance.eventPath);
        _genericEventInstance.fmodEvent.getParameter(_genericEventInstance.parameterName, out _genericEventInstance.eventParameter);
    }

    /// <summary>
    /// Use to instance Generic event with multiple parameter
    /// </summary>
    /// <param name="_genericEventInstance"></param>
    public void CreateGenericEventMultipleParameter(ref GenericEventMultipleParameter _genericEventInstance)
    {
        _genericEventInstance.fmodEvent = RuntimeManager.CreateInstance(_genericEventInstance.eventPath);
        //Set number of parameter
        int _parameterCount;
        _genericEventInstance.fmodEvent.getParameterCount(out _parameterCount);
        ///Riassigne the lenght of parameter
        _genericEventInstance.eventParameter = new ParameterInstance[_parameterCount];
        _genericEventInstance.parameterName = new string[_parameterCount];

        if(_parameterCount != 0)
        {
            GetParameterByCount(ref _genericEventInstance, _parameterCount);
        }
        else
            UnityEngine.Debug.LogWarning(_genericEventInstance.eventPath + " has not parameter");
    }

    /// <summary>
    /// Trigger a cue of event
    /// </summary>
    /// <param name="_genericEvent"></param>
    public void TriggerCue(GenericEvent _genericEvent)
    {
        _genericEvent.fmodEvent.triggerCue();
    }

    /// <summary>
    /// Get a parameter by index
    /// usefule for array of parameter
    /// </summary>
    /// <param name="_genericEvent">paramete</param>
    /// <param name="_index">index of paramete</param>
    public void GetParameterByCount(ref GenericEventMultipleParameter _genericEvent, int _parameterCount )
    {
        for (int i = 0; i < _parameterCount; i++)
        {
            _genericEvent.fmodEvent.getParameterByIndex(i, out _genericEvent.eventParameter[i]);
            _genericEvent.parameterName[i] = _genericEvent.eventParameter[i].ToString();
        }

    }

    public void StartEventFade(GenericEvent _genericEvent, float _speed)
    {
        ///check if event is null
        if (_genericEvent.fmodEvent.handle == null)
        {
            Debug.LogWarning("Fmod event doesent exist");
            return;
        }
        ///Start the current event
        StartEvent(_genericEvent);
        ///Lerp the event volume from 0 to 1 with a custom speed 
        StartCoroutine(C_LerpOverTime(_genericEvent.fmodEvent, _speed));
    }

    /// <summary>
    /// Start the current event bank
    /// </summary>
    /// <param name="_genericEvent"></param>
    public void StartEvent(GenericEvent _genericEvent)
    {
        if (_genericEvent.fmodEvent.handle != null)
        {
            ///Start event
            _genericEvent.fmodEvent.start();
        }
        else
            Debug.LogError(_genericEvent.eventPath + "FmodEvent not exist");
    }

    /// <summary>
    /// Stop the current event bank with the fade
    /// </summary>
    /// <param name="_eventInstance"></param>
    public void StopEventFade(EventInstance _eventInstance)
    {
        _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    /// <summary>
    /// Stop the current event without the fade
    /// </summary>
    /// <param name="_eventInstance"></param>
    public void StopEvent(EventInstance _eventInstance)
    {
        _eventInstance.stop(STOP_MODE.IMMEDIATE);
    }
   
    /// <summary>
    /// Stop all event on the master bus
    /// </summary>
    /// <param name="_masterBus">master bus</param>
    public void StopAllEvents(Bus _masterBus)
    {
        _masterBus.stopAllEvents(STOP_MODE.IMMEDIATE);
    }

    /// <summary>
    /// Change the volume value of a event
    /// </summary>
    /// <param name="_eventInstance"></param>
    /// <param name="_volume"></param>
    public void SetEventVolume(EventInstance _eventInstance, float _volume)
    {
        _eventInstance.setVolume(_volume);
    }

    /// <summary>
    /// Play a sound one shot on a current position
    /// </summary>
    /// <param name="_path">event path</param>
    /// <param name="_pos"> position emitter</param>
    public void PlaySoundOneShot(string _path, Vector3 _pos)
    {
        if (_path != "")
            RuntimeManager.PlayOneShot(_path, _pos);
        else
            Debug.LogWarning("Path dosen't found");
    }

    /// <summary>
    /// Play a sound one shot and attach to gameobject
    /// </summary>
    /// <param name="_path">event path</param>
    /// <param name="_gameObject">gameobject to attach</param>
    public void PlaySoundOneShot(string _path, GameObject _gameObject)
    {
        if (_path != "")
            RuntimeManager.PlayOneShotAttached(_path, _gameObject);
        else
            Debug.LogWarning("Path dosen't found");
    }

    /// <summary>
    /// Attach an event to current transform
    /// </summary>
    /// <param name="_eventInstance">fmod event instance</param>
    /// <param name="emitterTransform">transform to attach event</param>
    public void AttachSfx(EventInstance _eventInstance, Transform emitterTransform)
    {
        RuntimeManager.AttachInstanceToGameObject(_eventInstance, emitterTransform, GetComponent<Rigidbody2D>());
    }
    
    /// <summary>
    /// Change the event parameter with a new value
    /// </summary>
    /// <param name="_eventParameter"></param>
    /// <param name="value"></param>
    public void ChangeParameter(ref ParameterInstance _eventParameter, float value)
    {
        if(_eventParameter.handle == null)
        {
            Debug.LogWarning("parament doesen't exist");
            return;
        }

        _eventParameter.setValue(value);
    }
    /// <summary>
    /// Chang event parameter with a new value (Integer)
    /// </summary>
    /// <param name="_eventParameter"></param>
    /// <param name="value"></param>
    public void ChangeParameter(ref ParameterInstance _eventParameter, int value)
    {
        if (_eventParameter.handle == null)
        {
            Debug.LogWarning("parament doesen't exist");
            return;
        }

        _eventParameter.setValue(value);
    }

    /// <summary>
    /// Lerp the event audio volume from 0 to 1 with a speed parameter
    /// </summary>
    /// <param name="_eventInstance"></param>
    /// <param name="_step">increesing time from 0 to 1</param>
    /// <returns></returns>
    private IEnumerator C_LerpOverTime(EventInstance _eventInstance, float _step)
    {
        float elapsedTime = 0;
        const float startVolume = 0;
        float currentVolume = 0;
        const float targetVolume = 1;
        while (elapsedTime < _step)
        {
            currentVolume = (Mathf.Lerp(startVolume, targetVolume, (elapsedTime / _step)));
            _eventInstance.setVolume(currentVolume);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_bus">Variable instance</param>
    /// <param name="_path">Channel path</param>
    public void SetBus(Bus _bus, string _path)
    {
        _bus = RuntimeManager.GetBus(m_busPrefix + _path);
    }
    #endregion
}
