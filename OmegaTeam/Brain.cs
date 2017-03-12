using System;
using System.Threading;

using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using System.Collections.Generic;

namespace OmegaTeam {
	public static class Brain {

		//################################################################################
		//################################################################################

		public static bool stop = false;
		static int ANGLE = 5;

		//################################################################################
		//################################################################################

		static Sensors S = new Sensors ();
		static Motors M = new Motors ();

		public static int AngleDiff;
		public static List<int> Angles = new List<int> ();

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

			//return Math.Abs(WHITE[sensor] - S.GetColor(sensor)) * 0.05; // Formula per calcolare la correzione di posizione

			sbyte currentColor = S.GetColor (sensor);
			if (currentColor > Sensors.WHITE [sensor])
				currentColor = Sensors.WHITE [sensor];

			return Math.Pow (Math.Pow (Sensors.WHITE [sensor] - currentColor, 6), 1.0 / 7);
		}

		static void Print (string a) {

			LcdConsole.WriteLine (a);

		}

		static void AvoidObstacle () {

			M.GoStraight ((sbyte)-M.Speed, 0.5, true);

			M.V.SpinLeft (M.Speed, 500, true).WaitOne ();

			M.GoStraight (M.Speed, 0.5, true);

			M.SetSpeed ((sbyte)(M.Speed + 15), (sbyte)(M.Speed - 15));

			while (!S.GetState (0, true) && !S.GetState (1, true)) {
				Thread.Sleep (10);
			}

			M.V.SpinLeft (30, 150, true).WaitOne ();

			M.GoStraight (M.Speed);

			while (!S.GetState (0, true)) {
				Thread.Sleep (10);
			}

			M.Brake ();

			Thread.Sleep (500);

		}

		public static void LineFollower () {

			bool CL = S.GetState (0);
			bool CR = S.GetState (1);

			bool SILVER = (S.GetColor (0) >= 80 && S.GetColor (1) >= 80);

			int Angle = S.GetAngle ();
			AngleDiff = Math.Abs (Math.Abs (Angle) - Math.Abs (MainClass.Angle));
			Print (AngleDiff.ToString ());
			Angles.Add (Math.Abs (Math.Abs (Angle) - Math.Abs (MainClass.Angle)));

			if (CL && CR) {

				M.Brake ();

				switch (S.CheckGreen ()) {

				case 0:
					Print ("Verde sinistra");
					S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
					M.SetSpeed (-15, 45, 1, true);
					break;
				case 1:
					Print ("Verde destra");
					S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
					M.SetSpeed (45, -15, 1, true);
					break;
				case -1:
					if ((Angles.Count > 5) && (CheckAngle (Angles [Angles.Count - 1]) || CheckAngle (Angles [Angles.Count - 2]) || CheckAngle (Angles [Angles.Count - 3]))) {
						Print ("Avanti " + AngleDiff);
						M.GoStraight (M.Speed, 0.5);
						S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
					} else {
						Print ("Niente " + AngleDiff);
						S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.Turn (0.5, true);
					}

					break;
				}
			}


			if ((CL && !CR) || (!CL && CR)) {

				M.Turn (0.05); // 0.1

			}

			if (!CL && !CR) {

				M.GoStraight (M.Speed, 0.01); // 0.05

			}

			if (S.ObstacleNoticed ()) {

				AvoidObstacle ();

			}

			if (SILVER)
				TerminateProgram ();

			Buttons.EscapePressed += () => {
				TerminateProgram ();
			};

			Buttons.UpPressed += () => {
				Buttons.DownPressed += () => {
					MainClass.Angle = S.GetAngle ();
				};
			};

		}

		static bool CheckAngle (int val) {
			return (val > ANGLE) && (val < (255 - ANGLE));
		}

		public static void Rescue () {

			LcdConsole.WriteLine ("Inizio Rescue");

			Thread.Sleep (2000);

			Salvataggio.RunRescue ();

		}

	}
}