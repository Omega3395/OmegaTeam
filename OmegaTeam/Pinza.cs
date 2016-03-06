using System;
using System.Threading;

namespace OmegaTeam
{
	public class Pinza
	{
		
		private const int TachoP = 425;
		private bool isopen;
		private bool isclose;

		private static Motors M = new Motors ();

		public Pinza () {

			isopen = true;
			isclose = false;

		}

		private bool isOpen() {

			return isopen;

		}

		private bool isClose() {

			return isclose;

		}

		public void apri() {

			if (isclose) {
				bool p = true;

				M.resetTacho ();
				M.motB.Brake();

				M.motP.SetSpeed(-50);

				do {
					if (M.motP.GetTachoCount () <= -TachoP ) {

						M.motP.Brake ();
						p = false;

					}

				} while(p);

				isopen = true;
				isclose = false;
			}
		}

		public void chiudi() {

			if (isopen) {
				
				bool p = true;

				M.resetTacho();

				M.motB.SetSpeed(50);
				Thread.Sleep(100);
				M.Off();

				M.motP.SetSpeed(50);

				do
				{
					if (M.motP.GetTachoCount() >= TachoP)
					{

						M.motP.Brake();
						p = false;

					}

				} while (p);

				M.motB.SetSpeed(-50);
				Thread.Sleep(750);
				M.motB.Brake();

				isopen = false;
				isclose = true;

			}

		}
	}
}