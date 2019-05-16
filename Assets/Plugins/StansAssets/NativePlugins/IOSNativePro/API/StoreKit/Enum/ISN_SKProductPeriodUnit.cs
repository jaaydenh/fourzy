using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Values representing the duration of an interval, from a day up to a year.
/// </summary>
public enum ISN_SKProductPeriodUnit
{
    Day = 0,     //An interval lasting one day.
    Week = 1,    //An interval lasting one month.
    Month = 2,   //An interval lasting one week.
    Year = 3     //An interval lasting one year.
}