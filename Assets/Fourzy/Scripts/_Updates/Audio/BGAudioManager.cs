//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Fourzy._Updates.Audio
{
    /// <summary>
    /// Helps background audio playback handling.
    /// </summary>
    public class BGAudioManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton reference
        /// </summary>
        public static BGAudioManager instance;

        public AudioMixerGroup outputGroup;

        private List<BGAudio> currentlyPlayingAudios;

        /// <summary>
        /// Store singleton reference or delete itself if one already exsists
        /// </summary>
        protected void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            currentlyPlayingAudios = new List<BGAudio>();
        }

        /// <summary>
        /// Plays audio with given audio type
        /// </summary>
        /// <param name="type">Audio type</param>
        /// <param name="repeat">Repeat?</param>
        /// <param name="volume">Volume</param>
        /// <param name="fadeInTime">Fade in/out time</param>
        /// <returns></returns>
        public BGAudio PlayAudio(AudioTypes type, bool repeat, float volume, float fadeInTime)
        {
            AudioClip audioClip = AudioHolder.instance.GetAudioClip(type);

            if (audioClip == null)
                return null;

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = repeat;
            audioSource.outputAudioMixerGroup = outputGroup;

            VolumeTween volumeTween = gameObject.AddComponent<VolumeTween>();
            volumeTween.audioSource = audioSource;
            volumeTween.playbackTime = fadeInTime;
            volumeTween.to = volume;
            volumeTween.curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

            volumeTween.PlayForward(true);

            BGAudio bgAudio = new BGAudio() { audioSource = audioSource, volumeTween = volumeTween, audioClip = audioClip, };

            audioSource.Play();

            if (!repeat)
                bgAudio.playbackRoutine = StartCoroutine(BGAudioRoutine(bgAudio));

            currentlyPlayingAudios.Add(bgAudio);

            return bgAudio;
        }

        /// <summary>
        /// Stops bgAudio playback.
        /// </summary>
        /// <param name="bgAudio">Object reference</param>
        /// <param name="fadeOutTime">Fadeout time</param>
        public void StopBGAudio(BGAudio bgAudio, float fadeOutTime)
        {
            if (!currentlyPlayingAudios.Contains(bgAudio) || !this)
                return;

            bgAudio.volumeTween.playbackTime = fadeOutTime;

            if (bgAudio.playbackRoutine != null)
                StopCoroutine(bgAudio.playbackRoutine);

            StartCoroutine(BGAudioFadeOutRoutine(bgAudio));
        }

        /// <summary>
        /// Lifecylce routine for bgAudio object
        /// </summary>
        /// <param name="bgAudio">Object reference</param>
        /// <returns></returns>
        private IEnumerator BGAudioRoutine(BGAudio bgAudio)
        {
            yield return new WaitForSeconds(bgAudio.audioClip.length - bgAudio.volumeTween.playbackTime);

            StartCoroutine(BGAudioFadeOutRoutine(bgAudio));
        }

        /// <summary>
        /// Fadeout routine for bgAudio object
        /// </summary>
        /// <param name="bgAudio">Object reference</param>
        /// <returns></returns>
        private IEnumerator BGAudioFadeOutRoutine(BGAudio bgAudio)
        {
            bgAudio.volumeTween.PlayBackward(true);

            yield return new WaitForSeconds(bgAudio.volumeTween.playbackTime);

            RemoveBGAudio(bgAudio);
        }

        private void RemoveBGAudio(BGAudio bgAudio)
        {
            if (!currentlyPlayingAudios.Contains(bgAudio))
                return;

            Destroy(bgAudio.volumeTween);
            Destroy(bgAudio.audioSource);

            currentlyPlayingAudios.Remove(bgAudio);
        }

        public class BGAudio
        {
            public AudioSource audioSource;
            public AudioClip audioClip;

            public Coroutine playbackRoutine;
            public VolumeTween volumeTween;
        }
    }

}