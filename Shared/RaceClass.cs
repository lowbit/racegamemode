using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Shared.models;

namespace Shared.models
{
	public class RaceClass
	{
		public string Name { get; set; }
		public string Car { get; set; }
		public string Time { get; set; }
		public string Weather { get; set; }
		public bool Loops { get; set; }
		public int Laps { get; set; }
		public string Ymap { get; set; }
		public List<CheckpointClass> Checkpoints { get; set; }
		public List<SpawnpointClass> Spawnpoints { get; set; }
	}
}
