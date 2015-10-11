using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TWB_ass1
{
    class SkyBox : BasicModel
    {
        public SkyBox (Model model)
            : base(model)
        {

        }

        public override void Update(GameTime gameTime)
        {

            
            //SHIT TO DO: here is for moving the skybox based on the position of the camera
            base.Update(gameTime);
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            //for skybox only.
            device.SamplerStates[0] = SamplerState.LinearClamp;

            base.Draw(device, camera);
        }

        protected override Matrix GetWorld()
        {
            return Matrix.CreateScale(1000f);
        }
    }
}
