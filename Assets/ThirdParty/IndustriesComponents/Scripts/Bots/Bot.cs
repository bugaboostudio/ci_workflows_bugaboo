using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;
using Fusion.XR;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using Fusion.XR.Shared.Rig;
using Fusion.XR.Shared.Locomotion;
using Fusion.XR.Zone;

/**
 * 
 * Bot configures the bot when is brought to life on the network.
 * "Simple avatar" and ReadyPlayerMe avatars are supported.
 * It tries to computes a valid random destination position.
 * When bot reaches the destination position, it waits for a random period and then calculates a new destination
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class Bot : NetworkBehaviour, INonUserRig
    {
        [Networked(OnChanged = nameof(OnBotNameChange)), Capacity(32)]
        public string BotName { get; set; }

        public BotSpawner botSpawner;
        AvatarRepresentation avatarRepresentation;

        public bool moving = false;
        float moveStartTime = 0;

        NavMeshAgent agent;
        NetworkRig rig;
        UserInfo userInfo;
        NetworkLocomotionValidation networkLocomotionValidation;

        Vector3 headsetStartPosition;
        Vector3 leftHandStartPosition;
        Vector3 rightHandStartPosition;
        Quaternion headsetStartRotation;
        Quaternion leftHandStartRotation;
        Quaternion rightHandStartRotation;

        public float pauseEnd = -1;
        public bool useRandomRpmAvatar = false;

        public static int BotCount = 0;

        public List<string> randomRpmAvatars = new List<string>()
    {
        "%StreamingAssets%/RichardG.glb",
        "%StreamingAssets%/SebP.glb",
        "%StreamingAssets%/Man1.glb",
        "%StreamingAssets%/Woman1.glb",
        "%StreamingAssets%/Man2.glb",
        "%StreamingAssets%/Woman2.glb",
    };

        static void OnBotNameChange(Changed<Bot> changed)
        {
            changed.Behaviour.UpdateBotName();
        }

        void UpdateBotName()
        {
            gameObject.name = BotName;
        }

        private void Awake()
        {
            avatarRepresentation = GetComponentInChildren<AvatarRepresentation>();
            rig = GetComponent<NetworkRig>();
            userInfo = GetComponent<UserInfo>();
            networkLocomotionValidation = GetComponent<NetworkLocomotionValidation>();
        }

        // Configure the bot when is brought to life on the network
        public override void Spawned()
        {
            base.Spawned();

            headsetStartPosition = rig.transform.InverseTransformPoint(rig.headset.transform.position);
            leftHandStartPosition = rig.transform.InverseTransformPoint(rig.leftHand.transform.position);
            rightHandStartPosition = rig.transform.InverseTransformPoint(rig.rightHand.transform.position);
            headsetStartRotation = Quaternion.Inverse(rig.transform.rotation) * rig.headset.transform.rotation;
            leftHandStartRotation = Quaternion.Inverse(rig.transform.rotation) * rig.leftHand.transform.rotation;
            rightHandStartRotation = Quaternion.Inverse(rig.transform.rotation) * rig.rightHand.transform.rotation;

            // increment the number of bots
            BotCount++;

            if (!Object.HasStateAuthority) return;
            BotName = $"Bot-{Runner.LocalPlayer.PlayerId}-{BotCount}";

            ConfigureAgent();
            ConfigureAvatar();



        }

        // Configure the representation 
        void ConfigureAvatar()
        {
            // check which kind of avatar to use for the bots
            if (useRandomRpmAvatar)
            {
                // use a random ReadyPlayerMe avatar
                userInfo.AvatarURL = randomRpmAvatars[Random.Range(0, randomRpmAvatars.Count)];
            }
            else
            {
                // Change the avatar URL with a random combination and inform all players
                RandomizeAvatar();
            }
            userInfo.UserName = BotName;
        }

        // RandomizeAvatar update the avatar URL with a random combination
        public void RandomizeAvatar()
        {
            if (!avatarRepresentation) return;
            if (!Object.HasStateAuthority) return;
            // get the random avatar URL
            string avatarURL = avatarRepresentation.RandomAvatar();
            // inform all players that the avatar has changes
            userInfo.AvatarURL = avatarURL;
        }

        // ConfigureAgent add the navigation mesh agent to enable movement
        private void ConfigureAgent()
        {
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                agent.speed /= 2;
                agent.updatePosition = false;
                agent.updateRotation = false;
            }
        }

        // Manage the bot movements
        public override void FixedUpdateNetwork()
        {
            // check it is a bot owned by the local player
            if (!Object.HasStateAuthority) return;

            // check that the NavMeshAgent has been set
            if (!agent) return;

            // check if a timer has been set to stop the pause
            if (pauseEnd != -1)
            {   // yes a pause timer has been initialized
                // exit if the timer is not reached
                if (pauseEnd > Time.time) return;
                // else, pause timer can be reset
                pauseEnd = -1;
            }

            // Check if the bot is moving
            if (!moving)
            {
                // the bot is not moving
                // compute the next random position
                Vector3 pos = transform.position + 10 * Random.insideUnitSphere;
                pos = new Vector3(pos.x, transform.position.y, pos.z);
                int tries = 0;

                // try to 10 times to find a valid destination point near the random position
                while (tries < 10 && moving == false)
                {
                    // check if a destination has been found near the random position
                    if (NavMesh.SamplePosition(pos, out var hit, 1f, NavMesh.AllAreas))
                    {
                        // check if the destination is valid
                        if (networkLocomotionValidation.CanMoveHeadset(hit.position+rig.headset.transform.localPosition.y * Vector3.up))
                        {
                            // destination is valid
                            // the bot can move to this destination
                            moving = true;
                            moveStartTime = Time.time;
                            // set the navigation mesh agent destination and compute a new path
                            agent.SetDestination(hit.position);
                        }
                    }
                    tries++;
                }

            }
            // the bot is moving
            // Check if the bot is at destination or if the bot has been moving for more than 5 seconds
            else if ((transform.position - agent.destination).sqrMagnitude < 0.025f || (Time.time - moveStartTime) > 5)
            {
                // stop the bot & reset the bot path
                moving = false;
                agent.ResetPath();
                // compute a pause duration
                pauseEnd = Time.time + Random.Range(0f, 10f);
            }

            // position the bot rig parts
            rig.headset.transform.position = rig.transform.TransformPoint(headsetStartPosition);
            rig.leftHand.transform.position = rig.transform.TransformPoint(leftHandStartPosition);
            rig.rightHand.transform.position = rig.transform.TransformPoint(rightHandStartPosition);
            rig.headset.transform.rotation = rig.transform.rotation * headsetStartRotation;
            rig.leftHand.transform.rotation = rig.transform.rotation * leftHandStartRotation;
            rig.rightHand.transform.rotation = rig.transform.rotation * rightHandStartRotation;

            // rotate the bot toward the destination
            var direction = agent.steeringTarget - transform.position;
            direction = new Vector3(direction.x, 0, direction.z);
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.5f * (Time.time - moveStartTime));

            // check if the next position is valid (if it is not, but the bot is already in a forbidden position, we let it move to go out of the forbidden zone)
            if (networkLocomotionValidation.CanMoveHeadset(agent.nextPosition + rig.headset.transform.localPosition.y * Vector3.up) || !networkLocomotionValidation.CanMoveHeadset(rig.headset.transform.position))
            {
                // Let's move the bot
                transform.position = agent.nextPosition;
            }
            else
            {
                moving = false;
            }
        }

    }
}