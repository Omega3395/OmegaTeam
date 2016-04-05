using System.Threading;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;

namespace OmegaTeam
{
    public class Salvataggio
    {
        public static void  Salva() {

            M.motL.ResetTacho();
            M.motR.ResetTacho();

            for (int j = 0; j < 6; j++) {
                if (!fine) {
                    CaricaPallina();
                    ScaricaPallina.Scarica();
                    B.EnterPressed += () => {
                        fine = true;
                    };
                }
                else {
                    fine = false;
                    Salva();
                }
            }
        }

        //inizializzazione dei sensori e dei motori

        private static Sensors S = new Sensors();
        public static Motors M = new Motors();
        public static Pinza P = new Pinza();
        public static ButtonEvents B = new ButtonEvents();

        public static Motor motorA = new Motor(MotorPort.OutB);
        public static Motor motorB = new Motor(MotorPort.OutD);

        public static int Number = 100;
        public static bool fine = false;
        private static int[] Dist = new int[100];
        private static int[] Dist2 = new int[100];
        private static int[] distanza = new int[100];
        private static int[] distanza2 = new int[100];
        private static int[] tacho = new int[100];
        private static int[] diff = new int[100];
        private static int k = 0;


        //funzione principare della classe del salvataggio

        public static void CaricaPallina() {
            PosizionaRobot();
            Radar();
            EliminaRumori();
            Differenze();
            DIffMax();
            Allineamento();
            //M.turnRight(180);
            P.apri();
            Thread.Sleep(800);
            Avvicinamento();
            Thread.Sleep(800);
            P.chiudi();
            //Conclusione();
        }

        //definizione di tutte le funzioni usate nella funzione principale

        public static void PosizionaRobot() {
            LcdConsole.WriteLine("POSIZIONANDO ROBOT...");
            if (!fine) {
                M.goFor(60);
                M.Brake();
                //M.turnLeft(90);
                M.goFor(35);
                M.Brake();
                B.EnterPressed += () => {
                    fine = true;
                };
            }
        }

        public static void Radar() {
            LcdConsole.WriteLine("SCANNERIZZANDO STANZA...");
            for (int i = 0; i < Number; i++) {
                if (!fine) {
                    Thread.Sleep(250);
                    Dist[i] = S.getDist(1);
                    Dist2[i] = S.getDist(2);
                    tacho[i] = M.motR.GetTachoCount();
                    LcdConsole.WriteLine("    DIST " + Dist[i]);
                    M.V.SpinLeft(25, 25, true);
                    B.EnterPressed += () => {
                        fine = true;
                    };
                }
            }
        }

        public static void EliminaRumori() {
            for (int i = 0; i < Number - 3; i++) {
                if (!fine) {

                    distanza[i] = (Dist[i] + Dist[i + 1] + Dist[i + 2] + Dist[i + 3]) / 4;
                    distanza2[i] = (Dist2[i] + Dist2[i + 1] + Dist2[i + 2] + Dist2[i + 3]) / 4;
                    B.EnterPressed += () => {
                        fine = true;
                    };
                }

            }
            distanza[Number - 3] = Dist[Number - 3];
            distanza[Number - 2] = Dist[Number - 2];
            distanza[Number - 1] = Dist[Number - 1];
            distanza2[Number - 3] = Dist2[Number - 3];
            distanza2[Number - 2] = Dist2[Number - 2];
            distanza2[Number - 1] = Dist2[Number - 1];

        }

        public static void Differenze() {
            LcdConsole.WriteLine("ANALIZZANDO I DATI...");
            for (int i = 1; i < Number; i++) {
                if (!fine) {
                    diff[i] = distanza2[i] - distanza[i];
                    Thread.Sleep(10);
                    LcdConsole.WriteLine("    DIFF " + diff[i]);
                    B.EnterPressed += () => {
                        fine = true;
                    };
                }
            }
        }

        public static void DIffMax() {
            int max = 0;
            for (int i = 0; i < Number; i++) {
                if (diff[i] > max) {
                    max = diff[i];
                    k = i;
                }
            }
        }

        public static void Allineamento() {
            LcdConsole.WriteLine("ALLINEANDO CON L'OGGETTO...");
            while ((M.motR.GetTachoCount() > tacho[k] + 18) && (!fine)) {
                M.V.SpinRight(25, 25, true);
                Thread.Sleep(250);
                LcdConsole.WriteLine("    TACHO " + M.motR.GetTachoCount() + " SU " + tacho[k]);
                B.EnterPressed += () => {
                    fine = true;
                };
            }
        }

        public static void Avvicinamento() {
            LcdConsole.WriteLine("AVVICINANDO ALL'OGGETTO...");
            if (!fine) {
                int dist = S.getDist(1);
                int time = dist * 18;
                M.motL.SetSpeed(-45);
                M.motR.SetSpeed(-45);
                Thread.Sleep(time);
                B.EnterPressed += () => {
                    fine = true;
                };
            }
            Conclusione();
        }

        public static void Conclusione() {
            M.motL.Brake();
            M.motR.Brake();
        }
    }
}