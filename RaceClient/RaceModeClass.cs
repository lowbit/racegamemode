using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using RaceClient.models;
using static CitizenFX.Core.Native.API;
using static RaceClient.CommonClass;

namespace RaceClient
{
    public class RaceModeClass : BaseScript
    {
        public RaceModel currentRace;
        public Blip checkpointBlip;
        public CheckpointModel checkpoint;
        public SpawnpointModel spawnpoint;
        public int cpPos;
        public int currentLap;
        public int spawnPosition;
        List<MapModel> mapObjects;
        public CheckpointConfigModel checkpointConfig;
        public Vehicle vehicle;
        public RaceModeClass()
        {
            Tick += OnTick;
        }
        private async Task OnTick()
        {
            PlayerEnteredCheckpoint();
        }

        [EventHandler("clientRaceState")]
        public void OnclientRaceState(string raceData, string objectData, int position)
        {
            SendChatMessage("clientRaceState", 255, 0, 0);
            currentRace = new RaceModel();
            checkpointConfig = new CheckpointConfigModel();
            checkpoint = new CheckpointModel();
            spawnpoint = new SpawnpointModel();
            mapObjects = new List<MapModel>();
            cpPos = 1;
            currentLap = 1;
            spawnPosition = position;
            LoadRace(raceData, objectData);
            PositionPlayer();
            HandleNextCheckpoint(true);
            //RegisterCommands();
            //UpdateRaceInfo();
        }
        [EventHandler("clientRaceStateUnload")]
        public void OnclientRaceStateUnload()
        {
            checkpointConfig = new CheckpointConfigModel();
            if (checkpointBlip != null)
            {
                checkpointBlip.Delete();
            }
            if (currentRace != null && checkpoint != null)
            {
                DeleteCheckpoint(checkpoint.Handle);
            }
            if (vehicle != null)
                vehicle.Delete();
            //SendNuiMessage(JsonConvert.SerializeObject(currentRace));
            ClientUnloadXml(ref mapObjects);

            currentRace = new RaceModel();
            checkpointConfig = new CheckpointConfigModel();
            checkpoint = new CheckpointModel();
            spawnpoint = new SpawnpointModel();
            mapObjects = new List<MapModel>();
            cpPos = 1;
            currentLap = 1;
            spawnPosition = 0;
        }
        private void LoadRace(string raceData, string objectsData)
        {
            if (raceData != "")
            {
                currentRace = JsonConvert.DeserializeObject<RaceModel>(raceData);
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
            if (String.IsNullOrWhiteSpace(objectsData))
                mapObjects = ClientLoadXml(objectsData);
            else
                mapObjects = ClientLoadXml(JsonConvert.SerializeObject(currentRace.MapModels), true);
        }
        private async void PositionPlayer()
        {
            SendChatMessage($"{currentRace.Spawnpoints[spawnPosition].Position} - {currentRace.Spawnpoints[spawnPosition].Heading}", 255, 0, 0);
            vehicle = await World.CreateVehicle(currentRace.Car, currentRace.Spawnpoints[spawnPosition].Position, currentRace.Spawnpoints[spawnPosition].Heading);
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            FreezeEntityPosition(GetVehiclePedIsUsing(Game.PlayerPed.Handle), true);
        }

        [EventHandler("clientRaceStart")]
        public void OnClientRaceStart()
        {
            FreezeEntityPosition(GetVehiclePedIsUsing(Game.PlayerPed.Handle), false);
        }
        [EventHandler("clientRace1stPlayerFinished")]
        public void OnClientRace1stPlayerFinished()
        {

        }
        [EventHandler("clientRaceFinished")]
        public void OnClientRaceFinished()
        {
        }
        private void PlayerEnteredCheckpoint()
        {
            if (checkpoint?.Position != null)
            {
                float distance = Vector3.Distance(Game.PlayerPed.Position, checkpoint.Position);
                if (distance < 10)
                {
                    HandleNextCheckpoint();
                }
            }
        }
        private void HandleNextCheckpoint(bool starting = false)
        {
            SendChatMessage($"lap:{currentLap}, cp:{cpPos}", 255, 0, 0);
            if (!starting)
            {
                TriggerServerEvent("serverPlayerEnteredCheckpoint", cpPos, currentLap);
                RemoveCP();
                cpPos++;
            }
            if (cpPos == currentRace.Checkpoints.Count + 1)
            {
                if (currentRace.Laps == currentLap)
                {
                    PlayerFinishedRace();
                }
                else
                {
                    cpPos = 1;
                    currentLap++;
                    CreateCP();
                }
            }
            else
            {
                CreateCP();
            }
        }
        private void PlayerFinishedRace()
        {
            PlaySoundFrontend(-1, "ScreenFlash", "WastedSounds", false);
            //MEDAL_GOLD
            //MEDAL_SILVER
            //MEDAL_BRONZE
        }
        private void CreateCP()
        {
            checkpoint = currentRace.Checkpoints[cpPos - 1];
            if (currentRace.Checkpoints.Count == cpPos)
            {
                if (currentRace.Laps <= 1 || currentRace.Laps == currentLap)
                {
                    // finish checkpoint
                    checkpoint.Handle = CreateCheckpoint(4, currentRace.Checkpoints[cpPos - 1].Position.X, currentRace.Checkpoints[cpPos - 1].Position.Y, currentRace.Checkpoints[cpPos - 1].Position.Z - 10, 0, 0, 0, checkpointConfig.Radius, checkpointConfig.Red, checkpointConfig.Green, checkpointConfig.Blue, checkpointConfig.Alpha, 100);
                }
                else
                {
                    // last checkpoint but new lap
                    checkpoint.Handle = CreateCheckpoint(checkpointConfig.Type, currentRace.Checkpoints[cpPos - 1].Position.X, currentRace.Checkpoints[cpPos - 1].Position.Y, currentRace.Checkpoints[cpPos - 1].Position.Z - 10, currentRace.Checkpoints[0].Position.X, currentRace.Checkpoints[0].Position.Y, currentRace.Checkpoints[0].Position.Z, checkpointConfig.Radius, checkpointConfig.Red, checkpointConfig.Green, checkpointConfig.Blue, checkpointConfig.Alpha, 0);
                }
            }
            else
            {
                // normal checkpoint
                checkpoint.Handle = CreateCheckpoint(checkpointConfig.Type, currentRace.Checkpoints[cpPos - 1].Position.X, currentRace.Checkpoints[cpPos - 1].Position.Y, currentRace.Checkpoints[cpPos - 1].Position.Z - 10, currentRace.Checkpoints[cpPos].Position.X, currentRace.Checkpoints[cpPos].Position.Y, currentRace.Checkpoints[cpPos].Position.Z, checkpointConfig.Radius, checkpointConfig.Red, checkpointConfig.Green, checkpointConfig.Blue, checkpointConfig.Alpha, 0);
            }
            CreateCPBlip();
        }
        private void CreateCPBlip()
        {
            checkpointBlip = World.CreateBlip(checkpoint.Position);
            if (currentRace.Checkpoints.Count == cpPos)
            {
                if (currentRace.Laps <= 1 || currentRace.Laps == currentLap)
                {
                    //on finish
                    checkpointBlip.Sprite = BlipSprite.RaceFinish;
                }
                else
                {
                    checkpointBlip.Sprite = BlipSprite.Standard;
                }
            }
            else
            {
                checkpointBlip.Sprite = BlipSprite.Standard;
            }
            checkpointBlip.Name = "Race Blip";
            checkpointBlip.Color = BlipColor.Blue;
            checkpointBlip.IsFriendly = true;
        }
        private void RemoveCP()
        {
            PlaySoundFrontend(-1, "RACE_PLACED", "HUD_AWARDS", false);
            checkpointBlip.Delete();
            DeleteCheckpoint(checkpoint.Handle);
            checkpoint = new CheckpointModel();
        }

    }
}
