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
		private const double TURN_CONST = 5;

		//################################################################################
		//################################################################################

		public Motor motL;
		public Motor motR;
		public Motor motP;

		public Vehicle V = new Vehicle (MotorPort.OutB, MotorPort.OutD);


		public Motors () {

			motP = new Motor (MotorPort.OutC);
			motL = new Motor (MotorPort.OutB);
			motR = new Motor (MotorPort.OutD);
		
		}

		public sbyte Speed { get { return SPEED; } }

		public void Brake() {

			motL.Brake ();
			motR.Brake ();
			motP.Brake ();
		}

		public void Off() {

			motL.Off ();
			motR.Off ();
			motP.Off ();

		}

		public void resetTacho() {

			motL.ResetTacho ();
			motR.ResetTacho ();
			motP.ResetTacho ();

		}


		public void setSpeed(sbyte speedLeft,sbyte speedRight,double timeout=0,bool brake=false){

			motL.SetSpeed (speedLeft);
			motR.SetSpeed (speedRight);

			Thread.Sleep ((int)(timeout * 1000));

			if (brake)
				Brake ();

		}

		public void goStraight(sbyte speed=SPEED,double timeout=0) {
						
			motL.SetSpeed (speed);
			motR.SetSpeed (speed);

			Thread.Sleep ((int)(timeout * 1000));

		}

		public void goFor(int centimeters=0,bool forward=true,double timeout=0) {

			bool l = true, r = true;

			resetTacho ();
			DateTime TIni = DateTime.Now;

			if (forward) {
				
				setSpeed (SPEED, SPEED);

				do {

					TimeSpan t = DateTime.Now - TIni;

					if (t.Seconds > 20) {

						Brake ();
						return;

					}
					if (motL.GetTachoCount () >= centimeters * CENTIMETERS_CONST) {

						motL.Brake ();
						l = false;

					}
					if (motR.GetTachoCount () >= centimeters * CENTIMETERS_CONST) {

						motR.Brake ();
						r = false;

					}

				} while(l || r);
			} 

			else {
				
				setSpeed (-SPEED, -SPEED);

				do {

					TimeSpan t = DateTime.Now - TIni;

					if (t.Seconds > 20) {

						Brake ();
						return;

					}
					if (motL.GetTachoCount () <= -centimeters * CENTIMETERS_CONST) {

						motL.Brake ();
						l = false;

					}
					if (motR.GetTachoCount () <= -centimeters * CENTIMETERS_CONST) {

						motR.Brake ();
						r = false;

					}

				} while(l || r);
			}

			Thread.Sleep ((int)(timeout * 1000));
				
		}

		public void turnLeft(double timeout=0) {
			
			sbyte correction = Brain.correction (0);

			if (correction > 2) {
				
				motL.SetSpeed ((sbyte)(-SPEED * correction * 1.5));
				motR.SetSpeed ((sbyte)(SPEED * correction));

			} else {
				
				motL.SetSpeed ((sbyte)(-SPEED * correction * 0.8));
				motR.SetSpeed ((sbyte)(SPEED * correction));

			}

			Thread.Sleep ((int)(timeout * 1000));

		}

		public void turnLeft(int degrees,double timeout=0){

			bool l = true, r = true;

			resetTacho ();

			setSpeed (-SPEED, SPEED);
			DateTime TIni = DateTime.Now;

			do {

				TimeSpan t = DateTime.Now - TIni;

				if (t.Seconds > 20) {

					Brake ();
					return;

				}

				if (motL.GetTachoCount () <= -degrees * TURN_CONST) {
					motL.Brake ();
					l = false;
				}


				if (motR.GetTachoCount () >= degrees * TURN_CONST) {
					motR.Brake ();
					r = false;
				}

			} while (l || r);

			Thread.Sleep ((int)(timeout * 1000));

		}

		public void turnRight(double timeout=0) {

			sbyte correction = Brain.correction (1);

			if (correction > 2) {

				motL.SetSpeed ((sbyte)(SPEED * correction));
				motR.SetSpeed ((sbyte)(-SPEED * correction * 1.5));

			} else {

				motL.SetSpeed ((sbyte)(SPEED * correction));
				motR.SetSpeed ((sbyte)(-SPEED * correction * 0.8));

			}

			Thread.Sleep ((int)(timeout * 1000));

		}

		public void turnRight(int degrees,double timeout=0){

			bool l = true, r = true;

			resetTacho ();

			setSpeed (SPEED, -SPEED);
			DateTime TIni = DateTime.Now;

			do {

				TimeSpan t = DateTime.Now - TIni;

				if (t.Seconds > 20) {

					Brake ();
					return;

				}

				if (motL.GetTachoCount () >= degrees * TURN_CONST) {
					motL.Brake ();
					l = false;
				}

				if (motR.GetTachoCount () <= -degrees * TURN_CONST) {
					motR.Brake ();
					r = false;
				}

			} while (l || r);

			Thread.Sleep ((int)(timeout * 1000));

		}

		//

	}
}