using System;

using SA.Foundation.Patterns;

namespace SA.Foundation.Events
{

    public static class SA_MonoEvents 
    {

        private class SA_MonoEventsListner : SA_Singleton<SA_MonoEventsListner>
        {
            public SA_Event m_onApplicationQuit = new SA_Event();
            public SA_Event<bool> m_onApplicationFocus = new SA_Event<bool>();
            public SA_Event<bool> m_onApplicationPause = new SA_Event<bool>();

            public SA_Event m_onUpdate = new SA_Event();

            protected override void OnApplicationQuit() {
                base.OnApplicationQuit();
                m_onApplicationQuit.Invoke();
            }

            private void OnApplicationFocus(bool focus) {
                m_onApplicationFocus.Invoke(focus);
            }

            private void OnApplicationPause(bool pause) {
                m_onApplicationPause.Invoke(pause);
            }

            private void Update() {
                m_onUpdate.Invoke();
            }

        }




        public static SA_iEvent OnApplicationQuit {
            get {
                return SA_MonoEventsListner.Instance.m_onApplicationQuit;
            }
        }


        public static SA_iEvent<bool> OnApplicationFocus {
            get {
                return SA_MonoEventsListner.Instance.m_onApplicationFocus;
            }
        }

        public static SA_iEvent<bool> OnApplicationPause {
            get {
                return SA_MonoEventsListner.Instance.m_onApplicationPause;
            }
        }


        public static SA_iEvent OnUpdate {
            get {
                return SA_MonoEventsListner.Instance.m_onUpdate;
            }
        }



    }
}