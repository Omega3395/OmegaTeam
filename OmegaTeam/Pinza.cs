using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoBrickFirmware;
using System.Threading;

namespace OmegaTeam
{
	public class Pinza
	{
		private bool isopen;
		private bool isclose;

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
			Motors.setSpeedPinza (-50);
			Motors.Off ();
			isopen = true;
			isclose = false;
		}

		public void chiudi() {
			Motors.setSpeedPinza (50);
			Motors.Off();
			isopen = false;
			isclose = true;
		}

		private void ruota() {
			Motors.V.SpinRight (27, 1800, true);
		}

		private void avvicinati() {
			Motors.setSpeed(-10, -10, 1.5);
			Motors.Off ();
		}

		public void afferra() {
			ruota();
			Thread.Sleep (3000);
			avvicinati();
			chiudi();
		}

		public void rilascia() {
			ruota();
			Thread.Sleep (3000);
			avvicinati();
			apri();
		}

	}
}

