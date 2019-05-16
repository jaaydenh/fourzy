////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using SA.Foundation.Events;
using SA.Foundation.Templates;

namespace SA.iOS.ReplayKit.Internal
{
    internal class ISN_RPEditorAPI : ISN_iRRAPI
    {

        private SA_Event<ISN_RPStopResult> m_didStopRecording = new SA_Event<ISN_RPStopResult>();
        private SA_Event m_didChangeAvailability = new SA_Event();


      


        public SA_iEvent<ISN_RPStopResult> DidStopRecording {
            get {
                return m_didStopRecording;
            }
        }

        public SA_iEvent DidChangeAvailability {
            get {
                return m_didChangeAvailability;
            }
        }

        public void DiscardRecording(Action callback) {
           
        }

        public bool IsReplayKitAvaliable() {
            return false;
        }

        public bool IsReplayKitMicEnabled() {
            return false;
        }

        public bool IsReplayKitRecording() {
            return false;
        }

        public void ShowVideoShareDialog(Action<ISN_PRPreviewResult> callback) {
            
        }

        public void StartRecording(Action<SA_Result> callback) {
            
        }

        public void StopRecording(Action<ISN_RPStopResult> callback) {
            
        }

        public void SetMicrophoneEnabled(bool enabled) {
            
        }

    }
}