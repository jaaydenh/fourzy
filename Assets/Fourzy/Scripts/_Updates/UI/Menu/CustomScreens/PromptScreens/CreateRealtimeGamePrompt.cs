//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Toasts;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class CreateRealtimeGamePrompt : PromptScreen
    {
        public Image checkmark;
        public Sprite checkmarkOn;
        public Sprite checkmarkOff;
        public ScrollRect areasContainer;
        public PracticeScreenAreaSelectWidget areaWidgetPrefab;

        private bool passwordEnabled;
        private LoadingPromptScreen _prompt;

        public PracticeScreenAreaSelectWidget currentAreaWidget { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onCreateRoomFailed += OnCreateRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimerOut;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onCreateRoomFailed -= OnCreateRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimerOut;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateCheckmarkButton();

            _prompt = PersistantMenuController.Instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);

            //load areas
            bool first = true;
            foreach (Serialized.AreasDataHolder.GameArea areaData in GameContentManager.Instance.enabledAreas)
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

            _prompt
                .Prompt("Retrieving player stats...", "", null, null, null, null)
                .CloseOnDecline();

            UserManager.GetPlayerRating(rating =>
            {
                string password = "";

                if (passwordEnabled)
                    password = Guid.NewGuid().ToString().Substring(0, Constants.REALTIME_ROOM_PASSWORD_LENGTH);

                FourzyPhotonManager.CreateRoom(RoomType.LOBBY_ROOM, password: password);

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


            void SetAreaWidget(PracticeScreenAreaSelectWidget widget)
            {
                currentAreaWidget = widget;
                currentAreaWidget.Select();

                PlayerPrefsWrapper.SetCurrentArea((int)widget.area);
            }
        }

        protected PracticeScreenAreaSelectWidget AddAreaWidget(Area area)
        {
            PracticeScreenAreaSelectWidget instance = Instantiate(areaWidgetPrefab, areasContainer.content)
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

            AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.GAME_FOUND);
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);
        }

        private void OnCreateRoomFailed(string error)
        {
            if (isOpened)
            {
                GamesToastsController.ShowTopToast($"Failed: {error}");
            }

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }
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