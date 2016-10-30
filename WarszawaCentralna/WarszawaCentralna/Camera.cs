using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarszawaCentralna
{
    class Camera
    {
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; private set; }
        public Vector3 UpVector { get; private set; }
        float speed = 0.5F;

        public Camera(Vector3 _position, Vector3 _target, Vector3 _upVector, Matrix _projectionMatrix)
        {
            Position = _position;
            Target = _target;
            UpVector = _upVector;
            ProjectionMatrix = _projectionMatrix;
            CreateLookAt();
        }

        public void Update()
        {
            Vector3 cameraDirection = Target - Position;
            float angle = MathHelper.PiOver4 / 20;

            if (Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                cameraDirection.Normalize();
                Position += cameraDirection * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                cameraDirection.Normalize();
                Position -= cameraDirection * speed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Vector3 right = Vector3.Cross(UpVector, cameraDirection);
                right.Normalize();
                Position += right * speed;
                Target += right * speed;
                /*
                Vector3 right = Vector3.Cross(UpVector, cameraDirection);
                right.Normalize();
                Position += right * speed;
                */
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Vector3 right = Vector3.Cross(UpVector, cameraDirection);
                right.Normalize();
                Position -= right * speed;
                Target -= right * speed;
                /*
                //Position -= Vector3.Cross(UpVector, cameraDirection) * speed;
                Vector3 right = Vector3.Cross(UpVector, cameraDirection);
                right.Normalize();
                Position -= right * speed;
                */
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Position += UpVector * speed;
                Target += UpVector * speed;

                /*
                float lookAtVectorLength = cameraDirection.Length();
                Vector3 right = Vector3.Cross(UpVector, cameraDirection);
                right.Normalize();
                UpVector = Vector3.Transform(UpVector, Matrix.CreateFromAxisAngle(right, angle));
                UpVector.Normalize();
                cameraDirection = Vector3.Cross(right, UpVector);
                cameraDirection.Normalize();
                cameraDirection = cameraDirection * lookAtVectorLength;
                Position = Target - cameraDirection;
                */

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Position -= UpVector * speed;
                Target -= UpVector * speed;

                /*
                float lookAtVectorLength = cameraDirection.Length();
                Vector3 right = Vector3.Cross(UpVector, cameraDirection);
                right.Normalize();
                UpVector = Vector3.Transform(UpVector, Matrix.CreateFromAxisAngle(right, -angle));
                UpVector.Normalize();
                cameraDirection = Vector3.Cross(right, UpVector);
                cameraDirection.Normalize();
                cameraDirection = cameraDirection * lookAtVectorLength;
                Position = Target - cameraDirection;
                */
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(UpVector, angle));
                Target = Position + cameraDirection;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(UpVector, -angle));
                Target = Position + cameraDirection;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Vector3 right = Vector3.Cross(UpVector, cameraDirection);
                right.Normalize();
                cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(right, angle));
                UpVector = Vector3.Transform(UpVector, Matrix.CreateFromAxisAngle(right, angle));
                Target = Position + cameraDirection;
                UpVector.Normalize();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Vector3 right = Vector3.Cross(UpVector, cameraDirection);
                right.Normalize();
                cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(right, -angle));
                UpVector = Vector3.Transform(UpVector, Matrix.CreateFromAxisAngle(right, -angle));
                Target = Position + cameraDirection;
                UpVector.Normalize();
            }
            //Console.WriteLine("UP " + UpVector.ToString() + " POS " + Position.ToString() + " TAR " + Target.ToString());
            CreateLookAt();
        }

        private void CreateLookAt()
        {
            ViewMatrix = Matrix.CreateLookAt(Position, Target, UpVector);
        }
    }
}