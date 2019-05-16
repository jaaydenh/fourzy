////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.Contacts {

    /// <summary>
    /// Class that represents an immutable value object for contact properties, 
    /// such as the first name and phone numbers of a contact.
    /// 
    /// More filed can be added upon request
    /// <see href="https://developer.apple.com/documentation/contacts/cncontact?language=objc">CNContact</see>
    /// </summary>
	[Serializable]
    public class ISN_CNContact  {
#pragma warning disable 649
        [SerializeField] string m_givenName;
        [SerializeField] string m_familyName;
        [SerializeField] string m_nickname;
        [SerializeField] string m_organizationName;
        [SerializeField] string m_departmentName;
        [SerializeField] string m_jobTitle;

        [SerializeField] List<string> m_emails;
        [SerializeField] List<ISN_CNPhoneNumber> m_phones;

#pragma warning restore 649


        public ISN_CNContact(string givenName, string familyName) {
            m_givenName = givenName;
            m_familyName = familyName;
        }


        /// <summary>
        /// The given name of the contact.
        /// The given name is often known as the first name of the contact.
        /// </summary>
        public string GivenName {
            get {
                return m_givenName;
            }
        }

        /// <summary>
        /// The family name of the contact.
        /// The family name is often known as the last name of the contact.
        /// </summary>
        public string FamilyName {
            get {
                return m_familyName;
            }
        }


        public string Nickname {
            get {
                return m_nickname;
            }
        }

        /// <summary>
        /// The name of the organization associated with the contact.
        /// </summary>
        public string OrganizationName {
            get {
                return m_organizationName;
            }

            set {
                m_organizationName = value;
            }
        }


        /// <summary>
        /// The name of the department associated with the contact.
        /// </summary>
        public string DepartmentName {
            get {
                return m_departmentName;
            }
        }

        /// <summary>
        /// The contact’s job title.
        /// </summary>
        public string JobTitle {
            get {
                return m_jobTitle;
            }
        }

        /// <summary>
        /// An array of labeled email addresses for the contact.
        /// </summary>
        public List<string> Emails {
            get {
                return m_emails;
            }
        }

        /// <summary>
        /// An array of labeled phone numbers for a contact.
        /// </summary>
        public List<ISN_CNPhoneNumber> Phones {
            get {
                return m_phones;
            }
        }
    }
}