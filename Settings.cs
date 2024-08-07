using System;
using UnityModManagerNet;

namespace CustomNpcPortraits
{
	public class Settings : UnityModManager.ModSettings
	{
		public bool SwarmRiddance = false;

		public bool RightverseGroupPortraits = true;


		public bool ArueHalo = false;

		public bool EmberHalo = false;

		public bool ArueHaloAdded = false;

		public bool EmberHaloAdded = false;

		public bool DollroomHalo = true;

		public bool AutoBackup = true;

		public bool AutoSecret = false;

		public bool ManageCompanions = true;

		public bool isCleaned = false;

		public bool isCleaned2 = false;


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