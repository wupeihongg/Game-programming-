using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TWB_ass1
{
    class MousePick
    {
        GraphicsDevice device;
        Camera camera;
        Vector3? resultVector;
        Vector3? distance;
        Vector3 direction;
        public MousePick(GraphicsDevice device, Camera camera)
        {
            this.device = device;
            this.camera = camera;
        }
        
        public Vector3 getDirection()
        {
            return direction;
        }
        public Vector3? GetPosition()
        {
            MouseState mouseState = Mouse.GetState();
            Vector3 nearSource = new Vector3(mouseState.X, mouseState.Y, 0f);
            Vector3 farSource = new Vector3(mouseState.X, mouseState.Y, 1f);

            Vector3 nearPoint = device.Viewport.Unproject(
                nearSource,
                camera.projection,
                camera.view,
                Matrix.Identity);

            Vector3 farPoint = device.Viewport.Unproject(
                farSource,
                camera.projection,
                camera.view,
                Matrix.Identity);

            direction = farPoint - nearPoint;
            distance = direction;
            direction.Normalize();

            Ray pickRay = new Ray(nearPoint, direction);

            float? result = pickRay.Intersects(new Plane(Vector3.Up, 0f));
            
            resultVector = direction * result;

            Vector3? collisionPosition = nearPoint + resultVector;
            return collisionPosition;
        }
    }
}
