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

		private const sbyte SPEED = 10;

		//################################################################################
		//################################################################################

		public Motor motL;
		public Motor motR;
		public Motor motP;

		public Vehicle V = new Vehicle (MotorPort.OutB, MotorPort.OutD);


		public Motors () {

			motL = new Motor (MotorPort.OutB);
			motP = new Motor (MotorPort.OutC);
			motR = new Motor (MotorPort.OutD);
		
		}

		public void Brake() {

			motL.Brake ();
			motR.Brake ();

		}

		public void Off() {

			motL.Off ();
			motR.Off ();
			//motP.Off ();

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

		public void setCraneSpeed(sbyte speed, double timeout=1.5) {

			motP.SetSpeed (speed);
			Thread.Sleep ((int)(timeout * 1000));

		}

		public void goStraight(sbyte speed=SPEED, int centimeters =0, double timeout=0.1) {

			if (centimeters == 0) { // Non è stata specificata una distanza da percorrere
				
				motL.SetSpeed (speed);
				motR.SetSpeed (speed);
				Thread.Sleep ((int)(timeout * 1000));

			} else { // E' stata specificata una distanza da percorrere, da usare solo con SPEED

				double k = 19.73;
				bool l = true, r = true;

				resetTacho ();
				DateTime TIni = DateTime.Now;

				setSpeed (speed, speed);

				do {
					
					TimeSpan t = DateTime.Now - TIni;

					if (t.Seconds > 20) {
						
						Brake ();
						return;

					}
					if (motL.GetTachoCount () >= centimeters * k) {
						
						motL.Brake ();
						l = false;

					}
					if (motR.GetTachoCount () >= centimeters * k) {
						
						motR.Brake ();
						r = false;

					}

				} while(l || r);

				Thread.Sleep ((int)(timeout * 1000));
			
			}
		}

		public void turnLeft(double timeout=0) {
			
			sbyte correction = Brain.correction (0);

			if (correction > 2) {
				
				motL.SetSpeed ((sbyte)(-SPEED * correction * 1.5));
				motR.SetSpeed ((sbyte)(SPEED * correction));

			} else {
				
				motL.SetSpeed ((sbyte)(-SPEED * correction * 0.5));
				motR.SetSpeed ((sbyte)(SPEED * correction));

			}

			Thread.Sleep ((int)(timeout * 1000));

		}

		public void turnRight(double timeout=0) {

			sbyte correction = Brain.correction (1);

			if (correction > 2) {

				motL.SetSpeed ((sbyte)(SPEED * correction));
				motR.SetSpeed ((sbyte)(-SPEED * correction * 1.5));

			} else {

				motL.SetSpeed ((sbyte)(SPEED * correction));
				motR.SetSpeed ((sbyte)(-SPEED * correction * 0.5));

			}

			Thread.Sleep ((int)(timeout * 1000));

		}

		public void avoidObstacle() {

			turn (90, 0.1);

			V.TurnLeftForward (10, 60, 1000, true).WaitOne (); //1300

			turn (90, 0.1);

		}

		public void turn (int degrees, double timeout=0.1) {
			
			double k = 2.1;
			bool l = true, r = true;

			resetTacho ();

			if (degrees>0) { // Gira a destra
				
				setSpeed (SPEED, -SPEED);
				DateTime TIni = DateTime.Now;

				do {
					
					TimeSpan t = DateTime.Now - TIni;

					if (t.Seconds > 20) {
						
						Brake ();
						return;

					}

					if (motL.GetTachoCount () >= degrees * k) {
						
						motL.Brake ();
						l = false;

					}

					if (motR.GetTachoCount () <= -degrees * k) {
						
						motR.Brake ();
						r = false;

					}

				} while (l || r);

				Thread.Sleep ((int)(timeout * 1000));

			} else { // Gira a sinistra

				LcdConsole.WriteLine ("Valuses: " + l + " " + r);

				setSpeed (-SPEED, SPEED);
				DateTime TIni = DateTime.Now;

				do {
					
					TimeSpan t = DateTime.Now - TIni;

					if (t.Seconds > 20) {
						
						Brake ();
						return;

					}
						
					if (motL.GetTachoCount () <= degrees * k) {
						
						motL.Brake ();
						l = false;

					}


					if (motR.GetTachoCount () >= -degrees * k) {
						
						motR.Brake ();
						r = false;

					}
						
				} while (l || r);
					
				Thread.Sleep ((int)(timeout * 1000));

			}
		}

		//

	}
}