using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using RaceClient.models;

namespace RaceClient
{
	public class RaceModeClass : BaseScript
	{
		public RaceModel currentRace;
		public List<Blip> checkpointBlips;
		public CheckpointModel checkpoint;
		public SpawnpointModel spawnpoint;
		public List<int> objs;
		List<MapModel> mapObjects;
		public CheckpointConfigModel checkpointConfig;
		public RaceModeClass()
		{
			Tick += OnTick;
		}
		private async Task OnTick()
		{
		}
	}
}
