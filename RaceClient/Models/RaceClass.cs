using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RaceClient.models
{
	public class RaceClass
	{
		public RaceClass()
		{
			Name = "";
			Code = "";
			Car = "";
			Laps = 0;
			Time = "";
			Weather = "";
			Checkpoints = new List<CheckpointClass>();
			Spawnpoints = new List<SpawnpointClass>();
		}
		public string Name { get; set; }
		public string Code { get; set; }
		public string Car { get; set; }
		// 0 Laps - Straight, 1+ Laps - Round
		public int Laps { get; set; }
		public string Time { get; set; }
		public string Weather { get; set; }
		public List<CheckpointClass> Checkpoints { get; set; }
		public List<SpawnpointClass> Spawnpoints { get; set; }
	}
}
