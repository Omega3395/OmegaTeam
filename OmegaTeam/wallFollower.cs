using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display;

namespace OmegaTeam
{
	public class wallFollower
	{

		private static Sensors S = new Sensors ();
		private static Motors M = new Motors ();

		public wallFollower (){
		}

		private static void posizionamento() {

			LcdConsole.WriteLine ("Inizio Posizionamento");

			while (S.getDist () > 35 && S.getDist (true) > 20)
				
				M.setSpeed (M.Speed, M.Speed);

			M.Brake ();
			Thread.Sleep (100);

			if (S.getDist () <= 35) {

				while (S.getDist (true) >= 20) {
					M.V.SpinLeft (M.Speed);
				}

				M.Brake ();
			}

			if (S.getDist (true) <= 20) {

				LcdConsole.WriteLine ("Inizio 2"); //Temporaneo

				bool stop = false;
				int distanza = 0;

				M.V.SpinLeft (M.Speed);

				while (!stop) {

					distanza = S.getDist (true);
					Thread.Sleep (50);

					while (S.getDist (true) > distanza)

						M.V.SpinRight (M.Speed);

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

			LcdConsole.WriteLine ("Fine posizionamento");

			Thread.Sleep (2000);
		}

		public static void run(){

			LcdConsole.WriteLine ("Inizio Wall-Follower");

			posizionamento ();

			LcdConsole.WriteLine ("Fine Wall-Follower");

		}

	}
}

