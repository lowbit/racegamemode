using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using Newtonsoft.Json;
using RaceClient.models;
using static CitizenFX.Core.Native.API;

namespace RaceClient
{
	public static class CommonClass
	{
		public static void SendChatMessage(string message, int r, int g, int b, string from = "")
		{
			var msg = new Dictionary<string, object>
			{
				["color"] = new[] { r, g, b },
				["args"] = new[] { from == "" ? "[SERVER]" : from, message }
			};

			BaseScript.TriggerEvent("chat:addMessage", msg);
		}
		public static List<MapModel> ClientLoadXml(string objectsData, bool gtaOnline = false)
		{
			List<MapModel> newMapObjects = JsonConvert.DeserializeObject<List<MapModel>>(objectsData);
			if (newMapObjects.Count == 0)
			{
				SendChatMessage("Race does not have XML file", 255, 0, 0);
			}
			else
			{
				foreach (var obj in newMapObjects)
				{
					RequestModel((uint)obj.Hash);
					obj.ObjectId = CreateObjectNoOffset((uint)obj.Hash, obj.Position.X, obj.Position.Y, obj.Position.Z, false, false, obj.Door);
					SetEntityRotation(obj.ObjectId, obj.Rotation.X, obj.Rotation.Y, obj.Rotation.Z, 2, true);
					if(!gtaOnline)
						SetEntityQuaternion(obj.ObjectId, obj.Quaternion.X, obj.Quaternion.Y, obj.Quaternion.Z, obj.Quaternion.W);
					FreezeEntityPosition(obj.ObjectId, true);
					if(obj.Color!=0)
						SetObjectTextureVariant(obj.ObjectId, obj.Color);
				}
			}
			return newMapObjects;
		}

		public static void ClientUnloadXml(ref List<MapModel> mapObjects)
		{
			if (mapObjects.Count > 0)
			{
				for (int i = 0; i < mapObjects.Count; i++)
				{
					int j = mapObjects[i].ObjectId;
					DeleteObject(ref j);
				}
				mapObjects = new List<MapModel>();
			}
		}
	}
}
