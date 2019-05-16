using UnityEngine;
using System.Collections;
using System;
using SA.Foundation.Async;

namespace SA.Foundation.Tests {

    public class SA_SingleTestRunner {

        public event Action OnTestStart = delegate { };
        public event Action<SA_TestResult> OnTestFinished = delegate { };

        private SA_TestConfig m_testConfig;
        private SA_BaseTest m_test;
        //private Coroutine m_timeoutCoroutine;
        private static float TIMEOUT = 25f;

        public SA_SingleTestRunner(SA_TestConfig testConfig) {
            m_testConfig = testConfig;
        }

        public void Execute() {
            m_test = (SA_BaseTest)Activator.CreateInstance(m_testConfig.TestReference.Type);
            OnTestStart.Invoke();
            OnTestStart = delegate { };
            m_test.OnResult += TestResultHandler;
            if (!m_test.RequireUserInteraction) {
                // m_timeoutCoroutine = SA_Coroutine.WaitForSeconds(TIMEOUT, OnTimeOut);
                SA_Coroutine.WaitForSeconds(TIMEOUT, OnTimeOut);
            }
            try {
                m_test.Test();
            }
            catch (Exception e) {
                ClearListener();
                FireResult(SA_TestResult.WithError("Failed with Exception " + e.GetBaseException().Message));
            }

        }

        private void ClearListener() {
            m_test.OnResult -= TestResultHandler;

        }

        public string TestTitle {
            get {
                return m_test.Title;
            }
        }

        public bool TestStopsTestGroup {
            get {
                return m_testConfig.StopsNextTestsIfFail;
            }
        }

        private void FireResult(SA_TestResult obj) {
            OnTestFinished.Invoke(obj);
            OnTestFinished = delegate { };
        }

        private void OnTimeOut() {
            ClearListener();
            FireResult(SA_TestResult.TIMEOUT);
        }

        private void TestResultHandler(SA_BaseTest target, SA_TestResult obj) {
            ClearListener();
            FireResult(obj);
        }

    }
}