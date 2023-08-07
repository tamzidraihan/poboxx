using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartBox.Business.Shared
{
    public static class JobScheduler
    {
        public static string Schedule(Expression<Action> methodToCall, DateTimeOffset enqueueAt)
        {

            return BackgroundJob.Schedule(methodToCall, enqueueAt);
        }

        public static string Schedule(Expression<Func<Task>> methodToCall, DateTimeOffset enqueueAt)
        {
            return BackgroundJob.Schedule(methodToCall, enqueueAt);
        }

        public static string Schedule<T>(Expression<Func<T, Task>> methodToCall, DateTimeOffset enqueueAt)
        {
            return BackgroundJob.Schedule<T>(methodToCall, enqueueAt);
        }

        public static string Schedule<T>(Expression<Action<T>> methodToCall, DateTimeOffset enqueueAt)
        {
            return BackgroundJob.Schedule<T>(methodToCall, enqueueAt);
        }


        public static string Enqueue(Expression<Action> methodToCall)
        {
            return BackgroundJob.Enqueue(methodToCall);
        }
        public static string Enqueue(Expression<Func<Task>> methodToCall)
        {
            return BackgroundJob.Enqueue(methodToCall);
        }

        public static string Enqueue<T>(Expression<Func<T, Task>> methodToCall)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            return BackgroundJob.Enqueue<T>(methodToCall);
        }

        public static string Enqueue<T>(Expression<Action<T>> methodToCall)
        {
            return BackgroundJob.Enqueue<T>(methodToCall);
        }

        public static bool Delete(string jobId)
        {
            return BackgroundJob.Delete(jobId);
        }

        public static void AddOrUpdateRecurringJob<T>(string jobId, Expression<Func<T, Task>> methodToCall, string cronExpression)
        {
            RecurringJob.AddOrUpdate<T>(methodToCall, cronExpression);
        }
    }
}
