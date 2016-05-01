using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.Display.Dialogs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace OmegaTeam
{
	class Salvataggio
	{

		public static int Valore = 1;
		public static Vehicle V = new Vehicle(MotorPort.OutA, MotorPort.OutB);
		public static ManualResetEvent terminateprogram = new ManualResetEvent(false);
		public static ButtonEvents buts = new ButtonEvents();
		public static bool fine;
		public static MSDistanceSensor sensorA = new MSDistanceSensor(SensorPort.In1);
		public static MSDistanceSensor sensorB = new MSDistanceSensor(SensorPort.In2);
		public static EV3TouchSensor TouchSensor = new EV3TouchSensor(SensorPort.In3);
		public static Motor motorA = new Motor(MotorPort.OutA);
		public static Motor motorB = new Motor(MotorPort.OutB);
		public static Motor motorC = new Motor(MotorPort.OutC);
		public static bool founded = false;
		public static int[] Dist = new int[400];
		public static int[] Dist2 = new int[400];
		public static int[] Diff = new int[400];
		public static int[] Differenza = new int[400];
		public static int[] Tacho = new int[400];
		public static int Distanza;
		public static int Distanza2;
		public static int i = 0;
		public static int sinc = 0;

		public static void RunSalvataggio() {

			motorA.ResetTacho();
			motorB.ResetTacho();
			PosizionaRobot();

			while (sinc == 0) {

				salva();

				if (!fine) {

					motorA.SetSpeed(35);
					motorB.SetSpeed(35);
					Thread.Sleep(2351);
					V.Brake();

					V.SpinLeft(20, 535, true);
					Thread.Sleep(3000);


				} else {

					fine = false;

					buts.EscapePressed += () => {
						sinc = 1;
						fine = true;
					};

					motorA.ResetTacho();
					motorB.ResetTacho();

					PosizionaRobot();
					RunRescue();

				}

			}


		}

		public static void salva() {

			sensorA.PowerOn();
			sensorB.PowerOn();
			Thread.Sleep(500);

			RunRescue();

			//if (fine) {

			//	fine = false;
			//	RunRescue ();

			//}

		}


		public static void RunRescue() {
			
			if (!fine) {
				Caricapallina();
			}
			if (!fine) {
				Allinea();
			}
			if (!fine) {
				WallFollower();
			}
			if (!fine) {
				Conclusione();
			}

		}

		public static void Caricapallina() {
			
			if (!fine) {
				ScannerizzaStanza();
			}
			if (!fine) {
				DiffMassima();
			}
			if (!fine) {
				All();
			}
			if (!fine) {
				Ruota180();
			}
			if (!fine) {
				GoaBit(true);
			}
			if (!fine) {
				ApriPinza(10000);
			}
			if (!fine) {
				GoaBit(false);
			}
			if (!fine) {
				Avanza();
			}
			if (!fine) {
				ChiudiPinza(10250);
			}

		}

		public static void Allinea() {

			All2();
			Sfondamento(5000);

		}

		public static void PosizionaRobot() {

			Thread.Sleep(300);

			if (!fine) {

				LcdConsole.WriteLine("POSIZIONANDO ROBOT...");
				Line();
				V.TurnRightForward(40, 9, 2300, true);							//edit1
				Thread.Sleep(7000);
				buts.EnterPressed += () => {
					fine = true;
				};

				Thread.Sleep(3000);

			}

			Thread.Sleep(300);
		}

		public static void GoaBit(bool a) {

			if (a) {

				motorA.SetSpeed(30);
				motorB.SetSpeed(30);
				Thread.Sleep(1000);
				V.Brake();

			} else {

				motorA.SetSpeed(-30);
				motorB.SetSpeed(-30);
				Thread.Sleep(1000);
				V.Brake();

			}

		}

		public static void ScannerizzaStanza() {

			if (!fine) {
				LcdConsole.WriteLine("SCANNERIZZANDO STANZA...");
				Line();

				Thread.Sleep(200);

				for (i = 0; i < 70; i++) {

					if (!fine) {

						Thread.Sleep(60);

						//if ((sensorA.GetDistance() < 700) {

						Dist[i] = sensorA.GetDistance();
						Dist2[i] = sensorB.GetDistance();

						//} else {

						//	Dist[i] = 500;
						//	Dist2[i] = 500;

						//}

						GetDistanza();
						V.SpinRight(17, 21, true);											//edit2
						Thread.Sleep(150);
						Differenza[i] = Distanza - Distanza2;
						GetDifferenza();
						Tacho[i] = motorA.GetTachoCount();
						LcdConsole.WriteLine("    " + Distanza + "    " + Distanza2 + "    " + Diff[i]);	

						buts.EnterPressed += () => {
							fine = true;
						};

					}

				}

				Thread.Sleep(200);

				Line();
				Thread.Sleep(500);
				buts.EnterPressed += () => {
					fine = true;
				};
			}
		}


		public static void GetDifferenza() {

			switch (i) {

				case 0:
					Diff[i] = Differenza[0];
					break;

				case 1:

					Diff[i] = (Differenza[0] + Differenza[1]) / 2;
					break;

				default:

					Diff[i] = (Differenza[i - 2] + Differenza[i - 1] + Differenza[i]) / 3;
					break;

			}
		}


		public static void GetDistanza() {

			switch (i) {

				case 0:
					Distanza = Dist[0];
					Distanza2 = Dist2[0];
					break;

				case 1:

					Distanza = (Dist[0] + Dist[1]) / 2;
					Distanza2 = (Dist2[0] + Dist2[1]) / 2;
					break;

				default:

					Distanza = (Dist[i - 2] + Dist[i - 1] + Dist[i]) / 3;
					Distanza2 = (Dist2[i - 2] + Dist2[i - 1] + Dist2[i]) / 3;
					break;

			}

		}

		public static void Avanza() {

			if (!fine) {

				LcdConsole.WriteLine("AVANZANDO...");
				Line();
				int time = (Distanza * 19) / 8;
				motorA.SetSpeed(-38);
				motorB.SetSpeed(-38);
				Thread.Sleep(time);
				V.Brake();

				buts.EnterPressed += () => {
					fine = true;
				};

			}
		}

		public static void Conclusione() {

			ChiudiPinza(4100);

			motorA.SetSpeed(35);
			motorB.SetSpeed(35);
			Thread.Sleep(655);
			V.SpinLeft(20, 450, true);
			Thread.Sleep(3000);
			V.Brake();


		}

		public static void Line() {
			LcdConsole.WriteLine("");
		}

		public static void DiffMassima() {
			if (!fine) {
				int max = 0;
				for (int f = 0; f < 70; f++) {
					if (Diff[f] > max) {
						max = Diff[f];
						Valore = f;
					}
				}

				LcdConsole.WriteLine("    " + max + "    " + Valore + "    " + Tacho[Valore]);

				Thread.Sleep(1000);
			}
				
		}

		public static void All() {

			if (!fine) {

				Thread.Sleep(10);
				LcdConsole.WriteLine("ALLINEANDO CON L'OGGETTO...");
				Line();
				LcdConsole.WriteLine("    " + motorA.GetTachoCount() + " su " + Tacho[Valore]);

				while ((motorA.GetTachoCount() > Tacho[Valore]) && (!fine)) {												//edit2,5

					V.SpinLeft(14, 25, true);																			//edit3
					LcdConsole.WriteLine("    " + motorA.GetTachoCount() + " su " + Tacho[Valore]);
					Thread.Sleep(150);

					buts.EnterPressed += () => {
						fine = true;
					};

				}

				Thread.Sleep(50);

				buts.EnterPressed += () => {
					fine = true;
				};

			}

		}

		public static void Ruota180() {

			if (!fine) {

				Thread.Sleep(10);
				LcdConsole.WriteLine("RUOTANDO...");
				Line();
				V.SpinLeft(20, 1152, true);
				Thread.Sleep(6000);

				buts.EnterPressed += () => {
					fine = true;
				};

			}
		}

		public static void ApriPinza(int num) {

			if (!fine) {

				LcdConsole.WriteLine("APRENDO PINZA...");
				Line();
				motorC.SetSpeed(-90);
				Thread.Sleep((num / 2));
				motorC.Brake();

				buts.EnterPressed += () => {
					fine = true;
				};

			}

		}

		public static void ChiudiPinza(int num1) {

			if (!fine) {

				LcdConsole.WriteLine("CHIUDENDO PINZA...");
				Line();
				motorC.SetSpeed(90);
				Thread.Sleep((num1 / 2));
				motorC.Brake();

				buts.EnterPressed += () => {
					fine = true;
				};

			}
		}

		public static void All2() {

			if (!fine) {

				Thread.Sleep(100);
				LcdConsole.WriteLine("ALLINEANDO APPROSSIMATIVAMENTE...");
				Line();
				V.SpinRight(20, 387, true);
				Thread.Sleep(2500);

				while (motorA.GetTachoCount() > motorB.GetTachoCount() + 80) {
					V.SpinLeft(18, 40, true);
					Thread.Sleep(150);
				}
				while (motorA.GetTachoCount() < motorB.GetTachoCount() + 80) {
					V.SpinRight(18, 40, true);
					Thread.Sleep(150);
				}

				V.SpinLeft(20, 576, true);																	//edit4
				Thread.Sleep(3000);

				buts.EnterPressed += () => {
					fine = true;
				};

			}

		}

		public static void Sfondamento(int tempo) {

			if (!fine) {

				LcdConsole.WriteLine("SFONDANDANDO LA PARETE...");
				Line();

				motorA.SetSpeed(40);
				motorB.SetSpeed(40);
				Thread.Sleep(tempo);
				motorA.SetSpeed(-35);
				motorB.SetSpeed(-35);
				Thread.Sleep(1000);
				V.Brake();
				V.SpinRight(20, 578, true);														//edit5
				Thread.Sleep(3500);

				buts.EnterPressed += () => {
					fine = true;
				};

			}

		}

		public static void WallFollower() {
			if (!fine) {

				LcdConsole.WriteLine("SEGUENDO IL MURO...");
				Line();
				SeguiMuro();

			}

		}

		public static void SeguiMuro() {

			if (!fine) {

				//int g = 20;

				while (((sensorA.GetDistance() > 117) && (!TouchSensor.IsPressed())) && (sensorB.GetDistance() > 117)) {

					//int correction = (sensorB.ReadDistance() - g) / 2;
					//LcdConsole.WriteLine ("    " + sensorB.ReadDistance());
					//Line ();
					//int speedA = 20 + correction;
					//int speedB = 20 - correction;
					//motorA.SetSpeed ((sbyte)speedA);
					//motorB.SetSpeed ((sbyte)speedB);
					//Thread.Sleep (45);

					int speedA = 45;
					int speedB = 45;
					motorA.SetSpeed((sbyte)speedA);
					motorB.SetSpeed((sbyte)speedB);
					Thread.Sleep(45);


				}

				buts.EnterPressed += () => {
					fine = true;
				};

				FineMuro();

				buts.EnterPressed += () => {
					fine = true;
				};

			}

		}

		public static void FineMuro() {

			if (!fine) {

				if ((TouchSensor.IsPressed()) && (!fine)) {

					Line();
					LcdConsole.WriteLine("TROVATO ANGOLO...");
					Line();

					motorA.SetSpeed(-45);
					motorB.SetSpeed(-45);
					Thread.Sleep(417);
					V.Brake();
					V.SpinLeft(20, 1440, true);										//edit6												
					Thread.Sleep(6500);
					ApriPinza(4000);
					Thread.Sleep(2000);

					buts.EnterPressed += () => {
						fine = true;
					};

				} else {

					if (!fine) {

						Line();
						LcdConsole.WriteLine("FINE MURO...");
						Line();

						Sfondamento(870);
						SeguiMuro();

						buts.EnterPressed += () => {
							fine = true;
						};

					}

				}

			}

		}

	}

}