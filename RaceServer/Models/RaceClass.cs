﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RaceServer.models
{
	public class RaceClass
	{
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