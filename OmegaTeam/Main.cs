﻿using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display; // Da rimuovere
using MonoBrickFirmware.UserInput; // Da rimuovere

namespace OmegaTeam
{
	class MainClass
	{

		public static Motors M = new Motors();
		public static Sensors S = new Sensors();
		public static Brain B = new Brain();

		public static void Main (string[] args)
		{

			while (!Brain.stop) { // Quando il sensore di tatto non è premuto, esegui il programma

				B.lineFollower ();

			}

			Thread.Sleep (2000);

			Motors.Brake ();

		}
	}

}