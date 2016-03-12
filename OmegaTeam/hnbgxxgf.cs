using System.Threading;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;

namespace OmegaTeam
{
	public class hnbxxgf
	{
		public static void  hnbxxg()
		{
			//for (int j = 0; j < 6; j++) {
				if (!fine) {
					CaricaPallina ();
					buts.EnterPressed += () => {
						fine = true;
					};
				}
			//}
		}

		//inizializzazione dei sensori e dei motori

		public static Motors M = new Motors ();
		public static Pinza P = new Pinza ();
		public static int Number = 100;
		public static Vehicle v = new Vehicle(MotorPort.OutB, MotorPort.OutD);
		public static ManualResetEvent terminateprogram = new ManualResetEvent(false);
		public static ButtonEvents buts = new ButtonEvents();
		public static EV3IRSensor sensor = new EV3IRSensor(SensorPort.In3);
		public static EV3IRSensor sensorB = new EV3IRSensor(SensorPort.In4);
		public static Motor motorA = new Motor(MotorPort.OutB);
		public static Motor motorB = new Motor(MotorPort.OutD);
		public static bool fine = false;
		public static int[] Dist = new int[100];
		public static int[] Dist2 = new int[100];
		public static int[] distanza = new int[100];
		public static int[] distanza2 = new int[100];
		public static int[] tacho = new int[100];
		public static int[] diff = new int[100];
		public static int k = 0;


		//funzione principare della classe del salvataggio

		public static void CaricaPallina()
		{
			PosizionaRobot();
			Radar();
			EliminaRumori();
			Differenze();
			DIffMax ();
			Allineamento();
			M.turnRight (180);
			P.apri ();
			Thread.Sleep (800);
			Avvicinamento ();
			Thread.Sleep (800);
			P.chiudi ();
			//Conclusione();
		}

		//definizione di tutte le funzioni usate nella funzione principale

		public static void PosizionaRobot()
		{
			LcdConsole.WriteLine("POSIZIONANDO ROBOT...");
			if (!fine)
			{
				motorA.SetSpeed(45);
				motorB.SetSpeed(45);
				Thread.Sleep(4000);
				v.Brake();
				buts.EnterPressed += () => {
					fine = true;
				};
			}
		}

		public static void Radar()
		{
			motorA.ResetTacho();
			motorB.ResetTacho();
			LcdConsole.WriteLine("SCANNERIZZANDO STANZA...");
			for (int i = 0; i < Number; i++)
			{
				if (!fine)
				{
					Thread.Sleep(250);
					Dist[i] = sensor.ReadDistance();
					Dist2 [i] = sensorB.ReadDistance ();
					tacho[i] = motorB.GetTachoCount();
					LcdConsole.WriteLine("    DIST " + Dist[i]);
					v.SpinLeft(25, 25, true);
					buts.EnterPressed += () => {
						fine = true;
					};
				}
			}
		}

		public static void EliminaRumori()
		{
			for (int i = 0; i < Number - 3; i++)
			{
				if (!fine) {

					distanza [i] = (Dist [i] + Dist [i + 1] + Dist [i + 2] + Dist [i + 3]) / 4;
					distanza2 [i] = (Dist2 [i] + Dist2 [i + 1] + Dist2 [i + 2] + Dist2 [i + 3]) / 4;
					buts.EnterPressed += () => {
						fine = true;
					};
				}

			}
			distanza[Number - 3] = Dist[Number - 3];
			distanza[Number - 2] = Dist[Number - 2];
			distanza[Number - 1] = Dist[Number - 1];
			distanza2[Number - 3] = Dist2[Number - 3];
			distanza2[Number - 2] = Dist2[Number - 2];
			distanza2[Number - 1] = Dist2[Number - 1];

		}

		public static void Differenze()
		{
			LcdConsole.WriteLine("ANALIZZANDO I DATI...");
			for (int i = 1; i < Number; i++)
			{
				if (!fine) {
					diff [i] = distanza2 [i] - distanza [i];
					Thread.Sleep (10);
					LcdConsole.WriteLine ("    DIFF " + diff [i]);
					buts.EnterPressed += () => {
						fine = true;
					};
				}
			}
		}

		public static void DIffMax() {
			int max = 0;
			for (int i = 0; i < Number; i++) {
				if (diff [i] > max) {
					max = diff [i];
					k = i;
				}
			}
		}

		public static void Allineamento()
		{
			LcdConsole.WriteLine("ALLINEANDO CON L'OGGETTO...");
			while ((motorB.GetTachoCount() > tacho[k] + 18 )&&(!fine))
			{
				v.SpinRight(25, 25, true);
				Thread.Sleep (250);
				LcdConsole.WriteLine("    TACHO " + motorB.GetTachoCount() + " SU " + tacho[k]);
				buts.EnterPressed += () => {
					fine = true;
				};
			}
		}

		public static void Avvicinamento()
		{
			LcdConsole.WriteLine("AVVICINANDO ALL'OGGETTO...");
			if (!fine)
			{
				int dist = sensor.ReadDistance();
				int time = dist * 17;
				motorA.SetSpeed(-45);
				motorB.SetSpeed(-45);
				Thread.Sleep(time);
				buts.EnterPressed += () => {
					fine = true;
				};
			}
			Conclusione ();
		}

		public static void Conclusione()
		{
			motorA.Brake();
			motorB.Brake();
		}
	}
}