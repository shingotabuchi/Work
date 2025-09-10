using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Fwk.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour
    {
        private AudioSource _audioSource0;
        private AudioSource _audioSource1;
        private bool _isCrossfading;
        private CancellationTokenSource _crossfadeCts;
        private ISoundData _currentSoundData;
        private bool _isPaused = false;

        public ISoundData CurrentSoundData => _currentSoundData;

        private void Awake()
        {
            _audioSource0 = GetComponent<AudioSource>();
            _audioSource0.playOnAwake = false;
            _audioSource0.loop = false;

            // Create crossfade source
            var crossfadeObj = new GameObject("CrossfadeSource");
            crossfadeObj.transform.SetParent(transform);
            _audioSource1 = crossfadeObj.AddComponent<AudioSource>();
            _audioSource1.playOnAwake = false;
            _audioSource1.loop = false;
        }

        private void OnDestroy()
        {
            _crossfadeCts?.Cancel();
            _crossfadeCts?.Dispose();
        }

        public void PlayOneShot(ISoundData soundData, float volume = 1.0f)
        {
            if (soundData == null || soundData.Clip == null) return;
            _audioSource0.PlayOneShot(soundData.Clip, soundData.Volume * volume);
        }

        public void PlayBgm(ISoundData soundData, float volume = 1.0f)
        {
            if (soundData == null || soundData.Clip == null) return;
            CancelCrossfade();

            _currentSoundData = soundData;
            _isPaused = false;

            _audioSource1.Stop();
            _audioSource0.clip = soundData.Clip;
            _audioSource0.volume = soundData.Volume * volume;
            _audioSource0.loop = true;
            _audioSource0.Play();
        }

        public async UniTask CrossfadeBgm(ISoundData newSoundData, float duration = 1.0f, float volume = 1.0f)
        {
            if (newSoundData == null || newSoundData.Clip == null) return;

            // Cancel any ongoing crossfade
            CancelCrossfade();

            // Wait for any existing crossfade to complete
            while (_isCrossfading)
            {
                await UniTask.Yield();
            }

            _isCrossfading = true;
            _isPaused = false;

            var playingSource = _audioSource0.isPlaying ? _audioSource0 : _audioSource1;
            var nonPlayingSource = _audioSource0.isPlaying ? _audioSource1 : _audioSource0;

            // Setup crossfade source
            nonPlayingSource.clip = newSoundData.Clip;
            nonPlayingSource.volume = 0f;
            nonPlayingSource.loop = true;
            nonPlayingSource.Play();

            var startTime = Time.time;
            var startVolume = playingSource.volume;
            var endVolume = newSoundData.Volume * volume;

            // Update current sound data information
            _currentSoundData = newSoundData;

            try
            {
                while (Time.time - startTime < duration)
                {
                    float t = (Time.time - startTime) / duration;
                    playingSource.volume = Mathf.Lerp(startVolume, 0f, t);
                    nonPlayingSource.volume = Mathf.Lerp(0f, endVolume, t);
                    await UniTask.Yield(_crossfadeCts.Token);
                }

                // Final state
                playingSource.Stop();
                nonPlayingSource.volume = endVolume;
            }
            catch (System.OperationCanceledException)
            {
                // Handle cancellation
            }
            finally
            {
                _isCrossfading = false;
            }
        }

        public async UniTask StopBgm(float fadeDuration = 1.0f)
        {
            // Cancel any ongoing crossfade
            CancelCrossfade();
            _isPaused = false;

            // Wait for any existing crossfade to complete
            while (_isCrossfading)
            {
                await UniTask.Yield();
            }

            if (fadeDuration <= 0f || (!_audioSource0.isPlaying && !_audioSource1.isPlaying))
            {
                // Immediate stop if fade duration is 0 or no audio is playing
                _audioSource0.Stop();
                _audioSource1.Stop();
                return;
            }

            _isCrossfading = true;

            try
            {
                var playingSource0 = _audioSource0.isPlaying ? _audioSource0 : null;
                var playingSource1 = _audioSource1.isPlaying ? _audioSource1 : null;

                float startTime = Time.time;
                float startVolume0 = playingSource0 != null ? playingSource0.volume : 0f;
                float startVolume1 = playingSource1 != null ? playingSource1.volume : 0f;

                while (Time.time - startTime < fadeDuration)
                {
                    float t = (Time.time - startTime) / fadeDuration;

                    if (playingSource0 != null)
                        playingSource0.volume = Mathf.Lerp(startVolume0, 0f, t);

                    if (playingSource1 != null)
                        playingSource1.volume = Mathf.Lerp(startVolume1, 0f, t);

                    await UniTask.Yield(_crossfadeCts.Token);
                }

                // Final state
                _audioSource0.Stop();
                _audioSource1.Stop();
            }
            catch (System.OperationCanceledException)
            {
                // Handle cancellation
            }
            finally
            {
                _isCrossfading = false;
            }
        }

        // Immediate stop without fading
        public void StopBgmImmediate()
        {
            _audioSource0.Stop();
            _audioSource1.Stop();
            CancelCrossfade();
        }

        public async UniTask PauseBgm(float fadeDuration = 1.0f)
        {
            if (_isPaused)
            {
                Debug.LogWarning("BGM is already paused.");
                return;
            }

            _isPaused = true;

            if (fadeDuration <= 0f || (!_audioSource0.isPlaying && !_audioSource1.isPlaying))
            {
                PauseBgmImmediate();
                return;
            }

            // Cancel any ongoing crossfade
            CancelCrossfade();

            // Wait for any existing crossfade to complete
            while (_isCrossfading)
            {
                await UniTask.Yield();
            }

            _isCrossfading = true;

            try
            {
                var playingSource0 = _audioSource0.isPlaying ? _audioSource0 : null;
                var playingSource1 = _audioSource1.isPlaying ? _audioSource1 : null;

                float startTime = Time.time;
                float startVolume0 = playingSource0 != null ? playingSource0.volume : 0f;
                float startVolume1 = playingSource1 != null ? playingSource1.volume : 0f;

                while (Time.time - startTime < fadeDuration)
                {
                    float t = (Time.time - startTime) / fadeDuration;

                    if (playingSource0 != null)
                        playingSource0.volume = Mathf.Lerp(startVolume0, 0f, t);

                    if (playingSource1 != null)
                        playingSource1.volume = Mathf.Lerp(startVolume1, 0f, t);

                    await UniTask.Yield(_crossfadeCts.Token);
                }

                // Pause audio sources
                if (playingSource0 != null)
                    playingSource0.Pause();
                if (playingSource1 != null)
                    playingSource1.Pause();
            }
            catch (System.OperationCanceledException)
            {
                // Handle cancellation
            }
            finally
            {
                _isCrossfading = false;
            }
        }

        public void PauseBgmImmediate()
        {
            if (_isPaused)
            {
                Debug.LogWarning("BGM is already paused.");
                return;
            }
            _isPaused = true;
            CancelCrossfade();

            // Pause both audio sources
            if (_audioSource0.isPlaying)
                _audioSource0.Pause();
            if (_audioSource1.isPlaying)
                _audioSource1.Pause();
        }

        public async UniTask ResumeBgm(float fadeDuration = 1.0f)
        {
            if (!_isPaused)
            {
                Debug.LogWarning("BGM is not paused. Cannot resume.");
                return;
            }
            _isPaused = false;

            if (fadeDuration <= 0f)
            {
                ResumeBgmImmediate();
                return;
            }

            // Cancel any ongoing crossfade
            CancelCrossfade();

            // Wait for any existing crossfade to complete
            while (_isCrossfading)
            {
                await UniTask.Yield();
            }

            _isCrossfading = true;

            try
            {
                var pausedSource0 = _audioSource0;
                var pausedSource1 = _audioSource1;

                // Resume audio sources at the paused position
                if (pausedSource0 != null)
                {
                    pausedSource0.volume = 0f;
                    pausedSource0.UnPause();
                }
                if (pausedSource1 != null)
                {
                    pausedSource1.volume = 0f;
                    pausedSource1.UnPause();
                }

                var startTime = Time.time;
                var volume = _currentSoundData.Volume * _currentSoundData.PlayedVolume;
                var startVolume0 = pausedSource0 != null ? pausedSource0.volume : 0f;
                var startVolume1 = pausedSource1 != null ? pausedSource1.volume : 0f;

                while (Time.time - startTime < fadeDuration)
                {
                    var t = (Time.time - startTime) / fadeDuration;

                    if (pausedSource0 != null)
                        pausedSource0.volume = Mathf.Lerp(startVolume0, volume, t);

                    if (pausedSource1 != null)
                        pausedSource1.volume = Mathf.Lerp(startVolume1, volume, t);

                    await UniTask.Yield(_crossfadeCts.Token);
                }

                // Final state
                if (pausedSource0 != null)
                    pausedSource0.volume = volume;
                if (pausedSource1 != null)
                    pausedSource1.volume = volume;
            }
            catch (System.OperationCanceledException)
            {
                // Handle cancellation
            }
            finally
            {
                _isCrossfading = false;
            }
        }

        public void ResumeBgmImmediate()
        {
            if (!_isPaused)
            {
                Debug.LogWarning("BGM is not paused. Cannot resume.");
                return;
            }
            _isPaused = false;
            var volume = _currentSoundData.Volume * _currentSoundData.PlayedVolume;
            // Resume audio sources at the paused position with full volume
            if (_audioSource0.clip != null && !_audioSource0.isPlaying)
            {
                _audioSource0.volume = volume;
                _audioSource0.UnPause();
            }
            if (_audioSource1.clip != null && !_audioSource1.isPlaying)
            {
                _audioSource1.volume = volume;
                _audioSource1.UnPause();
            }
        }

        private void CancelCrossfade()
        {
            _crossfadeCts?.Cancel();
            _crossfadeCts?.Dispose();
            _crossfadeCts = new CancellationTokenSource();
        }

        public bool IsPlaying => _audioSource0.isPlaying || _audioSource1.isPlaying;
        public bool IsPaused => _isPaused;
    }
}