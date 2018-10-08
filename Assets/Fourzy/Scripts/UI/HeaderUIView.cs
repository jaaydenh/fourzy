using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class HeaderUIView : MonoBehaviour
    {
        [SerializeField] Text coinsLabel;

        private void Start()
        {
            coinsLabel.text = UserManager.Instance.coins.ToString();
        }

        private void OnEnable()
        {
            UserManager.OnUpdateUserInfo += UserManager_OnUpdateUserInfo;
        }

        private void OnDisable()
        {
            UserManager.OnUpdateUserInfo -= UserManager_OnUpdateUserInfo;
        }

        void UserManager_OnUpdateUserInfo()
        {
            coinsLabel.text = UserManager.Instance.coins.ToString();
        }

    }
}
