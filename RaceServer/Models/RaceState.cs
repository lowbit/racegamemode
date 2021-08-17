using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RaceServer.models
{
	public class RaceState
	{
		public RaceState()
		{
			RaceModel = new RaceModel();
		}
		public RaceModel RaceModel { get; set; }
		public List<Player> Racers { get; set; }
		public List<CheckpointEnter> CheckpointEnter {get; set;}
	}
}
