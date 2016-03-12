using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display;

namespace OmegaTeam
{
	public class wallFollower
	{

		//################################################################################
		//################################################################################

		private const int FDIST = 35;
		private const int LDIST = 20;
		private const sbyte SPEED = 10;

		//################################################################################
		//################################################################################

		private static Sensors S = new Sensors ();
		private static Motors M = new Motors ();

		public wallFollower (){
		}

		private static void avoidSilver() {
			M.Brake ();
			Thread.Sleep (100);

			M.goFor (30, false, 0.1);  //Necessita calibrazione

			if (S.getDist (true) > FDIST)
				M.turnRight (180, 0.1);  //Necessita calibrazione
			else
				M.turnLeft (180, 0.1);
		}

		private static void primo_Posizionamento() {

			LcdConsole.WriteLine ("Inizio Posizionamento");
			LcdConsole.WriteLine ("Fase 1");

			while (S.getDist () > FDIST && S.getDist (true) > LDIST) {
				M.setSpeed (SPEED, SPEED);

				/*if (S.isSilver ()) {  //Necessita calibrazione
					avoidSilver();
				}*/
			}

			M.Brake ();
			Thread.Sleep (100);

			if (S.getDist () <= FDIST) {

				while (S.getDist (true) >= FDIST) {
					M.V.SpinLeft (SPEED);
				}

				M.Brake ();
			}

			if (S.getDist (true) <= FDIST) {

				LcdConsole.WriteLine ("Inizio 2");

				bool stop = false;
				int distanza = 0;

				M.V.SpinLeft (SPEED);

				while (!stop) {

					distanza = S.getDist (true);
					Thread.Sleep (50);

					while (S.getDist (true) > distanza)

						M.V.SpinRight (SPEED);

					if (S.getDist (true) <= distanza) {

						int minimo = S.getDist (true);
						Thread.Sleep (50);

						while (S.getDist (true) <= minimo) {

							minimo = S.getDist (true);
							Thread.Sleep (50);

						}

						stop = true;
					}
				}
			}

			M.Brake ();

			Thread.Sleep (2000);
		}

		private static void secondo_Posizionamento() {

			LcdConsole.WriteLine ("Fase 2");

			while (S.getDist () < FDIST)
				M.setSpeed (-SPEED, -SPEED);

			while (S.getDist () > FDIST) {
				M.setSpeed (SPEED, SPEED);

				/*if (S.isSilver ()) {  //Necessita calibrazione
					avoidSilver();
				}*/
			}
			
			M.Brake ();

			if (S.getDist (true) > LDIST) {
				M.turnRight (90, 0.1);  //Necessita calibrazione

				while (S.getDist () > LDIST)
					M.setSpeed (SPEED, SPEED);
				
				M.Brake ();
				Thread.Sleep (100);

				M.turnRight (270, 0.1);  //Necessita calibrazione

			}

			LcdConsole.WriteLine ("Fine posizionamento");
		}

		private static void posizionamento() {
			primo_Posizionamento ();
			secondo_Posizionamento ();
		}

		public static void run(){

			LcdConsole.WriteLine ("Inizio Wall-Follower");

			posizionamento ();

			LcdConsole.WriteLine ("Fine Wall-Follower");

		}

	}
}

