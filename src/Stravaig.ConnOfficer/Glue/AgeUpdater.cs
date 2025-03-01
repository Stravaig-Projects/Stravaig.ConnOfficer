using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Timers;

namespace Stravaig.ConnOfficer.Glue;

public static class Age
{
    public static Disposable Updater(Func<DateTime> createdDateFunc, Func<TimeSpan, string> humaniseAgeFunc, Action<string> updateAgeAction)
    {
        var timer = new Timer();
        UpdateAge(createdDateFunc, humaniseAgeFunc, updateAgeAction, timer, DateTime.UtcNow);
        return new Disposable(timer);
    }

    private static void ScheduleDelayedAction(Func<DateTime> createdDateFunc, Func<TimeSpan, string> humaniseAgeFunc, Action<string> updateAgeAction, Timer timer)
    {
        timer.Elapsed -= null;

        timer.AutoReset = false;
        timer.Elapsed += (sender, args) => UpdateAge(createdDateFunc, humaniseAgeFunc, updateAgeAction, timer, args.SignalTime);
        var initialDelay = DelayUntilNextUpdate(DateTime.UtcNow, createdDateFunc());
        timer.Interval = initialDelay;
        Debug.WriteLine($"Scheduling next update in {(double)initialDelay / TimeSpan.TicksPerSecond:F3} seconds.");
        timer.Start();
    }

    private static void UpdateAge(Func<DateTime> createdDateFunc, Func<TimeSpan, string> humaniseAgeFunc, Action<string> updateAge, Timer timer, DateTime signalTime)
    {
        var now = signalTime.ToUniversalTime();
        var createdDate = createdDateFunc();
        var age = now - createdDate;
        var humanisedAge = humaniseAgeFunc(age);
        Debug.WriteLine($"Updating age to {humanisedAge}.");
        Dispatcher.UIThread.InvokeAsync(() => updateAge(humanisedAge));
        ScheduleDelayedAction(createdDateFunc, humaniseAgeFunc, updateAge, timer);
    }

    private static long DelayUntilNextUpdate(DateTime now, DateTime createdDate)
    {
        var age = (now - createdDate).Ticks;
        return age switch
        {
            < TimeSpan.TicksPerMinute => TimeSpan.TicksPerSecond - (age % TimeSpan.TicksPerSecond),
            < TimeSpan.TicksPerHour => TimeSpan.TicksPerMinute - (age % TimeSpan.TicksPerMinute),
            < TimeSpan.TicksPerDay => TimeSpan.TicksPerHour - (age % TimeSpan.TicksPerHour),
            _ => TimeSpan.TicksPerDay - (age % TimeSpan.TicksPerDay),
        };
    }

    public class Disposable : IDisposable
    {
        private readonly Timer _timer;

        public Disposable(Timer timer)
        {
            _timer = timer;
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Elapsed -= null;
            _timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
