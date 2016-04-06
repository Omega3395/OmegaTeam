using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Movement;

using MonoBrickFirmware.Display;

namespace OmegaTeam
{
	public class Motors
	{

		//################################################################################
		//################################################################################

		private const sbyte SPEED = 15;
		private const double CENTIMETERS_CONST = 34.61;
		private const double TURN_CONST = 14;

		//################################################################################
		//################################################################################

		public Motor motL;
		public Motor motR;
		public Motor motP;

		public Vehicle V = new Vehicle(MotorPort.OutB, MotorPort.OutD);

		public Motors() {

			motL = new Motor(MotorPort.OutB);
			motP = new Motor(MotorPort.OutC);
			motR = new Motor(MotorPort.OutD);
		
		}

		/// <summary>
		/// Gets the fixed default speed value.
		/// </summary>
		public sbyte Speed { get { return SPEED; } }

		/// <summary>
		/// Brakes all motors.
		/// </summary>
		public void Brake() {

			motL.Brake();
			motR.Brake();
			motP.Brake();
		}

		/// <summary>
		/// Turns all motors off.
		/// </summary>
		public void Off() {

			motL.Off();
			motR.Off();
			motP.Off();

		}

		/// <summary>
		/// Resets the tacho.
		/// </summary>
		public void resetTacho() {

			motL.ResetTacho();
			motR.ResetTacho();
			motP.ResetTacho();

		}

		/// <summary>
		/// Sets the speed.
		/// </summary>
		/// <param name="speedLeft">Speed left.</param>
		/// <param name="speedRight">Speed right.</param>
		/// <param name="timeout">Amount of time the speed is ran for.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake.</param>
		public void setSpeed(sbyte speedLeft, sbyte speedRight, double timeout = 0, bool brake = false) {

			motL.SetSpeed(speedLeft);
			motR.SetSpeed(speedRight);

			Thread.Sleep((int)(timeout * 1000));

			if (brake)
				Brake();

		}

		/// <summary>
		/// Moves forward with fixed speed.
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="timeout">Amount of time the speed is ran for.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake.</param>
		public void goStraight(sbyte speed, double timeout = 0, bool brake = false) {

			setSpeed(speed, speed);

			Thread.Sleep((int)(timeout * 1000));

			if (brake)
				Brake();

		}

		/// <summary>
		/// Turn with a fixed correction given by the sensor readings.
		/// </summary>
		/// <param name="timeout">Timeout at the end of the action.</param>
		public void turn(double timeout = 0) {

			double correctionL = Brain.getCorrection(0);
			double correctionR = Brain.getCorrection(1);

			if (correctionL <= correctionR) {
				
				motL.SetSpeed((sbyte)(SPEED + correctionR));
				motR.SetSpeed((sbyte)(SPEED - 2 * correctionR));

			} else {
				
				motL.SetSpeed((sbyte)(SPEED - 2 * correctionL));
				motR.SetSpeed((sbyte)(SPEED + correctionL));

			}

			Thread.Sleep((int)(timeout * 1000));
		}

		/// <summary>
		/// Moves forward by a fixed amount of centimeters.
		/// </summary>
		/// <param name="centimeters">Amount of entimeters.</param>
		/// <param name="forward">If set to <c>true</c> goes forward, if set to false goes backward.</param>
		/// <param name="timeout">Timeout at the end of the action.</param>
		public void goFor(int centimeters, bool forward = true, double timeout = 0) {

			bool l = true, r = true;

			resetTacho();
			DateTime TIni = DateTime.Now;

			if (forward) {

				setSpeed(SPEED, SPEED);

				do {

					TimeSpan t = DateTime.Now - TIni;

					if (t.Seconds > 20) {

						Brake();
						return;

					}
					if (motL.GetTachoCount() >= centimeters * CENTIMETERS_CONST) {

						motL.Brake();
						l = false;

					}
					if (motR.GetTachoCount() >= centimeters * CENTIMETERS_CONST) {

						motR.Brake();
						r = false;

					}

				} while(l || r);
			} else {

				setSpeed(-SPEED, -SPEED);

				do {

					TimeSpan t = DateTime.Now - TIni;

					if (t.Seconds > 20) {

						Brake();
						return;

					}
					if (motL.GetTachoCount() <= -centimeters * CENTIMETERS_CONST) {

						motL.Brake();
						l = false;

					}
					if (motR.GetTachoCount() <= -centimeters * CENTIMETERS_CONST) {

						motR.Brake();
						r = false;

					}

				} while(l || r);
			}

			Thread.Sleep((int)(timeout * 1000));

		}
	}
}