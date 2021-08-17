using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;
using RaceServer.models;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;

namespace RaceServer
{
    public class RaceServer : BaseScript
    {
        //todo hold race info in one class including leaderboard
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
        private async void OnPlayerJoining(string player)
        {
            string msg = $" Player {player} has joined!";
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
        [EventHandler("serverStartCreateMode")]
        private void OnServerStartCreateMode()
        {
            currentState = state.CREATING.ToString();
            TriggerClientEvent("clientNewStatePrepare", currentState);
            TriggerClientEvent("clientCreateState");
        }
        [EventHandler("serverStartRaceMode")]
        private async void StartRaceMode(string mapName)
        {
            currentState = state.RACING.ToString();
            TriggerClientEvent("clientNewStatePrepare", currentState);
            string[] raceFiles = Array.FindAll(Directory.GetFiles("resources/racegamemode/races/"), f => f.Contains(".json"));
            string filepath = "";
            foreach (var item in raceFiles)
            {
                if (item.Contains("/" + mapName + ".json"))
                {
                    filepath = item;
                }
            }
            if (filepath != "")
            {
                TriggerClientEvent("receiveMsg", filepath);
                string jsonData = "";
                if (File.Exists(filepath))
                {
                    TriggerClientEvent("receiveMsg", "exists");
                    jsonData = System.IO.File.ReadAllText(filepath);
                }
                string objectsData = ServerLoadXml(mapName);
                int counter = 0;
                //TODO randomize players and use players in race
                foreach (var player in Players)
                {
                    player.TriggerEvent("clientRaceState", jsonData, objectsData, counter++);
                }
                await RaceCountdown();
            }
            TriggerClientEvent("receiveMsg", "Race does not exist");
        }
        private async Task RaceCountdown()
        {
            string msg = $"Race starts in 20 seconds!";
            TriggerClientEvent("receiveMsg", msg);
            await Delay(15000);
            TriggerClientEvent("receiveMsg", "5");
            await Delay(1000);
            TriggerClientEvent("receiveMsg", "4");
            await Delay(1000);
            TriggerClientEvent("receiveMsg", "3");
            await Delay(1000);
            TriggerClientEvent("receiveMsg", "2");
            await Delay(1000);
            TriggerClientEvent("receiveMsg", "1");
            await Delay(1000);
            TriggerClientEvent("receiveMsg", "Go!");
            //todo use players in race
            foreach (var player in Players)
            {
                player.TriggerEvent("clientRaceStart");
            }
        }
        [EventHandler("serverPlayerEnteredCheckpoint")]
        private void OnServerPlayerEnteredCheckpoint([FromSource] Player p, int cpPos, int lap)
        {
            //todo player race data
            string msg = $"Player {p.Name} has entered {cpPos} checkpoint!";
            TriggerClientEvent("receiveMsg", msg);
            //todo check if finished against race game object containing all data
            //if(lap = )
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
            RaceModel newRace = JsonConvert.DeserializeObject<RaceModel>(jsonData);

            System.IO.File.WriteAllText($"resources/racegamemode/races/{newRace.Code}.json", jsonData);
            TriggerClientEvent("clientAfterSaveRace");
        }
        [EventHandler("serverLoadRace")]
        private void OnLoadRace(string mapName)
        {
            string filepath = $"resources/racegamemode/races/{mapName}.json";
            string jsonData = "";
            if (File.Exists(filepath))
            {
                jsonData = System.IO.File.ReadAllText(filepath);
            }
            string objectsData = ServerLoadXml(mapName);
            TriggerClientEvent("clientLoadRace", jsonData, objectsData);
        }
        [EventHandler("serverUnloadRace")]
        private void OnServerUnloadRace()
        {
            TriggerClientEvent("clientUnloadRace");
        }
        private string ServerLoadXml(string mapName)
        {
            List<MapModel> mapObjects = new List<MapModel>();
            if (!string.IsNullOrWhiteSpace(mapName))
            {
                XmlSerializer reader = new XmlSerializer(typeof(Map));
                string filepath = $"resources/racegamemode/races/{mapName}.xml";
                if (File.Exists(filepath))
                {
                    StreamReader file = new StreamReader(filepath);
                    Map map = (Map)reader.Deserialize(file);
                    file.Close();
                    foreach (var item in map.Objects)
                    {
                        MapModel mc = new MapModel();
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
            }
            return JsonConvert.SerializeObject(mapObjects);
        }


        [EventHandler("generateGTARace")]
        private async void GenerateGTARace(string raceName)
        {
            //var url = "https://prod.cloud.rockstargames.com/ugc/gta5mission/4998/" + raceId + "/0_0_en.json";

            var result = loadGTAOFile(raceName);
            if (result != "")
            {
                var root = (JObject)JsonConvert.DeserializeObject(result);
                RaceModel raceModel = new RaceModel();
                var numberOfProps = 0;
                var items = root.SelectToken("mission").Children().OfType<JProperty>().ToDictionary(p => p.Name, p => p.Value);
                foreach (var item in items)
                {
                    if (item.Key == "race")
                    {
                        var key = item.Value.SelectToken("").OfType<JProperty>().ToDictionary(p => p.Name, p => p.Value);
                        var checkpointsNumber = 0;
                        foreach (var k in key)
                        {
                            if (k.Key == "chp")
                            {
                                checkpointsNumber = (int)k.Value;
                            }
                            if (k.Key == "chl")//2
                            {
                                var checkpointLocations = JsonConvert.DeserializeObject<JArray>(k.Value.ToString());
                                foreach (var cp in checkpointLocations.Children())
                                {
                                    CheckpointModel cpm = new CheckpointModel();
                                    var itemProperties = cp.Children<JProperty>();
                                    Vector3 vec3 = new Vector3();
                                    vec3.X = (float)itemProperties.FirstOrDefault(zz => zz.Name == "x").Value;
                                    vec3.Y = (float)itemProperties.FirstOrDefault(zz => zz.Name == "y").Value;
                                    vec3.Z = (float)itemProperties.FirstOrDefault(zz => zz.Name == "z").Value;
                                    cpm.Position = vec3;
                                    cpm.ArrayPos = raceModel.Checkpoints.Count;
                                    raceModel.Checkpoints.Add(cpm);
                                }
                            }
                            if (k.Key == "chh")//1
                            {
                                //checkpoint heading ??
                                Console.WriteLine("Hello World!");
                            }
                            if (k.Key == "lap")
                            {
                                var laps = (int)k.Value;
                                raceModel.Laps = laps;
                            }
                            if (k.Key == "grid")
                            {
                                var X = (float)k.Value["x"];
                                var Y = (float)k.Value["y"];
                                var Z = (float)k.Value["z"];
                                TriggerClientEvent("receiveMsg", $"Race grid at X:{X}, Y:{Y}, Z:{Z}");
                            }
                            if (k.Key == "head")
                            {
                                var gridHeading = (float)k.Value;
                            }
                        }
                    }
                    if (item.Key == "prop")
                    {
                        var key = item.Value.SelectToken("").OfType<JProperty>().ToDictionary(p => p.Name, p => p.Value);
                        foreach (var k in key)
                        {
                            if (k.Key == "no")
                            {
                                numberOfProps = (int)k.Value;
                            }
                            if (k.Key == "loc")
                            {
                                var propLocations = JsonConvert.DeserializeObject<JArray>(k.Value.ToString());
                                foreach (var propLoc in propLocations.Children())
                                {
                                    MapModel mc = new MapModel();
                                    var itemProperties = propLoc.Children<JProperty>();
                                    mc.Position.X = (float)itemProperties.FirstOrDefault(zz => zz.Name == "x").Value;
                                    mc.Position.Y = (float)itemProperties.FirstOrDefault(zz => zz.Name == "y").Value;
                                    mc.Position.Z = (float)itemProperties.FirstOrDefault(zz => zz.Name == "z").Value;
                                    mc.Door = false;
                                    mc.Dynamic = false;
                                    raceModel.MapModels.Add(mc);
                                }
                            }
                            if (k.Key == "model")
                            {
                                var propModels = JsonConvert.DeserializeObject<JArray>(k.Value.ToString());
                                var i = 0;
                                foreach (var propModel in propModels.Children())
                                {
                                    raceModel.MapModels[i++].Hash = (int)propModel;
                                }
                            }
                            if (k.Key == "vRot")
                            {
                                var propRotations = JsonConvert.DeserializeObject<JArray>(k.Value.ToString());
                                var i = 0;
                                foreach (var propRot in propRotations.Children())
                                {
                                    var itemProperties = propRot.Children<JProperty>();
                                    raceModel.MapModels[i].Rotation.X = (float)itemProperties.FirstOrDefault(zz => zz.Name == "x").Value;
                                    raceModel.MapModels[i].Rotation.Y = (float)itemProperties.FirstOrDefault(zz => zz.Name == "y").Value;
                                    raceModel.MapModels[i].Rotation.Z = (float)itemProperties.FirstOrDefault(zz => zz.Name == "z").Value;
                                    ++i;
                                }
                            }
                            if (k.Key == "prpclr")
                            {
                                var propColors = JsonConvert.DeserializeObject<JArray>(k.Value.ToString());
                                var i = 0;
                                foreach (var propColor in propColors.Children())
                                {
                                    raceModel.MapModels[i++].Color = (int)propColor;
                                }
                            }
                        }
                    }
                }
                TriggerClientEvent("clientGenerateRace", JsonConvert.SerializeObject(raceModel));
            } else
            {
                TriggerClientEvent("receiveMsg", $"File {raceName}.json has not been added to gtaofiles for conversion");
            }
            /*var result = await content.ReadAsStringAsync();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    using (var content = response.Content)
                    {
                    }
                }
            }*/
        }

        public static string loadGTAOFile(string name)
        {
            string filepath = $"resources/racegamemode/gtaofiles/{name}.json";
            string jsonData = "";
            if (File.Exists(filepath))
            {
                jsonData = System.IO.File.ReadAllText(filepath);
            }
            return jsonData;
        }
    }
}
