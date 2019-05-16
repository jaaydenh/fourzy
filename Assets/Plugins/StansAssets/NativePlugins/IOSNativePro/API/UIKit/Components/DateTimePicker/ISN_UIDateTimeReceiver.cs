using System;

using SA.Foundation.Templates;
using SA.iOS.Utilities;

namespace SA.iOS.UIKit {


    /// <summary>
    /// FULLY DEPRECATED NEED TO BE REMOVED IN 2019 UPDATE
    /// </summary>
	public class ISN_UIDateTimeReceiver : ISN_Singleton<ISN_UIDateTimeReceiver>  {



		//--------------------------------------
		// Initialization
		//--------------------------------------

		public void Init() {
			
		}



		//--------------------------------------
		// Native Events
		//--------------------------------------


        /*
		void CalendarPickerClosed(string time) {
			iOS.UIKit.ISN_UICalendar.DatePicked (time);
		}
			
		void DateTimePickerClosed(string time) {
			iOS.UIKit.ISN_UIDateTimePicker.PickerClosed(time);
		}

		void DateTimePickerDateChanged(string time) {
			iOS.UIKit.ISN_UIDateTimePicker.DateChangedEvent(time);
		}*/
	}
}

