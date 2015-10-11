using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace TWB_ass1
{
    public class CubeProjectile : BasicModel
    {
        public List<BoundingBox> cubeProjectileBoxes = new List<BoundingBox>();
        public bool IsActive;
        public static Vector3 Position;
        public static Vector3 Velocity;
        public static bool ShotActive = false;
        public static bool HitProcessed = true;
        private static float elapsed = 0;
        public Vector3 currentPos;
        public Vector3 startPos;
        public Vector3 currentDir;
        int boxIndex = 0;
        int speed = 500;
        Matrix scale = Matrix.Identity;
        GraphicsDevice device;

        public CubeProjectile(Model model, GraphicsDevice device, Vector3 position, Vector3 direction)
            : base(model)
        {
            this.device = device;
            startPos = position;
            currentDir = direction;
            Velocity = direction;
            Position = position;
            scale = Matrix.CreateScale(0.01f, 0.01f, 0.01f);
            IsActive = true;
            MeshModel();
        }
        public void MeshModel()
        {
            for (int i = 0; i < cubeProjectileBoxes.Count() - 1; i++)
            {
                cubeProjectileBoxes.RemoveAt(i);
                i--;
            }  
            
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            
            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix meshTransform = transforms[mesh.ParentBone.Index];
                cubeProjectileBoxes.Add(BuildBoundingBox(mesh, meshTransform));
                boxIndex = cubeProjectileBoxes.Count() - 1;
            }
        }
        private BoundingBox BuildBoundingBox(ModelMesh mesh, Matrix meshTransform)
        {
            Vector3 meshMax = new Vector3(float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue);
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, stride);

                Vector3 vertPosition = new Vector3();

                for (int i = 0; i < vertexData.Length; i++)
                {
                    vertPosition = vertexData[i].Position;

                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }
            }
            meshMin = Vector3.Transform(meshMin, meshTransform);
            meshMax = Vector3.Transform(meshMax, meshTransform);

            BoundingBox box = new BoundingBox(meshMin, meshMax);
            return box;
        }
        public void ResetShot(Vector3 position, Vector3 velocity)
        {
            Position = position;
            Velocity = velocity;
            IsActive = true;
        }
        public override void Update(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds/1000;

            Position += (Velocity * elapsed*speed);

            if (Position.Y < 0)
            {
                IsActive = false;
            } 
            MeshModel();
            base.Update(gameTime);
        }
        public void Draw(Camera camera)
        {
            if (IsActive)
            {
                BoxRenderer.Render(cubeProjectileBoxes[0], device, camera.view, camera.projection, Color.Blue);
                model.Root.Transform = Matrix.Identity*scale*Matrix.CreateTranslation(Position);
                foreach(ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = model.Root.Transform;
                        effect.View = camera.view;
                        effect.Projection = camera.projection;
                        effect.TextureEnabled = true;
                        effect.Alpha = 1;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
