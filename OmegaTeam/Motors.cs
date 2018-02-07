using System.Threading;
using MonoBrickFirmware.Movement;

namespace OmegaTeam {
	public class M {

		public static Motor motL = new Motor(MotorPort.OutB); // Motore sinistro
        public static Motor motR = new Motor(MotorPort.OutA); // Motore destro
        public static Motor motP = new Motor(MotorPort.OutC); // Motore pinza

        public static Vehicle V = new Vehicle(MotorPort.OutA, MotorPort.OutB); // Classe helper

		/// <summary>
		/// Brakes motors.
		/// </summary>
		public static void Brake (int timeout = 0,bool motL = true, bool motR = true, bool motP = true) {

            if (motL)
				M.motL.Brake ();
            if (motR)
				M.motR.Brake ();
            if (motP)
				M.motP.Brake ();

			Thread.Sleep (timeout);

		}

		/// <summary>
		/// Turns all motors off.
		/// </summary>
		public static void Off () {

			motL.Off ();
			motR.Off ();
			motP.Off ();

		}

		/// <summary>
		/// Resets the tacho.
		/// </summary>
		public static void ResetTacho (bool motL = true, bool motR = true, bool motP = true) {

			if (motL)
				M.motL.ResetTacho ();
			if (motR)
				M.motR.ResetTacho ();
			if (motP)
				M.motP.ResetTacho ();

		}

		/// <summary>
		/// Sets the speed.
		/// </summary>
		/// <param name="speedLeft">Speed left.</param>
		/// <param name="speedRight">Speed right.</param>
		/// <param name="timeout">Amount of time the speed is ran for.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake.</param>
		public static void SetSpeed (sbyte speedLeft, sbyte speedRight, double timeout = 0, bool brake = false) {

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
		public static void GoStraight (sbyte speed, double timeout = 0, bool brake = false) {

			SetSpeed (speed, speed);

			Thread.Sleep ((int)(timeout * 1000));

			if (brake)
				Brake ();

		}
	}
}