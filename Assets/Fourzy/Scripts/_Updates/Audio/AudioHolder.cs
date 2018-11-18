//@vadym udod

using Fourzy._Updates.Serialized;
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
        /// <summary>
        /// Singleton reference
        /// </summary>
        public static AudioHolder instance;

        /// <summary>
        /// Selected mixer
        /// </summary>
        public AudioMixer mixer;
        /// <summary>
        /// Reference to audio data holder.
        /// </summary>
        public AudioDataHolder audioData;

        private AudioSource audioSource;
        private Dictionary<AudioClip, float> currentPlayedPool;
        private Queue<AudioClip> toRemove;

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

            audioSource = GetComponent<AudioSource>();

            currentPlayedPool = new Dictionary<AudioClip, float>();
            toRemove = new Queue<AudioClip>();
        }

        /// <summary>
        /// Update time on all current audio clips played.
        /// </summary>
        protected void Update()
        {
            if (currentPlayedPool.Count > 0)
            {
                foreach (AudioClip clip in currentPlayedPool.Keys.ToList())
                    if (currentPlayedPool[clip] - Time.deltaTime > 0f)
                        currentPlayedPool[clip] -= Time.deltaTime;
                    else
                        toRemove.Enqueue(clip);
            }

            while (toRemove.Count > 0)
                currentPlayedPool.Remove(toRemove.Dequeue());
        }

        /// <summary>
        /// Set sfxs' volume using mixer group
        /// </summary>
        /// <param name="value">Volume value</param>
        public void SfxVolume(float value)
        {
            mixer.SetFloat("SfxVolume", Mathf.Clamp01(1f - value) * -80f);
        }

        /// <summary>
        /// Set audio's volume using mixer group
        /// </summary>
        /// <param name="value">Volume value</param>
        public void AudioVolume(float value)
        {
            mixer.SetFloat("AudioVolume", Mathf.Clamp01(1f - value) * -80f);
        }

        /// <summary>
        /// Plays selected audio on self, also check if audio of same time is being played.
        /// Can only play same audio type again only if it played for more than 20% of its duration.
        /// </summary>
        /// <param name="type">Audio type</param>
        /// <param name="source">Audio source</param>
        /// <param name="volume">Volume</param>
        /// <returns></returns>
        public bool PlaySfxOneShotTracked(AudioTypes type, AudioSource source, float volume = 1f)
        {
            AudioClip clip = GetAudioClip(type);
            if (!clip)
                return false;

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

        public void PlaySelfSfxOneShotTracked(AudioTypes type, float volume = 1f)
        {
            PlaySfxOneShotTracked(type, audioSource, volume);
        }

        /// <summary>
        /// Gets random audio clip from given audio type
        /// </summary>
        /// <param name="type">Audio type</param>
        /// <returns></returns>
        public AudioClip GetAudioClip(AudioTypes type)
        {
            foreach (AudiosIDPair pair in audioData.data)
                if (pair.type == type)
                    return pair.audios[Random.Range(0, pair.audios.Length)];

            return null;
        }
    }

}