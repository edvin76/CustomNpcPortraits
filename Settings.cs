using System;
using UnityModManagerNet;

namespace CustomNpcPortraits
{
	public class Settings : UnityModManager.ModSettings
	{

		public bool AutoBackup = true;

		public bool AutoSecret = true;


		public Settings()
		{
		}

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			UnityModManager.ModSettings.Save<Settings>(this, modEntry);
		}
	}
}