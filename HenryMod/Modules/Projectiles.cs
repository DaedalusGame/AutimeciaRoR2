using System;
using Autimecia.Modules.ProjectileComponents;
using Autimecia.SkillStates;
using Autimecia.Utils;
using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.Networking;

namespace Autimecia.Modules
{
    enum ChaosElement
    {
        Fire,
        Water,
        Wind,
        Earth,
    }

    enum ChaosTarget
    {
        Projectile,
        Shield,
        Buff,
        Area,
    }

    internal static class Projectiles
    {
        internal static GameObject fedoraBeamPrefab;
        internal static GameObject reelElement;
        internal static GameObject reelTarget;
        internal static GameObject bombPrefab;
        internal static GameObject chaosFireball;
        internal static GameObject chaosWind;
        internal static GameObject chaosBubble;
        internal static GameObject chaosEarthWave;
        internal static GameObject chaosFireField;

        internal static GameObject effectSplash;
        internal static GameObject effectWaterExplosion;
        internal static GameObject effectWindArea;
        internal static GameObject effectEarthquake;
        internal static GameObject effectFireExplosion;

        internal static GameObject shieldFire;
        internal static GameObject shieldWind;
        internal static GameObject shieldWater;

        internal static GameObject orbFaust;
        internal static GameObject orbIcicle;

        internal static void RegisterProjectiles()
        {
            effectSplash = CreateSplashEffect();
            effectWaterExplosion = CreateWaterExplosionEffect();
            effectWindArea = CreateWindBlast();
            effectEarthquake = CreateChaosEarthExplosion();
            effectFireExplosion = CreateFireExplosionEffect();

            // only separating into separate methods for my sanity
            CreateBomb();
            fedoraBeamPrefab = CreateFedoraBeam();
            reelElement = CreateElementReel();
            reelTarget = CreateTargetReel();
            chaosFireball = CreateChaosFireball();
            chaosWind = CreateChaosWind();
            chaosBubble = CreateChaosBubble();
            chaosEarthWave = CreateChaosEarth();
            chaosFireField = CreateChaosFireField();

            shieldFire = CreateFireShield();
            shieldWind = CreateWindShield();
            shieldWater = CreateBubbleShield();

            orbFaust = CreateFaustOrb();
            orbIcicle = CreateIceSpikeOrb();

            AddProjectile(bombPrefab);
            AddProjectile(fedoraBeamPrefab);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Prefabs.projectilePrefabs.Add(projectileToAdd);
        }

        private static GameObject CreateBubbleShield()
        {
            GameObject shieldOrb = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Shield"), "WaterShieldOrb", false);
            var shieldPart = shieldOrb.AddComponent<ShieldPart>();
            shieldPart.popPrefab = effectWaterExplosion;
            var locator = shieldOrb.GetComponent<ChildLocator>();
            locator.FindChildGameObject("Hitbox").AddComponent<HitBox>();

            var bubble = AddBubble("Bubble", shieldOrb);

            GameObject shield = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Null"), "WaterShield", false);
            var networkId = shield.AddComponent<NetworkIdentity>();
            var shieldController = shield.AddComponent<ShieldChaos>();
            shieldController.shieldOrbPrefab = shieldOrb;
            shieldController.lifetime = 30f;
            var hitboxGroup = shield.AddComponent<HitBoxGroup>();

            PrefabAPI.RegisterNetworkPrefab(shield);

            return shield;
        }

        private static GameObject CreateWindShield()
        {
            GameObject shieldOrb = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Shield"), "WindShieldOrb", false);
            var shieldPart = shieldOrb.AddComponent<ShieldPart>();
            shieldPart.popPrefab = null;
            var locator = shieldOrb.GetComponent<ChildLocator>();
            locator.FindChildGameObject("Hitbox").AddComponent<HitBox>();

            var whirlwind = AddWhirlwind("Whirlwind", shieldOrb);
            whirlwind.transform.localScale = Vector3.one * 0.4f;

            GameObject shield = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Null"), "WindShield", false);
            var networkId = shield.AddComponent<NetworkIdentity>();
            var shieldController = shield.AddComponent<ShieldChaos>();
            shieldController.shieldOrbPrefab = shieldOrb;
            shieldController.lifetime = 30f;
            shieldController.resetInterval = 0.10f;
            var hitboxGroup = shield.AddComponent<HitBoxGroup>();

            PrefabAPI.RegisterNetworkPrefab(shield);

            return shield;
        }

        private static GameObject CreateFireShield()
        {
            GameObject shieldOrb = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Shield"), "FireShieldOrb", false);
            var shieldPart = shieldOrb.AddComponent<ShieldPart>();
            shieldPart.popPrefab = null;
            var locator = shieldOrb.GetComponent<ChildLocator>();
            locator.FindChildGameObject("Hitbox").AddComponent<HitBox>();

            var whirlwind = AddFireball("Fireball", shieldOrb);
            whirlwind.transform.localScale = Vector3.one * 0.7f;

            GameObject shield = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Null"), "FireShield", false);
            var networkId = shield.AddComponent<NetworkIdentity>();
            var shieldController = shield.AddComponent<ShieldChaos>();
            shieldController.damageType = DamageType.IgniteOnHit;
            shieldController.procChance = 0.6f;
            shieldController.shieldOrbPrefab = shieldOrb;
            shieldController.lifetime = 30f;
            shieldController.resetInterval = 0.10f;
            var hitboxGroup = shield.AddComponent<HitBoxGroup>();

            PrefabAPI.RegisterNetworkPrefab(shield);

            return shield;
        }

        private static GameObject CreateWindBlast()
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlWindArea"), "WindArea");

            var vfxAttributes = newPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            var effectComponent = newPrefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;

            EffectAPI.AddEffect(newPrefab);

            return newPrefab;
        }

        private static GameObject CreateSplashEffect()
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlWaterSplash"), "WaterSplash");

            var vfxAttributes = newPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Low;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            var effectComponent = newPrefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;

            EffectAPI.AddEffect(newPrefab);

            return newPrefab;
        }

        private static GameObject CreateWaterExplosionEffect()
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlWaterExplosion"), "WaterExplosion");

            var vfxAttributes = newPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            var effectComponent = newPrefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;

            EffectAPI.AddEffect(newPrefab);

            return newPrefab;
        }

        private static GameObject CreateFireExplosionEffect()
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/WilloWispExplosion"), "FireExplosion");

            newPrefab.transform.localPosition = Vector3.zero;
            GameObject.Destroy(newPrefab.transform.Find("Flames, Radial").gameObject);

            var particles = newPrefab.GetComponentsInChildren<ParticleSystem>();
            foreach(var particleSystem in particles)
            {
                var main = particleSystem.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            EffectAPI.AddEffect(newPrefab);

            return newPrefab;
        }

        private static GameObject CreateChaosWind()
        {
            var wind = CreateWaveBeam("ChaosWind");
            var ghost = wind.GetComponent<ProjectileController>().ghostPrefab;

            int followers = 3;

            for(int i = 0; i < followers; i++)
            {
                float angle = (i * Mathf.PI * 2) / followers;
                var follower = AddWhirlwind("Whirlwind" + i, ghost);
                follower.transform.localScale = 0.2f * Vector3.one;
                var wavebeam = follower.AddComponent<WaveBeamComponent>();
                wavebeam.oscillateSpeed = 1.5f;
                wavebeam.initialAngle = angle * Mathf.Rad2Deg;
                wavebeam.angularVelocity = 220;
                wavebeam.distance = 2f;
            }

            PrefabAPI.RegisterNetworkPrefab(wind);
            ProjectileAPI.Add(wind);

            SetLocalPlayerAuthority(wind);

            return wind;
        }

        private static GameObject CreateChaosBubble()
        {
            var bubble = CreateBouncer("ChaosBubble");
            var projectileController = bubble.GetComponent<ProjectileController>();
            var ghost = bubble.GetComponent<ProjectileController>().ghostPrefab;

            projectileController.allowPrediction = false;

            var follower = AddBubble("Bubble", ghost);
            follower.transform.localScale = 2f * Vector3.one;

            PrefabAPI.RegisterNetworkPrefab(bubble);
            ProjectileAPI.Add(bubble);

            //SetLocalPlayerAuthority(bubble);

            return bubble;
        }

        private static GameObject AddBubble(string name, GameObject parent)
        {
            var vfx = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlBubble"), name, false);
            vfx.transform.SetParent(parent.transform);

            return vfx;
        }

        private static GameObject AddWhirlwind(string name, GameObject parent)
        {
            var vfx = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlWhirlwind"), name, false);
            vfx.transform.SetParent(parent.transform);

            return vfx;
        }

        private static GameObject AddFireball(string name, GameObject parent)
        {
            var vfx = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/ProjectileGhosts/MegaFireballGhost"), name, false);

            vfx.transform.localPosition = Vector3.zero;
            GameObject.Destroy(vfx.GetComponent<ProjectileGhostController>());
            GameObject.Destroy(vfx.GetComponent<VFXAttributes>());
            vfx.transform.SetParent(parent.transform);

            return vfx;
        }

        private static GameObject CreateChaosFireField()
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/LunarExploderProjectileDotZone"), "ChaosFireArea");

            var decal = newPrefab.transform.Find("FX/ScaledOnImpact/Decal");
            var spores = newPrefab.transform.Find("FX/ScaledOnImpact/Spores");
            var fireBillboard = newPrefab.transform.Find("FX/ScaledOnImpact/Fire, Billboard");
            var fireStretched = newPrefab.transform.Find("FX/ScaledOnImpact/Fire, Stretched");
            var pointLight = newPrefab.transform.Find("FX/Point Light");
            GameObject.Destroy(newPrefab.transform.Find("FX/ScaledOnImpact/TeamAreaIndicator, GroundOnly").gameObject);

            AutimeciaPlugin.ModLogger.LogInfo(decal);
            AutimeciaPlugin.ModLogger.LogInfo(spores);
            AutimeciaPlugin.ModLogger.LogInfo(fireBillboard);
            AutimeciaPlugin.ModLogger.LogInfo(fireStretched);
            AutimeciaPlugin.ModLogger.LogInfo(pointLight);

            var decalComponent = decal.GetComponent<Decal>();

            var decalMaterial = decalComponent.Material = new Material(decalComponent.Material);
            AutimeciaPlugin.ModLogger.LogInfo(decalMaterial);
            var sporeMaterial = spores.GetComponent<ParticleSystemRenderer>().material;
            AutimeciaPlugin.ModLogger.LogInfo(sporeMaterial);
            var fireBillboardMaterial = fireBillboard.GetComponent<ParticleSystemRenderer>().material;
            AutimeciaPlugin.ModLogger.LogInfo(fireBillboardMaterial);
            var fireStretchedMaterial = fireStretched.GetComponent<ParticleSystemRenderer>().material;
            AutimeciaPlugin.ModLogger.LogInfo(fireStretchedMaterial);

            decalMaterial.SetColor("_Color", new Color(32f * 0.5f, 13.67843f * 0.5f, 3.913726f * 0.5f, 1f));
            decalMaterial.SetTexture("_RemapTex", Modules.Assets.mainAssetBundle.LoadAsset<Texture>("fire_ramp"));
            fireBillboardMaterial.SetTexture("_RemapTex", Modules.Assets.mainAssetBundle.LoadAsset<Texture>("fire_ramp"));
            fireStretchedMaterial.SetTexture("_RemapTex", Modules.Assets.mainAssetBundle.LoadAsset<Texture>("fire_ramp"));
            sporeMaterial.SetTexture("_RemapTex", Modules.Assets.mainAssetBundle.LoadAsset<Texture>("flare_ramp"));

            var light = pointLight.GetComponent<Light>();
            AutimeciaPlugin.ModLogger.LogInfo(light);
            light.color = new Color(1f, 0.3802547f, 0f, 1f);

            PrefabAPI.RegisterNetworkPrefab(newPrefab);
            ProjectileAPI.Add(newPrefab);

            return newPrefab;
        }

        private static GameObject CreateChaosEarth()
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/Sunder"), "ChaosEarthArea");

            GameObject.Destroy(newPrefab.GetComponent<ProjectileOverlapAttack>());

            var earthquakeTrail = newPrefab.AddComponent<ProjectileEarthquakeTrail>();
            earthquakeTrail.prefab = Projectiles.effectEarthquake;
            earthquakeTrail.damageMultiplier = 20f;
            earthquakeTrail.radius = 5f;
            earthquakeTrail.force = 2000f;
            earthquakeTrail.fireRate = 0.3f;

            PrefabAPI.RegisterNetworkPrefab(newPrefab);
            ProjectileAPI.Add(newPrefab);

            return newPrefab;
        }

        private static GameObject CreateChaosEarthExplosion()
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/BeetleGuardGroundSlam"), "ChaosEarthAreaExplosion");

            EffectAPI.AddEffect(newPrefab);

            return newPrefab;
        }

        private static GameObject CreateWaveBeam(string name)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("WaveBeam"), name);

            var teamFilter = newPrefab.AddComponent<TeamFilter>();
            var projectileController = newPrefab.AddComponent<ProjectileController>();
            projectileController.allowPrediction = true;
            var projectileDamage = newPrefab.AddComponent<ProjectileDamage>();
            var projectileNetTransform = newPrefab.AddComponent<ProjectileNetworkTransform>();
            //var networkIdentity = newPrefab.AddComponent<NetworkIdentity>();

            var locator = newPrefab.GetComponent<ChildLocator>();
            var hitbox = locator.FindChildGameObject("Hitbox").AddComponent<HitBox>();

            var hitboxGroup = newPrefab.AddComponent<HitBoxGroup>();
            hitboxGroup.hitBoxes = new[] { hitbox };

            var projectileOverlap = newPrefab.AddComponent<ProjectileOverlapAttack>();
            projectileOverlap.damageCoefficient = 1f;
            //var projectileSingleImpact = newPrefab.AddComponent<ProjectileSingleTargetImpact>();
            //projectileSingleImpact.destroyWhenNotAlive = true;
            //projectileSingleImpact.destroyOnWorld = false;
            var projectileSimple = newPrefab.AddComponent<ProjectileSimple>();
            projectileSimple.desiredForwardSpeed = 30;
            projectileSimple.lifetime = 10;

            projectileController.ghostPrefab = InitGhostPrefab(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("WaveBeamGhost"), name);

            return newPrefab;
        }

        private static GameObject CreateBouncer(string name)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Bouncer"), name);

            var teamFilter = newPrefab.AddComponent<TeamFilter>();
            var projectileController = newPrefab.AddComponent<ProjectileController>();
            projectileController.allowPrediction = true;
            var projectileDamage = newPrefab.AddComponent<ProjectileDamage>();
            var projectileNetTransform = newPrefab.AddComponent<ProjectileNetworkTransform>();
            //var networkIdentity = newPrefab.AddComponent<NetworkIdentity>();

            var locator = newPrefab.GetComponent<ChildLocator>();
            var hitbox = locator.FindChildGameObject("Hitbox").AddComponent<HitBox>();

            var hitboxGroup = newPrefab.AddComponent<HitBoxGroup>();
            hitboxGroup.hitBoxes = new[] { hitbox };

            var projectileOverlap = newPrefab.AddComponent<ProjectileOverlapAttack>();
            projectileOverlap.damageCoefficient = 1f;
            projectileOverlap.resetInterval = 0.5f;
            var projectileBounce = newPrefab.AddComponent<ProjectileBounce>();
            projectileBounce.bouncesLeft = 10;
            projectileBounce.forwardSpeed = 30;
            projectileBounce.lifetime = 10;
            projectileBounce.bounceEffectPrefab = effectSplash;
            projectileBounce.fadeEffectPrefab = effectWaterExplosion; 

            var projectileTarget = newPrefab.AddComponent<ProjectileTargetComponent>();

            var projectileSphereTarget = newPrefab.AddComponent<ProjectileSphereTargetFinderRandom>();
            projectileSphereTarget.lookRange = 25;
            projectileSphereTarget.targetSearchInterval = 1f;
            projectileSphereTarget.onlySearchIfNoTarget = false;
            projectileSphereTarget.allowTargetLoss = true;
            projectileSphereTarget.testLoS = false;
            projectileSphereTarget.ignoreAir = false;
            projectileSphereTarget.flierAltitudeTolerance = 3;

            newPrefab.AddComponent<ProjectileIgnoreSalvo>();

            projectileController.ghostPrefab = InitGhostPrefab(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("BouncerGhost"), name);

            return newPrefab;
        }


        private static GameObject CreateChaosFireball()
        {
            var fireball = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/Fireball"), "ChaosFireball", true);

            var projectileController = fireball.GetComponent<ProjectileController>();
            projectileController.procCoefficient = 0.2f;
            var projectileDamage = fireball.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.IgniteOnHit;

            projectileController.ghostPrefab = InitGhostPrefab(Resources.Load<GameObject>("Prefabs/ProjectileGhosts/FireballGhost"), "ChaosFireball");

            PrefabAPI.RegisterNetworkPrefab(fireball);
            ProjectileAPI.Add(fireball);

            return fireball;
        }

        private static GameObject CreateIceSpikeOrb()
        {
            var orb = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("IceSpikeOrb"), "IceSpikeOrb", false);

            var orbEffect = orb.AddComponent<OrbEffect>();
            orbEffect.startVelocity1 = new Vector3(-2, -2, -2);
            orbEffect.startVelocity2 = new Vector3(2, 2, 2);
            orbEffect.endVelocity1 = new Vector3(-2, -2, -2);
            orbEffect.endVelocity2 = new Vector3(2, 2, 2);
            orbEffect.movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
            orbEffect.faceMovement = false;

            var icicleEffect = orb.AddComponent<IcicleEffect>();

            var effectComponent = orb.AddComponent<EffectComponent>();

            var vfxAttributes = orb.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = RoR2.VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = RoR2.VFXAttributes.VFXPriority.Medium;

            PrefabAPI.RegisterNetworkPrefab(orb);
            EffectAPI.AddEffect(orb);
            OrbAPI.AddOrb(typeof(OrbIcicle));

            return orb;
        }

        private static GameObject CreateFaustOrb()
        {
            var orb = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("HatOrb"), "FaustOrb", false);

            var fedoraLocator = orb.GetComponent<ChildLocator>();
            fedoraLocator.FindChild("ScaleTransform").localScale = new Vector3(3, 3, 3);

            var orbEffect = orb.AddComponent<OrbEffect>();
            orbEffect.startVelocity1 = new Vector3(-12, 0, 8);
            orbEffect.startVelocity2 = new Vector3(12, 0, 16);
            orbEffect.endVelocity1 = new Vector3(0, 12, 0);
            orbEffect.endVelocity2 = new Vector3(0, 12, 0);
            orbEffect.movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
            orbEffect.faceMovement = true;

            var detachParticles = orb.AddComponent<DetachParticleOnDestroyAndEndEmission>();
            detachParticles.particleSystem = orb.GetComponentInChildren<ParticleSystem>();

            var effectComponent = orb.AddComponent<EffectComponent>();

            var vfxAttributes = orb.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = RoR2.VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = RoR2.VFXAttributes.VFXPriority.Medium;

            PrefabAPI.RegisterNetworkPrefab(orb);
            EffectAPI.AddEffect(orb);
            OrbAPI.AddOrb(typeof(OrbFaust));

            return orb;
        }

        private static GameObject CreateFedoraBeam()
        {
            var beam = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlBeam"), "Beam", false);
            var fedora = CreateFedora("ProjFedoraBeam");

            var entityStateMachine = fedora.AddComponent<EntityStateMachine>();
            entityStateMachine.customName = "Body";
            entityStateMachine.initialStateType.stateType = typeof(HatBeamThrow);
            entityStateMachine.mainStateType.stateType = typeof(HatDisappear);

            var networkStateMachine = fedora.AddComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = new[] { entityStateMachine };

            var projectileController = fedora.GetComponent<ProjectileController>();
            projectileController.allowPrediction = false;

            beam.transform.SetParent(projectileController.ghostPrefab.transform);
            beam.transform.localPosition += Vector3.forward;

            var beamLocator = beam.GetComponent<ChildLocator>();
            var beamRainbow = beamLocator.FindChildGameObject("Outer").AddComponent<RainbowComponent>();
            beamRainbow.changeEmission = true;

            projectileController.ghostPrefab.GetComponent<ChildLocator>().FindChild("ScaleTransform").localScale = new Vector3(3, 3, 3);

            MakeFedoraBig(projectileController.ghostPrefab, 3);
            MakeFedoraDullRainbow(projectileController.ghostPrefab, 1f);
            var rb = fedora.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            var projectileFedoraLaser = fedora.AddComponent<ProjectileFedoraLaser>();
            projectileFedoraLaser.distance = 7;
            projectileFedoraLaser.moveCurve = AnimationCurve.EaseInOut(0, 0, 0.15f, 1);
            PrefabAPI.RegisterNetworkPrefab(fedora);
            ProjectileAPI.Add(fedora);

            SetLocalPlayerAuthority(fedora);

            return fedora;
        }

        public static void MakeFedoraBig(GameObject fedora, float scale)
        {
            var fedoraLocator = fedora.GetComponent<ChildLocator>();
            fedoraLocator.FindChild("ScaleTransform").localScale = Vector3.one * scale;
        }

        public static void MakeFedoraRainbow(GameObject fedora, float hueRate)
        {
            var fedoraLocator = fedora.GetComponent<ChildLocator>();
            var fedoraRainbow = fedoraLocator.FindChildGameObject("Hat").AddComponent<RainbowComponent>();
            fedoraRainbow.changeTexture = true;
            fedoraRainbow.changeEmission = true;
            fedoraRainbow.hueRate = hueRate;
        }

        public static void MakeFedoraDullRainbow(GameObject fedora, float hueRate)
        {
            var fedoraLocator = fedora.GetComponent<ChildLocator>();
            var fedoraRainbow = fedoraLocator.FindChildGameObject("Hat").AddComponent<RainbowComponent>();
            fedoraRainbow.changeTexture = true;
            fedoraRainbow.hueRate = hueRate;
        }

        private static GameObject CreateTargetReel()
        {
            var reel = CreateReel("ReelTarget");
            var reelFire = AddReelBoardTarget(reel, "ReelBoardProjectile", "chaos_projectile", ChaosTarget.Projectile);
            var reelWater = AddReelBoardTarget(reel, "ReelBoardShield", "chaos_shield", ChaosTarget.Shield);
            var reelWind = AddReelBoardTarget(reel, "ReelBoardBuff", "chaos_buff", ChaosTarget.Buff);
            var reelEarth = AddReelBoardTarget(reel, "ReelBoardArea", "chaos_area", ChaosTarget.Area);

            return reel;
        }

        private static GameObject CreateElementReel()
        {
            var reel = CreateReel("ReelElement");
            var reelFire = AddReelBoardElement(reel, "ReelBoardFire", "chaos_fire", ChaosElement.Fire);
            var reelWater = AddReelBoardElement(reel, "ReelBoardWater", "chaos_water", ChaosElement.Water);
            var reelWind = AddReelBoardElement(reel, "ReelBoardWind", "chaos_wind", ChaosElement.Wind);
            var reelEarth = AddReelBoardElement(reel, "ReelBoardEarth", "chaos_earth", ChaosElement.Earth);

            return reel;
        }

        private static GameObject CreateFedora(string name)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("HatBullet"), name);

            
            var teamFilter = newPrefab.AddComponent<TeamFilter>();
            var projectileController = newPrefab.AddComponent<ProjectileController>();
            var projectileDamage = newPrefab.AddComponent<ProjectileDamage>();
            var projectileNetTransform = newPrefab.AddComponent<ProjectileNetworkTransform>();
            //var networkIdentity = newPrefab.AddComponent<NetworkIdentity>();
            //var ghostManager = newPrefab.AddComponent<ProjectileGhostController>();
            var modelLocator = newPrefab.AddComponent<ModelLocator>();

            newPrefab.AddComponent<DebugLifetimeComponent>();

            projectileController.ghostPrefab = InitGhostPrefab(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlTrilby"), name);

            return newPrefab;
        }

        private static GameObject CreateReel(string name)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Reel"), name, false);
            newPrefab.AddComponent<ReelComponent>();

            return newPrefab;
        }

        class ReelBoardElement : ReelBoardComponent
        {
            public ChaosElement element;

            public override object value => element;
        }

        class ReelBoardTarget : ReelBoardComponent
        {
            public ChaosTarget target;

            public override object value => target;
        }

        private static GameObject AddReelBoardElement(GameObject reel, string name, string spritePath, ChaosElement element)
        {
            var newPrefab = AddReelBoard(reel, name, spritePath);
            var board = newPrefab.AddComponent<ReelBoardElement>();
            board.element = element;

            return newPrefab;
        }

        private static GameObject AddReelBoardTarget(GameObject reel, string name, string spritePath, ChaosTarget target)
        {
            var newPrefab = AddReelBoard(reel, name, spritePath);
            var board = newPrefab.AddComponent<ReelBoardTarget>();
            board.target = target;

            return newPrefab;
        }

        private static GameObject AddReelBoard(GameObject reel, string name, string spritePath)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("ReelBoard"), name, false);
            Sprite sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>(spritePath);

            newPrefab.transform.SetParent(reel.transform);
            var locator = newPrefab.GetComponent<ChildLocator>();

            var billboardBase = locator.FindChildGameObject("Billboard");
            var billboard = billboardBase.AddComponent<Billboard>();

            var billboardIcon = locator.FindChildGameObject("Sprite");
            var spriteRenderer = billboardIcon.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

            return newPrefab;
        }

        private static void CreateBomb()
        {
            bombPrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBombProjectile");

            ProjectileImpactExplosion bombImpactExplosion = bombPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bombImpactExplosion);

            bombImpactExplosion.blastRadius = 16f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = Modules.Assets.bombExplosionEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("HenryBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;

            ProjectileController bombController = bombPrefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("HenryBombGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("HenryBombGhost");
            bombController.startSound = "";
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.explosionSoundString = "";
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeExpiredSoundString = "";
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static void SetLocalPlayerAuthority(GameObject prefab)
        {
            var networkIdentity = prefab.GetComponent<NetworkIdentity>();
            networkIdentity.localPlayerAuthority = true;
        }

        private static GameObject InitGhostPrefab(GameObject prefab, string name)
        {
            GameObject ghostPrefab = PrefabAPI.InstantiateClone(prefab, name + "Ghost");
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            return ghostPrefab;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}