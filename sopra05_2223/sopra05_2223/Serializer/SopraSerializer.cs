using Newtonsoft.Json;
using sopra05_2223.Core.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using sopra05_2223.Core;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Protagonists;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Serializer
{
    static class SopraSerializer
    {
        public static void SerializeSettings()
        {
            // Serialize screen resolution
            Serialize(Globals.SaveSettingsPath("ScreenResolution"), Globals.Resolution, typeof(Resolution));
            // Serialize SoundManager
            Serialize(Globals.SaveSettingsPath("SoundManager"), Globals.SoundManager, typeof(SoundManager));
        }

        // This Method can be used to serialize all kind of objects using the default settings.
        // path: path of the file that will be saved
        // obj: Any C# Object.
        // type: typeof(objectName)
        public static void Serialize(string path, object obj, System.Type type)
        {
            var serializer = new JsonSerializer();

            // Settings for serialization and output.
            serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            serializer.Formatting = Formatting.Indented;

            // Ensure path exists
            var dir = System.IO.Path.GetDirectoryName(path);
            if (dir == null)
            {
                return;
            }
            System.IO.Directory.CreateDirectory(dir);

            // Write to file
            var sw = new StreamWriter(path);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, obj, type);
        }

        public static SoundManager DeserializeSoundManager()
        {
            var path = Globals.SaveSettingsPath("SoundManager");
            if (!System.IO.File.Exists(path))
            {
                // Add Error msg.
                return null;
            }

            SoundManager deserializedSoundManager;

            try
            {
                deserializedSoundManager = JsonConvert.DeserializeObject<SoundManager>(File.ReadAllText(path),
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

            return deserializedSoundManager;
        }

        public static Resolution DeserializeScreenResolution()
        {
            var path = Globals.SaveSettingsPath("ScreenResolution");
            if (!System.IO.File.Exists(path))
            {
                // Add Error msg.
                return null;
            }

            Resolution deserializedResolution;

            try
            {
                // throws an exception. idk why.
                deserializedResolution = JsonConvert.DeserializeObject<Resolution>(File.ReadAllText(path),
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // does not catch the exception. idk why.
                return null;
            }

            return deserializedResolution;
        }

        // Object containing GameScreen-information that we deserialized
        internal sealed class GameScreenResult
        {
            internal readonly EntityManager mEntityManager;
            internal readonly Vector2 mCamPos;
            internal readonly float mCamZoom;
            internal readonly PlayerPlayer mPlayer;
            internal readonly PlayerKi mKi;
            internal readonly Dictionary<string, int> mRunStatistics;

            internal GameScreenResult(EntityManager em,
                Vector2 camPos,
                float camZoom,
                PlayerPlayer player,
                PlayerKi ki,
                Dictionary<string, int> runStatistics)
            {
                mEntityManager = em;
                mCamPos = camPos;
                mCamZoom = camZoom;
                mPlayer = player;
                mKi = ki;
                mRunStatistics = runStatistics;
            }
        }

        public static GameScreenResult DeserializeGameScreen(int slotNumber)
        {
            // Deserialize the GameScreen from its file path (slot one, two or three).
            // Validate slot number
            if (slotNumber is < 1 or > 3)
            {
                return null;
            }

            var path = Globals.SaveGamePath(slotNumber);
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            JObject jsonObj;

            try
            {
                jsonObj = JObject.Parse(File.ReadAllText(path));
            }
            catch (Exception)
            {
                // corrupt saveFile
                return null;
            }

            // Deserialize Protagonists
            PlayerPlayer player;
            PlayerKi ki;
            try
            {
                var pStr = jsonObj["mPlayer"]?.ToString();
                var kiStr = jsonObj["mKi"]?.ToString();

                if (pStr == null || kiStr == null)
                {
                    return null;
                }

                player = JsonConvert.DeserializeObject<PlayerPlayer>(pStr,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                            NullValueHandling = NullValueHandling.Ignore,
                        });
                ki = JsonConvert.DeserializeObject<PlayerKi>(kiStr,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                            NullValueHandling = NullValueHandling.Ignore,
                        });
                if (player is null || ki is null)
                {
                    return null;
                }
                player.SetOpponent(ki);
                ki.SetOpponent(player);
            }
            catch (Exception)
            {
                // corrupt saveFile
                return null;
            }

            var deserializedEntityManager = DeserializeEntityManager(jsonObj["mEntityManager"], player, ki);

            // ==== Camera ====
            Vector2 camPos;
            float camZoom;
            try
            {
                var cp = jsonObj["mCamera"]?["mPos"];
                var cz = jsonObj["mCamera"]?["Zoom"];
                if (cp == null || cz == null)
                {
                    return null;
                }
                // Vector2 is serialized as a string ("X, Y"), so we deserialize the values by splitting the string at the comma
                var cpArr = cp.ToString().Split(",");
                camPos = new Vector2(float.Parse(cpArr[0]), float.Parse(cpArr[1]));

                camZoom = float.Parse(cz.ToString());
            }
            catch(Exception)
            {
                return null;
            }

            // ==== RunStatistics ====
            Dictionary<string, int> runStatistics;
            try
            {
                var rs = jsonObj["mStatistics"];
                if (rs == null)
                {
                    return null;
                }

                runStatistics = JsonConvert.DeserializeObject<Dictionary<string, int>>(rs.ToString(),
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                            NullValueHandling = NullValueHandling.Ignore,
                        });
            }
            catch (Exception)
            {
                return null;
            }

            return new GameScreenResult(deserializedEntityManager, camPos, camZoom, player, ki, runStatistics);
        }

        private static EntityManager DeserializeEntityManager(JToken emObj, PlayerPlayer player, Player ki)
        {
            EntityManager deserializedEntityManager;

            try
            {
                deserializedEntityManager = JsonConvert.DeserializeObject<EntityManager>(emObj.ToString(),
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        NullValueHandling = NullValueHandling.Ignore,
                    });

                // Add the information we skipped in the process of serialization back to the EntityManager.
                if (deserializedEntityManager != null)
                {
                    player.mEm = deserializedEntityManager;
                    ki.mEm = deserializedEntityManager;

                    foreach (var entity in deserializedEntityManager.Entities)
                    {
                        // We skipped this information because it would have resulted in a serialization loop.
                        // Now add it back.
                        entity.mEntityManager = deserializedEntityManager;

                        // also serialization loop.
                        if (entity is ETransport eTransport)
                        {
                            foreach (var ec in eTransport.mCollectors)
                            {
                                ec.GetComponent<CollectCollector>().mOwnerTransport = eTransport;
                            }
                        }

                        foreach (var component in entity.mComponents.Values)
                        {
                            // We skipped this information because it would have resulted in a serialization loop.
                            // Now add it back.
                            component.mEntity = entity;

                            switch (component)
                            {
                                case CTeam team:
                                    switch (team.mTeam)
                                    {
                                        case Team.Player:
                                            team.mProtagonist = player;
                                            player.AddEntity(team.mEntity);
                                            // all textures are set to ki by default in Constructor, cause we don't set the Protagonists before
                                            team.mEntity.ChangeTexture();
                                            break;
                                        case Team.Ki:
                                            team.mProtagonist = ki;
                                            ki.AddEntity(team.mEntity);
                                            break;
                                        case Team.Neutral:
                                        default:
                                            break;
                                    }

                                    break;
                                case CTakeBase ctb:
                                    ctb.mPlayer = player;
                                    break;
                                case CGun gun:
                                    Globals.SoundManager.AddIAudible(gun);
                                    break;
                                case CollectTransporter c:
                                    if (component.mEntity is ETransport e)
                                    {
                                        c.SetCollectors(e.mCollectors);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return deserializedEntityManager;
        }

        // ==== ACHIEVEMENTS ====
        public static void SerializeAchievements()
        {
            var achievements = new Dictionary<string, Dictionary<string, bool>>
            {
                { "achievements", Globals.mAchievements },
                { "built", Globals.mBuilt },
                { "explored", Globals.mExplored }
            };
            Serialize(Globals.sAchievementsPath, achievements, typeof(Dictionary<string, Dictionary<string, bool>>));
        }

        // Helper function to ensure each of the Dictionary Keys is set while the Application runs.
        private static void SetMissingDictValuesToDefault<TK, TV>(IDictionary<TK, TV> dict, IReadOnlyDictionary<TK, TV> defaultDict)
        {
            foreach (var key in defaultDict.Keys)
            {
                if (!dict.ContainsKey(key))
                {
                    dict[key] = defaultDict[key];
                }
            }
        }

        // This is ugly, but works.
        public static void DeserializeAchievements()
        {
            var path = Globals.sAchievementsPath;
            if (!System.IO.File.Exists(path))
            {
                Globals.mAchievements = Globals.sAchievementsDefault;
                Globals.mExplored = Globals.sExploredDefault;
                Globals.mBuilt = Globals.sBuiltDefault;
            }

            try
            {
                var deserializedAchievements = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(File.ReadAllText(path),
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        NullValueHandling = NullValueHandling.Ignore
                    });

                if (deserializedAchievements is not null)
                {
                    Globals.mAchievements = deserializedAchievements["achievements"];
                    Globals.mBuilt = deserializedAchievements["built"];
                    Globals.mExplored = deserializedAchievements["explored"];
                }
            }
            catch (Exception)
            {
                Globals.mAchievements = Globals.sAchievementsDefault;
                Globals.mExplored = Globals.sExploredDefault;
                Globals.mBuilt = Globals.sBuiltDefault;
                return;
            }

            // Set missing values to default.
            SetMissingDictValuesToDefault(Globals.mBuilt, Globals.sBuiltDefault);
            SetMissingDictValuesToDefault(Globals.mExplored, Globals.sExploredDefault);
            SetMissingDictValuesToDefault(Globals.mAchievements, Globals.sAchievementsDefault);
        }

        public static void DeserializeStatistics()
        {
            var path = Globals.sStatisticsPath;
            if (!System.IO.File.Exists(path))
            {
                Globals.mStatistics = Globals.sStatisticsDefault;
                return;
            }

            try
            {
                var deserializedStatistics = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(path),
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        NullValueHandling = NullValueHandling.Ignore
                    });
                if (deserializedStatistics is not null)
                {
                    Globals.mStatistics = deserializedStatistics;
                }
            }
            catch (Exception)
            {
                Globals.mStatistics = Globals.sStatisticsDefault;
                return;
            }

            // Set missing Values to Default.
            SetMissingDictValuesToDefault(Globals.mStatistics, Globals.sStatisticsDefault);
        }

        public static void DeserializeKeys()
        {
            var path = Globals.SaveSettingsPath("Keybindings");
            if (!System.IO.File.Exists(path))
            {
                Globals.mKeys = Globals.sKeysDefault;
                return;
            }

            try
            {
                var deserializedKeys = JsonConvert.DeserializeObject<Dictionary<string, Keys>>(File.ReadAllText(path),
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        NullValueHandling = NullValueHandling.Ignore
                    });
                Globals.mKeys = deserializedKeys ?? Globals.sKeysDefault;
            }
            catch (Exception)
            {
                Globals.mKeys = Globals.sKeysDefault;
            }

            // Overwrite missing keys with defaults.
            SetMissingDictValuesToDefault(Globals.mKeys, Globals.sKeysDefault);
        }
    }
}
