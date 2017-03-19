using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Movement;

namespace OmegaTeam {
	class Salvataggio {

		static Sensors S = new Sensors ();
		static Motors M = new Motors ();

		public const int CONST = -5;
		public const int BALL_DIST = 300;
		public const sbyte CRAW_SPEED = 127;
		public const int RELEASE = -170;
		public const int UP = 0;
		public const int DOWN = 290;

		public static int Number_of_check = 0;
		public static bool fine = false;

		public static void RunRescue () {

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

				distR [i] = S.GetDist (2);
				distL [i] = S.GetDist (1);

				if (i > 5) {

					midL [l] = ((distL [i - 4] + distL [i - 3] + distL [i - 2] + distL [i - 1] + distL [i]) / 5);
					midR [l] = ((distR [i - 4] + distR [i - 3] + distR [i - 2] + distR [i - 1] + distR [i]) / 5);

					LcdConsole.WriteLine (midR [i] + "    " + midL [i] + "    " + S.GetDist (3));

					if (((midL [l] < BALL_DIST) && (midL [l] > midL [l - 1]) && (midL [l - 1] > midL [l - 2])) || ((midR [l] < BALL_DIST) && (midR [l] > midR [l - 1]) && (midR [l - 1] > midR [l - 2]))) {

						M.motL.Brake ();
						M.motR.Brake ();

						if (midR [l] < BALL_DIST) {
							takeBall (false);
							M.Brake (2000);
						} else {
							takeBall (true);
							M.Brake (2000);
						}

						distL [i - 4] = distL [i - 3] = distL [i - 2] = distL [i - 1] = distL [i] = BALL_DIST + 1;
						distR [i - 4] = distR [i - 3] = distR [i - 2] = distR [i - 1] = distR [i] = BALL_DIST + 1;

						M.SetSpeed (12, 12);

					}

				}

				Thread.Sleep (100);
				l++;
				i++;
				if (S.GetDist (3) <= 80) stop = true;

			}

			M.Brake ();

			Thread.Sleep (10);

			FindAngle ();

			Thread.Sleep (2000);

			GoTacho (M.motP, UP);

			M.motP.Off ();

			//RunRescue ();

		}

		static void FindAngle () {

			M.SetSpeed (50, 50);
			Thread.Sleep (1000);

			/*LcdConsole.WriteLine ("");
			LcdConsole.WriteLine (S.GetDist (2) + "    " + S.GetDist (1) + "    " + S.GetDist (3));

			Thread.Sleep (3000);

			if ((S.GetDist (2) < 300) || (S.GetDist (2) > 450)) {
				GoToAngle (2);
			} else {
				if ((S.GetDist (1) < 300) || (S.GetDist (1) > 450)) {
					GoToAngle (1);
				} else {
					if (Number_of_check < 2) {
						M.SetSpeed (-10, -10);
						Thread.Sleep (500);
						M.Brake ();
						Number_of_check++;
						FindAngle ();
					} else {
						GoToAngle (3);
					}
				}
			}*/

			GoToAngle (1);

		}

		static void PosizionaRobot () {


			Go (2.9);
			GoTacho (M.motP, RELEASE);
			turn90 (true, true);
			Go (1.5, -30, -30);
			Go (3.525);

			turn90 (false, true);
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
			case (1): {

					Go (0.8, -30, -30);
					turn90 (false);
					Go (0.85, 90, 90);
					Go (0.8, 50, -50);
					Go (0.8, 20, 20);
					GoTacho (M.motP, RELEASE);
					break;

				}
			case (2): {

					Go (0.8, -30, -30);
					turn90 (true);
					Go (0.85, 90, 90);
					Go (0.8, -50, 50);
					Go (0.8, 20, 20);
					GoTacho (M.motP, RELEASE);
					break;

				}
			case (3): {
					Go (1, -30, -30);
					Go (2.2, -100, 100);
					Go (7.5, 127, 95);                        //va dritto!! ass!!
					Go (0.3, -30, -30);
					turn90 (false);
					Go (0.9, 90, 90);
					Go (0.8, 50, -50);
					Go (0.8, 20, 20);
					GoTacho (M.motP, RELEASE);
					break;

				}
			}

		}

		static void GoTacho (Motor Mot, int goal) {

			while (Mot.GetTachoCount () > goal) {
				Mot.SetSpeed (-CRAW_SPEED);
				Thread.Sleep (5);
			}

			while (Mot.GetTachoCount () < goal) {
				Mot.SetSpeed (CRAW_SPEED);
				Thread.Sleep (5);
			}

			M.Brake ();

		}

		static void turn90 (bool right1, bool posizionarobot = false) {

			int TACHO_VALUE = 1200;        //970

			if (posizionarobot) { TACHO_VALUE = 1050; }

			M.motL.ResetTacho ();
			M.motR.ResetTacho ();

			if (!right1) {
				while (M.motR.GetTachoCount () < M.motL.GetTachoCount () + TACHO_VALUE) {
					M.SetSpeed (-25, 25);
					Thread.Sleep (5);
				}

			} else {
				while (M.motL.GetTachoCount () < M.motR.GetTachoCount () + TACHO_VALUE) {
					M.SetSpeed (25, -25);
					Thread.Sleep (5);

				}

			}

			M.Brake ();

		}

		static void CrawAction () {

			M.motP.SetSpeed (127);
			Thread.Sleep (300);                 //ferma i motori mentre va la pinza si abbassa
			M.motL.Brake ();
			M.motR.Brake ();
			Thread.Sleep (1200);
			M.motL.Brake ();
			Thread.Sleep (50);
			GoTacho (M.motP, UP);

		}
		static void takeBall (bool right1) {

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

	}
}