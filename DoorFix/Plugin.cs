using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using DunGen;
using UnityEngine;

namespace DoorFix
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static readonly ManualLogSource LOGGER = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
        
        private void Awake()
        {
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            LOGGER.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(DungeonUtil))]
        [HarmonyPatch(nameof(DungeonUtil.AddAndSetupDoorComponent))]
        public class DungeonUtilPatch
        {
            static void Postfix(Dungeon dungeon, GameObject doorPrefab, Doorway doorway)
            {
                if (!doorPrefab.name.StartsWith("SteelDoorMapSpawn"))
                {
                    return;
                }

                SpawnSyncedObject spawner = doorPrefab.GetComponent<SpawnSyncedObject>();
                foreach (Transform steeldoor_child in spawner.spawnPrefab.transform)
                {
                    if (!steeldoor_child.name.StartsWith("SteelDoor"))
                    {
                        continue;
                    }

                    foreach (Transform doormesh_child in steeldoor_child.transform)
                    {
                        if (!doormesh_child.name.StartsWith("DoorMesh"))
                        {
                            continue;
                        }
                        LOGGER.LogInfo(doormesh_child.ToString());

                        foreach (Transform cube_child in doormesh_child.transform)
                        {
                            if (cube_child.tag != "InteractTrigger")
                            {
                                continue;
                            }
                            LOGGER.LogInfo(cube_child.ToString());

                            BoxCollider[] colliders = cube_child.gameObject.GetComponents<BoxCollider>();
                            foreach (BoxCollider collider in colliders)
                            {
                                if (!collider.isTrigger)
                                {
                                    continue;
                                }
                                LOGGER.LogDebug("Patching door size");
                                collider.size = new Vector3(0.64F, 1F, 1F);
                            }
                        }
                    }
                }
            }
        }
    }
}