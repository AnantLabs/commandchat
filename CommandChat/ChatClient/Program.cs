using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;
using Utility;

namespace Client
{
    public static class Program
    {
        public static NetClient Spojeni;
        public static string Prezdivka;

        public static string Line = string.Empty;
        public static string Prefix = ">";

        public static void PopisProgramu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("ChatClient v1.0");
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

        public static bool ZkontrolujSpojeni()
        {
            if (Spojeni.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                return false;
            }

            return true;
        }

        public static void NavazatSpojeni()
        {
            Console.Write("Zadejte název serveru: ");
            NetPeerConfiguration Konfigurace = new NetPeerConfiguration(Console.ReadLine());

            Console.Write("Zadejte adresu serveru: ");
            string ip = Console.ReadLine();

            Console.Write("Zadejte port serveru: ");
            try
            {
                int port = Int32.Parse(Console.ReadLine());

                Spojeni = new NetClient(Konfigurace);
                Spojeni.Start();
                Spojeni.Connect(ip, port);
            }
            catch
            {
                Console.WriteLine("Nastavení připojení selhalo.");
                UkoncitProgram();
            }

            Console.Write("Probíhá připojování");
            if (!new CekaciSmycka(10, 1000, new CekaciPodminka(ZkontrolujSpojeni)).Proved())
            {
                Console.WriteLine("Nepodařilo se připojit k serveru, program bude ukončen.");
                UkoncitProgram();
            }
        }

        public static void NastavPrezdivku()
        {
            Console.Write("Zvolte si svoji přezdívku: ");
            Prezdivka = Console.ReadLine();
            Prezdivka.Trim();

            if (Prezdivka.Length < 3)
            {
                Console.WriteLine("Minimální délka přezdívky jsou 3 znaky.");
                NastavPrezdivku();
            }
        }

        static void Main(string[] args)
        {
            PopisProgramu();
            NavazatSpojeni();
            NastavPrezdivku();
            Chat();
        }

        public static void ClearLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(" ");
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void Chat()
        {
            while (true)
            {
                ClearLine();
                Console.Write(Prezdivka + Prefix + Line);

                ConsoleKeyInfo key = new ConsoleKeyInfo();

                while (Console.KeyAvailable == false)
                {
                    Listen();
                }
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    PosliZpravu(Line);
                    Line = string.Empty;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (Line.Length > 1) { Line = Line.Substring(0, Line.Length - 1); }
                }
                else
                {
                    Line += key.KeyChar;
                }
            }
        }

        public static void Listen()
        {
            NetIncomingMessage msg;

            while ((msg = Spojeni.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        string prezdivka = msg.ReadString();
                        string zprava = msg.ReadString();
                        VypisZpravu(prezdivka, zprava);
                        break;
                }
                Spojeni.Recycle(msg);
            }
        }

        public static void VypisZpravu(string prezdivka, string text)
        {
            ClearLine();
            if (Prezdivka == prezdivka) { Console.ForegroundColor = ConsoleColor.Blue; }
            else { Console.ForegroundColor = ConsoleColor.Red; }
            Console.WriteLine(prezdivka + Prefix + text);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(Prezdivka + Prefix + Line);
        }

        public static void PosliZpravu(string text)
        {
            NetOutgoingMessage zprava = Spojeni.CreateMessage();
            zprava.Write(Prezdivka);
            zprava.Write(text);
            Spojeni.SendMessage(zprava, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
