using UnityEngine;
using System;

/// Time iteration class.
///
/// Component of the sky dome parent game object.

public class TOD_Time : MonoBehaviour
{
    /// Day length inspector variable.
    /// Length of one day in minutes.
    public float DayLengthInMinutes = 30;

    /// Date progression inspector variable.
    /// Automatically updates Cycle.Day if enabled.
    public bool ProgressDate = true;

    /// Moon phase progression inspector variable.
    /// Automatically updates Moon.Phase if enabled.
    public bool ProgressMoonPhase = true;

    private TOD_Sky sky;
    private float hour = 12;
    private int day = 1;
    private int month = 3;
    private int year = 2000;
    private float moonPhase = 0;


    protected void Start()
    {
        sky = GetComponent<TOD_Sky>();

        //Set start values
        hour = sky.Cycle.Hour;
        day = sky.Cycle.Day;
        month = sky.Cycle.Month;
        moonPhase = sky.Cycle.MoonPhase;
    }

    public void Reset()
    {
        if (sky)
        {
            sky.Cycle.Hour = hour;
            sky.Cycle.Day = day;
            sky.Cycle.Month = month;
            sky.Cycle.MoonPhase = moonPhase;
        }
    }

    protected void Update()
    {
        float oneDay = DayLengthInMinutes * 60;
        float oneHour = oneDay / 24;

        float hourIter = Time.deltaTime / oneHour;
        float moonIter = Time.deltaTime / (30*oneDay) * 2;

        sky.Cycle.Hour += hourIter;

        if (ProgressMoonPhase)
        {
            sky.Cycle.MoonPhase += moonIter;
            if (sky.Cycle.MoonPhase < -1) sky.Cycle.MoonPhase += 2;
            else if (sky.Cycle.MoonPhase > 1) sky.Cycle.MoonPhase -= 2;
        }

        if (sky.Cycle.Hour >= 24)
        {
            sky.Cycle.Hour = 0;

            if (ProgressDate)
            {
                int daysInMonth = DateTime.DaysInMonth(sky.Cycle.Year, sky.Cycle.Month);
                if (++sky.Cycle.Day > daysInMonth)
                {
                    sky.Cycle.Day = 1;
                    if (++sky.Cycle.Month > 12)
                    {
                        sky.Cycle.Month = 1;
                        sky.Cycle.Year++;
                    }
                }
            }
        }
    }
}
