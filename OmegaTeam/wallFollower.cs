/*
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

			while (S.GetDist () > 35 && S.GetDist (true) > 20)
				
				M.SetSpeed (M.Speed, M.Speed);

			M.Brake ();
			Thread.Sleep (100);

			if (S.GetDist () <= 35) {

				while (S.GetDist (true) >= 20) {
					M.V.SpinLeft (M.Speed);
				}

				M.Brake ();
			}

			if (S.GetDist (true) <= 20) {

				LcdConsole.WriteLine ("Inizio 2"); //Temporaneo

				bool stop = false;
				int distanza = 0;

				M.V.SpinLeft (M.Speed);

				while (!stop) {

					distanza = S.GetDist (true);
					Thread.Sleep (50);

					while (S.GetDist (true) > distanza)

						M.V.SpinRight (M.Speed);

					if (S.GetDist (true) <= distanza) {

						int minimo = S.GetDist (true);
						Thread.Sleep (50);

						while (S.GetDist (true) <= minimo) {

							minimo = S.GetDist (true);
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
*/