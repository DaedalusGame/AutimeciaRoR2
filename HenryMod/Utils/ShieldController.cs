using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Autimecia.Utils
{
    public struct OwnerInfo
    {
        public OwnerInfo(GameObject gameObject)
        {
            this.gameObject = gameObject;
            if (gameObject)
            {
                transform = gameObject.transform;
                characterBody = gameObject.GetComponent<CharacterBody>();
                cameraTargetParams = gameObject.GetComponent<CameraTargetParams>();
                return;
            }
            transform = null;
            characterBody = null;
            cameraTargetParams = null;
        }

        public readonly GameObject gameObject;
        public readonly Transform transform;
        public readonly CharacterBody characterBody;
        public readonly CameraTargetParams cameraTargetParams;
    }

    class ShieldChaos : ShieldController
    {
        protected void Start()
        {
            int orbs = UnityEngine.Random.Range(1, 7);
            float minDist = UnityEngine.Random.Range(1f, 2f);
            float maxDist = minDist + UnityEngine.Random.Range(1f, 2f);
            float angleJiggle = UnityEngine.Random.Range(-1f, 1f);
            float distanceJiggle = UnityEngine.Random.Range(-1f, 1f);

            if (UnityEngine.Random.Range(0f, 1f) < 0.7f)
                angleJiggle = 0;
            if (UnityEngine.Random.Range(0f, 1f) < 0.7f)
                distanceJiggle = 0;

            SetParameters(orbs, UnityEngine.Random.Range(0, 360f), UnityEngine.Random.Range(45f, 720f), UnityEngine.Random.Range(45f, 180f), minDist, maxDist, angleJiggle, distanceJiggle);
        }
    }

    class ShieldController : NetworkBehaviour
    {
        public GameObject owner;
        public int partCount;
        public float angleBase;
        public float angleRate;
        public float distanceRate;
        public float minDistance;
        public float maxDistance;
        public float angleJiggle;
        public float distanceJiggle;

        NetworkInstanceId ownerNetId;
        OwnerInfo cachedOwnerInfo;

        public GameObject shieldOrbPrefab;
        public List<ShieldPart> shields = new List<ShieldPart>();

        HitBoxGroup hitboxGroup;
        OverlapAttack attack;

        float resetTime;
        public float resetInterval = 0.5f;

        float stopwatch;
        public float lifetime;

        public GameObject networkOwner
        {
            get
            {
                return owner;
            }
            [param: In]
            set
            {
                SetSyncVarGameObject(value, ref owner, 01U, ref ownerNetId);
            }
        }

        public void SetParameters(int partCount, float angleBase, float angleRate, float distanceRate, float minDistance, float maxDistance, float angleJiggle, float distanceJiggle)
        {
            this.partCount = partCount;
            this.angleBase = angleBase;
            this.angleRate = angleRate;
            this.distanceRate = distanceRate;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.angleJiggle = angleJiggle;
            this.distanceJiggle = distanceJiggle;
            SetDirtyBit(02U);
        }

        private void Awake()
        {
            hitboxGroup = gameObject.GetComponent<HitBoxGroup>();
        }

        private void UNetVersion()
        {
        }

        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if(forceAll)
            {
                writer.Write(owner);
                writer.Write(partCount);
                writer.Write(angleBase);
                writer.Write(angleRate);
                writer.Write(distanceRate);
                writer.Write(minDistance);
                writer.Write(maxDistance);
                writer.Write(angleJiggle);
                writer.Write(distanceJiggle);
                return false;
            }

            writer.WritePackedUInt32(syncVarDirtyBits);
            if ((syncVarDirtyBits & 01U) != 0)
                writer.Write(owner);
            if ((syncVarDirtyBits & 02U) != 0)
            {
                writer.Write(partCount);
                writer.Write(angleBase);
                writer.Write(angleRate);
                writer.Write(distanceRate);
                writer.Write(minDistance);
                writer.Write(angleJiggle);
                writer.Write(distanceJiggle);
            }

            return true;
        }

        public override void OnDeserialize(NetworkReader reader, bool forceAll)
        {
            if(forceAll)
            {
                ownerNetId = reader.ReadNetworkId();
                partCount = reader.ReadInt32();
                angleBase = reader.ReadSingle();
                angleRate = reader.ReadSingle();
                distanceRate = reader.ReadSingle();
                minDistance = reader.ReadSingle();
                maxDistance = reader.ReadSingle();
                angleJiggle = reader.ReadSingle();
                distanceJiggle = reader.ReadSingle();
            }

            int syncFlags = (int)reader.ReadPackedUInt32();

            if ((syncFlags & 01U) != 0)
                owner = reader.ReadGameObject();
            if ((syncFlags & 02U) != 0)
            {
                partCount = reader.ReadInt32();
                angleBase = reader.ReadSingle();
                angleRate = reader.ReadSingle();
                distanceRate = reader.ReadSingle();
                minDistance = reader.ReadSingle();
                maxDistance = reader.ReadSingle();
                angleJiggle = reader.ReadSingle();
                distanceJiggle = reader.ReadSingle();
            }
        }

        public override void PreStartClient()
        {
            if (!ownerNetId.IsEmpty())
            {
                networkOwner = ClientScene.FindLocalObject(ownerNetId);
            }
        }

        private void SetupOverlapAttack()
        {
            attack = new OverlapAttack();
            attack.procChainMask = default(ProcChainMask);
            attack.procCoefficient = 0.1f;
            attack.attacker = owner;
            attack.inflictor = gameObject;
            attack.teamIndex = cachedOwnerInfo.characterBody.teamComponent.teamIndex;
            attack.damage = cachedOwnerInfo.characterBody.damage * 2f;
            attack.forceVector = Vector3.zero;
            attack.hitEffectPrefab = null;
            attack.isCrit = false;
            attack.damageColorIndex = DamageColorIndex.Default;
            attack.damageType = DamageType.Generic;
            attack.maximumOverlapTargets = 1000;
            attack.hitBoxGroup = hitboxGroup;
            attack.Fire(new List<HurtBox>());
        }

        private void PopAllOrbs()
        {
            foreach (var shield in shields)
            {
                shield.Pop();
                GameObject.Destroy(shield.gameObject);
            }
        }

        public void FixedUpdate()
        {
            if (cachedOwnerInfo.gameObject != owner)
            {
                cachedOwnerInfo = new OwnerInfo(owner);
                SetupOverlapAttack();
            }

            if (NetworkServer.active)
            {
                if (!owner)
                {
                    GameObject.Destroy(gameObject);
                    return;
                }

                if (attack != null)
                {
                    attack.Fire(new List<HurtBox>());
                    resetTime += Time.fixedDeltaTime;
                    if(resetTime >= resetInterval)
                    {
                        attack.ResetIgnoredHealthComponents();
                        resetTime = 0;
                    }
                }

                stopwatch += Time.fixedDeltaTime;
                if(stopwatch >= lifetime)
                {
                    PopAllOrbs();
                    GameObject.Destroy(gameObject);
                    return;
                }
            }

            if (shields.Count != partCount)
            {
                //TODO: lol performance
                PopAllOrbs();
                shields.Clear();

                for(int i = 0; i < partCount; i++)
                {
                    var shield = GameObject.Instantiate(shieldOrbPrefab, transform);
                    var shieldPart = shield.GetComponent<ShieldPart>();
                    shieldPart.shieldController = this;
                    shieldPart.index = i;
                    shieldPart.angleOffset = 360f * i / partCount;
                    shields.Add(shieldPart);
                }
                hitboxGroup.hitBoxes = GetComponentsInChildren<HitBox>();
            }
        }
    }

    class ShieldPart : MonoBehaviour
    {
        public ShieldController shieldController;
        public float angleOffset;
        public float stopwatch;
        public int index;

        public GameObject popPrefab;

        public void Update()
        {
            stopwatch += Time.deltaTime;

            float angleMod = 1 + index * shieldController.angleJiggle;
            float distanceMod = 1 + index * shieldController.distanceJiggle;

            float angle = shieldController.angleBase + angleOffset * angleMod + stopwatch * shieldController.angleRate;
            float distance = Mathf.Lerp(shieldController.minDistance, shieldController.maxDistance, 0.5f + 0.5f * Mathf.Sin((360 * distanceMod + stopwatch * shieldController.distanceRate) * Mathf.Deg2Rad));

            Vector3 pos = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad)) * distance;

            transform.localPosition = pos;
        }

        public void Pop()
        {
            if(popPrefab)
            {
                EffectManager.SpawnEffect(popPrefab, new EffectData()
                {
                    origin = transform.position,
                    rotation = Quaternion.identity,
                }, true);
            }
        }
    }
}
