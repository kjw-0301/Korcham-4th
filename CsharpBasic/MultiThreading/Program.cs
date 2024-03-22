namespace MultiThreading
{
    internal class Program
    {
        const int MB = 1024 * 1024;
        static void Main(string[] args)
        {
            Barista barist1 = HirBarista("J");
            Barista barist2 = HirBarista("K");

            
            Thread t1 = new Thread(() =>
            {
                HirBarista("J").GoToWork().MakeRandomBeverage(); //함수 체이닝.
            },1*MB);
            t1.Name = barist1.Name;
            t1.IsBackground = true; 
            t1.Start();
            t1.Join(); // Main쓰레드가 t1 쓰레드를 호출.

            Thread.Sleep(2000); //호출한 쓰레드를 2000ms동안 유지? 한 뒤 쓰레드를 닫음.
            ThreadPool.SetMinThreads(1,0);
            ThreadPool.SetMaxThreads(4,4); //WorkerTheard는 cpubound task, completion은 IO bounds task.

            Task task1 = new Task(() =>
            {
                HirBarista("J").GoToWork().MakeRandomBeverage(); //함수 체이닝.

            }); //관리되는 쓰레드풀에서 알아서 관리해줌(누가? .Net의 가상머신이)
            task1.Start();
            task1.Wait(); //task1이 빌려쓰는 쓰레드를 기다리도록 합니다.(여러개의 쓰레드를 한번에 기다리게 할 수 있다.)

            //Task 10개를 배열로 만들어 관리.
            //★★멀티 쓰레드 환경에서는 Task를 할당하고 실행한 순서대로 출력이 진행된다는 보장이 없다.
            Task[] tasks = new Task[10];
            for(int i = 0; i <tasks.Length; i++)
            {
                int index = i;
                tasks[i] = new Task(() =>
                {
                    HirBarista($"바리스타{index}").GoToWork().MakeRandomBeverage(); //함수 체이닝
                });
                tasks[i].Start();
            }
            Task.WaitAll(tasks); //10개의 모든 tasks들이 대기!
        }


        static Barista HirBarista(string nickname)
        {
            return new Barista(nickname);   
        }
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
        private Random random;
        

        public Barista GoToWork()
        {
            Console.WriteLine($"바리스타 {Name}은 출근합니다..");
            return this;
        }
        
        public Beverage MakeRandomBeverage()
        {
            Beverage beverage = (Beverage)random.Next(1, Enum.GetValues(typeof(Beverage)).Length);
            Console.WriteLine($"바리스타{Name} 은 음료 +{beverage}를 제조했습니다.");
            return beverage;
        }
    }
}
