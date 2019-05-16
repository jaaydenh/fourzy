using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace SA.Foundation.Tests
{
    public class SA_GroupTestRunner
    {
        private Action m_callback;
        public Action<string> OnTestStarted = delegate { };
        public Action<SA_TestResult> OnTestFinished = delegate { };
        public Action<string> OnGroupTestStarted = delegate { };

        private SA_TestGroupConfig m_testGroupConfig;
        private List<SA_TestConfig> m_tests = new List<SA_TestConfig>();
        private SA_SingleTestRunner m_currentTest;
        public SA_GroupTestRunner(SA_TestGroupConfig testGroupConfig) {
            m_testGroupConfig = testGroupConfig;
            m_tests.AddRange(m_testGroupConfig.Tests.GetRange(0, m_testGroupConfig.Tests.Count));
        }

        public void Execute(Action callback) {
            m_callback = callback;
            OnGroupTestStarted.Invoke(m_testGroupConfig.Name);
            RunNextTest();

        }

        private void RunNextTest() {
            if (m_tests.Count > 0) {
                m_currentTest = new SA_SingleTestRunner(m_tests[0]);
                m_currentTest.OnTestStart += TestStartHandler;
                m_currentTest.OnTestFinished += TestFinishedHandler;
                m_tests.RemoveAt(0);
                m_currentTest.Execute();
            }
            else {
                Finish();
            }
        }

        private void Finish() {
            m_callback.Invoke();
            OnTestStarted = delegate { };
            OnTestFinished = delegate { };
            OnGroupTestStarted = delegate { };
        }

        private void TestFinishedHandler(SA_TestResult obj) {
            OnTestFinished.Invoke(obj);
            if (obj.IsFailed && m_currentTest.TestStopsTestGroup) {
                m_currentTest = null;
                m_callback.Invoke();
            }
            else {
                m_currentTest = null;
                RunNextTest();
            }


        }

        private void TestStartHandler() {
            OnTestStarted.Invoke(m_currentTest.TestTitle);
        }
    }
}