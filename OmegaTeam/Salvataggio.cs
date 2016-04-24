using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.Display.Dialogs;

namespace OmegaTeam
{
    class Salvataggio
    {

        public static int Valore = 1;
        public static Vehicle V = new Vehicle(MotorPort.OutA, MotorPort.OutB);
        public static ManualResetEvent terminateprogram = new ManualResetEvent(false);
        public static ButtonEvents buts = new ButtonEvents();
        public static bool fine;
        public static MSDistanceSensor sensorA = new MSDistanceSensor(SensorPort.In1);
        public static MSDistanceSensor sensorB = new MSDistanceSensor(SensorPort.In2);
        public static EV3TouchSensor TouchSensor = new EV3TouchSensor(SensorPort.In3);
        public static Motor motorA = new Motor(MotorPort.OutB);
        public static Motor motorB = new Motor(MotorPort.OutA);
        public static Motor motorC = new Motor(MotorPort.OutC);
        public static bool founded = false;
        public static int[] Dist = new int[400];
        public static int[] Dist2 = new int[400];
        public static int[] Diff = new int[400];
        public static int[] Differenza = new int[400];
        public static int[] Tacho = new int[400];
        public static int Distanza;
        public static int Distanza2;
        public static int i = 0;

        public static void salva()
        {

            sensorA.PowerOn();
            sensorB.PowerOn();
            Thread.Sleep(500);

            RunRescue();

            //if (fine) {

            //	fine = false;
            //	RunRescue ();

            //}

        }

        public static void RunRescue()
        {

            motorA.ResetTacho();
            motorB.ResetTacho();
            PosizionaRobot();
            Caricapallina();
            Allinea();
            WallFollower();
            Conclusione();

        }

        public static void Caricapallina()
        {

            ScannerizzaStanza();
            DiffMassima();
            All();
            ApriPinza();
            Ruota180();
            Avanza();
            ChiudiPinza();

        }

        public static void Allinea()
        {

            All2();
            Sfondamento(5000);

        }

        public static void PosizionaRobot()
        {


            if (!fine)
            {

                LcdConsole.WriteLine("POSIZIONANDO ROBOT...");
                Line();
                V.TurnLeftForward(20, 9, 2300, true);							//edit1
                Thread.Sleep(7000);
                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }
        }

        public static void ScannerizzaStanza()
        {

            if (!fine)
            {
                LcdConsole.WriteLine("SCANNERIZZANDO STANZA...");
                Line();

                for (i = 0; i < 50; i++)
                {

                    if (!fine)
                    {

                        Thread.Sleep(60);
                        Dist[i] = sensorA.GetDistance();
                        Dist2[i] = sensorB.GetDistance();
                        GetDistanza();
                        V.SpinLeft(14, 14, true);											//edit2
                        Thread.Sleep(100);
                        Differenza[i] = Distanza2 - Distanza;
                        GetDifferenza();
                        Tacho[i] = motorA.GetTachoCount();
                        LcdConsole.WriteLine("    " + Diff[i] + "    " + Tacho[i]);

                        buts.EnterPressed += () =>
                        {
                            fine = true;
                        };

                    }

                }

                Line();
                Thread.Sleep(500);
                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }

        }

        public static void GetDifferenza()
        {

            switch (i)
            {

                case 0:
                    Diff[i] = Differenza[0];
                    break;

                case 1:

                    Diff[i] = (Differenza[0] + Differenza[1]) / 2;
                    break;

                default:

                    Diff[i] = (Differenza[i - 2] + Differenza[i - 1] + Differenza[i]) / 3;
                    break;

            }
        }


        public static void GetDistanza()
        {

            switch (i)
            {

                case 0:
                    Distanza = Dist[0];
                    Distanza2 = Dist2[0];
                    break;

                case 1:

                    Distanza = (Dist[0] + Dist[1]) / 2;
                    Distanza2 = (Dist2[0] + Dist2[1]) / 2;
                    break;

                default:

                    Distanza = (Dist[i - 2] + Dist[i - 1] + Dist[i]) / 3;
                    Distanza2 = (Dist2[i - 2] + Dist2[i - 1] + Dist2[i]) / 3;
                    break;

            }

        }

        public static void Avanza()
        {

            if (!fine)
            {

                LcdConsole.WriteLine("AVANZANDO...");
                Line();
                int time = (Distanza * 17) / 8;
                motorA.SetSpeed(-30);
                motorB.SetSpeed(-30);
                Thread.Sleep(time);
                V.Brake();
                Thread.Sleep(500);

                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }
        }

        public static void Conclusione()
        {

            V.Off();
            motorC.Off();
            ChiudiPinza();
            sensorA.PowerOff();
            sensorB.PowerOff();

        }

        public static void Line()
        {
            LcdConsole.WriteLine("");
        }

        public static void DiffMassima()
        {
            int max = 0;
            for (int f = 0; f < 50; f++)
            {
                if (Diff[f] > max)
                {
                    max = Diff[f];
                    Valore = f;
                }
            }

            LcdConsole.WriteLine("    " + max + "    " + Valore + "    " + Tacho[Valore]);

            Thread.Sleep(1000);

        }

        public static void All()
        {

            if (!fine)
            {

                Thread.Sleep(100);
                LcdConsole.WriteLine("ALLINEANDO CON L'OGGETTO...");
                Line();
                LcdConsole.WriteLine("    " + motorA.GetTachoCount() + " su " + Tacho[Valore]);

                while ((motorA.GetTachoCount() > Tacho[Valore] - 36) && (!fine))
                {

                    V.SpinRight(14, 14, true);																			//edit3
                    LcdConsole.WriteLine("    " + motorA.GetTachoCount() + " su " + Tacho[Valore]);
                    Thread.Sleep(30);

                    buts.EnterPressed += () =>
                    {
                        fine = true;
                    };

                }

                Thread.Sleep(500);

                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }

        }

        public static void Ruota180()
        {

            if (!fine)
            {

                Thread.Sleep(100);
                LcdConsole.WriteLine("RUOTANDO...");
                Line();
                V.SpinLeft(20, 1152, true);
                Thread.Sleep(6000);

                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }
        }

        public static void ApriPinza()
        {

            if (!fine)
            {

                LcdConsole.WriteLine("APRENDO PINZA...");
                Line();
                motorC.SetSpeed(-45);
                Thread.Sleep(10000);
                motorC.Brake();

                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }

        }

        public static void ChiudiPinza()
        {

            if (!fine)
            {

                LcdConsole.WriteLine("CHIUDENDO PINZA...");
                Line();
                motorC.SetSpeed(45);
                Thread.Sleep(10500);
                motorC.Brake();

                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }
        }

        public static void All2()
        {

            if (!fine)
            {

                Thread.Sleep(100);
                LcdConsole.WriteLine("ALLINEANDO APPROSSIMATIVAMENTE...");
                Line();
                V.SpinRight(20, 387, true);
                Thread.Sleep(2500);

                while (motorA.GetTachoCount() > motorB.GetTachoCount() + 80)
                {
                    V.SpinLeft(14, 9, true);
                    Thread.Sleep(50);
                }
                while (motorA.GetTachoCount() < motorB.GetTachoCount() + 80)
                {
                    V.SpinRight(14, 9, true);
                    Thread.Sleep(50);
                }

                V.SpinRight(20, 576, true);																	//edit4
                Thread.Sleep(3000);

                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }

        }

        public static void Sfondamento(int tempo)
        {

            if (!fine)
            {

                LcdConsole.WriteLine("SFONDANDANDO LA PARETE...");
                Line();

                motorA.SetSpeed(40);
                motorB.SetSpeed(40);
                Thread.Sleep(tempo);
                motorA.SetSpeed(-26);
                motorB.SetSpeed(-26);
                Thread.Sleep(500);
                V.SpinLeft(20, 576, true);														//edit5
                Thread.Sleep(2500);

                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }

        }

        public static void WallFollower()
        {

            LcdConsole.WriteLine("SEGUENDO IL MURO...");
            Line();
            SeguiMuro();

        }

        public static void SeguiMuro()
        {

            if (!fine)
            {

                //int g = 20;

                while ((sensorB.GetDistance() > 105) && (!TouchSensor.IsPressed()))
                {

                    //int correction = (sensorB.ReadDistance() - g) / 2;
                    //LcdConsole.WriteLine ("    " + sensorB.ReadDistance());
                    //Line ();
                    //int speedA = 20 + correction;
                    //int speedB = 20 - correction;
                    //motorA.SetSpeed ((sbyte)speedA);
                    //motorB.SetSpeed ((sbyte)speedB);
                    //Thread.Sleep (45);

                    int speedA = 20;
                    int speedB = 20;
                    motorA.SetSpeed((sbyte)speedA);
                    motorB.SetSpeed((sbyte)speedB);
                    Thread.Sleep(45);


                }

                buts.EnterPressed += () =>
                {
                    fine = true;
                };

                FineMuro();

                buts.EnterPressed += () =>
                {
                    fine = true;
                };

            }

        }

        public static void FineMuro()
        {

            if (!fine)
            {

                if (TouchSensor.IsPressed())
                {

                    motorA.SetSpeed(-30);
                    motorB.SetSpeed(-30);
                    Thread.Sleep(350);
                    V.Brake();
                    V.SpinRight(20, 1440, true);
                    Thread.Sleep(4500);
                    ApriPinza();
                    Thread.Sleep(5000);

                    buts.EnterPressed += () =>
                    {
                        fine = true;
                    };

                }
                else
                {

                    Sfondamento(650);
                    SeguiMuro();

                    buts.EnterPressed += () =>
                    {
                        fine = true;
                    };

                }

            }

        }

    }

}