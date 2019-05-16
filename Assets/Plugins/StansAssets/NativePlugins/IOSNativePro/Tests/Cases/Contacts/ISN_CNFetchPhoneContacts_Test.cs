using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Tests;

using SA.iOS.Contacts;

namespace SA.iOS.Tests.Contacts
{
    public class ISN_CNFetchPhoneContacts_Test : SA_BaseTest
    {
        public override void Test() {
            ISN_CNContactStore.FetchPhoneContacts((result) => {
                if (result.IsSucceeded) {
                    if (result.Contacts.Count == 0) {
                        SetResult(SA_TestResult.WithError("No Contacts inside the Sucsses result"));
                        return;
                    }

                    PrintContacts(result.Contacts);
                }

                SetAPIResult(result);
            });
        }


        public static void PrintContacts(List<ISN_CNContact> contacts) {
            foreach (var contact in contacts) {
                Debug.Log("contact.GivenName: " + contact.GivenName);
                Debug.Log("contact.FamilyName: " + contact.FamilyName);
                Debug.Log("contact.Nickname: " + contact.Nickname);
                Debug.Log("contact.OrganizationName: " + contact.OrganizationName);
                Debug.Log("contact.DepartmentName: " + contact.DepartmentName);
                Debug.Log("contact.JobTitle: " + contact.JobTitle);

                foreach (var emails in contact.Emails) {
                    Debug.Log("contact.Emails: " + emails);
                }

                foreach (var phone in contact.Phones) {
                    Debug.Log("contact.Phones: " + phone.CountryCode + " / " + phone.Digits);
                }

            }
        }
    }
}