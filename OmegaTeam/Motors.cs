using System.Threading;
using MonoBrickFirmware.Movement;

namespace OmegaTeam {
	public class Motors {

		//################################################################################
		//################################################################################

		const sbyte SPEED = 30;
		const float REVERSE_CORRECTION = 3;

		//################################################################################
		//################################################################################

		public Motor motL;
		public Motor motR;
		public Motor motP;

		public Vehicle V;

		public Motors () {

			motL = new Motor (MotorPort.OutA);
			motR = new Motor (MotorPort.OutB);
			motP = new Motor (MotorPort.OutC);

			V = new Vehicle (MotorPort.OutA, MotorPort.OutB);

		}

		/// <summary>
		/// Gets the fixed default speed value.
		/// </summary>
		public sbyte Speed { get { return SPEED; } }

		/// <summary>
		/// Brakes all motors.
		/// </summary>
		public void Brake (int timeout = 0) {

			motL.Brake ();
			motR.Brake ();
			motP.Brake ();

			Thread.Sleep (timeout);

		}

		/// <summary>
		/// Turns all motors off.
		/// </summary>
		public void Off () {

			motL.Off ();
			motR.Off ();
			motP.Off ();

		}

		/// <summary>
		/// Resets the tacho.
		/// </summary>
		public void ResetTacho (bool motL = true, bool motR = true, bool motP = true) {

			if (motL)
				this.motL.ResetTacho ();
			if (motR)
				this.motR.ResetTacho ();
			if (motP)
				this.motP.ResetTacho ();

		}

		/// <summary>
		/// Sets the speed.
		/// </summary>
		/// <param name="speedLeft">Speed left.</param>
		/// <param name="speedRight">Speed right.</param>
		/// <param name="timeout">Amount of time the speed is ran for.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake.</param>
		public void SetSpeed (sbyte speedLeft, sbyte speedRight, double timeout = 0, bool brake = false) {

			motL.SetSpeed (speedLeft);
			motR.SetSpeed (speedRight);

			Thread.Sleep ((int)(timeout * 1000));

			if (brake)
				Brake ();

		}

		/// <summary>
		/// Moves forward with fixed speed.
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="timeout">Amount of time the speed is ran for.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake.</param>
		public void GoStraight (sbyte speed, double timeout = 0, bool brake = false) {

			SetSpeed (speed, speed);

			Thread.Sleep ((int)(timeout * 1000));

			if (brake)
				Brake ();

		}

		/// <summary>
		/// Turn with a fixed correction given by the sensor readings.
		/// </summary>
		/// <param name="timeout">Timeout at the end of the action.</param>
		public void Turn (double timeout = 0) {

			double correctionL = Brain.GetCorrection (0);
			double correctionR = Brain.GetCorrection (1);

			if (correctionL <= correctionR) {

				motL.SetSpeed ((sbyte)(SPEED + correctionR));
				motR.SetSpeed ((sbyte)(SPEED - REVERSE_CORRECTION * correctionR));

			} else {

				motL.SetSpeed ((sbyte)(SPEED - REVERSE_CORRECTION * correctionL));
				motR.SetSpeed ((sbyte)(SPEED + correctionL));

			}

			Thread.Sleep ((int)(timeout * 1000));

		}

	}
}