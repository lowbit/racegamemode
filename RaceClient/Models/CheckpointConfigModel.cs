using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceClient.models
{
	public class CheckpointConfigModel
	{
		public CheckpointConfigModel()
		{
			// Default values;
			Radius = 15;
			Type = 1;
			Red = 240;
			Green = 100;
			Blue = 0;
			Alpha = 140;
		}
		public float Radius { get; set; }
		public int Type { get; set; }
		public int Red { get; set; }
		public int Green { get; set; }
		public int Blue { get; set; }
		public int Alpha { get; set; }
	}
}
