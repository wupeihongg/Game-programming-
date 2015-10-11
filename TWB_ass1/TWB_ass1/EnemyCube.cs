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
    public class EnemyCube : BasicModel
    {
        ShotManager shotManager;
        Matrix scale = Matrix.Identity;
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix[] transforms;
        float time;
        bool playerHit = false;
        bool isColliding = false;
        bool dead = false;
        ModelBone firstBone;
        ModelBone secondBone;
        Vector3 direction;
        Vector3 currentPos = Vector3.Zero;
        float speed = 100;
        Player player;
        GraphicsDevice device;
        Game game;
        Boolean melee;
        public List<BoundingBox> enemyBoxes = new List<BoundingBox>();
        Camera camera;

        public EnemyCube(Model model, GraphicsDevice device, Game game, Player player, ShotManager shotManager, Vector3 position, Camera camera, Boolean melee)
            : base(model)
        {
            this.melee = melee;
            this.camera = camera;
            this.shotManager = shotManager;
            this.game = game;
            scale = Matrix.CreateScale(0.3f, 0.3f, 0.3f);
            this.player = player;
            this.device = device;
            firstBone = model.Bones.First();
            secondBone = model.Bones.Last();
            transforms = new Matrix[model.Bones.Count];
            currentPos = position;
            direction = new Vector3(0, 0, 0);
           
        }
        public void Movement(float time)
        {
            if (melee == true)
            {
                if (player.playerPos.X >= 0 || player.playerPos.X < 0)
                    direction = player.playerPos - currentPos;
                direction.Normalize();
                if (!isColliding)
                    currentPos += direction * speed * time;
                rotation = Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), (float)Math.Atan2(direction.X, direction.Z));
            }
            else if (melee == false)
            {
                if (player.playerPos.X >= 0 || player.playerPos.X < 0)
                    direction = -player.playerPos + currentPos;
                direction.Normalize();
                if (!isColliding)
                    currentPos += direction * speed * time;
                rotation = Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), (float)Math.Atan2(direction.X, direction.Z));
            }

        }
        
        public void MeshModel()
        {
            
            for(int i = 0; i < enemyBoxes.Count() - 1; i++)
            {
                enemyBoxes.RemoveAt(i);
                i--;
            }
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix meshTransform = transforms[mesh.ParentBone.Index];
                enemyBoxes.Add(BuildBoundingBox(mesh, meshTransform));
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
            meshMin = Vector3.Transform(meshMin, meshTransform) * scale.Scale + currentPos;
            meshMax = Vector3.Transform(meshMax, meshTransform) * scale.Scale + currentPos;

            BoundingBox box = new BoundingBox(meshMin, meshMax);
            return box;
        }
        public void HitByEnemySound()
        {
            SoundEffectInstance hitBy = ((Game1)game).hitByMusic;
            hitBy.Play();
        }
        public void checkIntersects()
        {
            isColliding = false;
            //Console.Out.WriteLine("proj:" + shotManager.CubeProjectiles.Count());
            for (int j = 0; j < enemyBoxes.Count() - 1; j++)
            {
                int inactive = 0;
                BoundingBox box1 = enemyBoxes[j];
                for (int f = 0; f < shotManager.CubeProjectiles.Count() - inactive - 1; f++)
                {
                    
                    for (int k = 0; k < shotManager.CubeProjectiles[f].cubeProjectileBoxes.Count() - 1; k++)
                    {
                        //Console.Out.WriteLine("boxes:" + shotManager.CubeProjectiles[f].cubeProjectileBoxes.Count());
                        BoundingBox box3 = shotManager.CubeProjectiles[f].cubeProjectileBoxes[k];
                        if (box1.Contains(box3) != ContainmentType.Disjoint)
                        {
                            shotManager.CubeProjectiles[f].cubeProjectileBoxes.RemoveAt(k);
                            k--;
                            GlobalVariables.score++;
                            shotManager.CubeProjectiles[f].IsActive = false;
                            dead = true;
                        }
                    } 
                    if (!shotManager.CubeProjectiles[f].IsActive)
                        inactive++;
                }
                for (int i = 0; i <= BoundingBoxes.playerBoxes.Count() - 1; i++)
                {
                    BoundingBox box2 = BoundingBoxes.playerBoxes[i];
               
                    if (box1.Contains(box2) != ContainmentType.Disjoint)
                    {
                        HitByEnemySound();
                        player.isHit = true;
                        playerHit = true;
                        dead = true;
                        isColliding = true;
                    }
                }
                if (dead)
                {
                    if(playerHit)
                        player.health -= 1;
                    
                    enemyBoxes.RemoveAt(j);
                    j--;
                    Enemies.enemyCubes.Remove(this);
                    ModelManager.models.Remove(this);
                }
            }
        }

        public void hitEnemy()
        {
            SoundEffectInstance hit = ((Game1)this.game).hitMusic;
            hit.Play();
        }


        public override void Update(GameTime gameTime)
        {
            if (!dead)
            {
                time = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
                translation.Translation = currentPos;

                MeshModel();
                checkIntersects();
                Movement(time);
            }
            base.Update(gameTime);
        }
        
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            BoxRenderer.Render(enemyBoxes[0], device, camera.view, camera.projection, Color.Red);
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
                    effect.AmbientLightColor = new Vector3(0.6f, 0.3f, 0.1f);
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
