﻿//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class CreateRealtimeGamePrompt : PromptScreen
    {
        [SerializeField]
        private Image checkmark;
        [SerializeField]
        private Sprite checkmarkOn;
        [SerializeField]
        private Sprite checkmarkOff;
        [SerializeField]
        private ScrollRect areasContainer;
        [SerializeField]
        private ButtonExtended magicButton;

        private bool passwordEnabled = true;
        private LoadingPromptScreen _prompt;

        public PracticeScreenAreaSelectWidget currentAreaWidget { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimerOut;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimerOut;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateCheckmarkButton();

            _prompt = PersistantMenuController.Instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);

            magicButton.SetActive(Constants.MAGIC_TOGGLE_ACTIVE_STATE);

            //load areas
            bool first = true;
            foreach (Serialized.AreasDataHolder.GameArea areaData in GameContentManager.Instance.areasDataHolder.areas)
            {
                if (first)
                {
                    first = false;
                    SetArea(AddAreaWidget(areaData.areaID));
                }
                else
                {
                    AddAreaWidget(areaData.areaID);
                }
            }

            //load tokens
        }

        public override void Accept(bool force = false)
        {
            base.Accept(force);

            FourzyPhotonManager.onCreateRoomFailed += OnCreateRoomFailed;
            _prompt
                .Prompt("Retrieving player stats...", "", null, null, null, () =>
                {
                    FourzyPhotonManager.onCreateRoomFailed -= OnCreateRoomFailed;
                })
                .CloseOnDecline();

            UserManager.GetPlayerRating(rating =>
            {
                CreateRoom();

                if (_prompt.isOpened)
                {
                    _prompt.promptTitle.text = "Creating new room";
                }
            }, () =>
            {
                if (_prompt.isOpened)
                {
                    _prompt.Decline(true);
                }
            });
        }

        private void CreateRoom()
        {
            string password = "";

            if (passwordEnabled)
            {
                password = Guid.NewGuid().ToString().Substring(0, Constants.REALTIME_ROOM_PASSWORD_LENGTH);
            }

            FourzyPhotonManager.CreateRoom(RoomType.LOBBY_ROOM, password: password);
        }

        public void ToggleCheckmark()
        {
            passwordEnabled = !passwordEnabled;
            UpdateCheckmarkButton();
        }

        public void ToggleMagic() => SettingsManager.Toggle(SettingsManager.KEY_REALTIME_MAGIC);

        public void ToggleRealtimeTimer() => SettingsManager.Toggle(SettingsManager.KEY_REALTIME_TIMER);

        public void SetArea(PracticeScreenAreaSelectWidget widget)
        {
            if (currentAreaWidget)
            {
                if (currentAreaWidget != widget)
                {
                    currentAreaWidget.Deselect();
                    SetAreaWidget(widget);
                }
            }
            else
            {
                SetAreaWidget(widget);
            }

            void SetAreaWidget(PracticeScreenAreaSelectWidget PracticeWidget)
            {
                currentAreaWidget = PracticeWidget;
                currentAreaWidget.Select();

                PlayerPrefsWrapper.SetCurrentArea((int)PracticeWidget.Area);
            }
        }

        protected PracticeScreenAreaSelectWidget AddAreaWidget(Area area)
        {
            PracticeScreenAreaSelectWidget instance = Instantiate(GameContentManager.GetPrefab<PracticeScreenAreaSelectWidget>("AREA_SELECT_WIDGET_SMALL"), areasContainer.content)
                .SetData(area);
            instance.button.onTap += data => SetArea(instance);

            return instance;
        }

        private void UpdateCheckmarkButton()
        {
            checkmark.sprite = passwordEnabled ? checkmarkOn : checkmarkOff;
        }

        private void OnJoinedRoom(string roomName)
        {
            if (!isOpened) return;

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }

            if (isOpened)
            {
                CloseSelf();
            }

            AudioHolder.instance.PlaySelfSfxOneShotTracked("game_found");
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);
        }

        private void OnCreateRoomFailed(string error)
        {
            CreateRoom();
        }

        private void OnConnectionTimerOut()
        {
            if (isOpened)
            {
                CloseSelf();
            }

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }
        }
    }
}