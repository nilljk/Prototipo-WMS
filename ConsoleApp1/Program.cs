using System;
using System.Net;

namespace ConsoleApp1 {
    class Program {
        static void Main(string[] args) {

            //   HttpListener listener = new HttpListener();

            //string[] url = { "http://192.168.0.245:5001/tela/", "http://192.168.0.245:5001/login/" };
            string[] url = { "http://192.168.1.114:5001/tela/", "http://192.168.1.114:5001/login/" };

            //ListenerDemo.Listener1(url);
            Piloto.Listener1(url);

            Console.WriteLine("Hello World!");
        }
    }
}
