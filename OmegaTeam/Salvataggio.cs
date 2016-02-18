/*using System;
using System.Threading;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;

namespace OmegaTeam
{
	class Salvataggio
	{

		public Salvataggio(){
		}

		public static void Salva() {
			
			Vehicle motrs = new Vehicle(MotorPort.OutA, MotorPort.OutD);
			ManualResetEvent terminateprogram = new ManualResetEvent(false);

			Pinza P = new Pinza ();

			var gyro = new EV3GyroSensor(SensorPort.In3);
			ButtonEvents buts = new ButtonEvents();
			var sensor = new EV3IRSensor(SensorPort.In1);
			bool fine = false;
			bool stop = false;
			int t, jj, dist, adesso, wall;
			int[] distanza = new int[1000000];
			int[] verifica = new int[1000000];
			Motor motorA = new Motor(MotorPort.OutA);
			Motor motorB = new Motor(MotorPort.OutD);
			int bussola = 0;
			for (int g = 1; g < 2; g++)
			{
				t = 1;
				dist = sensor.ReadDistance();
				adesso = 1;
				Thread.Sleep(300);
				distanza[0] = sensor.ReadDistance();
				distanza[1] = sensor.ReadDistance();
				while ((!fine) && (!stop))
				{
					LcdConsole.WriteLine(" distanza[" + t + "] = " + distanza[t]);                              //inizia a spazzolare
					motrs.SpinLeft(45, 10, true);
					t++;
					distanza[t] = sensor.ReadDistance();
					if (((distanza[t] - distanza[t - 1] <= -3) || (distanza[t] - distanza[t - 2] < -5)) && (!stop))          //se la distanza si accorcia
					{
						for (int i = 2; i < 2; i++)
						{
							if ((!fine) && (!stop))
							{
								motrs.SpinLeft(45, 30, true);                                                   //prova per 6 volte a vedere se la distanza poi si riallunga
								t++;
								Thread.Sleep(200);
								distanza[t] = sensor.ReadDistance();
								LcdConsole.WriteLine("LOOP  distanza[" + t + "] = " + distanza[t]);
								if (((distanza[t] - distanza[t - 1] >= 3) && (!fine)) && (!stop))
								{
									uint turn = (uint)(((45 / 2.0) * (i - 1)) - 10);
									motrs.SpinRight(45, turn, true);
									LcdConsole.WriteLine("AVVICINAMENTO");
									Thread.Sleep(200);
									motorA.SetSpeed(30);
									motorB.SetSpeed(30);
									Thread.Sleep(500);
									motorA.Brake();
									motorB.Brake();
									motrs.SpinRight(45, 45, true);
									Thread.Sleep(300);
									verifica[0] = sensor.ReadDistance();
									LcdConsole.WriteLine("VERIFICA [" + adesso + "] = " + verifica[adesso]);
									verifica[adesso] = sensor.ReadDistance();
									adesso++;
									verifica[adesso] = sensor.ReadDistance();
									LcdConsole.WriteLine("VERIFICA [" + adesso + "] = " + verifica[adesso]);
									while ((((verifica[adesso] - verifica[adesso - 1] <= 3) || (verifica[adesso] - verifica[adesso - 2] <= 4)) && (!fine)) && (!stop))
									{
										adesso++;
										verifica[adesso] = sensor.ReadDistance();
										LcdConsole.WriteLine("VERIFICA [" + adesso + "] = " + verifica[adesso]);
										motrs.SpinLeft(45, 8, true);
										Thread.Sleep(200);
										buts.EnterPressed += () =>
										{
											fine = true;
										};
									}
									Thread.Sleep(1000);
									motrs.SpinRight(45, 26, true);
									Thread.Sleep(1000);                                                                          //appena vede che la distanza si riallunga comincia l'avvicinamento
									dist = sensor.ReadDistance();
									motorA.SetSpeed(30);
									motorB.SetSpeed(30);
									jj = verifica[adesso - 1] * 50;
									Thread.Sleep(jj);
									motorA.Brake();
									motorB.Brake();
									stop = true;
								}
							}
						}
					}
					Thread.Sleep(200);                                                                          //in qualsiasi momento si può stoppare il programma premendo il tasto enter
					buts.EnterPressed += () =>
					{
						fine = true;
					};
				}
				LcdConsole.WriteLine(" ");
				LcdConsole.WriteLine("PINZA");
				LcdConsole.WriteLine(" ");

				P.afferra ();

				Thread.Sleep(5000);
				stop = false;
				Thread.Sleep(500);
				bussola = gyro.Read();
				while ((bussola < -2)||(bussola > 2))
				{
					motrs.SpinRight(27, 20, true);
					bussola = gyro.Read();
				}
				motorA.SetSpeed(30);
				motorB.SetSpeed(30);
				wall = sensor.ReadDistance() * 50;

				P.rilascia ();

				Thread.Sleep(wall);
			}
			motorA.Brake();
			motorB.Brake();
		}
	}
}*/