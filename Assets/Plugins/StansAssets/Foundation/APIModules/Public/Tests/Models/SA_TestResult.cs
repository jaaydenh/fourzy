using UnityEngine;
using System.Collections;
using SA.Foundation.Templates;
namespace SA.Foundation.Tests {
    public class SA_TestResult {
        public enum TestResultStatus {
            Ok,
            Timeout,
            Error
        }

        public TestResultStatus Status { get; private set; }
        public string Message { get; private set; }
        public bool IsFailed { get { return Status != TestResultStatus.Ok; } }

        public static SA_TestResult OK {
            get {
                return new SA_TestResult(TestResultStatus.Ok, "Test passed");
            }
        }

        public static SA_TestResult TIMEOUT {
            get {
                return new SA_TestResult(TestResultStatus.Timeout, "Reached Timeout");
            }
        }



        private SA_TestResult(TestResultStatus status, string message) {
            Status = status;
            Message = message;
        }

        public static SA_TestResult WithError(string message) {
            return new SA_TestResult(TestResultStatus.Error, message);
        }


        public static SA_TestResult FromSAResult(SA_Result result) {
            if (result == null) {
                return WithError("Result is null");
            }
            else if (result.IsFailed) {
                return WithError(result.Error.FullMessage);
            }
            else {
                return OK;
            }
        }
    }
}