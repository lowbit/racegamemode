using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Shared.models
{
	public class CheckpointClass
	{
		public Vector3 Position { get; set; }
		public Vector3 NextPosition { get; set; }
		public int Handle { get; set; }
		public int Blip { get; set; }
		public int ArrayPos { get; set; }
	}
}
