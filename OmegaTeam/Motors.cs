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
		private const double REVERSESPEED = -1;

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

		public static void turnLeft() {
			
			sbyte correction = Brain.correction (0);

			motL.SetSpeed ((sbyte)(10 * correction * REVERSESPEED));
			motR.SetSpeed ((sbyte)(10 * correction));

			//Motors.setSpeed ((sbyte)(SPEED * REVERSESPEED), SPEED); // Gira a sinistra di 45°
			Thread.Sleep(20);

		}

		public static void turnRight() {

			sbyte correction = Brain.correction (1);

			motL.SetSpeed ((sbyte)(10 * correction));
			motR.SetSpeed ((sbyte)(10 * correction * REVERSESPEED));

			//Motors.setSpeed (SPEED, (sbyte)(SPEED * REVERSESPEED)); // Gira a destra di 45°
			Thread.Sleep(20);

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