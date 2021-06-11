using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;
using RaceServer.models;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System;

namespace RaceServer
{
    public class RaceServer : BaseScript
    {
        enum state { WAITING, VOTING, RACING, CREATING }
        string currentState; 
        public RaceServer()
        {
            Tick += OnTick;
            Tick += TimeLoop3;
            Tick += TimeLoop7;
            currentState = state.WAITING.ToString();
        }
        private async Task OnTick()
        {

        }
        private async Task TimeLoop3()
        {
            await Delay(3000);
        }
        private async Task TimeLoop7()
        {
            await Delay(7000);
        }
        [EventHandler("playerJoining")]
        private async void OnPlayerJoining(Player player)
        {
            string msg = $" Player {player.Name} has joined!";
            TriggerClientEvent("receiveMsg", msg);
        }
        [EventHandler("entityCreated")]
        private void OnEntityCreated(int handler)
        {

        }
        [EventHandler("getServerState")]
        private void OnGetServerState()
        {
            TriggerClientEvent("getServerState", currentState);
        }
        [EventHandler("serverStateChange")]
        private void OnServerStateChange(string newState)
        {
            currentState = newState;
            TriggerClientEvent("clientStateChange", currentState);
        }
        [EventHandler("serverSpawnSpawnPoint")]
        private void OnServerSpawnSpawnPoint([FromSource] Player p, string jsonData)
        {
            TriggerClientEvent("clientSpawnSpawnPoint", jsonData);
            p.TriggerEvent("moveNextSpawnpoint");
        }
        [EventHandler("serverSpawnCheckpoint")]
        private void OnServerSpawnCheckpoint(Vector3 pos)
        {
            TriggerClientEvent("clientSpawnCheckpoint", pos);
        }
        [EventHandler("serverRemoveCheckpoint")]
        private void OnServerRemoveCheckpoint()
        {
            TriggerClientEvent("clientRemoveCheckpoint");
        }
        [EventHandler("serverRemoveSpawnpoint")]
        private void OnServerRemoveSpawnpoint()
        {
            TriggerClientEvent("clientRemoveSpawnpoint");
        }
        [EventHandler("serverUpdateRaceInfo")]
        private void OnServerUpdateRaceInfo(string jsonData)
        {
            TriggerClientEvent("clientUpdateRaceInfo", jsonData);
        }
        [EventHandler("saveRace")]
        private void OnSaveRace(string jsonData)
        {
            RaceClass newRace = JsonConvert.DeserializeObject<RaceClass>(jsonData);

            System.IO.File.WriteAllText($"resources/racegamemode/races/{newRace.Code}.json", jsonData);
            TriggerClientEvent("clientAfterSaveRace");
        }
        [EventHandler("serverLoadRace")]
        private void OnLoadRace(string name)
        {
            string jsonData = System.IO.File.ReadAllText($"resources/racegamemode/races/{name}.json");
            TriggerClientEvent("clientLoadRace", jsonData);
		}
        [EventHandler("serverUnloadRace")]
        private void OnServerUnloadRace()
        {
            TriggerClientEvent("clientUnloadRace");
        }
        //[EventHandler("serverStreamObjects")]
        //private void OnServerStreamObjects(string resName, bool isStreamed)
        //{
        //    TriggerClientEvent("clientStreamObjects", resName, isStreamed);
        //}
        [EventHandler("serverLoadXml")]
        private void OnServerLoadXml(string mapName, bool active)
        {
            List<MapClass> mapObjects = new List<MapClass>();
            if (!string.IsNullOrWhiteSpace(mapName))
            {
                XmlSerializer reader = new XmlSerializer(typeof(Map));
                StreamReader file = new StreamReader($"resources/racegamemode/races/{mapName}.xml");
                Map map = (Map)reader.Deserialize(file);
                file.Close();
                foreach (var item in map.Objects)
                {
                    MapClass mc = new MapClass();
                    mc.Door = item.Door;
                    mc.Dynamic = item.Dynamic;
                    mc.Hash = item.Hash;
                    mc.Position.X = (float)item.Position.X;
                    mc.Position.Y = (float)item.Position.Y;
                    mc.Position.Z = (float)item.Position.Z;
                    mc.Rotation.X = item.Rotation.X;
                    mc.Rotation.Y = item.Rotation.Y;
                    mc.Rotation.Z = (float)item.Rotation.Z;
                    mc.Quaternion.X = item.Quaternion.X;
                    mc.Quaternion.Y = item.Quaternion.Y;
                    mc.Quaternion.Z = (float)item.Quaternion.Z;
                    mc.Quaternion.W = item.Quaternion.W;
                    mapObjects.Add(mc);
                }
            }
			TriggerClientEvent("clientLoadXml", JsonConvert.SerializeObject(mapObjects), active);
        }
    }
}
