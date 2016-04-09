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
		/// <returns>The correction of the most dark sensor</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public static double getCorrection(sbyte sensor) {

			//return Math.Abs(WHITE[sensor] - S.getColor(sensor)) * 0.05; // Formula per calcolare la correzione di posizione

			sbyte currentColor = S.getColor(sensor);
			if (currentColor > Sensors.WHITE[sensor])
				currentColor = Sensors.WHITE[sensor];

			return Math.Pow(Math.Pow(S.getColor(sensor) - Sensors.WHITE[sensor], 2), 1.0 / 3); // y=(x-whiteval)^(2/3)
		}

		private static void print(string a) {
			
			LcdConsole.WriteLine(a);

		}

		/*private static void avoidObstacle() {

            M.turnRight(90);

            M.V.TurnLeftForward(M.Speed, 80, 1200, true).WaitOne();

        }*/
			
		public static void lineFollower() {

			bool CL = S.getState(0);
			bool CR = S.getState(1);

			bool SILVER = (S.getColor(0) >= 90 && S.getColor(1) >= 90); // Da rivisitare con LineLeader

			if (CL && CR) {

				M.Brake();

				switch (S.checkGreen()) {

					case 0:
						print("verde sx");
						S.setSensorsMode(MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.setSpeed(-8, 20, 0.8, true);
						break;
					case 1:
						print("verde dx");
						S.setSensorsMode(MonoBrickFirmware.Sensors.ColorMode.Reflection);
						M.setSpeed(20, -8, 0.8, true);
						break;
					case -1:
						print("Niente");
						S.setSensorsMode(MonoBrickFirmware.Sensors.ColorMode.Reflection);
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

			/*if (S.obstacleNoticed()) {

				print("Ostacolo!");
				avoidObstacle();

			}*/

			/*if (SILVER) {

				stop = true;

			}*/

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