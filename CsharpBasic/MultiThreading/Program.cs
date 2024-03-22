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
                HirBarista("J").GoToWork();
            },1*MB);
            t1.Start();
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
        }
        
        public string Name { get; set; }
        private Random random;
        

        public void GoToWork()
        {
            Console.WriteLine($"바리스타 {Name}은 출근합니다..");
        }
        
        public Beverage MakeRandomBeverage()
        {
            return (Beverage)random.Next(1, Enum.GetValues(typeof(Beverage)).Length);
        }
    }
}
