using System;
using System.Net;

namespace ConsoleApp1 {
    public class ListenerDemo {
        public static void Listener1(string[] prefixes) {
            if (!HttpListener.IsSupported) {
                Console.WriteLine("Seu ambiente não suporta os recursos da classe HttpListener.");
                return;
            }
            // Os prefixos URI são obrigatórios
            // por exemplo "http://macoratti.net:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");
            // Cria um listener.
            HttpListener listener = new HttpListener();
            // Adiciona os prefixos
            foreach (string s in prefixes) {
                listener.Prefixes.Add(s);
                
            }
            //inicia o listener
            listener.Start();
            Console.WriteLine("Escutando...");
            // O método GetContext bloqueia enquanto aguarda pela requisição
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            // Obtém o objeto response
            HttpListenerResponse response = context.Response;
            // Constrói uma resposta
            string responseString = "<HTML><BODY> Macoratti .Net - Quase tudo para .NET!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Obtem uma stream e escreve a resposta nela
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // fecha o stream
            output.Close();
            //para o listner
            listener.Stop();
        }
    }
}