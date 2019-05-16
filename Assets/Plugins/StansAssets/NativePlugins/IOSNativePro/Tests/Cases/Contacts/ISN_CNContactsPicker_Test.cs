using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Tests;

using SA.iOS.Contacts;

namespace SA.iOS.Tests.Contacts
{
    public class ISN_CNContactsPicker_Test : SA_BaseTest
    {
        public override void Test() {
            ISN_CNContactStore.ShowContactsPickerUI((result) => {
                if (result.IsSucceeded) {
                    ISN_CNFetchPhoneContacts_Test.PrintContacts(result.Contacts);
                } 

                SetAPIResult(result);
            });
        }
    }
}