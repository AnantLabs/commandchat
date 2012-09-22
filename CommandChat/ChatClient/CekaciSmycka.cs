using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utility
{
    public delegate bool CekaciPodminka();

    public class CekaciSmycka
    {
        private int PocetCyklu;
        private int DobaCyklu;
        private int AktualniCyklus;

        CekaciPodminka Podminka;

        public CekaciSmycka(int pocetCyklu, int dobaCyklu, CekaciPodminka podminka)
        {
            PocetCyklu = pocetCyklu;
            DobaCyklu = dobaCyklu;
            Podminka = podminka;
        }

        public bool Proved()
        {
            while (!Podminka())
            {
                Thread.Sleep(DobaCyklu);
                Console.Write(".");
                AktualniCyklus++;

                if (AktualniCyklus > PocetCyklu)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Chyba!\n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    return false;
                }
            }

            DodelejTecky();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("OK!\n");
            Console.ForegroundColor = ConsoleColor.Gray;

            return true;
        }

        private void DodelejTecky()
        {
            while (AktualniCyklus != PocetCyklu)
            {
                Console.Write(".");
                AktualniCyklus++;
            }
        }
    }
}
