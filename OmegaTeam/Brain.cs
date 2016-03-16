using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace OmegaTeam
{
    public class Brain
    {

        //################################################################################
        //################################################################################

        private static sbyte[] BLACK = { 30, 30 };
        // Valore per cui viene attivato "nero"
        private static sbyte[] WHITE = { 50, 50 };
        public static bool stop = false;

        //################################################################################
        //################################################################################

        private static Sensors S = new Sensors();
        private static Motors M = new Motors();

        private static ButtonEvents Buttons = new ButtonEvents();

        public Brain() {
        }

        private static bool state(sbyte sensor) {
			
            sbyte colorValue = S.getColor(sensor);

            if (colorValue <= BLACK[sensor])
                return true; // Sono sul nero, necessito di correzione

            return false; // Sono a metà, non necessito di correzione

        }

        public static double correction(sbyte sensor) {

            return Math.Abs(WHITE[sensor] - S.getColor(sensor)) * 0.05; // Formula per calcolare la correzione di posizione

        }

        private static void print(string a) {
			
            LcdConsole.WriteLine(a);

        }

        private static void avoidObstacle() {

            M.turnRight(90);

            M.V.TurnLeftForward(M.Speed, 80, 1200, true).WaitOne();

        }

        public static void lineFollower() {

            bool CL = state(0); // Bianco o nero?
            bool CR = state(1);
            bool SILVER = (S.getColor(0) >= 90 && S.getColor(1) >= 90);

            if (CL && CR) { // Nero Nero, forse Verde?

                M.Brake();
                M.goStraight((sbyte)(-M.Speed), 0.2, true);

                bool[] green = S.isGreen();

                bool GL = green[0];
                bool GR = green[1];

                if (GL) { // Verde a sinistra

                    print("Verde sinistra");
                    M.goStraight(M.Speed, 0.2);
                    M.setSpeed(-2, 20, 0.8);

                }

                if (GR) { // Verde a destra

                    print("Verde destra");
                    M.goStraight(M.Speed, 0.2);
                    M.setSpeed(20, -2, 0.8);

                }

                if (!GL && !GR) { // Nero nero

                    M.Brake();

                    if (S.getMaxColor()) { // Quale sensore è più sul bianco? 0 (sinistra) o 1 (destra)
                        M.turnLeft(0.2); // Il sensore destra è più sul bianco
                    }
                    else {
                        M.turnRight(0.2); // Il sensore sinistra è più sul bianco
                    }

                }

            }

            if (!CL && !CR) { // Bianco Bianco

                M.goStraight(M.Speed, 0.1);

            }

            if (CL && !CR) { //Nero Bianco

                M.turnLeft(0.1);

            }

            if (!CL && CR) { //Bianco Nero

                M.turnRight(0.1);

            }

            if (S.obstacle()) { // Attenzione... Ostacolo rilevato!

                print("Ostacolo!");
                avoidObstacle();

            }

            if (SILVER) {

                stop = true;

            }




            Buttons.EscapePressed += () => {

                stop = true;
                print("Fine seguilinea");

            };

        }

        public static void rescue() {

        }

    }
}