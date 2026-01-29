using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace OlympusVMS.Components.Pages.Calendar;

public partial class Calendar : ComponentBase
{
    // Europe-friendly: start week on Monday
    protected static readonly string[] DayHeaders = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    protected int Year { get; set; } = DateTime.Today.Year;
    protected int Month { get; set; } = DateTime.Today.Month;

    protected List<DateOnly?> Cells { get; set; } = new();
    protected int Weeks { get; set; }

    protected string CurrentMonthTitle =>
        new DateTime(Year, Month, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);

    protected override void OnInitialized()
    {
        BuildCalendar();
    }

    protected void BuildCalendar()
    {
        var first = new DateOnly(Year, Month, 1);
        var daysInMonth = DateTime.DaysInMonth(Year, Month);

        Console.WriteLine(Month);

        // DayOfWeek: Sunday=0 ... Saturday=6
        // Convert to Monday=0 ... Sunday=6
        var startOffset = (((int)first.DayOfWeek) + 6) % 7;

        var totalCells = startOffset + daysInMonth;

        // Most months fit in 5 weeks, some need 6; keep 5-6.
        Weeks = (int)Math.Ceiling(totalCells / 7.0);
        Weeks = Math.Max(5, Weeks);
        Weeks = Math.Min(6, Weeks);

        Cells = new List<DateOnly?>(Weeks * 7);

        for (int i = 0; i < Weeks * 7; i++)
        {
            var dayNumber = i - startOffset + 1;
            Cells.Add(dayNumber < 1 || dayNumber > daysInMonth
                ? null
                : new DateOnly(Year, Month, dayNumber));
        }
    }

    protected void PrevMonth()
    {
        var dt = new DateTime(Year, Month, 1).AddMonths(-1);
        Year = dt.Year;
        Month = dt.Month;
        BuildCalendar();
    }

    protected void NextMonth()
    {
        var dt = new DateTime(Year, Month, 1).AddMonths(1);
        Year = dt.Year;
        Month = dt.Month;
        BuildCalendar();
    }


    protected void PrevYear()
    {
        Year--;
        BuildCalendar();
    }

    protected void NextYear()
    {
        Year++;
        BuildCalendar();
    }

    protected void GoToday()
    {
        Year = DateTime.Today.Year;
        Month = DateTime.Today.Month;
        BuildCalendar();
    }

    protected string GetCellClass(DateOnly? date)
    {
        if (date is null) return "cal-cell cal-empty";

        var isToday = date.Value == DateOnly.FromDateTime(DateTime.Today);
        var isWeekend = date.Value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        return $"cal-cell{(isWeekend ? " cal-weekend" : "")}{(isToday ? " cal-today-cell" : "")}";
    }
}