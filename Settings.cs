using System;
using UnityModManagerNet;

namespace CustomNpcPortraits
{
	public class Settings : UnityModManager.ModSettings
	{

		public bool AutoBackup = true;

		public bool AutoSecret = true;

		public string enterPoint = "GrayGarrisonBasement1From1stFloor";

		public Settings()
		{
		}

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			UnityModManager.ModSettings.Save<Settings>(this, modEntry);
		}
	}
}