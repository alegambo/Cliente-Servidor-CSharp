using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Servidor
{
    class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clienteSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 100;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        private static readonly List<String> listajugadores = new List<string>() { "jugador1", "jugador2", "jugador3"};

        static void Main()
        {
            Console.Title = "Servidor";
            IniciarSocket();
            Console.ReadLine(); // When we press enter close everything
            CerrarSockets();
        }

        private static void IniciarSocket()
        {
            Console.WriteLine("Iniciando servidor...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AceptarSocket, null);
            Console.WriteLine("Servidor corriendo");
        }

       
        private static void CerrarSockets()
        {
            foreach (Socket socket in clienteSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            serverSocket.Close();
        }

        private static void AceptarSocket(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) 
            {
                return;
            }
          
               
                    clienteSockets.Add(socket);
                    socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, RecibirLlamado, socket);
                    Console.WriteLine("Detectamos una nueva conexion,mensaje del jugador...");
                    serverSocket.BeginAccept(AceptarSocket, null);
                
                }
        
        

        private static void RecibirLlamado(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Jugador forzadamente desconectado");
               
                current.Close(); 
                clienteSockets.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Recibiendo mensaje: " + text);

           if (text.ToLower() == "salir")
            {
                
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clienteSockets.Remove(current);
                Console.WriteLine("Jugador Desconectado");
                return;
            }
          

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, RecibirLlamado, current);
        }
    }
}
