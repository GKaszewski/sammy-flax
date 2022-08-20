using System;
using System.Collections.Generic;
using FlaxEngine;
using Game.Game;

namespace Game {
    public class Inventory : Script {
        public int Points { get; set; } = 0;
        public CrystalColor CrystalColor { get; set; }

        public Prefab CrystalPrefab { get; set; }
        public Actor CrystalSpawnPosition { get; set; }

        public Actor SammyModel { get; set; }

        public Collider TargetCollider { get; set; }

        public override void OnDisable() {
            TargetCollider.CollisionEnter -= OnCollisionEnter;
        }

        public override void OnStart() {
            //do rx later

            TargetCollider = (Collider)Actor;
            TargetCollider.CollisionEnter += OnCollisionEnter;
        }

        public void CollectPoints(int points) {
            Points += points;
            if (Points >= GameManager.Instance.MaxPoints) {
                CrystalColor = CrystalColor.MULTI;
            }
        }

        public override void OnUpdate() {
            if (Input.GetKeyDown(KeyboardKeys.P)) {
                PrefabManager.SpawnPrefab(CrystalPrefab, new Vector3(0f, 0, 0f));
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.OtherActor.Tag != "Crystal") return;
            var newCrystal = collision.OtherActor.Parent.GetScript<Crystal>();
            // Play pickup sound
            if (CrystalColor != CrystalColor.NONE && CrystalColor != CrystalColor.MULTI) {
                var newColor = Crystal.Mix(CrystalColor, newCrystal.Color);
                if (newColor == CrystalColor.NONE) {
                    var spawnedCrystal = SpawnCrystal(newColor, CrystalSpawnPosition.Position + (Transform.Forward * 2f));
                    spawnedCrystal.Player = SammyModel;
                    spawnedCrystal.ApplyForce();
                    
                } else {
                    //Play mix
                }

                CrystalColor = newColor != CrystalColor.NONE ? newColor : newCrystal.Color;
            } else if (CrystalColor == CrystalColor.NONE) {
                CrystalColor = newCrystal.Color;
            }

            newCrystal.Die();
        }

        private Crystal SpawnCrystal(CrystalColor color, Vector3 position) {
            var newCrystal = PrefabManager.SpawnPrefab(CrystalPrefab, position).GetScript<Crystal>();
            newCrystal.Color = color;
            newCrystal.HandleMaterial();

            return newCrystal;
        }

    }
}
