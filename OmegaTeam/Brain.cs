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

		//################################################################################
		//################################################################################

		private static Sensors S = new Sensors();
		private static Motors M = new Motors();

		private static ButtonEvents Buttons = new ButtonEvents();

		public Brain() {
		}

		/// <summary>
		/// Gets the correction.
		/// </summary>
		/// <returns>The correction of the darkest sensor</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public static double getCorrection(sbyte sensor) {

			//return Math.Abs(WHITE[sensor] - S.getColor(sensor)) * 0.05; // Formula per calcolare la correzione di posizione

			sbyte currentColor = S.getColor(sensor);
			if (currentColor > Sensors.WHITE[sensor])
				currentColor = Sensors.WHITE[sensor];

			return Math.Pow(Math.Pow(currentColor - Sensors.WHITE[sensor], 4), 1.0 / 5); // y=(x-whiteval)^(2/3)
		}

		private static void print(string a) {
			
			LcdConsole.WriteLine(a);

		}

		private static void avoidObstacle() {

			bool bl, br;

			M.V.SpinRight(30, 500, true).WaitOne();

			if (S.Ultra.Read() > 20 * 10) {

				M.V.TurnLeftForward(30, 66, 2600, true).WaitOne();

			} else {

				M.V.SpinLeft(30, 1000, true).WaitOne();

				M.V.TurnRightForward(30, 66, 2600, true).WaitOne();

			}

			M.setSpeed(20, 20);

			do {
			} while((!(bl = S.getState(0))) || (!(br = S.getState(1))));

			M.Brake();

		}

		public static void lineFollower() {

			bool CL = S.getState(0);
			bool CR = S.getState(1);

			bool SILVER = (S.getColor(0) >= 70 && S.getColor(1) >= 70); // Da rivisitare con LineLeader

			if (CL && CR) {

				M.Brake();

				switch (S.checkGreen()) {

					case 0:
						print("Verde sinistra");
						S.setColorSensorsMode(MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.setSpeed(-10, 30, 1, true);
						break;
					case 1:
						print("Verde destra");
						S.setColorSensorsMode(MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.setSpeed(30, -10, 1, true);
						break;
					case -1:
						print("Niente");
						S.setColorSensorsMode(MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.turn(0.2);
						break;

				}

				Thread.Sleep(50);
					

			}

			if ((CL && !CR) || (!CL && CR)) {
				
				M.turn(0.1);

			}

			if (!CL && !CR) { // Da rivisitare con LineLeader

				M.goStraight(M.Speed, 0.1);

			}

			if (S.obstacleNoticed()) {

				print("Ostacolo");
				avoidObstacle();

			}

			if (SILVER) {
				
				print("Entro in modalità Salvataggio...");
				stop = true;

			}

			Buttons.EscapePressed += () => {

				M.Brake();
				stop = true;
				print("Fine seguilinea");

			};

		}

		public static void rescue() {

		}

	}
}