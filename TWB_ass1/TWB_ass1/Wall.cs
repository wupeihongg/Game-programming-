using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TWB_ass1
{
    public class Wall : BasicModel
    {
        Matrix scale = Matrix.Identity;
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        //Matrix meshTransform = Matrix.Identity;
        //Model model;
        Matrix[] transforms;
        //float speed = 3;
        float time;
        float distance;
        bool isMoving = false;
        bool isRotating = false;
        bool isColliding = false;
        float newAngle;
        ModelBone firstBone;
        ModelBone secondBone;
        Vector3 direction;
        Vector3 currentPos = Vector3.Zero;
        Vector3 targetPos;
        Vector3 pastPos;
        Vector3? collisionPosition;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        float speed = 100;
        int count;
        Player player;
        BoundingBox bounds;
        GraphicsDevice device;
        int boxIndex;
        bool ground = false;
        public List<BoundingBox> wallBoxes = new List<BoundingBox>();

        public Wall(Model model, GraphicsDevice device, Vector3 pos, Matrix Scale)
            : base(model)
        {
            scale = Scale;
            this.device = device;
            firstBone = model.Bones.First();
            secondBone = model.Bones.Last();
            transforms = new Matrix[model.Bones.Count];
            currentPos = pos;
            translation.Translation = pos;

        }
        

        public void MeshModel()
        {
            for (int i = 0; i < wallBoxes.Count() - 1; i++)
            {
                wallBoxes.RemoveAt(i);
                i--;
            }

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix meshTransform = transforms[mesh.ParentBone.Index];
                wallBoxes.Add(BuildBoundingBox(mesh, meshTransform));
            }
        }
        private BoundingBox BuildBoundingBox(ModelMesh mesh, Matrix meshTransform)
        {
            // Create initial variables to hold min and max xyz values for the mesh
            Vector3 meshMax = new Vector3(float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue);
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                // The stride is how big, in bytes, one vertex is in the vertex buffer
                // We have to use this as we do not know the make up of the vertex
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, stride);

                // Find minimum and maximum xyz values for this mesh part
                Vector3 vertPosition = new Vector3();

                for (int i = 0; i < vertexData.Length; i++)
                {
                    vertPosition = vertexData[i].Position;

                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }
            }

            // transform by mesh bone matrix
            meshMin = Vector3.Transform(meshMin, meshTransform) * scale.Scale + currentPos;
            meshMax = Vector3.Transform(meshMax, meshTransform) * scale.Scale + currentPos;
            // Create the bounding box
            BoundingBox box = new BoundingBox(meshMin, meshMax);
            return box;
        }
        
        public override void Update(GameTime gameTime)
        {
           
            time = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
            translation.Translation = currentPos;

            MeshModel();
            //checkIntersects();
            base.Update(gameTime);
            
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            
            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = transforms[mesh.ParentBone.Index] * GetWorld();
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = new Vector3(1f, 0.5f, 0.5f);
                    effect.EmissiveColor = new Vector3(0.2f, 0.2f, 0.2f);
                }
                mesh.Draw();
            }
            
        }
        protected override Matrix GetWorld()
        {
            return scale * rotation * translation;
        }
    }
}
