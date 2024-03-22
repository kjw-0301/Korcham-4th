namespace Practice
{
    class CharacterController : Object
    {

    }
    struct ItemData
    {
        public int ID;
        public int Num;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            object obj = new CharacterController();
            obj = new ItemData { ID = 5, Num = 4 }; //Boxing
            ItemData data = (ItemData)obj;
            long a = 10;
            int n = (int)a; //명시적 형 변환
            a = n;  //승격 
            Console.WriteLine("Hello, World!");
        }
    }
}
