using System;
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
		const sbyte SPEED = 30;
		const float REVERSE_CORRECTION = 3;

		//################################################################################
		//################################################################################

		static ButtonEvents Buttons = new ButtonEvents ();

		/// <summary>
		/// Gets the default speed value.
		/// </summary>
		public static sbyte Speed { get { return SPEED; } }

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
			if (currentColor > S.WHITE [sensor]) // Se il bianco è troppo bianco, lo porto al valore massimo
				currentColor = S.WHITE [sensor];

			return Math.Pow (Math.Pow (S.WHITE [sensor] - currentColor, 6), 1.0 / 7); // Formula per calcolare la correzione di posizione
		}

		static void Print (string a) {
			LcdConsole.WriteLine (a);
		}

		/// <summary>
		/// Turn with a fixed correction given by the sensor readings.
		/// </summary>
		/// <param name="timeout">Timeout at the end of the action.</param>
		public static void Turn(double timeout = 0) {

			double correctionL = Brain.GetCorrection(0);
			double correctionR = Brain.GetCorrection(1);

			if (correctionL <= correctionR) { // Correggi il sensore che è più sul nero, caso destra

				M.motL.SetSpeed((sbyte)(SPEED + correctionR));
				M.motR.SetSpeed((sbyte)(SPEED - REVERSE_CORRECTION * correctionR));

			} else { // Caso sinistra

				M.motL.SetSpeed((sbyte)(SPEED - REVERSE_CORRECTION * correctionL));
				M.motR.SetSpeed((sbyte)(SPEED + correctionL));

			}

			Thread.Sleep((int)(timeout * 1000));

		}

		static void AvoidObstacle () { // Aggira l'ostacolo

			M.GoStraight ((sbyte)-Speed, 0.5, true);

			M.V.SpinRight (Speed, 500, true).WaitOne ();

			M.GoStraight (Speed, 0.7, true);

			M.SetSpeed ((sbyte)(Speed - 15), (sbyte)(Speed + 15));

			while (!S.GetState (0, true) && !S.GetState (1, true)) {
				Thread.Sleep (10);
			}

			M.V.SpinRight (30, 150, true).WaitOne ();

			M.GoStraight (Speed);

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

				switch (CheckGreen ()) { // Controlla se c'è il verde, confronta il risultato

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

				case -1: // Bumper
					M.Brake ();
					Print ("Niente");
					S.SetSensorsMode (ColorMode.Reflection);
					for (int i = 0; i < 10; i++)
						Turn (0.05);
					break;

				case -2: // NERO NERO
					M.Brake ();
					S.SetSensorsMode (ColorMode.Reflection);
					Print ("NERO NERO");
					M.GoStraight (Speed, 0.5);
					break;
				}

			}


			if ((CL && !CR) || (!CL && CR)) { // Bianco Nero / Nero Bianco, gira con correzione normale

				Turn (0.05); // 0.1

			}

			if (!CL && !CR) { // Bianco Bianco, vai avanti

				M.GoStraight (Speed, 0.01); // 0.05

			}

			if (S.ObstacleNoticed ()) { // Ostacolo rilevato

				Print ("Ostacolo!");
				AvoidObstacle ();

			}

			if (SILVER) { // Striscia riflettente, fine lineFollower
				Print ("SILVER");
				TerminateProgram ();
			}

			Buttons.EscapePressed += () => { // Premi tasto fine, fine lineFollower
				TerminateProgram ();
			};

		}

		/// <summary>
		/// Checks the green.
		/// </summary>
		/// <returns>Value (0: green left, 1: green right, -1: nothing, -2: Bumper).</returns>
		public static int CheckGreen() {

			S.SetSensorsMode(ColorMode.Color);
			bool greenL, greenR, bb = false, bb2 = false, bb3 = false;

			byte c1 = S.colL.Read();
			byte c2 = S.colR.Read();
            byte c3, c4;
			bb = (c1 == c2 && c1 == (byte)Color.Black);

			//New cicle function
			M.SetSpeed(Speed, Speed);
			for (int i = 0; i < 6; i++) { // Obrobrio, da riscrivere!!!
				c1 = S.colL.Read();
				c2 = S.colR.Read();

				greenL = c1 == (byte)Color.Green;
				greenR = c2 == (byte)Color.Green;

				bb2 = (c1 == c2 && c1 == (byte)Color.White);
				
				Thread.Sleep(10);

                c3 = S.colL.Read();
                c4 = S.colR.Read();

                greenL = (c1 == c3) && greenL;
                greenR = (c2 == c4) && greenR;

                if (greenL) return 0;
                if (greenR) return 1;
            }
			M.Brake();

			M.SetSpeed((sbyte)-Speed, (sbyte)-Speed);
            for (int i = 0; i < 12; i++) {
                c1 = S.colL.Read();
                c2 = S.colR.Read();

                greenL = c1 == (byte)Color.Green;
                greenR = c2 == (byte)Color.Green;

                bb3 = (c1 == c2 && c1 == (byte)Color.White);

                Thread.Sleep(10);

                c3 = S.colL.Read();
                c4 = S.colR.Read();

                greenL = (c1 == c3) && greenL;
                greenR = (c2 == c4) && greenR;

                if (greenL) return 0;
                if (greenR) return 1;
            }
            M.Brake();

			M.SetSpeed(Speed, Speed);
			for (int i = 0; i < 15; i++) {
				c1 = S.colL.Read();
				c2 = S.colR.Read();
				Thread.Sleep(10);
			}
			M.Brake();

			if (bb && bb2 && bb3) // Bumper
				return -2;
			return -1;

		}

		public static void Rescue () {

			LcdConsole.WriteLine ("Inizio Rescue");

			Thread.Sleep (1000);

			Salvataggio.RunRescue ();

		}

	}
}