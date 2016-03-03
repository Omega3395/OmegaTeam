using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace OmegaTeam
{
	public class Salvataggio
	{

		private static Sensors S = new Sensors ();
		private static Motors M = new Motors();
		private static ButtonEvents Buttons = new ButtonEvents();

		private static int Number,min,Diminuzione,Diminuzione2;
		private static bool  fine;
		private static int[] distanza;	
		private static int[] tacho;

		public Salvataggio () {

			Number = 100;
			min = 100;
			fine = false;
			distanza = new int[1000000];
			tacho = new int[1000000];

		}

		public void CaricaPallina() { // Funzione principale
			
			PosizionaRobot ();
			Radar ();
			AnalisiDati ();
			AnalisiDati2 ();
			Allineamento ();
			Rotazione ();
			Avvicinamento ();
			Conclusione ();

		}

		public void PosizionaRobot() {
			
			LcdConsole.WriteLine ("POSIZIONANDO ROBOT...");

			if (!fine) {
				M.setSpeed (25, 25);
				Thread.Sleep (1200);
				M.V.Brake ();
				Buttons.EnterPressed += () => {
					fine = true;
				};
			}

		}

		public void Radar() {
			
			M.resetTacho ();

			LcdConsole.WriteLine ("SCANNERIZZANDO STANZA...");

			for (int i = 0; i < Number; i++) {
				if (!fine) {
					Thread.Sleep (150);
					distanza [i] = S.getDist (true);
					tacho [i] = M.motR.GetTachoCount ();
					LcdConsole.WriteLine ("    DIST " + distanza [i]);
					M.V.SpinLeft (25, 10, true);
					Buttons.EnterPressed += () => {
						fine = true;
					};
				}
			}

		}

		public void AnalisiDati() {
			
			int diff = 0;
			LcdConsole.WriteLine ("ANALIZZANDO I DATI...");

			for (int i = 1; i < Number; i++) {
				if (distanza [i - 1] - distanza [i] > diff) {
					diff = distanza [i - 1] - distanza [i];
					LcdConsole.WriteLine ("    DIFF = " + diff);
					Diminuzione = i;
					LcdConsole.WriteLine ("    DIM = " + Diminuzione);
				}
			}

		}

		public static void AnalisiDati2() {
			for (int i = Diminuzione; i < Diminuzione + 5; i++) {
				if (distanza [i] < min) {
					min = distanza [i];
					Diminuzione2 = i;
				}
			}
		}

		public void Allineamento() {
			
			LcdConsole.WriteLine ("ALLINEANDO CON L'OGGETTO...");

			while (M.motR.GetTachoCount () > tacho [Diminuzione]) {
				M.V.SpinRight (45, 10, true);
				Thread.Sleep (5);
				LcdConsole.WriteLine ("    TACHO " + M.motR.GetTachoCount ());
			}

		}

		public static void Rotazione () {
			
			LcdConsole.WriteLine ("RUOTANDO IL ROBOT...");

			Thread.Sleep (200);
			M.V.SpinRight (45, 438, true);
			Thread.Sleep (2000);
		
		}

		public static void Avvicinamento() {
			
			LcdConsole.WriteLine ("AVVICINANDO ALL'OGGETTO...");

			if (!fine) {
				int dist = S.getDist (true);
				int time = dist * 17;
				M.setSpeed (-45, -45);
				Thread.Sleep (time);
			}

		}

		public static void Conclusione() {

			M.Brake ();

		}

	}
}