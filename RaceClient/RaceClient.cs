using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using RaceClient.models;
using static CitizenFX.Core.Native.API;
using static RaceClient.CommonClass;

namespace RaceClient
{
	public class RaceClient : BaseScript
	{
		enum state { CREATING, VOTING, RACING, WAITING }
		string currentState;
		public List<Blip> playerLocations = new List<Blip>();
		public RaceClient()
		{
			Tick += OnTick;
			currentState = state.WAITING.ToString();
			TriggerServerEvent("getServerState");
		}
		private async Task OnTick()
		{
			UpdatePlayerBlips();
		}
		[EventHandler("onClientResourceStart")]
		private void OnClientResourceStart(string resourceName)
		{
			if (GetCurrentResourceName() != resourceName) return;
			RegisterKeyMappings();
			RegisterCommands();
		}
		private void RegisterKeyMappings()
		{
			RegisterKeyMapping("spawnCheckpoint", "Spawn Checkpoint", "keyboard", "");
			RegisterKeyMapping("spawnSpawnPoint", "Spawn Spawnpoint", "keyboard", "");
			RegisterKeyMapping("removeCheckpoint", "Remove Checkpoint", "keyboard", "");
			RegisterKeyMapping("removeSpawnpoint", "Remove Spawnpoint", "keyboard", "");
		}
		private void RegisterCommands()
		{
			RegisterCommand("carspawn", new Action<int, List<object>, string>(async (source, args, raw) =>
			{
				var model = "comet2";
				if (args.Count > 0)
				{
					model = args[0].ToString();
				}
				var hash = (uint)GetHashKey(model);
				if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
				{
					SendChatMessage($"{model} does not exist", 255, 0, 0, "[CarSpawner]");
					return;
				}
				var vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position, Game.PlayerPed.Heading);
				Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
				SendChatMessage($"Enjoy your new ^*{model}!", 255, 0, 0, "[CarSpawner]");
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
			RegisterCommand("wait2", new Action<int, List<object>, string>((source, args, raw) =>
			{
				TriggerServerEvent("serverStateChange", state.WAITING.ToString());
			}), false);
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
			if (newState != currentState)
			{
				if (currentState == state.CREATING.ToString())
				{
					TriggerEvent("clientCreateStateUnload");
				}
				else if (currentState == state.RACING.ToString())
				{

				}
				else if (currentState == state.VOTING.ToString())
				{

				}
				else if (currentState == state.WAITING.ToString())
				{

				}

				if (newState == state.CREATING.ToString())
				{
					TriggerEvent("clientCreateState");
				}
				else if (newState == state.RACING.ToString())
				{

				}
				else if (newState == state.VOTING.ToString())
				{

				}
				else if (newState == state.WAITING.ToString())
				{

				}
				currentState = newState;
				SendChatMessage("Current state: " + currentState, 0, 255, 0);
				SendNuiMessage(JsonConvert.SerializeObject(new { type = "state", text = currentState }));
			}
		}
		[EventHandler("receiveMsg")]
		private void OnReceiveMsg(string msg)
		{
			SendChatMessage(msg, 255, 0, 0);
		}
	}
}
