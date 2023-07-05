using Fusion;
using Fusion.XR;
using Fusion.XR.Shared.Rig;
using Fusion.XR.Zone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

/**
* 
* BotSpawner checks if the player press a button to spawn a bot (short press) or a bunch of bots (long press)
* Then it try to create the required number of bot(s) arround the player at a valid position
* 
**/

namespace Fusion.Samples.IndustriesComponents
{
    public class BotSpawner : NetworkBehaviour
    {
        public InputActionProperty botSpawnAction;
        public RigInfo rigInfo;
        public NetworkObject botPrefab;
        NetworkRig localUser;
        public float longPressDuration = 2;
        public int botToSpawnAfterLongPress = 50;
        public int maxBot = 150;

        public float botMaxDistanceToLocalUser = 10;
        public float botMinDistanceToLocalUser = 2;

        float pressStartTime = -1;
        public int botsToCreate = 0;
        public float lastBotCreation = -1;
        public bool forbidZoneToNonUserRig = true;
        public Managers managers;

        private void Start()
        {
            botSpawnAction.action.Enable();
            if (botPrefab == null)
            {
                Debug.LogError("A bot prefab has to be provided");
            }
        }


        private void Update()
        {
            // Reinitialized the press start time when the button is pressed
            if (pressStartTime == -1 && botSpawnAction.action.IsPressed())
            {
                pressStartTime = Time.time;
            }

            // Check if the button was just released
            if (botSpawnAction.action.WasReleasedThisFrame())
            {
                // button was just released, check the press duration
                if ((Time.time - pressStartTime) > longPressDuration)
                {
                    // it was a long press, a bunch of bots must be spawned
                    botsToCreate += botToSpawnAfterLongPress;
                }
                else
                {
                    // it was a short press, spawn only one bot
                    botsToCreate++;
                }
                // reinitialized the press start time
                pressStartTime = -1;

                // limit the bot creation number to maxBot
                if (Bot.BotCount >= maxBot)
                    botsToCreate = 0;
                else if (botsToCreate > (maxBot - Bot.BotCount))
                    botsToCreate = maxBot - Bot.BotCount;
            }
        }

        // spawn bots on the network during the FixedUpdateNetwork
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            // is there a bot to create ?
            if (botsToCreate > 0)
            {
                // check that a minimum of time has elapsed since the last bot creation
                if (lastBotCreation != -1 && (Time.time - lastBotCreation) < 0.05f) return;
                botsToCreate--;
                lastBotCreation = Time.time;
                // spawn a new bot
                SpawnBot();
            }
        }

        //  SpawnBot is in charge to spawn a bot arround the player at a valid position
        void SpawnBot()
        {
            // search for the zoneManager in order to get authorized zones
            if (managers == null) managers = Managers.FindInstance(runner: Object.Runner);


            bool spawnPointFound = false;
            int tries = 0;
            Vector3 pos = Vector3.zero;

            // try several time to find an allowed position around the player
            while (spawnPointFound == false && tries < 10)
            {
                // create a random position arround the local player
                pos = Random.onUnitSphere;
                pos = new Vector3(pos.x, 0, pos.z);
                pos = Random.Range(botMinDistanceToLocalUser, botMaxDistanceToLocalUser) * pos.normalized;
                pos += LocalUserPosition();

                // check if the position is included in the navigation mesh
                if (NavMesh.SamplePosition(pos, out var hit, 1f, NavMesh.AllAreas))
                {
                    spawnPointFound = true;
                    pos = hit.position;

                    // check if this is a valid position, regarding existing zones
                    if (managers.zoneManager)
                    {
                        // for each zone registered in the zone manager
                        foreach (var zone in managers.zoneManager.zones)
                        {
                            // check if the bot is in the zone
                            if (zone.IsRigHeadPositionInZone(hit.position))
                            {
                                // check if the zone accept bot and if the zone is not full
                                if (forbidZoneToNonUserRig || !zone.CanAcceptNewRig)
                                {
                                    spawnPointFound = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                tries++;
            }

            // exit if the spawn position has not been found
            if (!spawnPointFound)
            {
                Debug.LogError("Unable to create bot");
                return;
            }

            // spawn the bot on the network at the correct position
            var rigObject = Object.Runner.Spawn(botPrefab, pos, onBeforeSpawned: (runner, obj) =>
            {
                var bot = obj.GetComponent<Bot>();
                bot.botSpawner = this;
            });
        }


        // LocalUserPosition search the local player and return its position in order to spawn the bots arround him
        private Vector3 LocalUserPosition()
        {
            if (rigInfo == null) rigInfo = RigInfo.FindRigInfo(Object.Runner);
            if (rigInfo && localUser == null)
            {
                localUser = rigInfo.localNetworkedRig;
            }
            if (!localUser)
                throw new System.Exception("Can not spawn bot because local user not found !");
            return localUser.transform.position;
        }
    }
}
