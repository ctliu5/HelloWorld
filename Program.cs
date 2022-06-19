using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace D008.利用TAP工作建立大量並行工作練習 {
    class Program {
        static object __lockObj = new object();
        static void Main(string[] args) {
            string URL = "http://mocky.azurewebsites.net/api/delay/2000";

            List<Task> Tasks = new List<Task>();
            for (int i = 0; i < 10; i++) {
                var index = string.Format("{0:D2}", (i + 1));
                Tasks.Add(SendHttpAsync(index, URL, 1));
                Tasks.Add(SendHttpAsync(index, URL, 2));
            }

            Task.WaitAll(Tasks.ToArray());
            Console.WriteLine("按下任一按鍵，結束處理程序");
            Console.ReadKey();
        }

        private static async Task SendHttpAsync(string index, string URL, int trial) {
            var tid = String.Format("{0:D2}", Thread.CurrentThread.ManagedThreadId);
            HttpClient client = new HttpClient();
            ShowDebugInfo(index, trial, tid, ">>>>");
            var result = await client.GetStringAsync(URL);
            ShowDebugInfo(index, trial, tid, "<<<<", result);
        }

        private static void ShowDebugInfo(string index, int trial, string tid, string sep, string result = null) {
            lock (__lockObj) {
                ConsoleColor orig = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{index}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" << ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{trial}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" >> 測試 (TID: ");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{tid}");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($")");

                if (result != null) {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.Write($" {sep} ");

                if (result != null) {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }
                Console.Write($"{DateTime.Now}");

                Console.ForegroundColor = ConsoleColor.Cyan;
                if (result != null) {
                    Console.Write($" {result}");
                }
                Console.WriteLine();

                Console.ForegroundColor = orig;
            }
        }
    }
}
