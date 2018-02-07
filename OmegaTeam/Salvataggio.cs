using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Movement;
using System.Collections.Generic;

namespace OmegaTeam {
	class Salvataggio {

		public const int CONST = -5;
		public const int BALL_DIST = 300;                   //Distanza "limite"
        const int VIEW_LIMIT = BALL_DIST + 30;
		public const sbyte CRAW_SPEED = 127;
		public const int RELEASE = -170;
		public const int UP = 0;
		public const int DOWN = 290;
        public const sbyte TURN_SPEED = 40;


        public static int Number_of_check = 0;
		public static bool fine = false;

		public static void RunRescue () { // Obrobrio, da riscrivere!!!

			M.motP.ResetTacho ();                                 //IL MOTORE DEVE TROVARSI NELLO STATO "UP" A INIZIO RESCUE

			LcdConsole.WriteLine ("Ciao");

			int [] distL = new int [800];
			int [] distR = new int [800];
			int [] midL = new int [800];
			int [] midR = new int [800];

			midL [0] = 500;
			midR [0] = 500;
			int l = 1;
			int i = 0;

			bool stop = false;

			PosizionaRobot ();

			M.SetSpeed (12, 12);

			while (!stop) {

				distR [i] = S.GetDist (2); if (distR[i] > VIEW_LIMIT) distR[i] = VIEW_LIMIT;
				distL [i] = S.GetDist (1); if (distL[i] > VIEW_LIMIT) distL[i] = VIEW_LIMIT;

                if (i > 5) {

					midL [l] = ((distL [i - 4] + distL [i - 3] + distL [i - 2] + distL [i - 1] + distL [i]) / 5);
					midR [l] = ((distR [i - 4] + distR [i - 3] + distR [i - 2] + distR [i - 1] + distR [i]) / 5);

					LcdConsole.WriteLine (midR [i] + "    " + midL [i] + "    " + S.GetDist (3));

					if (((midL [l] < BALL_DIST) && (midL [l] > midL [l - 1]) && (midL [l - 1] > midL [l - 2])) || ((midR [l] < BALL_DIST) && (midR [l] > midR [l - 1]) && (midR [l - 1] > midR [l - 2]))) {

                        M.Brake(0, true, true);

						if (midR [l] < BALL_DIST) {
							takeBall (false);           // Palla a sinistra
							M.Brake (2000);
						} else {
							takeBall (true);            // Palla a destra
							M.Brake (2000);
						}

						distL [i - 4] = distL [i - 3] = distL [i - 2] = distL [i - 1] = distL [i] = BALL_DIST + 1;
						distR [i - 4] = distR [i - 3] = distR [i - 2] = distR [i - 1] = distR [i] = BALL_DIST + 1;

						M.SetSpeed (12, 12);

					}

                    /*
                    if (theSame(distL,i-4,i,255)) { // Palla a sinistra molto vicina
                        takeBall(false);           
                        M.Brake(2000);
                    }
                    if (theSame(distR, i - 4, i, 255)) { // Palla a sinistra molto vicina
                        takeBall(false);
                        M.Brake(2000);
                    }
                    */

                }

				Thread.Sleep (100);
				l++;
				i++;
                if (S.GetDist(3) <= 80 && (midL[i] > midL[i - 1] || midR[i] > midR[i - 1]))
                    stop = true;

			}

			M.Brake ();

			Thread.Sleep (10);

			FindAngle ();

			Thread.Sleep (2000);

			GoTacho (M.motP, UP);

			M.motP.Off ();

			//RunRescue ();

		}

        static bool theSame(int[] arr, int start, int end, int sameValue) {
        
            for(int i = start; i < end; i++) 
                if (arr[i] != sameValue)
                    return false;
            
            return true;

        }

        static void FindAngle() {

            M.SetSpeed(50, 50);
            Thread.Sleep(1000);

            LcdConsole.WriteLine("");
            LcdConsole.WriteLine(S.GetDist(2) + "    " + S.GetDist(1) + "    " + S.GetDist(3));

            Thread.Sleep(3000);

            if ((S.GetDist(2) < 300) || (S.GetDist(2) > 450)) {
                GoToAngle(2);  //angolo alto dx
            } else if ((S.GetDist(1) < 300) || (S.GetDist(1) > 450)) {
                GoToAngle(1); //angolo alto sx
            } else {
                if (Number_of_check < 2) {
                    M.SetSpeed(-10, -10);
                    Thread.Sleep(500);
                    M.Brake();
                    Number_of_check++;
                    FindAngle();
                } else {
                    GoToAngle(3);
                }
            }
        }

		static void PosizionaRobot () {

            bool leftEnter;

			Go (2.9);
			GoTacho (M.motP, RELEASE);

            if (S.GetDist(1) < S.GetDist(2))
                leftEnter = true;
            else
                leftEnter = false;
            LcdConsole.WriteLine("Entrata a sinistra" + leftEnter);

            turn90 (leftEnter, true);
			Go (1.5, -30, -30);
			Go (3.525);

			turn90 (!leftEnter, true);
			Go (3.17, -30, -30);
			Go (1.15);
			GoTacho (M.motP, UP);
			Thread.Sleep (250);
		}

		static void Go (double time, sbyte left1 = 30, sbyte right1 = 30) {
			M.SetSpeed (left1, right1);
			Thread.Sleep ((int)(time * 1000));
			M.Brake ();
		}

		static void GoToAngle (int Position) {

			switch (Position) {
			case (1): {                     //Angolo alto a sinistra

					Go (0.8, -30, -30);
					turn90 (false);
					Go (0.85, 90, 90);
					Go (0.8, 50, -50);
					Go (0.8, 20, 20);
					GoTacho (M.motP, RELEASE);
					break;

				}
			case (2): {                      //Angolo alto a destra

					Go (0.8, -30, -30);
					turn90 (true);
					Go (0.85, 90, 90);
					Go (0.8, -50, 50);  //ruota 45 gradi 
					Go (0.8, 20, 20);
					GoTacho (M.motP, RELEASE);
					break;

				}
			case (3): {                    //angolo indetro rispetto all'entrata

					Go (1, -30, -30);
					Go (2.2, -100, 100);
					Go (7.5, 127, 95);                        //va dritto!! ass!!
					Go (0.3, -30, -30);
					turn90 (false);                            //varia in base all'entrata
					Go (0.9, 90, 90);
					Go (0.8, 50, -50);
					Go (0.8, 20, 20);
					GoTacho (M.motP, RELEASE);
					break;

				}
			}

		}

		public static void GoTacho (Motor Mot, int goal) { //Solo per la pinza

			while (Mot.GetTachoCount () > goal) {
				Mot.SetPower (-CRAW_SPEED);
				Thread.Sleep (2);
			}

			while (Mot.GetTachoCount () < goal) {
				Mot.SetPower (CRAW_SPEED);
				Thread.Sleep (2);
			}

			M.Brake ();

		}

		public static void turn90 (bool right1, bool posizionarobot = false) { //momentaneamente pubblica

			int TACHO_VALUE = 1200;        //970

			if (posizionarobot) { TACHO_VALUE = 1050; }

			M.motL.ResetTacho ();
			M.motR.ResetTacho ();

			if (!right1) {
				while (M.motR.GetTachoCount () < M.motL.GetTachoCount () + TACHO_VALUE) {
					M.SetSpeed (-TURN_SPEED, TURN_SPEED);
					Thread.Sleep (2);
				}
			} else {
				while (M.motL.GetTachoCount () < M.motR.GetTachoCount () + TACHO_VALUE) {
					M.SetSpeed (TURN_SPEED, -TURN_SPEED);
					Thread.Sleep (2);
				}
			}

			M.Brake ();

		}

		public static void CrawAction () {   //prendi la pallina con l'azione della pinza

            //metdono 1: speed 127 per 300ms

            //metodo 2:
            M.motP.ResetTacho();
            GoTacho(M.motP, 290); //mentre va indietro la pinza si abbassa

            //metodo 3
            //crawDown();

            M.Brake(1250, true, true);
			GoTacho (M.motP, UP);

		}

		static void takeBall (bool right1) {  // abbassa e alza la pinza

			turn90 (right1);

			M.SetSpeed (-50, -50);
			Thread.Sleep (1300);

			CrawAction ();                              //i motori stanno ancora andando

			M.Brake ();

			Thread.Sleep (50);
			Go (1.6, 50, 50);

			turn90 (!right1);

			Go (0.1, 50, -50);
		}

        static void crawDown() {
            M.motP.SetSpeed(CRAW_SPEED);
            List<int> val = new List<int>();
            bool stop = false;
            while (!stop) {
                val.Add(M.motP.GetTachoCount());
                if (val.Count >= 6)
                    if (theSame(val.ToArray(), val.Count - 6, val.Count - 1, val[val.Count - 1]))
                        stop = true;
                Thread.Sleep(20);
            }
        }

	}
}