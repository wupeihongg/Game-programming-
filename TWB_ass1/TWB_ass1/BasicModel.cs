using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//this is a parent
namespace TWB_ass1
{
    public class BasicModel
    {
        public Model model { get; protected set; }

        public BasicModel(Model model)
        {
            this.model = model;

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GraphicsDevice device, Camera camera)
        {
            //this is where the work is done
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.World = mesh.ParentBone.Transform * GetWorld();
                    effect.World = transforms[mesh.ParentBone.Index] * GetWorld();
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.TextureEnabled = true;
                    effect.Alpha = 1;
                }
                mesh.Draw();

            }
        }

            protected virtual Matrix GetWorld()
            {
                return Matrix.Identity;
            }

    }
}
