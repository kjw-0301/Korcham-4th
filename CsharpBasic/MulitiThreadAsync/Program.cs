namespace MulitiThreadAsync
{
    internal class Program
    {
        public readonly static object Lock = new object(); //감시를 하기위한 자물쇠?
        public static int Ticks;
        //어떤 Task를 취소시키기 위해 신호를 보내는 객체.
        //Task 할당시 이 source를 통해 Token을 발행해서 줄 수 있고
        //이 Source의 Cancel() 요청이 발생했을 때, Token을 발행받은 모든 Task를 취소시킬 수 있다.
        static CancellationTokenSource cts;
        static void Main(string[] args)
        {
            cts = new CancellationTokenSource();
            Task t1 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("~~");

            }, cts.Token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            cts.Cancel();
            if (t1.IsCanceled)
            {
                //todo -> Do exception handling
            }

            // _ 는 반환 내용에 대해 무시하겠다는 명시.
            //_ = HirBarista("알바").GoToWork().MakeRandomBeverage(); //>무시항목설정

            //C# 버전등의 이슈로 async를 사용할 수 없는 함수에서는 직접 Task 참조를 통해 Wait등을 수행해야한다.
            //Task<Beverage> task = HirBarista("알바").GoToWork().MakeRandomBeverage();
            //task.Wait();

            
            //실행 함수들을 쓰레드풀에서 실행하도록 할당하여 대기열에 등록. >> Token을 발행하여 Task의 실행관리를 할 수 있따.
            Task.Run(() => HirBarista("알바").GoToWork().MakeRandomBeverage()); 
            Console.WriteLine("왔으면 일해라!");
        }
        static Barista HirBarista(string nickname)
        {
            return new Barista(nickname);
        }
        public enum Beverage
        {
            None,
            Coffee,
            Latte,
            Lemonade,

        }
        public class Barista
        {
            public Barista(string name)
            {
                Name = name;
                random = new Random();
            }

            public string Name { get; set; }

            private static Dictionary<Beverage, int> s_delayTimes = new Dictionary<Beverage, int>
            {
                { Beverage.Coffee, 5000},
                { Beverage.Lemonade, 6000},
                { Beverage.Latte, 7000},

            };


            private Random random;


            public Barista GoToWork()
            {
                Console.WriteLine($"바리스타 {Name}은 출근합니다..");
                return this;
            }

            public async Task<Beverage> MakeRandomBeverage()
            {
                Beverage beverage = (Beverage)random.Next(1, Enum.GetValues(typeof(Beverage)).Length);
                Console.WriteLine($"바리스타{Name} 은 음료 {beverage}를 제조를 시작했습니다.");


                //Task delayTask = Delay(s_delayTimes[beverage]);
                //delayTask.Start();
                //delayTask.Wait();
                //await Delay(s_delayTimes[beverage]); //위 3줄의 코드를 한 줄로 줄이면 다음과 같이 됨.

                await Task.Delay(s_delayTimes[beverage]);

                //lock 키워드 : 현재 어플리케이션 내에서 둘 이상의 쓰레드 접근을 막기위한 키워드
                lock (Program.Lock)
                {
                    for(int i = 0; i < 10000; i++)
                    {
                        Program.Ticks++;
                    }
                    
                }

                Monitor.Enter(Program.Lock); //동기화를 위한 감시시작
                
                //Critical Section(임계영역) : 둘 이상의 쓰레드가 접근하면 안되는 공유 자원에 접근하는 영역
                //Critical 영역 시작
                for (int i = 0; i < 100000; i++) 
                {
                    //Program.Ticks++; 
                    Interlocked.Increment(ref Program.Ticks); //위의 public readonly static object Lock = new object(); 없이 lock을 사용가능
                }
                //critical 영역 끝
                Monitor.Exit(Program.Lock); //동기화를 위한 감시 끝

                //Semaphore
                Semaphore pool = new Semaphore(0, 3);
                pool.WaitOne(); // 한 자리가 날 때 까지 기다림
                //todo => Critical section 입력!
                pool.Release();//점유하고있는 자리 비움.

                //Mutex
                Mutex mutex = new Mutex();
                mutex.WaitOne();
                //todo => Critical section 입력!
                mutex.ReleaseMutex();

                Console.WriteLine($"바리스타{Name} 은 음료 {beverage}를 제조를 완료했습니다.");
                return beverage;
            }

            private Task Delay(int ms)
            {
                return new Task(() =>
                {
                    Thread.Sleep(ms);
                });
            }
        }
    }
}
