//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Fourzy._Updates.Audio
{
    /// <summary>
    /// Manages sfx's and stores references to all game audio.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioHolder : MonoBehaviour
    {
        public static AudioHolder instance;

        public AudioMixer mixer;
        public AudioMixerGroup outputGroup;
        public AudioDataHolder audioData;

        private AudioSource audioSource;
        private Dictionary<AudioClip, float> currentPlayedPool;
        private Queue<AudioClip> toRemove;

        public List<BGAudio> currentlyPlayingBG { get; private set; }

        protected void Awake()
        {
            if (instance != null) return;

            instance = this;

            audioSource = GetComponent<AudioSource>();

            currentlyPlayingBG = new List<BGAudio>();
            currentPlayedPool = new Dictionary<AudioClip, float>();
            toRemove = new Queue<AudioClip>();

            SettingsManager.onSfx += (state) => SfxVolume(state ? 1f : 0f);
            SettingsManager.onAudio += (state) => AudioVolume(state ? 1f : 0f);
        }

        protected void Start()
        {
            SfxVolume(SettingsManager.Get(SettingsManager.KEY_SFX) ? 1f : 0f);
            AudioVolume(SettingsManager.Get(SettingsManager.KEY_AUDIO) ? 1f : 0f);
        }

        protected void Update()
        {
            if (currentPlayedPool.Count > 0)
            {
                foreach (AudioClip clip in currentPlayedPool.Keys.ToList())
                {
                    if (currentPlayedPool[clip] - Time.deltaTime > 0f)
                    {
                        currentPlayedPool[clip] -= Time.deltaTime;
                    }
                    else
                    {
                        toRemove.Enqueue(clip);
                    }
                }
            }

            while (toRemove.Count > 0)
            {
                currentPlayedPool.Remove(toRemove.Dequeue());
            }
        }

        public void SetMasterVolume(float value) => mixer.SetFloat("MasterVolume", Mathf.Clamp01(1f - value) * -80f);

        public void SfxVolume(float value) => mixer.SetFloat("SfxVolume", Mathf.Clamp01(1f - value) * -80f);

        public void AudioVolume(float value) => mixer.SetFloat("AudioVolume", Mathf.Clamp01(1f - value) * -80f);

        public bool PlaySfxOneShotTracked(string type, AudioSource source, float volume = 1f)
        {
            if (!SettingsManager.Get(SettingsManager.KEY_SFX)) return false;

            AudioClip clip = GetAudioClip(type);
            if (!clip) return false;

            if (currentPlayedPool.ContainsKey(clip))
            {
                //if played more than 20% of clip, play again
                if (currentPlayedPool[clip] / clip.length < .8f)
                {
                    source.PlayOneShot(clip, volume);
                    currentPlayedPool[clip] = clip.length;   //update time
                    return true;
                }

                return false;
            }
            else
            {
                //add to pool
                currentPlayedPool.Add(clip, clip.length);
                source.PlayOneShot(clip, volume);
                return true;
            }
        }

        public void PlaySelfSfxOneShotTracked(string type, float volume = 1f) =>
            PlaySfxOneShotTracked(type, audioSource, volume);

        public void PlaySfxOneShot(string type, AudioSource source, float volume = 1f)
        {
            AudioClip clip = GetAudioClip(type);

            if (!clip) return;

            source.PlayOneShot(clip, volume);
        }

        public void PlaySelfSfxOneShot(string type, float volume = 1f) => PlaySfxOneShot(type, audioSource, volume);

        public BGAudio PlaySelfSfxOneShot(string type, float volume, float pitch)
        {
            AudioClip clip = GetAudioClip(type);

            if (clip == null) return null;

            AudioSource _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = clip;
            _audioSource.loop = false;
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            _audioSource.Play();

            BGAudio bgAudio = new BGAudio()
            {
                type = type,
                audioSource = _audioSource,
                audioClip = clip,
            };

            bgAudio.playbackRoutine = StartCoroutine(BGAudioRoutine(bgAudio));

            currentlyPlayingBG.Add(bgAudio);

            return bgAudio;
        }

        /// <summary>
        /// Gets random audio clip from given audio type
        /// </summary>
        /// <param name="type">Audio type</param>
        /// <returns></returns>
        public AudioClip GetAudioClip(string type)
        {
            foreach (AudiosIDPair pair in audioData.data)
            {
                if (pair.type == type)
                {
                    return pair.clips[Random.Range(0, pair.clips.Length)];
                }
            }

            return null;
        }

        public bool IsBGAudioPlaying(string type) => currentlyPlayingBG.Find(audio => audio.type == type) != null;

        public bool IsBGAudioPlaying(BGAudio audio) => currentlyPlayingBG.Contains(audio);

        public BGAudio PlayBGAudio(string type, bool repeat, float volume, float fadeInTime)
        {
            AudioClip clip = GetAudioClip(type);

            if (clip == null) return null;

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.loop = repeat;
            audioSource.outputAudioMixerGroup = outputGroup;

            VolumeTween volumeTween = gameObject.AddComponent<VolumeTween>();
            volumeTween.audioSource = audioSource;
            volumeTween.playbackTime = fadeInTime;
            volumeTween.to = volume;
            volumeTween.curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

            volumeTween.PlayForward(true);

            BGAudio bgAudio = new BGAudio()
            {
                type = type,
                audioSource = audioSource,
                volumeTween = volumeTween,
                audioClip = clip,
            };

            audioSource.Play();

            if (!repeat)
            {
                bgAudio.playbackRoutine = StartCoroutine(BGAudioRoutine(bgAudio));
            }

            currentlyPlayingBG.Add(bgAudio);

            return bgAudio;
        }

        public void StopBGAudio(BGAudio bgAudio, float fadeOutTime)
        {
            if (!currentlyPlayingBG.Contains(bgAudio) || !this) return;

            bgAudio.volumeTween.playbackTime = fadeOutTime;

            if (bgAudio.playbackRoutine != null)
            {
                StopCoroutine(bgAudio.playbackRoutine);
            }

            StartCoroutine(BGAudioFadeOutRoutine(bgAudio));
        }

        public void StopBGAudio(string type, float fadeOutTime)
        {
            BGAudio bgAudio = currentlyPlayingBG.Find(_bgAudio => _bgAudio.type == type);

            if (bgAudio != null)
            {
                StopBGAudio(bgAudio, fadeOutTime);
            }
        }

        public void StopAllBGAudio()
        {
            while (currentlyPlayingBG.Count > 0)
            {
                if (currentlyPlayingBG[0].playbackRoutine != null)
                {
                    StopCoroutine(currentlyPlayingBG[0].playbackRoutine);
                }

                RemoveBGAudio(currentlyPlayingBG[0]);
            }
        }

        private IEnumerator BGAudioRoutine(BGAudio bgAudio)
        {
            if (bgAudio.volumeTween)
            {
                yield return new WaitForSeconds(bgAudio.audioClip.length - bgAudio.volumeTween.playbackTime);
                StartCoroutine(BGAudioFadeOutRoutine(bgAudio));
            }
            else
            {
                yield return new WaitForSeconds(bgAudio.audioClip.length);
                RemoveBGAudio(bgAudio);
            }
        }

        private IEnumerator BGAudioFadeOutRoutine(BGAudio bgAudio)
        {
            bgAudio.volumeTween.PlayBackward(true);

            yield return new WaitForSeconds(bgAudio.volumeTween.playbackTime);

            RemoveBGAudio(bgAudio);
        }

        private void RemoveBGAudio(BGAudio bgAudio)
        {
            if (!currentlyPlayingBG.Contains(bgAudio)) return;

            if (bgAudio.volumeTween)
            {
                Destroy(bgAudio.volumeTween);
            }
            Destroy(bgAudio.audioSource);

            currentlyPlayingBG.Remove(bgAudio);
        }

        public class BGAudio
        {
            public string type;

            public AudioSource audioSource;
            public AudioClip audioClip;

            public Coroutine playbackRoutine;
            public VolumeTween volumeTween;
        }
    }
}