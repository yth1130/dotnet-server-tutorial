using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    // 18. Thread Local Storage(TLS)
    class Program
    {
        // static string ThreadName; //모든 스레드가 공유한다.
        // static ThreadLocal<string> ThreadName = new ThreadLocal<string>(); //스레드 자신만의 공간에 저장을 한다.
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => $"My name is {Thread.CurrentThread.ManagedThreadId}"); //초기화가 안되어있으면 호출할 때 파라미터의 함수로 초기화한다.

        static void WhoAmI()
        {
            // ThreadName = $"My name is {Thread.CurrentThread.ManagedThreadId}";
            // ThreadName.Value = $"My name is {Thread.CurrentThread.ManagedThreadId}";
            bool repeat = ThreadName.IsValueCreated; //이미 만들어져있으면 true.
            if (repeat)
                Console.WriteLine(ThreadName.Value + "(repeat)");
            else
                Console.WriteLine(ThreadName.Value);

            // Thread.Sleep(1000);
            // Console.WriteLine(ThreadName);
            // Console.WriteLine(ThreadName.Value);
        }
        
        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);

            // ThreadName.Dispose(); //날리기.
        }
    }
}