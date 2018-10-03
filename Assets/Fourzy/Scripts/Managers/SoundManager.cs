using UnityEngine;

namespace Fourzy
{
    public class SoundManager : Singleton<SoundManager>
    {
        public AudioSource efxSource;
        public float lowPitchRange = .9f;
        public float highPitchRange = 1.1f;

        public void PlaySingle(AudioClip clip)
        {
            efxSource.clip = clip;
            efxSource.Play();
        }

        //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
        public void PlayRandomizedSfx(params AudioClip[] clips)
        {
            int randomIndex = Random.Range(0, clips.Length);
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);

            efxSource.pitch = randomPitch;
            efxSource.PlayOneShot(clips[randomIndex]);
        }

        public void Mute(bool shouldMute)
        {
            efxSource.mute = shouldMute;
        }
    }
}
