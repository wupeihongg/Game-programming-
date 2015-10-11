using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TWB_ass1
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Vector3 cameraPos { get; set; }
        public Vector3 cameraDir { get; set; }
        public float cameraSpeed;
        public Vector3 cameraLookAt;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        public float xAngle;
        public float yAngle;
        public Vector3 cameraPosition;
        Vector2 screenCenter;
        float YrotationSpeed;
        float XrotationSpeed;
        float yaw = 0;
        float pitch = 0;
        Vector3 cameraFinalTarget;

        //properties

        public Matrix view { get; set; }
        public Matrix projection { get; set; }

        public Camera(Game game, Vector3 position, Vector3 target, Vector3 rotation, float speed)
            : base(game)
        {
            
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
               (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height,
                1, 3000);
            YrotationSpeed = 0.2f;
            XrotationSpeed = 0.2f;

            screenCenter = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height) / 2;
        }
        public Vector3 QuaternionToEuler(Quaternion q)
        {
            Vector3 v = new Vector3();

            v.X = (float)Math.Atan2
                (
                2 * q.Y * q.W - 2 * q.X * q.Z,
                    1 - 2 * Math.Pow(q.Y, 2) - 2 * Math.Pow(q.Z, 2)
            );

            v.Y = (float)Math.Asin
            (
                2 * q.X * q.Y + 2 * q.Z * q.W
            );

            v.Z = (float)Math.Atan2
            (
                2 * q.X * q.W - 2 * q.Y * q.Z,
                1 - 2 * Math.Pow(q.X, 2) - 2 * Math.Pow(q.Z, 2)
            );

            if (q.X * q.Y + q.Z * q.W == 0.5)
            {
                v.X = (float)(2 * Math.Atan2(q.X, q.W));
                v.Z = 0;
            }

            else if (q.X * q.Y + q.Z * q.W == -0.5)
            {
                v.X = (float)(-2 * Math.Atan2(q.X, q.W));
                v.Z = 0;
            }

            return v;
        }
        public void UpdateCamera(float yaw, float pitch, Vector3 position)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);
            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            cameraDir = cameraRotatedTarget;
            cameraFinalTarget = position + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, cameraRotation);

            view = Matrix.CreateLookAt(position, cameraFinalTarget, cameraRotatedUpVector);
        }
        
        private void HandleMouse(GameTime gameTime)
        {
            currentMouseState = Mouse.GetState();
            float amount = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            if (currentMouseState.X != screenCenter.X)
            {
                yaw -= YrotationSpeed * (currentMouseState.X - screenCenter.X) * amount;
            }
            if (currentMouseState.Y != screenCenter.Y)
            {
                pitch -= XrotationSpeed * (currentMouseState.Y - screenCenter.Y) * amount;
                if (pitch > MathHelper.ToRadians(90))
                    pitch = MathHelper.ToRadians(90);
                if (pitch < MathHelper.ToRadians(-50)) //Preventing the user from looking all the way down (and seeing the character doesn't have any legs..)
                    pitch = MathHelper.ToRadians(-50);
            }
            try
            {
                Mouse.SetPosition((int)screenCenter.X, (int)screenCenter.Y);
            }
            catch { }
        }
        public override void Update(GameTime gameTime)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            currentMouseState = Mouse.GetState();
            HandleMouse(gameTime);
            UpdateCamera(yaw, pitch, cameraPosition);
            //Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            previousMouseState = currentMouseState;
            //CreateLookAt();
            base.Update(gameTime);
        }
    }
}