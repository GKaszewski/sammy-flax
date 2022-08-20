using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game {
   
    public class PointsCollector : Script {
        public int PointsToAdd { get; set; } = 10;
        
        private void OnCollisionEnter(Collision collision) {
            Debug.Log($"Collision with {collision.OtherActor.Name}");
            var a = 20;
            var b = 30;
            var c = a + b;
            if (collision.OtherActor.Tag != "Point") return;
            GameManager.Instance.Points += PointsToAdd;
            Destroy(collision.OtherActor.Parent);
        }
    }
}
