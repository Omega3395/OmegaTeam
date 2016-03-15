using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace OmegaTeam
{
    public class wallFollower
    {

        //################################################################################
        //################################################################################

        public const int FDIST = 30;
        public const int LDIST = 20;
        public const int ERROR = 2;
        public const int OUT_ERROR = 20;
        public const sbyte CORRECTION = 5;
        private const sbyte SPEED = 20;

        //################################################################################
        //################################################################################

        private static Sensors S = new Sensors();
        private static Motors M = new Motors();

        public wallFollower()
        {
        }

        private static void avoidSilver()
        {
            LcdConsole.WriteLine("RIENTRATA");

            M.Brake();
            Thread.Sleep(100);

            M.goFor(30, false, 0.1);

            if (S.getDist(3) > FDIST)
                M.turnRight(180, 0.1);
            else
                M.turnLeft(180, 0.1);
        }

        private static void primo_Posizionamento()
        {

            LcdConsole.WriteLine("Inizio posizionamento");
            LcdConsole.WriteLine("Fase 1-1");

            while (S.getDist(2) > FDIST && S.getDist(3) > LDIST)
                M.setSpeed(SPEED, SPEED);

            M.Brake();
            Thread.Sleep(100);

            if (S.getDist(2) <= FDIST)
            {

                while (S.getDist(3) >= FDIST)
                {
                    M.V.SpinLeft(SPEED);
                }

                M.Brake();
            }

            if (S.getDist(3) <= FDIST)
            {

                LcdConsole.WriteLine("Fase 1-2");

                bool stop = false;
                int distanza = 0;

                M.V.SpinLeft(SPEED);

                while (!stop)
                {

                    distanza = S.getDist(3);
                    Thread.Sleep(50);

                    while (S.getDist(3) > distanza)

                        M.V.SpinRight(SPEED);

                    if (S.getDist(3) <= distanza)
                    {

                        int minimo = S.getDist(3);
                        Thread.Sleep(50);

                        while (S.getDist(3) <= minimo)
                        {

                            minimo = S.getDist(3);
                            Thread.Sleep(50);

                        }

                        stop = true;
                    }
                }
            }

            M.Brake();

            Thread.Sleep(500);
        }

        private static void secondo_Posizionamento()
        {

            LcdConsole.WriteLine("Fase 2");

            while (S.getDist(2) < FDIST)
                M.setSpeed(-SPEED, -SPEED);

            while (S.getDist(2) > FDIST)
            {
                M.setSpeed(SPEED, SPEED);
            }

            M.Brake();

            if (S.getDist(3) > LDIST)
            {
                M.turnRight(90, 0.1);

                while (S.getDist(2) > LDIST)
                    M.setSpeed(SPEED, SPEED);

                M.Brake();
                Thread.Sleep(100);

                M.turnLeft(90, 0.1);

            }

            LcdConsole.WriteLine("Fine posizionamento");
        }

        private static void trovaAngolo()
        {
            LcdConsole.WriteLine("Ricerca angolo");

            while (!S.obstacle())
            {
                if (S.getDist(2) >= FDIST && S.isNearTheWall())
                    M.setSpeed(SPEED, SPEED);

                if (S.getDist(2) >= FDIST && S.getDist(3) < LDIST - ERROR)
                    M.setSpeed(SPEED - CORRECTION, SPEED + CORRECTION);

                if (S.getDist(2) >= FDIST && S.getDist(3) > LDIST + ERROR)
                {

                    if (S.getDist(3) > LDIST + OUT_ERROR)
                    {
                        LcdConsole.WriteLine("USCITA");
                        avoidSilver();
                        posizionamento();
                    }
                    else
                    {
                        M.setSpeed(SPEED + CORRECTION, SPEED - CORRECTION);
                    }
                }

                if (S.getDist(2) <= FDIST)
                {
                    M.turnLeft(90);

                    while (!S.isNearTheWall())
                        M.setSpeed(SPEED + CORRECTION, SPEED - CORRECTION);
                }
            }

            LcdConsole.WriteLine("Angolo trovato");
        }

        private static void posizionamento()
        {
            primo_Posizionamento();
            secondo_Posizionamento();
        }

        public static void run()
        {

            LcdConsole.WriteLine("Inizio Wall-Follower");

            posizionamento();

            Thread.Sleep(500);

            trovaAngolo();

            LcdConsole.WriteLine("Fine Wall-Follower");
        }

    }
}
