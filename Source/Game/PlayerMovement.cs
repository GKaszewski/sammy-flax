using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game {
    public class PlayerMovement : Script {
        // Groups names for UI
        private const string MOVEMENT_GROUP = "Movement";
        private const string CAMERA_GROUP = "Camera";

        private CharacterController _controller;
        private Vector3 _velocity;

        private float _yaw;
        private float _pitch;
        private float _defaultFov;

        private float _inputH;
        private float _inputV;
        private Vector3 _movement;
        private Vector3 _movementDirection;

        private bool _isJumping = false;

        // Movement
        [ExpandGroups]
        [Tooltip("The character model"), EditorDisplay(MOVEMENT_GROUP, "Character"), EditorOrder(2)]
        public Actor CharacterObj { get; set; } = null;

        [Range(0f, 300f), Tooltip("Movement speed factor"), EditorDisplay(MOVEMENT_GROUP, "Speed"), EditorOrder(3)]
        public float Speed { get; set; } = 250;

        [Range(0f, 600f), Tooltip("Movement speed factor"), EditorDisplay(MOVEMENT_GROUP, "Sprint Speed"), EditorOrder(4)]
        public float SprintSpeed { get; set; } = 300;
        public float RotationSpeed { get; set; } = 100;

        [Limit(-20f, 20f), Tooltip("Gravity of this character"), EditorDisplay(MOVEMENT_GROUP, "Gravity"), EditorOrder(5)]
        public float Gravity { get; set; } = -9.81f;

        [Range(0f, 25f), Tooltip("Jump factor"), EditorDisplay(MOVEMENT_GROUP, "Jump Strength"), EditorOrder(6)]
        public float JumpStrength { get; set; } = 10;

        // Camera
        [ExpandGroups]
        [Tooltip("The camera view for player"), EditorDisplay(CAMERA_GROUP, "Camera View"), EditorOrder(8)]
        public Camera CameraView { get; set; } = null;

        [Range(0, 10f), Tooltip("Sensitivity of the mouse"), EditorDisplay(CAMERA_GROUP, "Mouse Sensitivity"), EditorOrder(9)]
        public float MouseSensitivity { get; set; } = 100f;

        [Range(0f, 20f), Tooltip("Lag of the camera, lower = slower"), EditorDisplay(CAMERA_GROUP, "Camera Lag"), EditorOrder(10)]
        public float CameraLag { get; set; } = 10;

        [Range(0f, 100f), Tooltip("How far to zoom in, lower = closer"), EditorDisplay(CAMERA_GROUP, "FOV Zoom"), EditorOrder(11)]
        public float FOVZoom { get; set; } = 50;

        [Tooltip("Determines the min and max pitch value for the camera"), EditorDisplay(CAMERA_GROUP, "Pitch Min Max"), EditorOrder(12)]
        public Vector2 PitchMinMax { get; set; } = new Vector2(-45, 45);


        public override void OnStart() {
            _controller = (CharacterController)Actor;

            if (!CameraView || !CharacterObj) {
                Debug.LogError("No Character or Camera assigned!");
                return;
            }

            _defaultFov = CameraView.FieldOfView;
        }

        public override void OnFixedUpdate() {
            HandleCameraRotation();
            HandlePlayerMovement();
        }

        private void GetCameraInput() {
            _yaw += Input.GetAxis("Mouse X") * MouseSensitivity * Time.DeltaTime; // H
            _pitch += Input.GetAxis("Mouse Y") * MouseSensitivity * Time.DeltaTime; // V
            _pitch = Mathf.Clamp(_pitch, PitchMinMax.X, PitchMinMax.Y);
        }

        private void GetKeyboardInput() {
            _inputH = Input.GetAxis("Horizontal");
            _inputV = Input.GetAxis("Vertical");
        }

        private void HandleCameraOrientation() {
            if (_isJumping) return;
            //CameraView.Parent.Orientation = Quaternion.Lerp(CameraView.Parent.Orientation, Quaternion.Euler(Vector3.Up * _inputH * RotationSpeed), Time.DeltaTime * CameraLag);
            //CharacterObj.Orientation = Quaternion.Euler(Vector3.Up * _inputH * RotationSpeed);
            CameraView.Parent.EulerAngles += Vector3.Up * _inputH * RotationSpeed * Time.DeltaTime;
            CharacterObj.EulerAngles += Vector3.Up * _inputH * RotationSpeed * Time.DeltaTime;
        }

        private void HandleCameraRotation() {
            GetCameraInput();
            HandleCameraOrientation();
        }

        private void ApplyMovement() {
            _movement = Actor.Transform.Forward * _inputV;
            _movementDirection = CameraView.Transform.TransformDirection(_movement);
        }

        private void HandleJumping() {
            if (_controller.IsGrounded && Input.GetAction("Jump")) {
                _isJumping = true;
                _velocity.Y = Mathf.Sqrt(JumpStrength * -2f * Gravity);
            } else _isJumping = false;
        }

        private void ApplyGravity() {
            // Apply gravity
            _velocity.Y += Gravity * Time.DeltaTime;
            _movementDirection += (_velocity * 0.5f);
        }

        private void ApplyMovementToController() {
            // Apply controller movement, evaluate whether we are sprinting or not
            _controller.Move(_movementDirection * Time.DeltaTime * (Input.GetAction("Sprint") ? SprintSpeed : Speed));
        }

        private void HandlePlayerMovement() {
            GetKeyboardInput();
            ApplyMovement();
            HandleJumping();
            ApplyGravity();
            ApplyMovementToController();
        }
    }
}
