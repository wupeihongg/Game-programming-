using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TWB_ass1
{
    class Ground : BasicModel
    {
        public Ground (Model model)
            : base(model)
        {
           
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            device.SamplerStates[0] = SamplerState.LinearWrap; //repeat
            base.Draw(device, camera);
        }
}
}
