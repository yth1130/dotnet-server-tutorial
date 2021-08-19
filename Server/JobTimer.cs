using System;
using ServerCore;

namespace Server
{
    struct JobTimerElement : IComparable<JobTimerElement>
    {
        public int execTick; // 실행 시간.
        public Action action;

        public int CompareTo(JobTimerElement other)
        {
            return other.execTick - execTick;
        }
    }

    class JobTimer
    {
        PriorityQueue<JobTimerElement> queue = new PriorityQueue<JobTimerElement>();
        object _lock = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElement job;
            job.execTick = Environment.TickCount + tickAfter;
            job.action = action;

            lock (_lock)
            {
                queue.Push(job);
            }
        }

        public void Flush()
        {
            while(true)
            {
                int now = Environment.TickCount;

                JobTimerElement job;

                lock (_lock)
                {
                    if (queue.Count == 0)
                        break;
                    
                    job = queue.Peek();
                    if (job.execTick > now)
                        break;
                    
                    queue.Pop();
                }

                job.action.Invoke();
            }
        }
    }
}