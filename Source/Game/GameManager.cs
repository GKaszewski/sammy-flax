using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game {

    public class GameManager : Script {
        public static GameManager Instance {get; private set;}
        
        public int MaxPoints { get; set; }
        public int Points { get; set; }

        public override void OnAwake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;
        }
    }
}
