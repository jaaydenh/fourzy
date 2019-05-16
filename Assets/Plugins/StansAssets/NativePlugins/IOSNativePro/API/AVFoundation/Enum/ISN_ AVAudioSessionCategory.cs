using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.AVFoundation
{

    public enum ISN_AVAudioSessionCategory
    {
        //The category for an app in which sound playback is nonprimary—that is, your app can be used successfully with the sound turned off.
        Ambient,
        //The default audio session category.
        SoloAmbient,
        //The category for playing recorded music or other sounds that are central to the successful use of your app.
        Playback,
        //The category for recording audio; this category silences playback audio.
        Record,
        //The category for recording (input) and playback (output) of audio, such as for a VoIP (Voice over Internet Protocol) app.
        PlayAndRecord,
        //The category for routing distinct streams of audio data to different output devices at the same time.
        MultiRoute
    }
}