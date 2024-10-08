﻿using Microsoft.Xna.Framework;
using System;

namespace MonoEditorEndless.Engine
{
    internal class Camera
    {
        // Camera inital values
        private float _yaw = 0.0f;
        private float _pitch = 0.0f;
        public float _speed = 1.0f;
        public float _sensitivity = 0.1f;
        const float ZOOM = 45.0f;

        Vector3 _cameraPosition;
        public Vector3 _frontVector;
        public Vector3 _upVector;
        public Vector3 _rightVector;

        private Matrix _view;
        public Matrix GetView() { return _view; }
        public Vector3 GetPosition() { return _cameraPosition; }
        public void SetPosition(Vector3 position) { _cameraPosition = position; }

        public Camera()
        {
            _frontVector = Vector3.UnitX;
            _rightVector = Vector3.UnitZ;
            _upVector = Vector3.UnitY;
            _cameraPosition = new Vector3(-150, 50, 0);
            _view = Matrix.CreateLookAt(_cameraPosition, Vector3.Zero, _upVector);
        }

        public void MoveLeft(float amount)
        {
            _cameraPosition += _rightVector * amount * _speed;

            // TODO: Move to camera update
            //_view = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + _frontVector, _upVector);
        }
        public void MoveUp(float amount)
        {
            _cameraPosition += _upVector * amount * _speed;
            // TODO: Move to camera update
            //_view = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + _frontVector, _upVector);
        }
        public void MoveForward(float amount)
        {
            _cameraPosition += _frontVector * amount * _speed;
            // TODO: Move to camera update
            
        }
        public void Rotate(float amountX, float amountY)
        {
            _yaw += _sensitivity * amountX;
            _pitch -= _sensitivity * amountY;

            Vector3 frontVector = Vector3.Zero;
            double yawRadian = (Math.PI / 180) * _yaw;
            double pitchRadian = (Math.PI / 180) * _pitch;

            // Calculate new vector with having changes in angles in Yaw and pitch
            frontVector.X = (float)Math.Cos(yawRadian) * (float)Math.Cos(pitchRadian);
            frontVector.Y = (float)Math.Sin(pitchRadian);
            frontVector.Z = (float)Math.Sin(yawRadian) * (float)Math.Cos(pitchRadian);

            _frontVector = Vector3.Normalize(frontVector);

            // Re-calculate the Right and Up vector
            _rightVector = Vector3.Normalize(Vector3.Cross(_frontVector, Vector3.UnitY));
            _upVector = Vector3.Normalize(Vector3.Cross(_rightVector, _frontVector));
            _view = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + _frontVector, _upVector);
        }
        public void LookAtTarget(Vector3 targetPosition, Vector3 targetForward, float offset = 2f, float height = 0f, float distance = 1f)
        {
            _view = Matrix.CreateLookAt(
                targetPosition + height * Vector3.UnitY - targetForward * offset,
                targetPosition + targetForward * distance,
                _upVector);
        }

        public void Update()
        {
            _view = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + _frontVector, _upVector);
        }

    }
}
