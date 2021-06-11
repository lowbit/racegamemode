using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using RaceClient.models;
using static CitizenFX.Core.Native.API;

namespace RaceClient
{
	public class RaceClient : BaseScript
	{
		public static readonly List<string> weatherTypes = new List<string>()
		{
			"EXTRASUNNY",
			"CLEAR",
			"NEUTRAL",
			"SMOG",
			"FOGGY",
			"CLOUDS",
			"OVERCAST",
			"CLEARING",
			"RAIN",
			"THUNDER",
			"BLIZZARD",
			"SNOW",
			"SNOWLIGHT",
			"XMAS",
			"HALLOWEEN"
		};
		enum state { WAITING, VOTING, RACING, CREATING }
		string currentState;
		public RaceClass currentRace = new RaceClass();
		public List<Blip> playerLocations = new List<Blip>();
		public List<Blip> checkpointBlips = new List<Blip>();
		public List<CheckpointClass> checkpoints = new List<CheckpointClass>();
		public List<SpawnpointClass> spawnpoints = new List<SpawnpointClass>();
		public List<int> objs = new List<int>();
		public CheckpointConfigClass checkpointConfig = new CheckpointConfigClass();
		public RaceClient()
		{
			Tick += OnTick;
			currentRace.Checkpoints = new List<CheckpointClass>();
			currentRace.Spawnpoints = new List<SpawnpointClass>();
			currentState = state.WAITING.ToString();
			TriggerServerEvent("getServerState");
			checkpointConfig.Radius = 15;
			checkpointConfig.Type = 1;
			checkpointConfig.Red = 240;
			checkpointConfig.Green = 100;
			checkpointConfig.Blue = 0;
			checkpointConfig.Alpha = 140;
		}
		private async Task OnTick()
		{
			UpdatePlayerBlips();
			DrawMarkers();
		}
		[EventHandler("onClientResourceStart")]
		private void OnClientResourceStart(string resourceName)
		{
			if (GetCurrentResourceName() != resourceName) return;
			RegisterKeyMapping("spawnCheckpoint", "Spawn Checkpoint", "keyboard", "");
			RegisterKeyMapping("spawnSpawnPoint", "Spawn Spawnpoint", "keyboard", "");
			RegisterKeyMapping("removeCheckpoint", "Remove Checkpoint", "keyboard", "");
			RegisterKeyMapping("removeSpawnpoint", "Remove Spawnpoint", "keyboard", "");
			//
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
			RegisterCommand("xmlload", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				if (args.Count == 0)
				{
					TriggerServerEvent("serverLoadXml", true);
				}
			}), false);
			RegisterCommand("xmlunload", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				if (args.Count == 0)
				{
					TriggerServerEvent("serverLoadXml", false);
				}
			}), false);
			RegisterCommand("stream", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				if (args.Count == 1)
				{
					TriggerServerEvent("serverStreamObjects", (string)args[0], true);
				}
			}), false);
			RegisterCommand("unstream", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				if (args.Count == 1)
				{
					TriggerServerEvent("serverStreamObjects", (string)args[0], false);
				}
			}), false);
			//
			RegisterCommand("spawnSpawnPoint", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
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
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
				TriggerServerEvent("serverSpawnCheckpoint", Game.PlayerPed.Position);
			}), false);
			RegisterCommand("removeCheckpoint", new Action<int, List<object>, string>((source, args, raw) =>
			{
				// send actual cp and pritn out handle see if  it differs from player to player ?
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
				if (currentRace == null || checkpoints == null || checkpoints.Count < 1)
				{
					SendChatMessage("There are no checkpoints to remove!", 255, 0, 0);
					return;
				}
				TriggerServerEvent("serverRemoveCheckpoint");
			}), false);
			RegisterCommand("removeSpawnpoint", new Action<int, List<object>, string>((source, args, raw) =>
			{
				// send actual cp and pritn out handle see if  it differs from player to player ?
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
				if (currentRace == null || spawnpoints == null || spawnpoints.Count < 1)
				{
					SendChatMessage("There are no spawnpoints to remove!", 255, 0, 0);
					return;
				}
				TriggerServerEvent("serverRemoveSpawnpoint");
			}), false);

			RegisterCommand("name", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
				currentRace.Name = "";
				foreach (var item in args)
				{
					currentRace.Name += (string)item + " ";
				}
				if (currentRace.Name.Length <= 3)
				{
					SendChatMessage("Race name too short", 255, 0, 0);
					currentRace.Name = "";
					return;
				}
				if (currentRace.Name.Length >= 20)
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
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
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
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
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
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
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
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
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
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
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
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
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
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}
				TriggerServerEvent("serverUnloadRace");
			}), false);
			RegisterCommand("load", new Action<int, List<object>, string>((source, args, raw) =>
			{
				if (currentState != state.CREATING.ToString())
				{
					SendChatMessage("You must be in Creating mode!", 255, 0, 0);
					return;
				}

				string name = "";
				foreach (var item in args)
				{
					name += (string)item;
				}
				if (name.Length <= 3)
				{
					SendChatMessage("Race name too short", 255, 0, 0);
					name = "";
					return;
				}
				if (name.Length >= 20)
				{
					SendChatMessage("Race name too long", 255, 0, 0);
					name = "";
					return;
				}
				TriggerServerEvent("serverLoadRace", name);
			}), false);
			RegisterCommand("carspawn", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				var model = "adder";
				if (args.Count > 0)
				{
					model = args[0].ToString();
				}
				var hash = (uint)GetHashKey(model);
				if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
				{
					TriggerEvent("chat:addMessage", new
					{
						color = new[] { 255, 0, 0 },
						args = new[] { "[CarSpawner]", $"It might have been a good thing that you tried to spawn a {model}. Who even wants their spawning to actually ^*succeed?" }
					});
					return;
				}
				var vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position, Game.PlayerPed.Heading);
				Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
				TriggerEvent("chat:addMessage", new
				{
					color = new[] { 255, 0, 0 },
					args = new[] { "[CarSpawner]", $"Woohoo! Enjoy your new ^*{model}!" }
				});
			}), false);
			RegisterCommand("vote", new Action<int, List<object>, string>((source, args, raw) =>
			{
				TriggerServerEvent("serverStateChange", state.VOTING.ToString());
			}), false);
			RegisterCommand("race", new Action<int, List<object>, string>((source, args, raw) =>
			{
				TriggerServerEvent("serverStateChange", state.RACING.ToString());
			}), false);
			RegisterCommand("create", new Action<int, List<object>, string>((source, args, raw) =>
			{
				TriggerServerEvent("serverStateChange", state.CREATING.ToString());
			}), false);
		}
		private void UpdateCreateRaceInfo()
		{
			TriggerServerEvent("serverUpdateRaceInfo", JsonConvert.SerializeObject(currentRace));
		}
		private void UpdatePlayerBlips()
		{
			foreach (Blip blip in playerLocations)
			{
				blip.Delete();
			}
			foreach (var player in Players)
			{
				if (player != Game.Player)
				{
					Blip blip = World.CreateBlip(player.Character.Position);
					blip.Sprite = BlipSprite.Player;
					blip.Name = player.Name;
					blip.Color = BlipColor.FranklinGreen;
					blip.IsFriendly = true;
					blip.Rotation = (int)player.Character.Rotation.Z;
					blip.Scale = 0.7f;
					playerLocations.Add(blip);
				}
			}
		}
		private void DrawMarkers()
		{
			if (currentState == state.CREATING.ToString())
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
		public static void SendChatMessage(string message, int r, int g, int b)
		{
			var msg = new Dictionary<string, object>
			{
				["color"] = new[] { r, g, b },
				["args"] = new[] { "[SERVER]", message }
			};

			TriggerEvent("chat:addMessage", msg);
		}
		[EventHandler("receiveMsg")]
		private void OnReceiveMsg(string msg)
		{
			SendChatMessage(msg, 255, 0, 0);
		}
		[EventHandler("getServerState")]
		private void OnGetServerState(string newState)
		{
			currentState = newState;
			SendChatMessage("state: " + currentState, 255, 0, 0);
			SendNuiMessage(JsonConvert.SerializeObject(new { type = "state", text = currentState }));
		}
		[EventHandler("clientStateChange")]
		private void OnClientStateChange(string newState)
		{
			currentState = newState;
			SendChatMessage("state: " + currentState, 255, 0, 0);
			SendNuiMessage(JsonConvert.SerializeObject(new { type = "state", text = currentState }));
			if (newState.Equals(state.CREATING.ToString()))
			{
				currentRace = new RaceClass();
				currentRace.Checkpoints = new List<CheckpointClass>();
				currentRace.Spawnpoints = new List<SpawnpointClass>();
				spawnpoints = new List<SpawnpointClass>();
				checkpoints = new List<CheckpointClass>();
				UpdateCreateRaceInfo();
			}
		}
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
			currentRace.Spawnpoints = spawnpoints;
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
		[EventHandler("clientStreamObjects")]
		private void OnClientStreamObjects(string resName, bool isStreamed)
		{
			if (isStreamed)
			{
				RequestIpl(resName);
				SendChatMessage($"Adding Stream {resName}", 255, 0, 0);
			}
			else
			{
				RemoveIpl(resName);
				SendChatMessage($"Removing Stream {resName}", 255, 0, 0);
			}
		}
		[EventHandler("clientUnloadRace")]
		private void OnClientUnloadRace()
		{
			if (string.IsNullOrWhiteSpace(currentRace.Code))
				RemoveIpl(currentRace.Code);
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
			currentRace.Checkpoints = new List<CheckpointClass>();
			currentRace.Spawnpoints = new List<SpawnpointClass>();
			checkpoints = new List<CheckpointClass>();
			spawnpoints = new List<SpawnpointClass>();
			currentRace.Name = "";
			SendNuiMessage(JsonConvert.SerializeObject(currentRace));
		}
		[EventHandler("clientLoadRace")]
		private void OnClientLoadRace(string jsonData)
		{
			OnClientUnloadRace();
			RequestIpl(currentRace.Code);
			currentRace = JsonConvert.DeserializeObject<RaceClass>(jsonData);
			checkpoints = currentRace.Checkpoints;
			spawnpoints = currentRace.Spawnpoints;
			//CitizenFX.Core.ClientScript.
			//CitizenFX.Core.World.CreateCheckpoint();
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
		[EventHandler("clientLoadXml")]
		private void OnClientLoadXml(string jsonData, bool isStreamed)
		{
			List<MapClass> mapObjects = JsonConvert.DeserializeObject<List<MapClass>>(jsonData);
			SendChatMessage("this", 255, 0, 0);
			if (isStreamed) {
				foreach (var obj in mapObjects)
				{
					RequestModel(obj.Hash);
					objs.Add(CreateObject(Convert.ToInt32(obj.Hash), obj.Position.X, obj.Position.Y, obj.Position.Z, false, false, obj.Door));
					SetEntityRotation(objs[objs.Count-1], obj.Rotation.X, obj.Rotation.Y, obj.Rotation.Z, 2, true);
					SetEntityQuaternion(objs[objs.Count - 1], obj.Quaternion.X, obj.Quaternion.Y, obj.Quaternion.Z, obj.Quaternion.W);
					FreezeEntityPosition(objs[objs.Count - 1], true);
				}
			} else {

				//DetachEntity(
				for (int i = 0; i < objs.Count; i++)
				{
					int j = objs[i];
					DeleteObject(ref j);
				}
			}
		}
	}
}
