using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RaceServer.models
{
	public class CheckpointEnter
	{
		public CheckpointEnter()
		{
		}
		public int ChekcpointNumber { get; set; }
		public DateTime Time { get; set; }
		public Player Racer { get; set; }
}
}
