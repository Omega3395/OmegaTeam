using System;
using System.Collections.Generic;
using System.Threading;

using MonoBrickFirmware.Display;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.UserInput;

namespace OmegaTeam {
	public static class Brain {

		//################################################################################
		//################################################################################

		public static bool stop = false;
		static int SILVER_VALUE = 70;

		//################################################################################
		//################################################################################

		static Sensors S = new Sensors ();
		static Motors M = new Motors ();

		static ButtonEvents Buttons = new ButtonEvents ();

		/// <summary>
		/// Terminates the program.
		/// </summary>
		public static void TerminateProgram () {
			M.Brake ();
			stop = true;
		}

		/// <summary>
		/// Gets the correction.
		/// </summary>
		/// <returns>The correction of the darkest sensor</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public static double GetCorrection (sbyte sensor) {

			sbyte currentColor = S.GetColor (sensor);
			if (currentColor > Sensors.WHITE [sensor])
				currentColor = Sensors.WHITE [sensor];

			return Math.Pow (Math.Pow (Sensors.WHITE [sensor] - currentColor, 6), 1.0 / 7); // Formula per calcolare la correzione di posizione
		}

		static void Print (string a) {

			LcdConsole.WriteLine (a);

		}

		static void AvoidObstacle () {

			M.GoStraight ((sbyte)-M.Speed, 0.5, true);

			M.V.SpinRight (M.Speed, 500, true).WaitOne ();

			M.GoStraight (M.Speed, 0.7, true);

			M.SetSpeed ((sbyte)(M.Speed - 15), (sbyte)(M.Speed + 15));

			while (!S.GetState (0, true) && !S.GetState (1, true)) {
				Thread.Sleep (10);
			}

			M.V.SpinRight (30, 150, true).WaitOne ();

			M.GoStraight (M.Speed);

			while (!S.GetState (1, true)) {
				Thread.Sleep (10);
			}

			M.Brake ();

			Thread.Sleep (500);

		}

		public static void LineFollower () {

			bool CL = S.GetState (0);
			bool CR = S.GetState (1);

			bool SILVER = (S.GetColor (0) >= SILVER_VALUE && S.GetColor (1) >= SILVER_VALUE);

			if (CL && CR) { // Nero Nero

				M.Brake ();

				switch (S.CheckGreen2 ()) {

				case 0:
					M.Brake ();
					Print ("Verde sinistra");
					S.SetSensorsMode (ColorMode.Reflection);
					M.SetSpeed (-15, 45, 1, true);
					break;
				case 1:
					M.Brake ();
					Print ("Verde destra");
					S.SetSensorsMode (ColorMode.Reflection);
					M.SetSpeed (45, -15, 1, true);
					break;
				case -1:

					M.Brake ();
					Print ("Niente");
					S.SetSensorsMode (ColorMode.Reflection);
					for (int i = 0; i < 8; i++)
						M.Turn (0.05);

					break;

				case -2:
					M.Brake ();
					S.SetSensorsMode (ColorMode.Reflection);
					Print ("NERO NERO");
					M.GoStraight (M.Speed, 0.5);

					break;
				}

			}


			if ((CL && !CR) || (!CL && CR)) { // Bianco Nero / Nero Bianco

				M.Turn (0.05); // 0.1

			}

			if (!CL && !CR) { // Bianco Bianco

				M.GoStraight (M.Speed, 0.01); // 0.05

			}

			if (S.ObstacleNoticed ()) {

				Print ("Ostacolo!");
				AvoidObstacle ();

			}

			if (SILVER) {
				Print ("SILVER");
				TerminateProgram ();
			}

			Buttons.EscapePressed += () => {
				TerminateProgram ();
			};

		}

		public static void Rescue () {

			LcdConsole.WriteLine ("Inizio Rescue");

			Thread.Sleep (2000);

			Salvataggio.RunRescue ();

		}

	}
}