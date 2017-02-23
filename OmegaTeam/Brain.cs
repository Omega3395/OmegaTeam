using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace OmegaTeam {
	public class Brain {

		//################################################################################
		//################################################################################

		public static bool stop = false;
		static sbyte green = 0;
		static sbyte obstacle = 0;
		static int t = 0;
		static TimeSpan diff = new TimeSpan ();

		//################################################################################
		//################################################################################

		static Sensors S = new Sensors ();
		static Motors M = new Motors ();

		static ButtonEvents Buttons = new ButtonEvents ();

		public Brain () {
		}

		/// <summary>
		/// Terminates the program.
		/// </summary>
		static void TerminateProgram () {
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

			/*M.GoFor(5, false);
			M.V.SpinRight(30, 500, true).WaitOne();

			if (S.GetDist(1) > 25 * 10) { // Sorpassa l'ostacolo a destra

				M.GoFor(5);
				M.SetSpeed((sbyte)(M.Speed - 10), (sbyte)(M.Speed + 10));
				while (!S.GetState(0, true) && !S.GetState(1, true)) {
					Thread.Sleep(10);
				}

				M.V.SpinRight(30, 300, true).WaitOne();

			} else { // Sorpassa l'ostacolo a sinistra

				M.V.SpinLeft(30, 1000, true).WaitOne();
				M.GoFor(5);
				M.SetSpeed((sbyte)(M.Speed + 10), (sbyte)(M.Speed - 10));
				while (!S.GetState(0, true) && !S.GetState(1, true)) {
					Thread.Sleep(10);
				}

				M.V.SpinLeft(30, 300, true).WaitOne();

			}

			M.Brake();*/

			M.GoFor (5, false);
			M.V.SpinLeft (30, 500, true).WaitOne ();
			M.GoFor (5);
			M.SetSpeed ((sbyte)(M.Speed + 10), (sbyte)(M.Speed - 10));
			while (!S.GetState (0, true) && !S.GetState (1, true)) {
				Thread.Sleep (10);
			}

			M.V.SpinLeft (30, 300, true).WaitOne ();

			M.GoStraight (M.Speed);

			while (!S.GetState (0)) {
				Thread.Sleep (10);
			}

			M.Brake ();

			Thread.Sleep (500);

		}

		public static void LineFollower () {

			bool CL = S.GetState (0);
			bool CR = S.GetState (1);

			bool SILVER = (S.GetColor (0) >= 70 && S.GetColor (1) >= 70);

			if (CL && CR) {

				M.Brake ();

				switch (S.CheckGreen ()) {

				case 0:
					Print ("Verde sinistra");
					S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
					M.SetSpeed (-10, 30, 1, true);
					break;
				case 1:
					Print ("Verde destra");
					S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
					M.SetSpeed (30, -10, 1, true);
					break;
				case -1:
					if (green < 4) {
						green++;
						Print ("Niente " + green);
						S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.Turn (0.4, true);
					} else {
						Print ("Avanti");
						M.GoStraight (M.Speed, 1.5);
						S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
					}

					break;
				case -2:
					if (green < 1)
						t = DateTime.Now.Second;

					if (DateTime.Now.Second - t < 5) {
						green++;
						Print ("Niente " + green);
						S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.Turn (0.4, true);
					} else {
						Print ("Avanti");
						M.GoStraight (M.Speed, 1.5);
						S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
					}

					break;
				}

			}

			if ((CL && !CR) || (!CL && CR)) {

				M.Turn (0.05); // 0.1
				green = 0;

			}

			if (!CL && !CR) {

				M.GoStraight (M.Speed, 0.01); // 0.05
				green = 0;

			}

			/*if (S.ObstacleNoticed()) {
				
				obstacle++;

				if (obstacle % 3 == 0) {
					Print("Ostacolo " + obstacle);
					AvoidObstacle();
				} else {
					Print("Ostacolo " + obstacle);
				}

			}*/

			/*if (SILVER || S.Touch.IsPressed())
				TerminateProgram();*/

			Buttons.EscapePressed += () => {
				TerminateProgram ();
			};

		}

		public static void Rescue () {

			Thread.Sleep (2000);

			Salvataggio.RunSalvataggio ();

		}

	}
}