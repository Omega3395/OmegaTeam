using System;
using System.Threading;

namespace OmegaTeam
{
	public class Pinza
	{
		private bool isopen;
		private bool isclose;

		private static Motors M = new Motors ();

		public Pinza () {
			
			isopen = true;
			isclose = false;

		}

		public bool isOpen() {
			
			return isopen;

		}

		public bool isClose() {
			
			return isclose;

		}

		public void apri() {
			
			if (isclose) {
				M.setCraneSpeed (-50);
				M.Off ();
				isopen = true;
				isclose = false;
			}

		}

		public void chiudi() {
			
			if (isopen) {
				
				M.setCraneSpeed (50);
				M.Off ();
				isopen = false;
				isclose = true;

			}

		}

		private void ruota() {
			
			M.V.SpinRight (27, 1800, true);

		}

		private void avvicinati() {
			
			M.setSpeed (-10, -10, 1.5);
			M.Off ();

		}

		public void afferra() {
			
			ruota ();
			Thread.Sleep (3000);
			avvicinati ();
			chiudi ();

		}

		public void rilascia() {
			
			ruota ();
			Thread.Sleep (3000);
			avvicinati ();
			apri ();

		}

	}
}