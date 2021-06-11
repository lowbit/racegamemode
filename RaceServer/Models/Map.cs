using System;
using System.Collections.Generic;
using System.Text;

namespace RaceServer.models
{
	// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
	public partial class Map
	{

		private MapMapObject[] objectsField;

		private MapRemoveFromWorld removeFromWorldField;

		private object markersField;

		private MapMetadata metadataField;

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("MapObject", IsNullable = false)]
		public MapMapObject[] Objects
		{
			get
			{
				return this.objectsField;
			}
			set
			{
				this.objectsField = value;
			}
		}

		/// <remarks/>
		public MapRemoveFromWorld RemoveFromWorld
		{
			get
			{
				return this.removeFromWorldField;
			}
			set
			{
				this.removeFromWorldField = value;
			}
		}

		/// <remarks/>
		public object Markers
		{
			get
			{
				return this.markersField;
			}
			set
			{
				this.markersField = value;
			}
		}

		/// <remarks/>
		public MapMetadata Metadata
		{
			get
			{
				return this.metadataField;
			}
			set
			{
				this.metadataField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapMapObject
	{

		private string typeField;

		private MapMapObjectPosition positionField;

		private MapMapObjectRotation rotationField;

		private double hashField;

		private bool dynamicField;

		private MapMapObjectQuaternion quaternionField;

		private bool doorField;

		/// <remarks/>
		public string Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		/// <remarks/>
		public MapMapObjectPosition Position
		{
			get
			{
				return this.positionField;
			}
			set
			{
				this.positionField = value;
			}
		}

		/// <remarks/>
		public MapMapObjectRotation Rotation
		{
			get
			{
				return this.rotationField;
			}
			set
			{
				this.rotationField = value;
			}
		}

		/// <remarks/>
		public double Hash
		{
			get
			{
				return this.hashField;
			}
			set
			{
				this.hashField = value;
			}
		}

		/// <remarks/>
		public bool Dynamic
		{
			get
			{
				return this.dynamicField;
			}
			set
			{
				this.dynamicField = value;
			}
		}

		/// <remarks/>
		public MapMapObjectQuaternion Quaternion
		{
			get
			{
				return this.quaternionField;
			}
			set
			{
				this.quaternionField = value;
			}
		}

		/// <remarks/>
		public bool Door
		{
			get
			{
				return this.doorField;
			}
			set
			{
				this.doorField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapMapObjectPosition
	{

		private decimal xField;

		private decimal yField;

		private decimal zField;

		/// <remarks/>
		public decimal X
		{
			get
			{
				return this.xField;
			}
			set
			{
				this.xField = value;
			}
		}

		/// <remarks/>
		public decimal Y
		{
			get
			{
				return this.yField;
			}
			set
			{
				this.yField = value;
			}
		}

		/// <remarks/>
		public decimal Z
		{
			get
			{
				return this.zField;
			}
			set
			{
				this.zField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapMapObjectRotation
	{

		private float xField;

		private float yField;

		private decimal zField;

		/// <remarks/>
		public float X
		{
			get
			{
				return this.xField;
			}
			set
			{
				this.xField = value;
			}
		}

		/// <remarks/>
		public float Y
		{
			get
			{
				return this.yField;
			}
			set
			{
				this.yField = value;
			}
		}

		/// <remarks/>
		public decimal Z
		{
			get
			{
				return this.zField;
			}
			set
			{
				this.zField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapMapObjectQuaternion
	{

		private float xField;

		private float yField;

		private decimal zField;

		private float wField;

		/// <remarks/>
		public float X
		{
			get
			{
				return this.xField;
			}
			set
			{
				this.xField = value;
			}
		}

		/// <remarks/>
		public float Y
		{
			get
			{
				return this.yField;
			}
			set
			{
				this.yField = value;
			}
		}

		/// <remarks/>
		public decimal Z
		{
			get
			{
				return this.zField;
			}
			set
			{
				this.zField = value;
			}
		}

		/// <remarks/>
		public float W
		{
			get
			{
				return this.wField;
			}
			set
			{
				this.wField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapRemoveFromWorld
	{

		private MapRemoveFromWorldMapObject mapObjectField;

		/// <remarks/>
		public MapRemoveFromWorldMapObject MapObject
		{
			get
			{
				return this.mapObjectField;
			}
			set
			{
				this.mapObjectField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapRemoveFromWorldMapObject
	{

		private string typeField;

		private MapRemoveFromWorldMapObjectPosition positionField;

		private MapRemoveFromWorldMapObjectRotation rotationField;

		private double hashField;

		private bool dynamicField;

		private MapRemoveFromWorldMapObjectQuaternion quaternionField;

		private bool doorField;

		private byte idField;

		/// <remarks/>
		public string Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		/// <remarks/>
		public MapRemoveFromWorldMapObjectPosition Position
		{
			get
			{
				return this.positionField;
			}
			set
			{
				this.positionField = value;
			}
		}

		/// <remarks/>
		public MapRemoveFromWorldMapObjectRotation Rotation
		{
			get
			{
				return this.rotationField;
			}
			set
			{
				this.rotationField = value;
			}
		}

		/// <remarks/>
		public double Hash
		{
			get
			{
				return this.hashField;
			}
			set
			{
				this.hashField = value;
			}
		}

		/// <remarks/>
		public bool Dynamic
		{
			get
			{
				return this.dynamicField;
			}
			set
			{
				this.dynamicField = value;
			}
		}

		/// <remarks/>
		public MapRemoveFromWorldMapObjectQuaternion Quaternion
		{
			get
			{
				return this.quaternionField;
			}
			set
			{
				this.quaternionField = value;
			}
		}

		/// <remarks/>
		public bool Door
		{
			get
			{
				return this.doorField;
			}
			set
			{
				this.doorField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public byte Id
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapRemoveFromWorldMapObjectPosition
	{

		private decimal xField;

		private decimal yField;

		private decimal zField;

		/// <remarks/>
		public decimal X
		{
			get
			{
				return this.xField;
			}
			set
			{
				this.xField = value;
			}
		}

		/// <remarks/>
		public decimal Y
		{
			get
			{
				return this.yField;
			}
			set
			{
				this.yField = value;
			}
		}

		/// <remarks/>
		public decimal Z
		{
			get
			{
				return this.zField;
			}
			set
			{
				this.zField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapRemoveFromWorldMapObjectRotation
	{

		private byte xField;

		private byte yField;

		private byte zField;

		/// <remarks/>
		public byte X
		{
			get
			{
				return this.xField;
			}
			set
			{
				this.xField = value;
			}
		}

		/// <remarks/>
		public byte Y
		{
			get
			{
				return this.yField;
			}
			set
			{
				this.yField = value;
			}
		}

		/// <remarks/>
		public byte Z
		{
			get
			{
				return this.zField;
			}
			set
			{
				this.zField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapRemoveFromWorldMapObjectQuaternion
	{

		private byte xField;

		private byte yField;

		private decimal zField;

		private decimal wField;

		/// <remarks/>
		public byte X
		{
			get
			{
				return this.xField;
			}
			set
			{
				this.xField = value;
			}
		}

		/// <remarks/>
		public byte Y
		{
			get
			{
				return this.yField;
			}
			set
			{
				this.yField = value;
			}
		}

		/// <remarks/>
		public decimal Z
		{
			get
			{
				return this.zField;
			}
			set
			{
				this.zField = value;
			}
		}

		/// <remarks/>
		public decimal W
		{
			get
			{
				return this.wField;
			}
			set
			{
				this.wField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class MapMetadata
	{

		private string creatorField;

		private string nameField;

		private object descriptionField;

		private object loadingPointField;

		private object teleportPointField;

		/// <remarks/>
		public string Creator
		{
			get
			{
				return this.creatorField;
			}
			set
			{
				this.creatorField = value;
			}
		}

		/// <remarks/>
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		/// <remarks/>
		public object Description
		{
			get
			{
				return this.descriptionField;
			}
			set
			{
				this.descriptionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
		public object LoadingPoint
		{
			get
			{
				return this.loadingPointField;
			}
			set
			{
				this.loadingPointField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
		public object TeleportPoint
		{
			get
			{
				return this.teleportPointField;
			}
			set
			{
				this.teleportPointField = value;
			}
		}
	}
}