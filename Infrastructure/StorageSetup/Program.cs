using System;

namespace StorageSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connection string: {0}", Config.ConnectionStringStorageAccount);
        }
    }
}
