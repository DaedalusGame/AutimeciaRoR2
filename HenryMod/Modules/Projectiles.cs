using System;
using Autimecia.Modules.ProjectileComponents;
using Autimecia.Utils;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Autimecia.Modules
{
    internal static class Projectiles
    {
        internal static GameObject fedoraBeamPrefab;
        internal static GameObject reelElement;
        internal static GameObject reelTarget;
        internal static GameObject bombPrefab;
        internal static GameObject chaosFireball;
        internal static GameObject chaosWind;
        internal static GameObject chaosBubble;

        internal static GameObject effectSplash;
        internal static GameObject effectWaterExplosion;
        internal static GameObject effectWindArea;

        internal static GameObject shieldWater;

        internal static void RegisterProjectiles()
        {
            effectSplash = CreateSplashEffect();
            effectWaterExplosion = CreateWaterExplosionEffect();
            effectWindArea = CreateWindBlast();

            // only separating into separate methods for my sanity
            CreateBomb();
            fedoraBeamPrefab = CreateFedoraBeam();
            reelElement = CreateElementReel();
            reelTarget = CreateTargetReel();
            chaosFireball = CreateChaosFireball();
            chaosWind = CreateChaosWind();
            chaosBubble = CreateChaosBubble();

            shieldWater = CreateBubbleShield();

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

        private static GameObject CreateWindBlast()
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlWindArea"), "WindArea");

            var vfxAttributes = newPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.High;

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
            var ghost = bubble.GetComponent<ProjectileController>().ghostPrefab;

            var follower = AddBubble("Bubble", ghost);
            follower.transform.localScale = 2f * Vector3.one;

            PrefabAPI.RegisterNetworkPrefab(bubble);
            ProjectileAPI.Add(bubble);

            SetLocalPlayerAuthority(bubble);

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

        private static GameObject CreateFedoraBeam()
        {
            var beam = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlBeam"), "Beam", false);
            var fedora = CreateFedora("ProjFedoraBeam");

            beam.transform.SetParent(fedora.transform);
            beam.transform.localPosition += Vector3.forward;
           
            var beamSimple = beam.AddComponent<BeamSimple>();
            beamSimple.projectileController = fedora.GetComponent<ProjectileController>();
            beamSimple.projectileDamage = fedora.GetComponent<ProjectileDamage>();
            beamSimple.multiHitDelay = 0.1f;
            beamSimple.multiHitMultiplier = 0.1f;
            beamSimple.maxDistance = 100;
            beamSimple.radius = 0.5f;

            var beamLocator = beam.GetComponent<ChildLocator>();
            var beamRainbow = beamLocator.FindChildGameObject("Outer").AddComponent<RainbowComponent>();
            beamRainbow.changeEmission = true;

            var fedoraLocator = fedora.GetComponent<ChildLocator>();
            fedoraLocator.FindChild("ScaleTransform").localScale = new Vector3(3, 3, 3);
            var fedoraRainbow = fedoraLocator.FindChildGameObject("Hat").AddComponent<RainbowComponent>();
            fedoraRainbow.changeTexture = true;
            fedoraRainbow.changeEmission = true;
            var rb = fedora.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            var projectileFedoraLaser = fedora.AddComponent<ProjectileFedoraLaser>();
            projectileFedoraLaser.lifetime = 3;
            projectileFedoraLaser.firetime = 0.40f;
            projectileFedoraLaser.movetime = 0.20f;
            projectileFedoraLaser.distance = 7;
            projectileFedoraLaser.moveCurve = AnimationCurve.EaseInOut(0, 0, 0.15f, 1);
            PrefabAPI.RegisterNetworkPrefab(fedora);
            ProjectileAPI.Add(fedora);

            return fedora;
        }

        private static GameObject CreateTargetReel()
        {
            var reel = CreateReel("ReelTarget");
            var reelFire = AddReelBoard(reel, "ReelBoardProjectile", "chaos_projectile", 10);
            var reelWater = AddReelBoard(reel, "ReelBoardShield", "chaos_shield", AnimationBlendMode.Additive);
            var reelWind = AddReelBoard(reel, "ReelBoardBuff", "chaos_buff", new object());
            var reelEarth = AddReelBoard(reel, "ReelBoardArea", "chaos_area", new object());

            return reel;
        }

        private static GameObject CreateElementReel()
        {
            var reel = CreateReel("ReelElement");
            var reelFire = AddReelBoard(reel, "ReelBoardFire", "chaos_fire", 10);
            var reelWater = AddReelBoard(reel, "ReelBoardWater", "chaos_water", AnimationBlendMode.Additive);
            var reelWind = AddReelBoard(reel, "ReelBoardWind", "chaos_wind", new object());
            var reelEarth = AddReelBoard(reel, "ReelBoardEarth", "chaos_earth", new object());

            return reel;
        }

        private static GameObject CreateFedora(string name)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlTrilby"), name);

            
            var teamFilter = newPrefab.AddComponent<TeamFilter>();
            var projectileController = newPrefab.AddComponent<ProjectileController>();
            var projectileDamage = newPrefab.AddComponent<ProjectileDamage>();
            var projectileNetTransform = newPrefab.AddComponent<ProjectileNetworkTransform>();
            //var networkIdentity = newPrefab.AddComponent<NetworkIdentity>();
            //var ghostManager = newPrefab.AddComponent<ProjectileGhostController>();

            newPrefab.AddComponent<DebugLifetimeComponent>();

            projectileController.ghostPrefab = InitGhostPrefab(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlTrilbyGhost"), name);

            return newPrefab;
        }

        private static GameObject CreateReel(string name)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Reel"), name, false);
            newPrefab.AddComponent<ReelComponent>();

            return newPrefab;
        }

        private static GameObject AddReelBoard(GameObject reel, string name, string spritePath, object value)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("ReelBoard"), name, false);
            Sprite sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>(spritePath);

            newPrefab.transform.SetParent(reel.transform);
            var locator = newPrefab.GetComponent<ChildLocator>();

            var board = newPrefab.AddComponent<ReelBoardComponent>();
            board.value = value;

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