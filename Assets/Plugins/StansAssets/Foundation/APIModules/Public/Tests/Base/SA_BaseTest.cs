using UnityEngine;
using System.Collections;
using System;
using SA.Foundation.Templates;


namespace SA.Foundation.Tests
{
    public abstract class SA_BaseTest
    {
        private SA_TestResult m_result = null;
        public event Action<SA_BaseTest, SA_TestResult> OnResult = delegate { };


        public abstract void Test();


        public virtual bool RequireUserInteraction {
            get {
                return false;
            }
        }

        
        
        public virtual string Title {
            get {
                //TODO make it nice
                return GetType().Name;
            }
        }

        protected void SetResult(SA_TestResult result) {
            m_result = result;
            OnResult.Invoke(this, m_result);
        }

        protected void SetAPIResult(SA_Result result) {
            SetResult(SA_TestResult.FromSAResult(result));
        }

      

    }
}