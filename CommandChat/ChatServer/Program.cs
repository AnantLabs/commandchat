using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;

namespace Server
{
    public static class Program
    {
        public static NetServer Server;

        public static void PopisProgramu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("ChatServer v1.0");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" David Janoušek 2012\n");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void UkoncitProgram()
        {
            Console.WriteLine("Pokračujte stisknutím libovolné klávesy.");
            Console.ReadKey();
            Environment.Exit(1);
        }

        public static void VytvoritServer()
        {
            Console.Write("Zvolte název Serveru: ");
            NetPeerConfiguration Konfigurace = new NetPeerConfiguration(Console.ReadLine());

            Console.Write("Zvolte port Serveru: ");
            try
            {
                Konfigurace.Port = Int32.Parse(Console.ReadLine());
                Server = new NetServer(Konfigurace);
                Server.Start();
            }
            catch
            {
                Console.WriteLine("Nastavení Serveru selhalo.");
                UkoncitProgram();
            }

            Console.WriteLine("Server spuštěn.");
        }

        static void Main(string[] args)
        {
            PopisProgramu();
            VytvoritServer();

            NetIncomingMessage msg;
            while (true)
            {
                while ((msg = Server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            string prezdivka = msg.ReadString();
                            string zprava = msg.ReadString();

                            NetOutgoingMessage odpoved = Server.CreateMessage();
                            odpoved.Write(prezdivka);
                            odpoved.Write(zprava);
                            Server.SendToAll(odpoved, NetDeliveryMethod.ReliableOrdered);
                            break;
                    }

                    Server.Recycle(msg);
                }
            }
        }
    }
}
