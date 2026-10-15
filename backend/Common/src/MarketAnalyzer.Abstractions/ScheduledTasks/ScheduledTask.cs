using System;

namespace MarketAnalyzer.Abstractions.ScheduledTasks
{
    /// <summary>
    /// شناسه تسک که به شکل زیر تعیین میشود:
    /// <para>typeof(ScheduledTask).FullName</para>
    /// </summary>
    public class ScheduledTask : AggregateRoot<string>
    {
        public ScheduledTask(string id) : base(id)
        {

        }

        /// <summary>
        /// دوره‌ی اجرای تسک
        /// </summary>
        public TimeSpan Period { get; private set; }
        /// <summary>
        /// آخرین زمان اجرای تسک
        /// </summary>
        public DateTime? LastRunTime { get; private set; }
        /// <summary>
        /// آخرین اجرا موفق بوده یا خیر
        /// </summary>
        public bool LastRunSucceed { get; private set; }
        /// <summary>
        /// زمان شروع
        /// </summary>
        public TimeOnly StartTime { get; private set; }
        /// <summary>
        /// زمان پایان
        /// </summary>
        public TimeOnly EndTime { get; private set; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public bool Disabled { get; private set; }
        /// <summary>
        /// شرحی از کاری که این تسک انجام میدهد
        /// </summary>
        public string Description { get; private set; }

        public static ScheduledTask Create(
            string id,
            TimeSpan period,
            TimeOnly startTime,
            TimeOnly endTime,
            string description)
        {
            return new ScheduledTask(id)
            {
                Period = period,
                StartTime = startTime,
                EndTime = endTime,
                Description = description,
                Disabled = false
            };
        }

        public void Update(
            TimeSpan period,
            TimeOnly startTime,
            TimeOnly endTime,
            string description)
        {
            Period = period;
            StartTime = startTime;
            EndTime = endTime;
            Description = description;
        }

        public void Disable(bool disabled)
        {
            Disabled = disabled;
        }

        public void UpdateLastRun(DateTime lastRunTime, bool lastRunSucceed)
        {
            LastRunTime = lastRunTime;
            LastRunSucceed = lastRunSucceed;
        }

    }
}
