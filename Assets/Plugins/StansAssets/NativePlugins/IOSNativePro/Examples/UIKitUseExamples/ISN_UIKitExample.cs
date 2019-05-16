using System;
using UnityEngine;

using SA.Foundation.Async;
using SA.iOS.UIKit;
using SA.iOS.Utilities;

namespace SA.iOS.Examples
{

    public class ISN_UIKitExample : ISN_BaseIOSFeaturePreview
    {

        [SerializeField] Texture2D m_icon = null;

        private void Awake() {
            ISN_UIDateTimePicker.OnPickerDateChanged.AddListener((DateTime date) => {
                ISN_Logger.Log("User changed a date to: " + date.ToLongDateString());
            });
        }


        void OnGUI() {
            UpdateToStartPos();

            GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Popups", style);



            StartY += YLableStep;
            if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Load Store")) {
                ISN_UIAlertController alert = new ISN_UIAlertController("My Alert", "granted", ISN_UIAlertControllerStyle.Alert);
                ISN_UIAlertAction defaultAction = new ISN_UIAlertAction("Ok", ISN_UIAlertActionStyle.Default, () => {
                    //Do something
                });

                defaultAction.SetImage(m_icon);

                alert.AddAction(defaultAction);
                alert.Present();


                SA_Coroutine.WaitForSeconds(3f, () => {
                    alert.Dismiss();
                });
            }




            StartX = XStartPos;
            StartY += YButtonStep;

            if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Perform Buy #1")) {
                ISN_UIAlertController alert = new ISN_UIAlertController("My Alert", "declined", ISN_UIAlertControllerStyle.Alert);
                ISN_UIAlertAction defaultAction = new ISN_UIAlertAction("Ok", ISN_UIAlertActionStyle.Default, () => {
                    //Do something
                });

                ISN_UIAlertAction defaultAction2 = new ISN_UIAlertAction("No", ISN_UIAlertActionStyle.Default, () => {
                    //Do something
                });

                defaultAction.Enabled = true;
                defaultAction2.Enabled = false;


                ISN_UIAlertAction prefAction = new ISN_UIAlertAction("Hit me", ISN_UIAlertActionStyle.Default, () => {
                    //Do something
                    Debug.Log("Got it!!!!");
                });

                prefAction.MakePreffered();

                alert.AddAction(defaultAction);
                alert.AddAction(defaultAction2);
                alert.AddAction(prefAction);
                alert.Present();

               
            }

            StartX += XButtonStep;
            if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Perform Buy #2")) {
                ISN_Preloader.LockScreen();

                SA_Coroutine.WaitForSeconds(3f, () => {
                    ISN_Preloader.UnlockScreen();
                });
            }


            StartX = XStartPos;
            StartY += YButtonStep;
            if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Calendar Picker")) {

                ISN_UICalendar.PickDate((DateTime date) => {
                    ISN_Logger.Log("User picked date: " + date.ToLongDateString());
                });
            }


            StartX += XButtonStep;
            if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Date Time Picker")) {
               
                //20 days ago
                DateTime starDate = DateTime.Now;
                starDate = starDate.AddDays(-20);

                ISN_UIDateTimePicker picker = new ISN_UIDateTimePicker();
                picker.SetDate(starDate);

                picker.Show((DateTime date) => {
                    ISN_Logger.Log("User picked date: " + date.ToLongDateString());
                });
            }

            StartX += XButtonStep;
            if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Date Picker")) {

                //20 days ago
                DateTime starDate = DateTime.Now;
                starDate = starDate.AddDays(-20);

                ISN_UIDateTimePicker picker = new ISN_UIDateTimePicker();
                picker.SetDate(starDate);
                picker.DatePickerMode = ISN_UIDateTimePickerMode.Date;

                picker.Show((DateTime date) => {
                    ISN_Logger.Log("User picked date: " + date.ToLongDateString());
                });
            }

            StartX += XButtonStep;
            if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Time Picker")) {

                //20 hours ago
                DateTime starDate = DateTime.Now;
                starDate =  starDate.AddHours(-20);

                ISN_UIDateTimePicker picker = new ISN_UIDateTimePicker();
                picker.SetDate(starDate);
                picker.DatePickerMode = ISN_UIDateTimePickerMode.Time;
                picker.MinuteInterval = 30;

                picker.Show((DateTime date) => {
                    ISN_Logger.Log("User picked date: " + date.ToLongDateString());
                });
            }

            StartX += XButtonStep;
            if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Countdown Timer")) {
            
                ISN_UIDateTimePicker picker = new ISN_UIDateTimePicker();
             
                picker.DatePickerMode = ISN_UIDateTimePickerMode.CountdownTimer;
                picker.MinuteInterval = 10;

                //Set countdown start duration
                int hours = 5;
                int minutes = 20;
                int seconds = 0;
                TimeSpan span = new TimeSpan(hours, minutes, seconds);
                picker.SetCountDownDuration(span);

                picker.Show((DateTime date) => {
                    ISN_Logger.Log("User picked date: " + date.ToLongDateString());
                });
            }

          
        }




    }
}