using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlaxEngine;
using Game.Game;

namespace Game {
    public class Crystal : Script {
        private float _rotationTime = 0.2f;
        
        public RigidBody RigidBody { get; set; }
        public CrystalColor Color { get; set; }
        public float Force { get; set; } = 2f;
        public int ResetRotationTimer { get; set; } = 10;

        [Tooltip("RED, BLUE, GREEN, YELLOW, ORANGE, PURPLE")]
        public List<Material> CrystalMaterials { get; set; } = new List<Material>();

        public Actor Player { get; set; }

        public override void OnStart() {
            HandleMaterial();
        }

        public override void OnUpdate() {
            if (Player) DebugDraw.DrawLine(Actor.Position, (Player.Transform.Forward - Transform.Up).Normalized * Force, FlaxEngine.Color.Azure);
        }

        public void Die() {
            Destroy(Actor);
        }

        public async void ApplyForce() {
            if (RigidBody == null) return;
            RigidBody.AddForce(Player.LocalTransform.Forward * Force, ForceMode.Impulse);
            await Task.Delay(ResetRotationTimer*1000);
            ResetRotation();

        }

        private void ResetRotation() {
            //Actor.Orientation = Quaternion.Lerp(Actor.Orientation, Quaternion.Identity, _rotationTime);
        }

        public void HandleMaterial() {
            var renderer = Actor.GetChild<StaticModel>();
            switch (Color) {
                case CrystalColor.RED:
                    renderer.SetMaterial(0, CrystalMaterials[0]);
                    break;
                case CrystalColor.BLUE:
                    renderer.SetMaterial(0, CrystalMaterials[1]);
                    break;
                case CrystalColor.GREEN:
                    renderer.SetMaterial(0, CrystalMaterials[2]);
                    break;
                case CrystalColor.YELLOW:
                    renderer.SetMaterial(0, CrystalMaterials[3]);
                    break;
                case CrystalColor.ORANGE:
                    renderer.SetMaterial(0, CrystalMaterials[4]);
                    break;
                case CrystalColor.PURPLE:
                    renderer.SetMaterial(0, CrystalMaterials[5]);
                    break;
            }
        }

        public static CrystalColor Mix(CrystalColor color, CrystalColor otherCrystal) {
            switch (color) {
                case CrystalColor.RED:
                    switch (otherCrystal) {
                        case CrystalColor.BLUE:
                            return CrystalColor.PURPLE;
                        case CrystalColor.YELLOW:
                            return CrystalColor.ORANGE;
                    }
                    break;
                case CrystalColor.BLUE:
                    switch (otherCrystal) {
                        case CrystalColor.RED:
                            return CrystalColor.PURPLE;
                        case CrystalColor.YELLOW:
                            return CrystalColor.GREEN;
                    }
                    break;
                case CrystalColor.YELLOW:
                    switch (otherCrystal) {
                        case CrystalColor.BLUE:
                            return CrystalColor.GREEN;
                        case CrystalColor.RED:
                            return CrystalColor.ORANGE;
                    }
                    break;
                case CrystalColor.MULTI:
                    return CrystalColor.MULTI;
            }

            return CrystalColor.NONE;
        }
    }
}
