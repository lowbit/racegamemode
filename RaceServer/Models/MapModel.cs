using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceServer.models
{
	public class MapModel
	{
		public MapModel()
		{
			Position = new CoordinatePosition();
			Rotation = new CoordinateRotation();
			Quaternion = new Quaternion();
		}
		public double Hash { get; set; }
		public int Color { get; set; }
		public bool Door { get; set; }
		public bool Dynamic { get; set; }
		public CoordinatePosition Position { get; set; }
		public CoordinateRotation Rotation { get; set; }
		public Quaternion Quaternion { get; set; }
	}
	public class CoordinatePosition
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
	}
	public class CoordinateRotation
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
	}
	public class Quaternion
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float W { get; set; }
	}
}
