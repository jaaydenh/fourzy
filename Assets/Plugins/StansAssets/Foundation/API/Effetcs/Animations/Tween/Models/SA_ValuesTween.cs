////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using SA.Foundation.Events;

namespace SA.Foundation.Animation {

	public class SA_ValuesTween : MonoBehaviour {


        private SA_Event m_onComplete = new SA_Event();
        private SA_Event<float> m_onValueChanged = new SA_Event<float>();
        private SA_Event<Vector3> m_onVectorValueChanged = new SA_Event<Vector3>();


        public bool DestoryGameObjectOnComplete = true;

		private float FinalFloatValue;
		private Vector3 FinalVectorValue;

      

        //--------------------------------------
        // Initialization
        //--------------------------------------

        public static SA_ValuesTween Create() {
            return new GameObject(typeof(SA_ValuesTween).FullName).AddComponent<SA_ValuesTween>();
		}


        //--------------------------------------
        // Unity Event
        //--------------------------------------

        void Update() {

            if(gameObject == null || gameObject.Equals(null)) {
                return;
            }

            m_onValueChanged.Invoke(transform.position.x);
			m_onVectorValueChanged.Invoke(transform.position);
		}



        //--------------------------------------
        // Public Methods
        //--------------------------------------


        public void ValueTo(float from, float to, float time, SA_EaseType easeType = SA_EaseType.linear) {
			Vector3 pos = transform.position;
			pos.x = from;
			transform.position = pos;
			FinalFloatValue = to;
			
			SA_iTween.MoveTo(gameObject, SA_iTween.Hash("x", to,  "time", time, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", gameObject));
		}
		

		public void VectorTo(Vector3 from, Vector3 to, float time,  SA_EaseType easeType = SA_EaseType.linear) {
			transform.position = from;
			FinalVectorValue = to;

			SA_iTween.MoveTo(gameObject, SA_iTween.Hash("position", to,  "time", time, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", gameObject));

		}

        public void RotateTo(Vector3 from, Vector3 to, float time, SA_EaseType easeType = SA_EaseType.linear) {

            transform.rotation = Quaternion.Euler(from);
            FinalVectorValue = to;

            SA_iTween.RotateTo(gameObject, SA_iTween.Hash("rotation", to, "time", time, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", gameObject));

        }

        public void ScaleTo(Vector3 from, Vector3 to, float time,  SA_EaseType easeType = SA_EaseType.linear) {

			transform.localScale = from;
			FinalVectorValue = to;

			SA_iTween.ScaleTo(gameObject, SA_iTween.Hash("scale", to,  "time", time, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", gameObject));

		}
		

		public void VectorToS(Vector3 from, Vector3 to, float speed, SA_EaseType easeType = SA_EaseType.linear) {
			transform.position = from;
			FinalVectorValue = to;
			SA_iTween.MoveTo(gameObject, SA_iTween.Hash("position", to,  "speed", speed, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", gameObject));
		}

		public void Stop() {
			SA_iTween.Stop(gameObject);
			Destroy(gameObject);
		}

        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public SA_iSafeEvent OnComplete {
            get {
                return m_onComplete;
            }
        }

        public SA_iSafeEvent<float> OnValueChanged {
            get {
                return m_onValueChanged;
            }
        }

        public SA_iSafeEvent<Vector3> OnVectorValueChanged {
            get {
                return m_onVectorValueChanged;
            }
        }


        //--------------------------------------
        // Private Methods
        //--------------------------------------

        private void onTweenComplete() {

            if (gameObject == null || gameObject.Equals(null)) {
                return;
            }

            m_onValueChanged.Invoke(FinalFloatValue);
			m_onVectorValueChanged.Invoke(FinalVectorValue);

			m_onComplete.Invoke();

			if(DestoryGameObjectOnComplete) {
				Destroy(gameObject);
			} else {
				Destroy(this);
			}

		}

	}
}

