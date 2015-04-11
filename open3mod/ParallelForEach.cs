using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Amib.Threading;

namespace open3mod
{
    // Adapted from https://github.com/robvolk/Helpers.Net/blob/master/Src/Helpers.Net/ParallelProcessor.cs
    //
    // Restricted inputs to a List. We need to iterate it twice anyway to break it into chunks.
    public static class ParallelForEach
    {
        public const int DefaultMinChunkSize = 200;

        private static readonly SmartThreadPool DefaultSmartThreadPool = new SmartThreadPool(1000, 16, 8);

        /// <summary>
        /// Break |list| into slices of at least |minChunkSize| and schedule them on a thread pool.
        /// |action| is executed on each slice.
        /// </summary>
        public static void ParallelDo<T>(this IList<T> list, Action<IEnumerable<T>> action,
            int minChunkSize = DefaultMinChunkSize,
            SmartThreadPool threadPool = null)
        {
            threadPool = threadPool ?? DefaultSmartThreadPool;
            const int maxHandles = 64; // Maximum for WaitHandle
            var count = list.Count();
            if (count == 0)
            {
                return;
            }
            // WaitAll() does not support waiting for multiple handles on STA threads.
            if (count < minChunkSize * 2)
            {
                action(list);
                return;
            }
            var parallelism = Math.Min(maxHandles, Math.Max(1, count / minChunkSize));
            var effectiveChunkSize = count/parallelism;
            var items = new IWorkItemResult[parallelism];
            for (var offset = 0; offset < parallelism; offset++)
            {
                var start = effectiveChunkSize * offset;
                var chunk = list.Skip(start).Take(offset == parallelism - 1 ? count - start : effectiveChunkSize);
                items[offset] = threadPool.QueueWorkItem(() => action(chunk));
            }
            try
            {
                SmartThreadPool.WaitAll(items);
            }
            catch (ThreadInterruptedException ex)
            {
                foreach (var item in items)
                {
                    item.Cancel();
                }
                throw;
            }
        }

        /// <summary>
        /// Break |list| into slices of at least |minChunkSize| and schedule them on a thread pool.
        /// |action| is executed on each individual item.
        /// </summary>
        public static void ParallelDo<T>(this IList<T> list, Action<T> action,
            int minChunkSize = DefaultMinChunkSize,
            SmartThreadPool threadPool = null)
        {
            list.ParallelDo(
                (chunk) =>
                {
                    foreach (var elem in chunk)
                    {
                        action(elem);
                    }
                }, minChunkSize, threadPool);
        }
    }
}
