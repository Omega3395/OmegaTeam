using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Movement;

namespace OmegaTeam
{
	public class Motors
	{

		//################################################################################
		//################################################################################

		private const sbyte SPEED = 10;

		//################################################################################
		//################################################################################

		public static Motor motL;
		public static Motor motR;
		public static Motor motP;

		public static Vehicle V = new Vehicle (MotorPort.OutB, MotorPort.OutD);
		public static WaitHandle waitHandle;

		public Motors () {

			motL = new Motor (MotorPort.OutB);
			motP = new Motor (MotorPort.OutC);
			motR = new Motor (MotorPort.OutD);
		
		}

		public static void Brake() {

			motL.Brake (); // Frena i due motori
			motR.Brake ();

		}

		public static void Off() {

			motL.Off ();
			motR.Off ();

		}

		public static void setSpeed(sbyte speedLeft,sbyte speedRight,double timeout=0){

			motL.SetSpeed (speedLeft);
			motR.SetSpeed (speedRight);
			Thread.Sleep ((int)(timeout * 1000));

		}

		public static void goStraight(sbyte speed=SPEED, double timeout=0.1) {

			motL.SetSpeed (speed);
			motR.SetSpeed (speed);
			Thread.Sleep ((int)(timeout * 1000));

		}

		public static void turnLeft(double timeout=0) {
			
			sbyte correction = Brain.correction (0);

			if (correction > 2) {
				
				motL.SetSpeed ((sbyte)(-15 * correction));
				motR.SetSpeed ((sbyte)(10 * correction));

			} else {
				
				motL.SetSpeed ((sbyte)(-10 * correction * 0.5));
				motR.SetSpeed ((sbyte)(10 * correction));

			}

			Thread.Sleep ((int)(timeout * 1000));

		}

		public static void turnRight(double timeout=0) {

			sbyte correction = Brain.correction (1);

			if (correction > 2) {

				motL.SetSpeed ((sbyte)(10 * correction));
				motR.SetSpeed ((sbyte)(-15 * correction));

			} else {

				motL.SetSpeed ((sbyte)(10 * correction));
				motR.SetSpeed ((sbyte)(-10 * correction * 0.5));

			}

			Thread.Sleep ((int)(timeout * 1000));

		}

		public static void avoidObstacle() {

			Motors.setSpeed (10, -10, 1);
			Motors.Brake ();

			Motors.setSpeed (10, 10, 3);
			Motors.Brake ();

			Motors.setSpeed (-10, 10, 1);
			Motors.Brake ();

			Motors.setSpeed (10, 10, 3);
			Motors.Brake ();

			Motors.setSpeed (-10, 10, 1);
			Motors.Brake ();

			Motors.setSpeed (10, 10, 3);
			Motors.Brake ();

			Motors.setSpeed (10, -10, 1);
			Motors.Brake ();

		}



	}
}