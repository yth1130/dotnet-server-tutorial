using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    // class Program
    // {
#region 1.스레드 / 2.스레드 풀 / 3.태스크.
        // static void MainThread(object state)
        // {
        //     // while(true)
        //     for (int i = 0; i < 5; i++)
        //         Console.WriteLine("Hello Thread!");
        // }
        
        // static void Main(string[] args)
        // {
        //     // // 1. 스레드.
        //     // Thread thread = new Thread(MainThread); // 기본적으로 foreground 스레드로 만들어진다.
        //     // thread.Name = "Test Thread";
        //     // thread.IsBackground = true; // 백그라운드로 동작하게 되면 메인 프로그램이 끝나면 스레드도 끝난다.
        //     // thread.Start();
        //     // Console.WriteLine("Waiting for Thread!");
        //     // thread.Join(); // 스레드가 끝날 때까지 기다린다.
        //     // Console.WriteLine("Finish");

        //     // 2. 스레드 풀.
        //     ThreadPool.QueueUserWorkItem(MainThread); // 따로 스레드를 만들지 않고 제공되는 스레드 사용. 백그라운드로 돈다. 짧은 일감을 주는 것이 좋다.
        //     //긴 일감을 줄 경우 모든 스레드가 사용중이 되어 ThreadPool이 먹통이 될 수있다.
        //     // while(true) {}

        //     // ThreadPool.SetMinThreads(1, 1);
        //     // ThreadPool.SetMaxThreads(5, 5);
        //     // for (int i = 0; i < 5; i++) // 최대 다섯개인데 그 다섯개가 전부 오래 걸리는 일을 하고있으면 밑에 다른 작업을 줄 수 없다. 네개로 하면 한개가 남아서 가능.
        //     //     ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });
        //     // Console.WriteLine("Hello World!");
        //     // ThreadPool.QueueUserWorkItem(MainThread);

        //     // // 3. 태스크.
        //     // Task task = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning); // 스레드풀에서 작업을 처리한다.
        //     // task.Start();

        //     // ThreadPool.SetMinThreads(1, 1);
        //     // ThreadPool.SetMaxThreads(5, 5);
        //     // for (int i = 0; i < 5; i++)
        //     // {
        //     //     Task task = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning); // 오래 걸리는 작업이라는 것을 알려서 별도로 처리. 최소/최대 갯수에서 제외.
        //     //     task.Start();
        //     // }
        //     // Console.WriteLine("Hello World!");
        //     // ThreadPool.QueueUserWorkItem(MainThread);

        //     // 정리.
        //     // 스레드를 직접 관리(정직원)할 일이 거의 없고 스레드 풀(알바, 인력사무소)을 사용.
        //     // 오래 걸리는 일은 태스크(외주?)로 만들어서 실행.
        // }
#endregion

#region 4.컴파일러의 코드 최적화에 따른 오류, volatile / 5.캐시 테스트.
        // // static bool stop = false;
        // volatile static bool stop = false; //밑에서의 최적화를 통한 오류를 막기 위해 volatile 키워드를 붙힌다. 해당 필드가 여러 스레드에 의해 수정될 수 있음을 나타낸다. 쓰지 않는것이 좋다..
        // static void ThreadMain()
        // {
        //     Console.WriteLine("스레드 시작");
        //     while(stop == false) // stop 변수가 해당 블럭에서 바뀔 여지가 없으면 릴리즈 모드에서 최적화를 통해 코드를 바꿀 가능성이 있음. -> if(stop == false) { while(true){ } } 
        //     {
        //         //stop신호를 기다린다.
        //     }
        //     Console.WriteLine("스레드 종료");
        // }

        // static void Main(string[] args)
        // {
        //     // 4. 컴파일러가 코드 최적화를 하면 멀티 스레드 프로그램에서 오류를 유발할 수 있다.
        //     Task task = new Task(ThreadMain);
        //     task.Start();

        //     Thread.Sleep(1000); //메인 스레드를 1초동안 대기.
        //     stop = true;

        //     Console.WriteLine("Stop 호출");
        //     Console.WriteLine("종료 대기중");
        //     task.Wait(); // 태스크가 끝나는것을 기다린다. 스레드의 Join 같은 기능.
        //     Console.WriteLine("종료 성공");

        //     // 5.캐시 테스트. 시간적으로 방금 접근했던 것을 또 사용할 확률이 높음. 공간적으로 근처의 메모리를 접근할 확률이 높음.
        //     // CPU(코어(ALU/연산, 캐시(레지스터, L1캐시, L2캐시)), 코어, 코어...)
        //     int[,] array = new int[10000, 10000];
        //     {
        //         long now = DateTime.Now.Ticks;
        //         for (int y = 0; y < 10000; y++)
        //         {
        //             for (int x = 0; x < 10000; x++)
        //             {
        //                 array[y, x] = 1; // 인접 주소를 캐시.
        //             }
        //         }
        //         long end = DateTime.Now.Ticks;
        //         Console.WriteLine($"(y, x) 순서 걸린 시간 : {end - now}");
        //     }
        //     {
        //         long now = DateTime.Now.Ticks;
        //         for (int y = 0; y < 10000; y++)
        //         {
        //             for (int x = 0; x < 10000; x++)
        //             {
        //                 array[x, y] = 1; // 주소가 한참 떨어져있다.
        //             }
        //         }
        //         long end = DateTime.Now.Ticks;
        //         Console.WriteLine($"(x, y) 순서 걸린 시간 : {end - now}");
        //     }
        // }
#endregion

#region 6.하드웨어의 코드 최적화에 따른 오류, 메모리 배리어, 가시성.
        // // 6.하드웨어 최적화에 의한 오류. CPU가 명령어들간에 의존성이 없다고 판단하면 명령어의 순서를 바꿀 수 있다.

        // // 메모리 배리어?
        // // 1) 코드 재배치 억제.
        // // 2) 가시성. 캐시에 적용된 정보를 주기억장치에 올린다 / 캐시의 정보를 주기억장치의 정보로 최신화 한다.
        // // 원리를 이해하면 좋다. 실제로 쓸일은 없을듯?

        // // 종류.
        // // 1) Full Memory Barrier (ASM(어셈블리 명령어) MFENCE에 해당, C# Thread.MemoryBarrier) : Store/Load 둘다 막는다.
        // // 2) Store Memory Barrier (ASM SFENCE) : Store만 막는다.
        // // 3) Load Memory Barrier (ASM LFENCE) : Load만 막는다.

        // static int x = 0;
        // static int y = 0;
        // static int r1 = 0;
        // static int r2 = 0;

        // // 안의 두 명령이 연관성이 없기 때문에 CPU가 성능이 더 좋다고 판단되는 순서로 실행 순서를 바꿀 수 있음.
        // static void Thread1()
        // {
        //     // y = 1; // Store y.
        //     // r1 = x; // Load x.

        //     y = 1; // Store y.
        //     Thread.MemoryBarrier();
        //     r1 = x; // Load x.
        // }

        // static void Thread2()
        // {
        //     // x = 1; // Store x.
        //     // r2 = y; // Load y.
            
        //     x = 1; // Store x.
        //     Thread.MemoryBarrier();
        //     r2 = y; // Load y.
        // }

        // static void Main(string[] args)
        // {
        //     int count = 0;
        //     while(true)
        //     {
        //         count++;
        //         x = y = r1 = r2 = 0;

        //         Task task1 = new Task(Thread1);
        //         Task task2 = new Task(Thread2);
        //         task1.Start();
        //         task2.Start();

        //         Task.WaitAll(task1, task2);

        //         if (r1 == 0 && r2 == 0)
        //             break;
        //     }
        //     Console.WriteLine($"{count}번만에 빠져나옴!");
        // }
        
        // int answer;
        // bool complete;
        // void A()
        // {
        //     answer = 123;
        //     Thread.MemoryBarrier(); // Barrier 1.코드 재배치와 가시성.
        //     complete = true;
        //     Thread.MemoryBarrier(); // Barrier 2. complete멤버의 가시성을 위해서.
        // }
        // void B()
        // {
        //     Thread.MemoryBarrier(); // Barrier 3. 가시성을 위해.
        //     if (complete)
        //     {
        //         Thread.MemoryBarrier(); // Barrier 4. 가시성.
        //         Console.WriteLine(answer);
        //     }
        // }
        // static void Main(string[] args)
        // {
            
        // }
#endregion
        
#region 7.Race Condition(경합 조건), Atomic(원자성), Interlocked
        // // 경합조건 : 두 스레드가 동시에 같은 기억장소에 액세스할 때 수행 결과를 예측할 수 없게 되는 것.
        // // atominc = 원자성. 더 이상 쪼갤수 없는 성질.

        // // 골드 -= 100;
        //  // 서버 다운. <- 위 아래 명령이 한번에 처리되지 않으면 골드는 소비하고 검은 얻지 못하는 상황이 생김.
        // // 인벤 += 검;

        // // User2인벤 += 집행검;
        //  // 서버 다운 <- 아이템 복사!
        // // User1인벤 -= 집행검;

        // static int number = 0;
        // static void Thread1()
        // {
        //     for (int i = 0; i < 1000000; i++)
        //     {
        //         // number++;
        //         // 위의 코드는 어셈블리 코드로 세단계에 걸쳐 실행. 
        //         // number를 불러와 1을 더하고 다시 넣어준다. 밑에 세줄과 같음.
        //         // int temp = number;
        //         // temp += 1;
        //         // number = temp;
 
        //         Interlocked.Increment(ref number); // 위의 증가연산을 원자적으로 처리. Memory Barrier를 간접적으로 사용.
        //     }
        // }
        // static void Thread2()
        // {
        //     for (int i = 0; i < 1000000; i++)
        //     {
        //         // number--;
        //         Interlocked.Decrement(ref number);
        //     }
        // }
        // static void Main(string[] args)
        // {
        //     Task task1 = new Task(Thread1);
        //     Task task2 = new Task(Thread2);
        //     task1.Start();
        //     task2.Start();

        //     Task.WaitAll(task1, task2);

        //     Console.WriteLine(number);
        // }
#endregion

#region 8.상호 배제, 데드락, Monitor, lock
        // // 상호 배제(Mutual Exclusive) : 공유 불가능한 자원의 동시 사용을 피한다.
        // // C++ : CriticalSection std:mutex
        // // C# : Monitor

        // static int number = 0;
        // static object obj = new Object();

        // static void Thread1()
        // {
        //     for (int i = 0; i < 100000; i++)
        //     {
        //         // Monitor.Enter(obj); // 문을 잠그는 행위.
        //         // {
        //         //     number++;
        //         //     //return; // 잠금을 풀지 않게 된다. Thread2에서 계속 대기하게 됨. -> 데드락(Deadlock)
        //         // }
        //         // Monitor.Exit(obj); // 잠금을 풀어준다.

        //         // 잠금을 풀어주지 않는 상황을 막는 방법.
        //         // 1.finally블럭 사용하기.
        //         // try
        //         // {
        //         //     Monitor.Enter(obj);
        //         //     number++;
        //         //     return;
        //         // }
        //         // finally
        //         // {
        //         //     Monitor.Exit(obj);
        //         // }

        //         // 2.lock 사용하기(일반적)
        //         lock (obj)
        //         {
        //             number++;
        //         }
        //     }
        // }
        // static void Thread2()
        // {
        //     for (int i = 0; i < 100000; i++)
        //     {
        //         // Monitor.Enter(obj);

        //         // number--;
                
        //         // Monitor.Exit(obj);

        //         lock(obj)
        //         {
        //             number--;
        //         }
        //     }
        // }
        // static void Main(string[] args)
        // {
        //     Task task1 = new Task(Thread1);
        //     Task task2 = new Task(Thread2);

        //     task1.Start();
        //     task2.Start();

        //     Task.WaitAll(task1, task2);
        //     Console.WriteLine($"number:{number}");
        // }
#endregion

    // }
    
#region 9.데드락

    // // 2.id를 부여해 추적하는 방법.
    // class FastLock
    // {
    //     int id; //어떤 클래스에서 락을 사용하는지.
    //     // 다른 FastLock을 가진 클래스를 사용할 때 그 FaskLock의 id값이 자신보다 높으면 크래시를 내는 방식.
    // }

    // class SessionManager
    // {
    //     FastLock fastLock;
    //     static object lockObj = new object();
    //     public static void TestSession()
    //     {
    //         lock(lockObj)
    //         {

    //         }
    //     }
    //     public static void Test()
    //     {
    //         lock(lockObj)
    //         {
    //             UserManager.TestUser();
    //         }
    //     }
    // }
    // class UserManager
    // {
    //     FastLock fastLock;
    //     static object lockObj = new object();
    //     public static void TestUser()
    //     {
    //         lock(lockObj)
    //         {

    //         }
    //     }
    //     public static void Test()
    //     {
    //         lock(lockObj)
    //         {
    //             SessionManager.TestSession();
    //         }
    //     }
    // }

    // class Program
    // {
    //     static int number = 0;
    //     static void Thread1()
    //     {
    //         for (int i = 0; i < 100; i++)
    //         {
    //             SessionManager.Test();
    //         }
    //     }
    //     static void Thread2()
    //     {
    //         for (int i = 0; i < 100; i++)
    //         {
    //             UserManager.Test();
    //         }
    //     }
    //     static void Main(string[] args)
    //     {
    //         Task task1 = new Task(Thread1);
    //         Task task2 = new Task(Thread2);

    //         task1.Start();
    //         Thread.Sleep(100); // 1.약간의 텀을 줘서 교착상태 방지.
    //         task2.Start();

    //         Task.WaitAll(task1, task2);
    //         Console.WriteLine($"number:{number}");
    //     }
    // }
#endregion

#region 10.Lock 구현 이론.
    // 1.계속 체크하면서 기다리기(스핀락). cpu 점유율이 급격하게 올라갈 수 있다.
    // 2.일정 시간 후에 다시 체크. 컨텍스트 스위칭에 따른 부담이 있고, 다른 일을 하는 일정 시간동안에 다른 스레드가 점유할 수 있다.
    // 3.운영체제로부터 사용 가능하다는 것을 이벤트로 통보받는다.
#endregion

#region 11.SpinLock

    // // 동시에 두 스레드가 Acquire를 실행?
    // class SpinLock
    // {
    //     // volatile bool locked = false;
    //     volatile int locked = 0;

    //     public void Acquire()
    //     {
    //         // while(locked)
    //         // {
    //         //     // 잠금이 풀리기를 기다린다.
    //         // }
    //         // locked = true;
    //         // 체크하고 잠그는 행위는 원자적으로 일어나야 한다.

    //         while(true)
    //         {
    //             // 1.Interlocked.Exchange 사용. 값을 넣어주고 원래 값을 반환한다.
    //             // int original = Interlocked.Exchange(ref locked, 1);
    //             // if (original == 0) //원래 열려있었음.
    //             //     break;
                
    //             // 2.Interlocked.CompareExchange 사용(좀 더 범용적). 비교해서 예상한 값이 맞으면 원하는 값을 넣는다. 원래 값 반환. 
    //             // CAS. Compare And Swap 함수.
    //             int expected = 0; //예상한 값.
    //             int desired = 1; //원하는 값.
    //             if (Interlocked.CompareExchange(ref locked, desired, expected) == expected) // if(locked == 0) locked = 1; 
    //                 break;
    //         }

    //     }
    //     public void Release()
    //     {
    //         // locked = false;
    //         locked = 0; //경합의 여지가 없기 때문에 별도 처리가 필요 없다.
    //     }
    // }

    // class Program
    // {
    //     static int num = 0;
    //     static SpinLock spinLock = new SpinLock();
    //     static void Thread1()
    //     {
    //         for (int i = 0; i < 1000000; i++)
    //         {
    //             spinLock.Acquire();
    //             num++;
    //             spinLock.Release();
    //         }
    //     }
    //     static void Thread2()
    //     {
    //         for (int i = 0; i < 1000000; i++)
    //         {
    //             spinLock.Acquire();
    //             num--;
    //             spinLock.Release();
    //         }
    //     }

    //     static void Main(string[] args)
    //     {
    //         Task task1 = new Task(Thread1);
    //         Task task2 = new Task(Thread2);

    //         task1.Start();
    //         task2.Start();

    //         Task.WaitAll(task1, task2);
    //         System.Console.WriteLine(num);
    //     }
    // }
#endregion

#region 12.Random Matter?

    // // SpinLock에서 잠겨있을 때 하는 행위가 약간의 시간동안 쉬다 다시 시도.
    // class SpinLock
    // {
    //     volatile int locked = 0;

    //     public void Acquire()
    //     {
    //         while(true)
    //         {
    //             // 1.Interlocked.Exchange 사용. 값을 넣어주고 원래 값을 반환한다.
    //             // int original = Interlocked.Exchange(ref locked, 1);
    //             // if (original == 0) //원래 열려있었음.
    //             //     break;
                
    //             // 2.Interlocked.CompareExchange 사용(좀 더 범용적). 비교해서 예상한 값이 맞으면 원하는 값을 넣는다. 원래 값 반환. 
    //             // CAS. Compare And Swap 함수.
    //             int expected = 0; //예상한 값.
    //             int desired = 1; //원하는 값.
    //             if (Interlocked.CompareExchange(ref locked, desired, expected) == expected) // if(locked == 0) locked = 1; 
    //                 break;

    //             //쉰다.
    //             Thread.Sleep(1); // 무조건 휴식. 1ms정도. 얼마나 쉴지는 모름. 운영체제가 정해줌.
    //             Thread.Sleep(0); // 조건부 양보. 나보다 우선순위가 낮은 애들한테는 양보 불가. 우선순위가 같거나 높은 스레드가 없으면 다시 본인한테.
    //             Thread.Yield(); // 관대한 양보. 지금 실행이 가능한 스레드가 있으면 실행. 실행 가능한게 없으면 남은 시간 소진.
    //         }

    //     }
    //     public void Release()
    //     {
    //         // locked = false;
    //         locked = 0; //경합의 여지가 없기 때문에 별도 처리가 필요 없다.
    //     }
    // }

    // class Program
    // {
    //     static int num = 0;
    //     static SpinLock spinLock = new SpinLock();
    //     static void Thread1()
    //     {
    //         for (int i = 0; i < 1000000; i++)
    //         {
    //             spinLock.Acquire();
    //             num++;
    //             spinLock.Release();
    //         }
    //     }
    //     static void Thread2()
    //     {
    //         for (int i = 0; i < 1000000; i++)
    //         {
    //             spinLock.Acquire();
    //             num--;
    //             spinLock.Release();
    //         }
    //     }

    //     static void Main(string[] args)
    //     {
    //         Task task1 = new Task(Thread1);
    //         Task task2 = new Task(Thread2);

    //         task1.Start();
    //         task2.Start();

    //         Task.WaitAll(task1, task2);
    //         System.Console.WriteLine(num);
    //     }
    // }
#endregion

#region 13.컨텍스트 스위칭. 
    // 한 스레드에서 다른 스레드로 작업을 전환하는 것. 너무 잦으면 오버헤드 발생.
    // 코어 안의 캐시안의 레지스터는 용도가 다양하다. 코드가 어디까지 실행되었는지 추적하는 레지스터도 있음.
    // 코어가 스레드를 다시 실행할 때에는 컨텍스트를 램에서 레지스터로 복원해야 한다.
#endregion

#region 14.Event. Auto Reset Event.

    // // 직원에게 부탁해 통보받기.
    // // 커널 레벨에서. 컨텍스트 스위칭이 일어난다.
    // // -> 느림.

    // // Auto Reset Event : 문이 자동으로 잠긴다.
    // // Manual Reset Event : 문이 수동으로 잠긴다.

    // class Lock
    // {
    //     // 커널에서 관리하는 bool값?
    //     // AutoResetEvent available = new AutoResetEvent(true); // true:열려있는 상태.
    //     // public void Acquire()
    //     // {
    //     //     available.WaitOne(); // 입장 시도.
    //     //     // available.Reset(); // 닫기. 안해도 알아서 된다.
    //     // }
    //     // public void Release()
    //     // {
    //     //     available.Set(); //열기. 값을 true로.
    //     // }
        
    //     ManualResetEvent available = new ManualResetEvent(false);
    //     public void Acquire()
    //     {
    //         available.WaitOne(); // 입장 시도.
    //         available.Reset(); // 닫기.
    //         // => 입장과 닫기가 두 줄로 나눠져있다. -> 원자적이지 않음.
    //         // ManualResetEvent는 딱 한 스레드만 할 필요가 없을 때?
    //         // 어떤 작업이 끝나면 모든 스레드가 한 번에 돌아가는 코드일 때?
    //         // 일반적인 락 처리와는 다른 케이스
    //     }
    //     public void Release()
    //     {
    //         available.Set(); //열기. 값을 true로.
    //     }
    // }

    // class Program
    // {
    //     static int num = 0;
    //     static Lock lockObject = new Lock();
    //     static void Thread1()
    //     {
    //         for (int i = 0; i < 1000000; i++)
    //         {
    //             lockObject.Acquire();
    //             num++;
    //             lockObject.Release();
    //         }
    //     }
    //     static void Thread2()
    //     {
    //         for (int i = 0; i < 1000000; i++)
    //         {
    //             lockObject.Acquire();
    //             num--;
    //             lockObject.Release();
    //         }
    //     }

    //     static void Main(string[] args)
    //     {
    //         Task task1 = new Task(Thread1);
    //         Task task2 = new Task(Thread2);

    //         task1.Start();
    //         task2.Start();

    //         Task.WaitAll(task1, task2);
    //         System.Console.WriteLine(num);
    //     }
    // }
#endregion

#region 15.Mutex.

    // Mutex역시 커널 동기화 객체. -> 느림.
    // Event가 bool값 하나로 이루어졌다면 Mutex는 좀 더 많은 정보를 갖고있다.
    // 몇 번 잠궜는지 카운팅, 여러번 잠그고 여러번 풀기?
    // 스레드id가 있어서 release시에 다른 애가 풀려고 할 때 처리 등.

    // 거의 쓸일 없음.

    // class Program
    // {
    //     static int num = 0;
    //     static Mutex lockObject = new Mutex();

    //     static void Thread1()
    //     {
    //         for (int i = 0; i < 1000000; i++)
    //         {
    //             lockObject.WaitOne(); // 입장 시도.
    //             num++;
    //             lockObject.ReleaseMutex(); //열기.
    //         }
    //     }
    //     static void Thread2()
    //     {
    //         for (int i = 0; i < 1000000; i++)
    //         {
    //             lockObject.WaitOne();
    //             num--;
    //             lockObject.ReleaseMutex();
    //         }
    //     }

    //     static void Main(string[] args)
    //     {
    //         Task task1 = new Task(Thread1);
    //         Task task2 = new Task(Thread2);

    //         task1.Start();
    //         task2.Start();

    //         Task.WaitAll(task1, task2);
    //         System.Console.WriteLine(num);
    //     }
    // }
#endregion

#region 16.정리. ReaderWriterLock.
    // 1.근성.
    // 2.양보.
    // 3.갑질.
    // class Program
    // {
    //     // 상호 배제.
    //     static object _lock = new object();
    //     static SpinLock _lock2 = new SpinLock(); // 계속 시도하다가 양보도 한다.
    //     static Mutex _lock3 = new Mutex(); // 별도의 프로그램끼리 동기화를 할 때 사용하면 좋음?
    //     // 직접 만들기.

    //     static void Main(string[] args)
    //     {
    //         lock(_lock)
    //         {

    //         }
    //         bool lockTaken = false;
    //         try
    //         {
    //             _lock2.Enter(ref lockTaken);
    //         }
    //         finally
    //         {
    //             if (lockTaken)
    //                 _lock2.Exit();
    //         }
    //     }

    //     // 컨텐츠를 멀티스레드로. 모든 코드에서 멀티스레드. 난이도가 확 올라감. 심리스 mmorpg를 만들 때 이점.
    //     // 일반적인 mmorpg는 코어만 멀티스레드. 컨텐츠는 싱글스레드.

    //     // 퀘스트 보상을 세개를 받는다 가정할 때. 운영자가 퀘스트 보상을 추가할 수 있다면?
    //     class Reward
    //     {

    //     }
    //     // static Reward GetRewardById(int id)
    //     // {
    //     //     lock(_lock) // 자주 안일어나는데 락을 사용하는게 아쉬움..
    //     //     {
                
    //     //     }
    //     //     return null;
    //     // }
    //     //get할 때는 동시다발적으로 쓰다가 쓸 때(퀘스트 보상을 바꿀 때) 상호 배타적으로 막으면 효율적일듯.
    //     //일반적일 땐 lock이 없는것처럼 사용.
    //     //=> ReaderWriterLock(RWLock).

    //     static ReaderWriterLockSlim _lock4 = new ReaderWriterLockSlim();
    //     static Reward GetRewardById(int id)
    //     {
    //         // 여러개의 스레드가 들어왔는데 lock을 잡고있지 않다면 없는것처럼 사용.
    //         _lock4.EnterReadLock();

    //         _lock4.ExitReadLock();
    //         return null;
    //     }
    //     // 일주일에 한 번 호출될까말까.
    //     static void AddReward(Reward reward)
    //     {
    //         _lock4.EnterWriteLock(); //이게 잡히면 read안됨.

    //         _lock4.ExitReadLock();
    //     }
    // }

#endregion

#region 17.ReaderWriterLock 구현.

    // // 재귀적 락을 허용할지? 이걸 허용할 시 누가 Write를 잡고있는지 알아야 함. 허용 시 WriteLock->WriteLock OK, WriteLock->ReadLock OK, ReadLock->WriteLock NO.
    // // 스핀락 정책 : 5000번 시도 후 yield
    // class Lock
    // {
    //     const int EMPTY_FLAG = 0x00000000;
    //     const int WRITE_MASK = 0x7FFF0000;
    //     const int READ_MASK = 0x0000FFFF;
    //     const int MAX_SPIN_COUNT = 5000;

    //     int _flag = EMPTY_FLAG; // 32비트. [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
    //     //WriteThreadId: Write락은 한번에 한 스레드만. 그게 누구인지.
    //     //ReadCount: 여러 스레드가 리드를 잡을 수 있다. 그걸 카운팅.

    //     int _writeCount = 0; // 이건 한 스레드만 사용하기 때문에 별도의 멀티스레드 처리가 필요없음.

    //     public void WriteLock()
    //     {
    //         //=========================================================
    //         //WriteLock->WriteLock
    //         // 동일 스레드가 WriteLock을 이미 획득하고 있는 지 확인.
    //         int lockThreadId = (_flag & WRITE_MASK) >> 16;
    //         if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
    //         {
    //             _writeCount++;
    //             return;
    //         }
    //         //=========================================================

    //         // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을 얻는다.
    //         int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK; // 해당하는 자리에 아이디 셋.
    //         while(true)
    //         {
    //             for (int i = 0; i < MAX_SPIN_COUNT; i++)
    //             {
    //                 //시도해서 성공하면 return.
    //                 // if (_flag == EMPTY_FLAG)
    //                 //     _flag = desired;
    //                 if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
    //                 {
    //                     _writeCount = 1;
    //                     return;
    //                 }
    //             }
    //             Thread.Yield();
    //         }
    //     }
    //     public void WriteUnlock()
    //     {
    //         // Interlocked.Exchange(ref _flag, EMPTY_FLAG);

    //         int lockCount = --_writeCount;
    //         if (lockCount == 0)
    //             Interlocked.Exchange(ref _flag, EMPTY_FLAG);
    //     }

    //     public void ReadLock()
    //     {
    //         //=========================================================
    //         //WriteLock->ReadLock
    //         // 동일 스레드가 WriteLock을 이미 획득하고 있는 지 확인.
    //         int lockThreadId = (_flag & WRITE_MASK) >> 16;
    //         if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
    //         {
    //             Interlocked.Increment(ref _flag);
    //             return;
    //         }
    //         //=========================================================

    //         // 아무도 WriteLock을 획득하고 있지 않으면 ReadCount를 1 증가시킨다.
    //         while(true)
    //         {
    //             for (int i = 0; i < MAX_SPIN_COUNT; i++)
    //             {
    //                 // if ((_flag & WRITE_MASK) == 0)
    //                 // {
    //                 //     _flag = _flag + 1;
    //                 //     return;
    //                 // }
    //                 int expected = (_flag & READ_MASK); // ReadCount만 뽑아온다. 예상 값은 write가 없는 값.
    //                 // 예상 값이 아닌 경우는 1.Write중일 때. 2.동시에 여러 스레드가 Read를 해서 증가된 상태일 때.
    //                 // 2번의 경우에는 한번 실패 후 다음번 시도에서 통과할 수 있다.
    //                 if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
    //                     return;
    //             }
    //             Thread.Yield();
    //         }
    //     }
    //     public void ReadUnlock()
    //     {
    //         Interlocked.Decrement(ref _flag);
    //     }
    // }
    // class Program
    // {
    //     static volatile int count = 0;
    //     static Lock _lock = new Lock();

    //     static void Main(string[] args)
    //     {
    //         Task t1 = new Task(delegate ()
    //         {
    //             for (int i = 0; i < 100000; i++)
    //             {
    //                 _lock.ReadLock();
    //                 count++;
    //                 _lock.ReadUnlock();
    //             }
    //         });
    //         Task t2 = new Task(delegate ()
    //         {
    //             for (int i = 0; i < 100000; i++)
    //             {
    //                 // _lock.WriteLock();
    //                 // count--;
    //                 // _lock.WriteUnlock();
    //                 _lock.ReadLock();
    //                 count--;
    //                 _lock.ReadUnlock();
    //             }
    //         });

    //         t1.Start();
    //         t2.Start();

    //         Task.WaitAll(t1, t2);
    //         Console.WriteLine(count);
    //     }
    // }
#endregion
}