using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Clientes
{
    class Program
    {
        private static readonly Socket SocketCliente = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 100;

        static void Main()
        {
            Console.Title = "Jugador";
            ConectarAServer();
            MantenerseEnSocket();
            Salir();
        }

        private static void ConectarAServer()
        {
            int intentos = 0;

            while (!SocketCliente.Connected) 
            {
                try
                {
                    intentos++;
                    Console.WriteLine("Intentando conexion" + intentos);
                    SocketCliente.Connect(IPAddress.Loopback, PORT);
                }
                catch (SocketException) 
                {
                    Console.Clear();
                }
            }

            Console.Clear();
            Console.WriteLine("Conectado");
        }

        private static void MantenerseEnSocket()
        {
            Console.WriteLine(@"envie su nombre de usuario");
            Console.WriteLine(@"Digite Salir para desconectarse");

            while (true)
            {
                EnviaRespuesta();
                RecibirRespuesta();
            }
        }

       
        private static void Salir()
        {
            EnviarMensaje("salir");
            SocketCliente.Shutdown(SocketShutdown.Both);
            SocketCliente.Close();
            Environment.Exit(0);
        }

        private static void EnviaRespuesta()
        {
            Console.Write("Envia el mensaje al servidor: ");
            string request = Console.ReadLine();
            EnviarMensaje(request);

            if (request.ToLower() == "salir")
            {
                Salir();
            }
        }

        
        private static void EnviarMensaje(string mensaje)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(mensaje);
            SocketCliente.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private static void RecibirRespuesta()
        {
            var buffer = new byte[2048];
            int received = SocketCliente.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            Console.WriteLine(text);
        }
    }
}
