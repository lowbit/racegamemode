using System;
using System.Collections.Generic;
using CitizenFX.Core;
using Newtonsoft.Json;
using RaceClient.models;
using static RaceClient.CommonClass;
using static CitizenFX.Core.Native.API;
using System.Threading.Tasks;

namespace RaceClient
{
	public class CreateModeClass : BaseScript
	{
		public static readonly List<string> weatherTypes = new List<string>()
		{
			"EXTRASUNNY", "CLEAR", "NEUTRAL", "SMOG", "FOGGY", "CLOUDS", "OVERCAST", "CLEARING", "RAIN", "THUNDER", "BLIZZARD", "SNOW", "SNOWLIGHT", "XMAS", "HALLOWEEN"
		};
		public RaceClass currentRace;
		public List<Blip> checkpointBlips;
		public List<CheckpointClass> checkpoints;
		public List<SpawnpointClass> spawnpoints;
		public List<int> objs;
		List<MapClass> mapObjects;
		public CheckpointConfigClass checkpointConfig;
		public CreateModeClass()
		{
			currentRace = new RaceClass();
			checkpointConfig = new CheckpointConfigClass();
			checkpointBlips = new List<Blip>();
			checkpoints = new List<CheckpointClass>();
			spawnpoints = new List<SpawnpointClass>();
			objs = new List<int>();
			mapObjects = new List<MapClass>();
			Tick += OnTick;
		}
		[EventHandler("clientCreateState")]
		public void OnclientCreateState()
		{
			currentRace = new RaceClass();
			checkpointConfig = new CheckpointConfigClass();
			checkpointBlips = new List<Blip>();
			checkpoints = new List<CheckpointClass>();
			spawnpoints = new List<SpawnpointClass>();
			objs = new List<int>();
			mapObjects = new List<MapClass>();
			RegisterCommands();
			UpdateCreateRaceInfo();
		}
		[EventHandler("clientCreateStateUnload")]
		public void OnclientCreateStateUnload()
		{
			OnClientUnloadRace();
			checkpointConfig = new CheckpointConfigClass();
		}
		private async Task OnTick()
		{
			DrawMarkers();
		}
		#region Register commands
		public void RegisterCommands()
		{
			RegisterCommand("testcarspawn", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				var vehicle = await World.CreateVehicle("comet2", spawnpoints[0].Position, spawnpoints[0].Heading);
				Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
				FreezeEntityPosition(GetVehiclePedIsUsing(Game.PlayerPed.Handle), true);
			}), false);
			RegisterCommand("unfreeze", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				FreezeEntityPosition(GetVehiclePedIsUsing(Game.PlayerPed.Handle), false);
			}), false);
			RegisterCommand("stream", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				if (args.Count == 1)
				{
					TriggerServerEvent("serverLoadXml", (string)args[0], true);
				}
			}), false);
			RegisterCommand("unstream", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				TriggerServerEvent("serverLoadXml", "", false);
			}), false);
			RegisterCommand("spawnSpawnPoint", new Action<int, List<object>, string>((source, args, raw) =>
			{
				SpawnpointClass sp = new SpawnpointClass();
				if (Game.PlayerPed.IsInVehicle())
				{
					sp.Heading = Game.PlayerPed.CurrentVehicle.Heading;
					sp.Position = Game.PlayerPed.CurrentVehicle.Position;
				}
				else
				{
					SendChatMessage("You must be in vehicle used for race to set correct spawnpoint", 255, 0, 0);
					return;
				}
				TriggerServerEvent("serverSpawnSpawnPoint", JsonConvert.SerializeObject(sp));
			}), false);
			RegisterCommand("spawnCheckpoint", new Action<int, List<object>, string>((source, args, raw) =>
			{
				TriggerServerEvent("serverSpawnCheckpoint", Game.PlayerPed.Position);
			}), false);
			RegisterCommand("removeCheckpoint", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (currentRace == null || checkpoints == null || checkpoints.Count < 1)
				{
					SendChatMessage("There are no checkpoints to remove!", 255, 0, 0);
					return;
				}
				TriggerServerEvent("serverRemoveCheckpoint");
			}), false);
			RegisterCommand("removeSpawnpoint", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (currentRace == null || spawnpoints == null || spawnpoints.Count < 1)
				{
					SendChatMessage("There are no spawnpoints to remove!", 255, 0, 0);
					return;
				}
				TriggerServerEvent("serverRemoveSpawnpoint");
			}), false);

			RegisterCommand("name", new Action<int, List<object>, string>((source, args, raw) =>
			{
				currentRace.Name = "";
				for (int i = 0; i < args.Count; i++)
				{
					currentRace.Name += (string)args[i];
					if (i != args.Count - 2)
						currentRace.Name += " ";
				}
				if (currentRace.Name.Length <= 3)
				{
					SendChatMessage("Race name too short", 255, 0, 0);
					currentRace.Name = "";
					return;
				}
				if (currentRace.Name.Length >= 40)
				{
					SendChatMessage("Race name too long", 255, 0, 0);
					currentRace.Name = "";
					return;
				}
				currentRace.Name.Remove(currentRace.Name.Length - 1);
				UpdateCreateRaceInfo();
			}), false);
			RegisterCommand("car", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (args.Count != 1)
				{
					SendChatMessage("Invalid number of arguments!", 255, 0, 0);
					return;
				}
				var hash = (uint)GetHashKey(args[0].ToString());
				if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
				{
					SendChatMessage("Invalid car model!", 255, 0, 0);
					return;
				}
				currentRace.Car = (string)args[0];
				UpdateCreateRaceInfo();
			}), false);
			RegisterCommand("laps", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (args.Count != 1)
				{
					SendChatMessage("Invalid number of arguments!", 255, 0, 0);
					return;
				}
				string stringlaps = (string)args[0];
				int laps = int.Parse(stringlaps);
				if (laps < 0 || laps > 10)
				{
					SendChatMessage("You can set 0 - 10 laps only!", 255, 0, 0);
					SendChatMessage("Set 0 for straight tracks!", 0, 255, 0);
					SendChatMessage("Set 1 or more for round tracks!", 0, 255, 0);
					return;
				}
				currentRace.Laps = laps;
				UpdateCreateRaceInfo();
			}), false);
			RegisterCommand("code", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (args.Count != 1)
				{
					SendChatMessage("Invalid number of arguments!", 255, 0, 0);
					return;
				}
				currentRace.Code = (string)args[0];
				if (currentRace.Code.Length <= 3)
				{
					SendChatMessage("Code name too short", 255, 0, 0);
					currentRace.Code = "";
					return;
				}
				UpdateCreateRaceInfo();
			}), false);
			RegisterCommand("weather", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (args.Count != 1)
				{
					SendChatMessage("Invalid number of arguments!", 255, 0, 0);
					return;
				}
				if (!weatherTypes.Contains((string)args[0]))
				{
					string weathers = "";
					foreach (var item in weatherTypes)
					{
						weathers += item + ", ";
					}
					SendChatMessage($"Available Weathers: {weathers}", 255, 0, 0);
					return;
				}
				currentRace.Weather = (string)args[0];
				UpdateCreateRaceInfo();
			}), false);
			RegisterCommand("time", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (args.Count != 1)
				{
					SendChatMessage("Invalid number of arguments!", 255, 0, 0);
					return;
				}
				int timeArg = int.Parse((string)args[0]);
				if (timeArg < 0 || timeArg > 23)
				{
					SendChatMessage("Time can only be set from 0-23", 255, 0, 0);
					return;
				}
				currentRace.Time = (string)args[0];
				UpdateCreateRaceInfo();
			}), false);
			RegisterCommand("save", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (string.IsNullOrEmpty(currentRace.Code))
				{
					SendChatMessage("Code for Race not correctly set!", 255, 0, 0);
					return;
				}
				if (string.IsNullOrEmpty(currentRace.Car))
				{
					SendChatMessage("Car for Race not correctly set!", 255, 0, 0);
					return;
				}
				if (string.IsNullOrEmpty(currentRace.Name))
				{
					SendChatMessage("Name for Race not correctly set!", 255, 0, 0);
					return;
				}
				if (!(currentRace.Laps >= 0 && currentRace.Laps <= 10))
				{
					SendChatMessage("Laps for Race not correctly set!", 255, 0, 0);
					return;
				}
				if (checkpoints.Count < 5)
				{
					SendChatMessage("Race must have at least 5 checkpoints!", 255, 0, 0);
					return;
				}
				if (spawnpoints.Count < 5)
				{
					SendChatMessage("Race must have at least 10 spawnpoints!", 255, 0, 0);
					return;
				}
				currentRace.Checkpoints = checkpoints;
				currentRace.Spawnpoints = spawnpoints;
				TriggerServerEvent("saveRace", JsonConvert.SerializeObject(currentRace));
			}), false);
			RegisterCommand("unload", new Action<int, List<object>, string>((source, args, raw) =>
			{
				TriggerServerEvent("serverUnloadRace");
			}), false);
			RegisterCommand("load", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (args.Count != 1)
				{
					SendChatMessage("Invalid number of arguments!", 255, 0, 0);
					return;
				}
				string code = (string)args[0];
				if (code.Length <= 3)
				{
					SendChatMessage("Race code too short", 255, 0, 0);
					return;
				}
				if (code.Length >= 30)
				{
					SendChatMessage("Race code too long", 255, 0, 0);
					return;
				}
				TriggerServerEvent("serverLoadRace", code);
			}), false);
			//RegisterCommand("stream", new Action<int, List<object>, string>(async (source, args, raw) =>
			//{
			//	if (args.Count == 1)
			//	{
			//		TriggerServerEvent("serverStreamObjects", (string)args[0], true);
			//	}
			//}), false);
			//RegisterCommand("unstream", new Action<int, List<object>, string>(async (source, args, raw) =>
			//{
			//	if (args.Count == 1)
			//	{
			//		TriggerServerEvent("serverStreamObjects", (string)args[0], false);
			//	}
			//}), false);
			//
		}
		#endregion
		#region Eventhandlers
		[EventHandler("clientSpawnCheckpoint")]
		private void OnClientSpawnCheckpoint(Vector3 pos)
		{
			SendChatMessage("Checkpoint at X: " + pos.X + "Y: " + pos.Y + "Z: " + pos.Z, 255, 0, 0);
			AddCheckpoint(pos.X, pos.Y, pos.Z, 0, 0, 0);
		}
		[EventHandler("clientSpawnSpawnPoint")]
		private void OnClientSpawnSpawnPoint(string jsonData)
		{
			SpawnpointClass sp = JsonConvert.DeserializeObject<SpawnpointClass>(jsonData);
			SendChatMessage("Spawnpoint at X: " + sp.Position.X + "Y: " + sp.Position.Y + "Z: " + sp.Position.Z, 0, 255, 0);
			spawnpoints.Add(sp);
		}
		[EventHandler("moveNextSpawnpoint")]
		private void OnMoveNextSpawnpoint()
		{
			Vector3 newPos = GetObjectOffsetFromCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z,
				Game.PlayerPed.Heading, spawnpoints.Count % 2 == 0 ? -3 : 3, -5, 0);
			if (Game.PlayerPed.IsInVehicle())
			{
				Game.PlayerPed.CurrentVehicle.Position = newPos;
			}
			else
			{
				Game.PlayerPed.Position = newPos;
			}
		}

		[EventHandler("clientRemoveCheckpoint")]
		private void OnClientRemoveCheckpoint()
		{
			CheckpointClass currentCp = checkpoints[checkpoints.Count - 1];
			foreach (Blip b in checkpointBlips)
			{
				if (b.Handle == currentCp.Blip)
					b.Delete();
			}
			DeleteCheckpoint(currentCp.Handle);
			checkpoints.Remove(currentCp);
			currentRace.Checkpoints = checkpoints;
		}
		[EventHandler("clientRemoveSpawnpoint")]
		private void OnClientRemoveSpawnpoint()
		{
			SpawnpointClass currentSp = spawnpoints[spawnpoints.Count - 1];
			spawnpoints.Remove(currentSp);
			currentRace.Spawnpoints = spawnpoints;
		}
		[EventHandler("clientAfterSaveRace")]
		private void OnClientAfterSaveRace()
		{
			SendChatMessage("Race has been saved", 0, 255, 0);
		}
		[EventHandler("clientUpdateRaceInfo")]
		private void OnClientUpdateRaceInfo(string jsonData)
		{
			SendNuiMessage(jsonData);
		}
		//[EventHandler("clientStreamObjects")]
		//private void OnClientStreamObjects(string resName, bool isStreamed)
		//{
		//	if (isStreamed)
		//	{
		//		RequestIpl(resName);
		//		SendChatMessage($"Adding Stream {resName}", 255, 0, 0);
		//	}
		//	else
		//	{
		//		RemoveIpl(resName);
		//		SendChatMessage($"Removing Stream {resName}", 255, 0, 0);
		//	}
		//}
		[EventHandler("clientUnloadRace")]
		private void OnClientUnloadRace()
		{
			if (checkpointBlips != null && checkpointBlips.Count > 0)
			{
				foreach (Blip b in checkpointBlips)
				{
					b.Delete();
				}
				checkpointBlips.Clear();
			}
			if (currentRace != null && checkpoints != null && checkpoints.Count > 0)
			{
				foreach (var item in checkpoints)
				{
					DeleteCheckpoint(item.Handle);
				}
			}
			currentRace = new RaceClass();
			checkpoints = new List<CheckpointClass>();
			spawnpoints = new List<SpawnpointClass>();
			SendNuiMessage(JsonConvert.SerializeObject(currentRace));
			ClientUnloadXml();
		}
		[EventHandler("clientLoadRace")]
		private void OnClientLoadRace(string jsonData, string objectsData)
		{
			OnClientUnloadRace();
			if (jsonData != "")
			{
				currentRace = JsonConvert.DeserializeObject<RaceClass>(jsonData);
				checkpoints = currentRace.Checkpoints;
				spawnpoints = currentRace.Spawnpoints;
				for (int i = 0; i < checkpoints.Count; i++)
				{
					// Handle "Point to" Logic
					if (i == checkpoints.Count - 1 && currentRace.Laps > 0)
					{
						checkpoints[i].NextPosition = checkpoints[0].Position;
						checkpoints[i].Handle = CreateCheckpoint(4, checkpoints[i].Position.X, checkpoints[i].Position.Y, checkpoints[i].Position.Z - 10, checkpoints[i].NextPosition.X, checkpoints[i].NextPosition.Y, checkpoints[i].NextPosition.Z, checkpointConfig.Radius, checkpointConfig.Red, checkpointConfig.Green, checkpointConfig.Blue, checkpointConfig.Alpha, 0);
					}
					else
						checkpoints[i].Handle = CreateCheckpoint(checkpointConfig.Type, checkpoints[i].Position.X, checkpoints[i].Position.Y, checkpoints[i].Position.Z - 10, checkpoints[i].NextPosition.X, checkpoints[i].NextPosition.Y, checkpoints[i].NextPosition.Z, checkpointConfig.Radius, checkpointConfig.Red, checkpointConfig.Green, checkpointConfig.Blue, checkpointConfig.Alpha, 0);

					Blip blip = World.CreateBlip(checkpoints[i].Position);
					blip.Sprite = BlipSprite.Standard;
					blip.Name = "Race Blip";
					blip.Color = BlipColor.Blue;
					blip.IsFriendly = true;
					blip.NumberLabel = i + 1;
					checkpoints[i].Blip = blip.Handle;
					checkpointBlips.Add(blip);
				}
				if (!string.IsNullOrEmpty(currentRace.Weather))
					SetWeatherTypeOvertimePersist(currentRace.Weather, 6);

				if (!string.IsNullOrEmpty(currentRace.Time))
					NetworkOverrideClockTime(int.Parse(currentRace.Time), 00, 00);
				SendNuiMessage(JsonConvert.SerializeObject(currentRace));
			}
			else
			{
				SendChatMessage("Race does not exist", 255, 0, 0);
			}
			ClientLoadXml(objectsData);
		}
		private void ClientLoadXml(string objectsData)
		{
			mapObjects = JsonConvert.DeserializeObject<List<MapClass>>(objectsData);
			if (mapObjects.Count == 0)
			{
				SendChatMessage("Race does not have XML file", 255, 0, 0);
			}
			else
			{
				foreach (var obj in mapObjects)
				{
					RequestModel((uint)obj.Hash);
					objs.Add(CreateObjectNoOffset((uint)obj.Hash, obj.Position.X, obj.Position.Y, obj.Position.Z, false, false, obj.Door));
					SetEntityRotation(objs[objs.Count - 1], obj.Rotation.X, obj.Rotation.Y, obj.Rotation.Z, 2, true);
					SetEntityQuaternion(objs[objs.Count - 1], obj.Quaternion.X, obj.Quaternion.Y, obj.Quaternion.Z, obj.Quaternion.W);
					FreezeEntityPosition(objs[objs.Count - 1], true);
				}
			}
		}
		private void ClientUnloadXml()
		{
			if (mapObjects.Count > 0)
			{
				for (int i = 0; i < objs.Count; i++)
				{
					int j = objs[i];
					DeleteObject(ref j);
				}
				mapObjects = new List<MapClass>();
				objs = new List<int>();
			}
		}
		#endregion
		private void AddCheckpoint(float posX1, float posY1, float posZ1, float posX2, float posY2, float posZ2)
		{
			var cp = new CheckpointClass();
			cp.ArrayPos = checkpoints.Count;
			cp.Position = new Vector3(posX1, posY1, posZ1);
			cp.NextPosition = new Vector3(posX2, posY2, posZ2);
			cp.Handle = CreateCheckpoint(checkpointConfig.Type, cp.Position.X, cp.Position.Y, cp.Position.Z - 10, cp.NextPosition.X, cp.NextPosition.Y, cp.NextPosition.Z, checkpointConfig.Radius, checkpointConfig.Red, checkpointConfig.Green, checkpointConfig.Blue, checkpointConfig.Alpha, 0);
			if (checkpoints.Count >= 1)
			{
				checkpoints[checkpoints.Count - 1].NextPosition = cp.Position;
			}
			Blip blip = World.CreateBlip(cp.Position);
			blip.Sprite = BlipSprite.Standard;
			blip.Name = "Race Blip";
			blip.Color = BlipColor.Blue;
			blip.IsFriendly = true;
			blip.NumberLabel = checkpoints.Count + 1;
			cp.Blip = blip.Handle;
			checkpointBlips.Add(blip);
			checkpoints.Add(cp);
			currentRace.Checkpoints = checkpoints;
		}
		private void UpdateCreateRaceInfo()
		{
			TriggerServerEvent("serverUpdateRaceInfo", JsonConvert.SerializeObject(currentRace));
		}

		private void DrawMarkers()
		{
			if (currentRace != null && spawnpoints != null && spawnpoints.Count > 0)
			{
				foreach (var sp in spawnpoints)
				{
					DrawMarker(1, sp.Position.X, sp.Position.Y, sp.Position.Z - 1, 0, 0, 0, 0, 0, 0, 2, 2, 2, 255, 100, 0, 200, false, false, 2, false, null, null, false);
				}
			}
		}
	}
}
