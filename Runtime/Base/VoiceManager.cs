using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using App.Scripts.Utils;
using Epic.OnlineServices;
using UnityEngine;

public class RTCVoiceSource : MonoBehaviour {
    public AudioSource VoiceSource;
    private short[] _audioFrameBuffer = Array.Empty<short>();
    private ConcurrentQueue<short[]> _audioFrameQueue = new();
    private bool _catchUp;
    private short[] _currentVoiceFrame;
    private ProductUserId _epicVoiceId;
    private bool _isInitialized;
    private int _voiceFrameIndex;
    private int _sampleRate = 48000;
    private int position;
    public volatile float[] sample;
    public AudioListener audioListener;

    private void Awake() {
        audioListener = FindObjectOfType<AudioListener>();
        _sampleRate = AudioSettings.outputSampleRate;
        if (VoiceSource != null) {
            return;
        }

        var hasAudioSource = TryGetComponent(out VoiceSource);
        if (!hasAudioSource) {
            VoiceSource = gameObject.AddComponent<AudioSource>();
            VoiceSource.playOnAwake = true;
        }

        var audioConfig = AudioSettings.GetConfiguration();
        audioConfig.dspBufferSize = 480;
        AudioSettings.Reset(audioConfig);
    }
    
    private void Start() {
        if (VoiceSource is null) {
            Debug.LogError("Voice source does not exist");
            return;
        }
        
        VoiceSource.loop = true;
        VoiceSource.volume = 1;
        var dummy = AudioClip.Create("FlatLine", 1, 1, AudioSettings.outputSampleRate,false);
        dummy.SetData(new float[] { 1 }, 0);
        VoiceSource.spatialBlend = 1;
        VoiceSource.clip = dummy;
        VoiceSource.Play();
    }
    
    private void OnEnable() {
        if (!_isInitialized) {
            return;
        }

        VoiceManager.Instance.voiceSourceMap[_epicVoiceId] = this;
    }

    private void OnDisable() {
        VoiceManager.Instance.voiceSourceMap.Remove(_epicVoiceId);
    }

    private void OnAudioFilterRead(float[] data, int channels) {
        if (_audioFrameQueue?.Count > 48000 / 1000 || _catchUp) {
            _catchUp = true;
            _audioFrameQueue?.TryDequeue(out var _);
            _catchUp = _audioFrameQueue?.Count <= 20;
        }
        
        //Audio data is interleaved, so we need to loop through the samples in pairs
        for (var i = 0; i < data.Length; i += channels) {
            if (_audioFrameQueue is null) continue;
            if (_currentVoiceFrame is null || _voiceFrameIndex >= _currentVoiceFrame.Length) {
                if (!_audioFrameQueue.TryDequeue(out var frame)) continue;
                _voiceFrameIndex = 0;
                _currentVoiceFrame = frame;
            }
            
            //Write the sample to the buffer
            for (var j = 0; j < channels; j++) {
                data[i + j] *= _currentVoiceFrame[_voiceFrameIndex] / (float) short.MaxValue;
            }
            
            _voiceFrameIndex++;
        }
    }

    public void Initialize(ProductUserId userId) {
        _epicVoiceId = userId;
        VoiceManager.Instance.voiceSourceMap.TryAdd(_epicVoiceId, this);
        _isInitialized = true;
    }

    public void EnqueueAudioFrame(ProductUserId epicId, short[] frames) {
        if (!_isInitialized || _epicVoiceId != epicId) {
            return;
        }

        if (_audioFrameQueue.Count > 48000 / 500) {
            _audioFrameQueue = new ConcurrentQueue<short[]>();
        }
        _audioFrameQueue.Enqueue(frames);
    }
}

public class VoiceManager : Singleton<VoiceManager> {
    public Dictionary<ProductUserId, RTCVoiceSource> voiceSourceMap = new();

    public void EnqueueAudioFrame(ProductUserId epicId, short[] frames) {
        if (voiceSourceMap is null || epicId is null || frames is null) {
            return;
        }

        if (voiceSourceMap.ContainsKey(epicId)) {
            voiceSourceMap[epicId].EnqueueAudioFrame(epicId, frames);
        }
    }
}