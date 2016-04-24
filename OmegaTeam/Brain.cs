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

		public static bool stop = false;
		static sbyte green = 0;
		static bool CheckGreen = true;

		//################################################################################
		//################################################################################

		static Sensors S = new Sensors();
		static Motors M = new Motors();

		static ButtonEvents Buttons = new ButtonEvents();

		public Brain() {
		}

        /// <summary>
        /// Terminates the program.
        /// </summary>
        static void TerminateProgram(){
            M.Brake();
            Print("Fine seguilinea");
            stop = true;
        }

		/// <summary>
		/// Gets the correction.
		/// </summary>
		/// <returns>The correction of the darkest sensor</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public static double GetCorrection(sbyte sensor) {

			//return Math.Abs(WHITE[sensor] - S.GetColor(sensor)) * 0.05; // Formula per calcolare la correzione di posizione

			sbyte currentColor = S.GetColor(sensor);
			if (currentColor > Sensors.WHITE[sensor])
				currentColor = Sensors.WHITE[sensor];

			return Math.Pow (Math.Pow (Sensors.WHITE [sensor] - currentColor, 4), 1.0 / 5);
		}

		static void Print(string a) {
			
			LcdConsole.WriteLine(a);

		}

		static void Align(bool direction) {

			Print ("Allineamento");

			if (direction) {
				if (S.GetState (0) && !S.GetState (1)) {
					M.GoStraight (M.Speed);
					while (!S.GetState (1)) {
					}
				}

				if (!S.GetState (0) && S.GetState (1)) {
					M.GoStraight (M.Speed);
					while (!S.GetState (0)) {
					}
					M.Brake ();
					M.V.SpinRight (30, 500, true).WaitOne ();
				}

				if (S.GetState (0) && S.GetState (1)) {
					M.V.SpinRight (30, 500, true).WaitOne ();
				}

				M.Brake ();
			} else {
				if (!S.GetState (0) && S.GetState (1)) {
					M.GoStraight (M.Speed);
					while (!S.GetState (0)) {
					}
				}

				if (S.GetState (0) && !S.GetState (1)) {
					M.GoStraight (M.Speed);
					while (!S.GetState (1)) {
					}
					M.Brake ();
					M.V.SpinLeft (30, 500, true).WaitOne ();
				}

				if (S.GetState (0) && S.GetState (1)) {
					M.V.SpinLeft (30, 500, true).WaitOne ();
				}
			}

			M.Brake ();

		}

		static void AvoidObstacle() {

			M.V.SpinRight (30, 500, true).WaitOne ();

			if (S.GetDist (1) > 25 * 10) { // Sorpassa l'ostacolo a destra
				M.GoFor (5);
				M.SetSpeed ((sbyte)(M.Speed - 10), (sbyte)(M.Speed + 10));
				while (!S.GetState (0) && !S.GetState (1)) {
				}

				Align (true);

			} else { // Sorpassa l'ostacolo a sinistra

				M.V.SpinLeft (30, 1000, true).WaitOne ();
				M.GoFor (5);
				M.SetSpeed ((sbyte)(M.Speed + 10), (sbyte)(M.Speed - 10));
				while (!S.GetState (0) && !S.GetState (1)) {
				}

				Align (false);

			}

			M.Brake ();

			/*
			M.V.SpinRight (30, 500, true).WaitOne ();

			if (S.GetDist (1) > 20 * 10) {

				M.V.TurnLeftForward (30, 66, 2600, false).WaitOne ();

			} else {

				M.V.SpinLeft (30, 1000, true).WaitOne ();

				M.V.TurnRightForward (30, 66, 2600, false).WaitOne ();

			}

			M.SetSpeed (20, 20);

			do {
			} while(!S.GetState (0) || !S.GetState (1));

			M.Brake();
			*/

		}

		public static void lineFollower() {

			bool CL = S.GetState(0);
			bool CR = S.GetState(1);

			bool SILVER = (S.GetColor(0) >= 70 && S.GetColor(1) >= 70); // Da rivisitare con LineLeader

			if (CL && CR) {

				M.Brake ();

				if (CheckGreen) {
					switch (S.CheckGreen ()) {

					case 0:
						Print ("Verde sinistra");
						S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.SetSpeed (-10, 30, 1, true);
						CheckGreen = false;
						break;
					case 1:
						Print ("Verde destra");
						S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.SetSpeed (30, -10, 1, true);
						CheckGreen = false;
						break;
					case -1:
						if (green < 5) { // Da confermare
							Print ("Niente");
							S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
							green++;
							M.Turn (0.2);
						} else {
							M.SetSpeed (M.Speed, M.Speed, 1.5);
							S.SetSensorsMode (MonoBrickFirmware.Sensors.ColorMode.Reflection);
						}

						break;

					}
				} else {
					M.Turn (0.2);
					CheckGreen = true;
				}
					
			}

			if ((CL && !CR) || (!CL && CR)) {
				
				M.Turn(0.1);
                green = 0;

			}

			if (!CL && !CR) {

				M.GoStraight(M.Speed, 0.1);
                green = 0;

			}

			if (S.ObstacleNoticed()) {

				Print("Ostacolo");
				AvoidObstacle();

			}

            if (SILVER||S.Touch.IsPressed ())
                TerminateProgram ();
            
			Buttons.EscapePressed += () =>{
				TerminateProgram();
			};


		}

		public static void rescue() {

		}

	}
}