/*
using System.Threading;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;

namespace OmegaTeam
{
    class ScaricaPallina
    {

        public static void Scarica() {

            if (!Salvataggio.fine)
                Allineamento3();
            if (!Salvataggio.fine)                          //se in alto a destra    M.turnRight(90);   M.turnLeft(90);   M.turnLeft(135); 
                Avvicinamento3();
            if (!Salvataggio.fine)                          //se in basso a sinistra   M.turnLeft(90);    M.turnLeft(90);   M.turnLeft(135);
                //M.turnLeft(90);
            if (!Salvataggio.fine)                          //se in basso a destra   M.turnRight(90);    M.turnRight(90);    M.turnRight(135);
                RaggiungiAngolo();
            if (!Salvataggio.fine)
               // M.turnLeft(135);
            if (!Salvataggio.fine) {
                P.apri();
                Thread.Sleep(250);
                Salvataggio.Salva();
            }
            else {
                Salvataggio.fine = false;
                Salvataggio.Salva();
            }


        }

        private static Sensors S = new Sensors();
        public static Motors M = new Motors();
		public static Claw = new Claw()
        public static ButtonEvents B = new ButtonEvents();

        public static Motor motorA = new Motor(MotorPort.OutB);
        public static Motor motorB = new Motor(MotorPort.OutD);

        public static void Allineamento3() {
            int tachoA = M.motR.GetTachoCount();
            int tachoB = M.motL.GetTachoCount();
            if (tachoA > tachoB) {
                int temp = tachoA;
                tachoA = tachoB;
                tachoB = temp;

                while (tachoB > tachoA) {
                    M.V.SpinRight(25, 25, true);
                    Thread.Sleep(250);
                }

            }
            else {

                while (tachoB > tachoA) {
                    M.V.SpinLeft(25, 25, true);
                    Thread.Sleep(250);
                }

            }



        }

        public static void Avvicinamento3() {
            int Distanza = S.getDist(2);                  //il sensore in alto
            int time = Distanza * 18;                 //tarare il tempo
            M.motL.SetSpeed(45);
            M.motR.SetSpeed(45);
            Thread.Sleep(time);
        }

        public static void RaggiungiAngolo() {
            while (!S.Touch.IsPressed()) {
                M.goFor(1);
                //Thread.Sleep(100);
            }
        }


    }

}
*/