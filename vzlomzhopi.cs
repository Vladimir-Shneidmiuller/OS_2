using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OS_Practice_2
{

    class OS_Thread
    {
        readonly Thread thread;
        public static readonly List<string> OS_PASSWORDS = new List<string>();
        readonly int OS_ThreadNumber;
        readonly int OS_ThreadCount;
        static int OS_ThreadRemaining;
        static bool OS_Cancel;
        static DateTime startBruteforce = DateTime.Parse("01.01.2001 00:00");
        static DateTime endBruteforce;

        public OS_Thread(string hash, int threadNum, int threads)
        {
            thread = new Thread(this.OS_Bruteforce);
            OS_ThreadNumber = threadNum;
            OS_ThreadCount = OS_ThreadRemaining = threads;
            OS_Cancel = false;
            thread.Start(hash);
        }

        public void OS_Bruteforce(object num)
        {
            if (startBruteforce == DateTime.Parse("01.01.2001 00:00"))
                startBruteforce = DateTime.Now;
            string OS_Current_Hash = num.ToString();
            int OS_Start = 11881376 / OS_ThreadCount * OS_ThreadNumber;
            int OS_Limit = 11881376 / OS_ThreadCount * (OS_ThreadNumber + 1);
            if (OS_Limit > 11881376) OS_Limit = 11881376;
            var crypt = new System.Security.Cryptography.SHA256Managed();
            for (int OS_Counter = OS_Start; OS_Counter <= OS_Limit; OS_Counter++)
            {
                if (OS_Cancel) return;
                string password = OS_PASSWORDS[OS_Counter];
                Console.WriteLine("Поток [" + OS_ThreadNumber.ToString() + "]: пароль " + password);
                var hash = new System.Text.StringBuilder();
                byte[] crypto = crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }

                if (hash.ToString() == OS_Current_Hash)
                {
                    endBruteforce = DateTime.Now;
                    Console.WriteLine("\n\n Пароль найден > " + password);
                    Console.WriteLine(" Времени прошло > " + endBruteforce.Subtract(startBruteforce).ToString());
                    Console.WriteLine(" Нажмите любую клавишу для выхода в меню");
                    OS_Cancel = true;
                    startBruteforce = DateTime.Parse("01.01.2001 00:00");
                    return;
                }
            }
            if (--OS_ThreadRemaining <= 0)
            {
                endBruteforce = DateTime.Now;
                Console.WriteLine("\n\n Пароль не найден :( ");
                Console.WriteLine(" Времени прошло > " + endBruteforce.Subtract(startBruteforce).ToString());
                Console.WriteLine(" Нажмите любую клавишу для выхода в меню");
                OS_Cancel = true;
                startBruteforce = DateTime.Parse("01.01.2001 00:00");
                return;
            }
        }

    }

    class Program
    {
        static string OS_ComputeHash(string hashString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hashString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        static void OS_ComputePasswords()
        {
            Console.WriteLine(" Подождите, ща все будет");
            for (char a = 'a'; a <= 'z'; a++)
                for (char b = 'a'; b <= 'z'; b++)
                    for (char c = 'a'; c <= 'z'; c++)
                        for (char d = 'a'; d <= 'z'; d++)
                            for (char e = 'a'; e <= 'z'; e++)
                                OS_Thread.OS_PASSWORDS.Add(a.ToString() + b.ToString() + c.ToString() + d.ToString() + e.ToString());
        }

        static string OS_HashToBruteforce()
        {
            Console.Clear();
            Console.WriteLine("Выберите хэш для брутфорса?\n");
            Console.WriteLine(" [1]: 1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad");
            Console.WriteLine(" [2]: 3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b");
            Console.WriteLine(" [3]: 74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f");
            Console.WriteLine(" [4]: Прочитать хэш из файла");
            Console.Write(" [5]: Выход\n\n > ");
            string r = Console.ReadLine();
            if (r == "1")
                return "1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad";
            if (r == "2")
                return "3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b";
            if (r == "3")
                return "74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f";
            if (r == "4")
            {
                Console.Write(" Введите путь к файлу > ");
                string OS_Path = Console.ReadLine();
                FileStream file = new FileStream(OS_Path, FileMode.OpenOrCreate);
                byte[] array = new byte[file.Length];
                file.Read(array, 0, array.Length);
                string textFromFile = System.Text.ASCIIEncoding.ASCII.GetString(array);
                Console.WriteLine(" Хэш > " + textFromFile);
                return textFromFile;
            }
            return "";
        }

        static int OS_Threads()
        {
        _OS_incorrect_input_threads:
            Console.Write("\n Сколько потоков использовать?\n > ");
            int x;
            try
            {
                x = Int32.Parse(Console.ReadLine());
                if (x < 1) throw new Exception();
            }
            catch
            {
                goto _OS_incorrect_input_threads;
            }
            return x;
        }

        static void OS_Bruteforce(string hash, int threads)
        {
            List<OS_Thread> myThread = new List<OS_Thread>();
            for (int i = 0; i < threads; i++)
                myThread.Add(new OS_Thread(hash, i, threads));
            Console.WriteLine("\n Подбираем пароль...");
        }

        static void Main()
        {
            OS_ComputePasswords();
        _OS_start:
            string OS_Hash = OS_HashToBruteforce();
            if (OS_Hash == "") return;
            int OS_ThreadsNumber = OS_Threads();
            OS_Bruteforce(OS_Hash, OS_ThreadsNumber);
            Console.ReadKey();
            goto _OS_start;
        }
    }
}
