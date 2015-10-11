using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


namespace TWB_ass1
{
    public class Player : BasicModel
    {
        public Vector3 playerPos{ get; protected set; }
        public Vector3 playerDir { get; protected set; }
        Vector3 moveDirection;
        Vector3 cameraUp;
        Vector3 target;
        String boxName = "Player";
        MouseState currentMouse;
        
        
        float xzSpeed = 0;
        float shotSpeed = 1f;
        float xzMax = 300;
        float xzMin = -300;
        float xzDefault = 300;
        float ySpeed = 0;
        float yMin = 50;
        float yPos = 50;
        float xPos = 0;
        float time = 0;
        float xzAcceleration = 20;
        float yAcceleration = 30;
        public int health = 3;

        bool crouching = false;
        public bool isHit = false;
        bool jumping = false;
        bool jumpPressed = false;
        bool sprinting = false;
        bool strafing = false;
        bool xzInput = false;
        bool isFloorColliding = false;
        bool isWallColliding = false;
        bool maxCrouched = false;
        Matrix scale = Matrix.Identity;
        Matrix normalScale = Matrix.CreateScale(0.25f, 1, 0.25f);
        Matrix crouchingScale = Matrix.CreateScale(0.25f, 0.5f, 0.25f);
        List<BoundingBox> boundingBoxes;
        BoundingBox currentBox;
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix[] transforms;
        float crouchHeight;
        public Camera camera{ get; protected set; }
        MouseState prevMouseState;
        int boxIndex;
        bool first = true;
        Game game;


        public Player (Model model, GraphicsDevice device, Game game)
            : base (model)
        {
            this.game = game;

            scale = normalScale;
            cameraUp = Vector3.Up;
            //target = new Vector3(0, 50, 0);
            
            camera = ((Game1)game).camera;
            playerPos = camera.cameraPosition;
            translation.Translation = playerPos;
            Mouse.SetPosition(game.Window.ClientBounds.Width / 2,
                game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();
            CreateLookAt();
            
        }
        public void checkIntersects()
        {
            isFloorColliding = false;
            foreach (BoundingBox playerBox in BoundingBoxes.playerBoxes)
            {
                foreach (Platform plat in Platforms.platforms)
                {
                    foreach (BoundingBox platBox in plat.platformBoxes)
                    {
                        if (playerBox.Contains(platBox) != ContainmentType.Disjoint)
                        {
                            isFloorColliding = true;
                        }
                    }
                }
            }
            isWallColliding = false;
            foreach (BoundingBox playerBox in BoundingBoxes.playerBoxes)
            {
                foreach (Wall wall in Walls.walls)
                {
                    foreach (BoundingBox wallBox in wall.wallBoxes)
                    {
                        if (playerBox.Contains(wallBox) != ContainmentType.Disjoint)
                        {
                            isWallColliding = true;
                        }
                    }
                }
            }
        }
        public void MeshModel()
        {
            
            for (int i = 0; i < BoundingBoxes.playerBoxes.Count() - 1; i++)
            {
                if(i!=boxIndex)
                    BoundingBoxes.playerBoxes.RemoveAt(i);
                
            }
            
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix meshTransform = transforms[mesh.ParentBone.Index];
                BoundingBoxes.playerBoxes.Add(BuildBoundingBox(mesh, meshTransform));
                boxIndex = BoundingBoxes.playerBoxes.Count()-1;
                
            }
        }
        private void setCurrentBox(BoundingBox currentBox)
        {
            this.currentBox = currentBox;
        }
        private BoundingBox getCurrentBox(){
            return currentBox;
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
            meshMin = Vector3.Transform(meshMin, meshTransform) * scale.Scale + playerPos;
            meshMax = Vector3.Transform(meshMax, meshTransform) * scale.Scale + playerPos;

            BoundingBox box = new BoundingBox(meshMin, meshMax);
            currentBox = box;
            //
            
            
            return box;
        }
        public Vector3 getPlayerPosition()
        {
            return playerPos;
        }
        private void CreateLookAt()
        {

            camera.cameraPosition = playerPos;
            //camera.cameraRot = playerDir;
        }
        private void Gravity(float time)
        {
            if(!strafing)
                moveDirection = playerDir;
            if (!isFloorColliding && !jumpPressed)
            {
                ySpeed -= yAcceleration * time;
            }
            else if (jumpPressed)
            {
                jumpPressed = false;
                if (!crouching)
                {
                    ySpeed = yAcceleration/ 3;
                }
                else
                    ySpeed = yAcceleration / 3.5f;
            }
            else if(isFloorColliding)
            {
                ySpeed = 0;
                jumping = false;
            } 
            yPos += ySpeed;
            playerPos = new Vector3(playerPos.X, yPos, playerPos.Z);
        }
        
        private void xzAccelerate(float time)
        {
            xzInput = false;
            strafing = false;
            sprinting = false;
            crouching = false;
            Strafe();
            Sprint();
            //Crouch();
            setDefault();
            
            if (isFloorColliding && !xzInput && xzSpeed > 0)
            {
                SoundEffectInstance move = ((Game1)this.game).movingMusic;
                move.Play();
                xzSpeed -= xzAcceleration/4;
                if (xzSpeed < 0)
                {
                    xzSpeed = 0;
                }
            } else if (isFloorColliding && !xzInput && xzSpeed < 0)
            {
                SoundEffectInstance move = ((Game1)this.game).movingMusic;
                move.Play();
                xzSpeed += xzAcceleration/4;
                if (xzSpeed > 0)
                {
                    xzSpeed = 0;
                }
            }
            else if (isFloorColliding && xzInput)
            {
                SoundEffectInstance move = ((Game1)this.game).movingMusic;
                move.Play();
            }
            if (xzSpeed < xzMin) 
            { 
                xzSpeed = xzMin; 
            } else if (xzSpeed > xzMax)
            {
                xzSpeed = xzMax;
            }
            if (xzSpeed == 0)
            {
                SoundEffectInstance move = ((Game1)this.game).movingMusic;
                move.Stop();
            }
            if (isWallColliding)
                playerPos -= moveDirection * xzSpeed * time;
            else
                playerPos += moveDirection * xzSpeed * time;
        }
        private void Jump()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !jumping)
            {
                isFloorColliding = false;
                jumping = true;
                jumpPressed = true;
            }
        }
        private void Strafe()
        {
            if(strafing && !crouching && !sprinting)
            {
                xzMax = xzDefault * 0.75f;
                xzMin = -xzDefault * 0.75f;
            }
        }
        private void Crouch()
        {

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                crouching = true;
                scale = crouchingScale;
                
                if (!strafing && !jumping)
                {
                    xzMax = xzDefault * 0.35f;
                    xzMin = -xzDefault * 0.35f;
                }else if(!jumping)
                {
                    xzMax = xzDefault * 0.15f;
                    xzMin = -xzDefault * 0.15f;
                }
            }
            else
            {
                crouching = false;
                scale = normalScale;

            }
            
        }
        
        private void setDefault()
        {
            if (!crouching && !strafing && !sprinting)
            {
                xzMax = xzDefault;
                xzMin = -xzDefault;
            }   
        }
        private void Sprint()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.LeftShift) && !crouching)
                sprinting = true;
            if (sprinting && !strafing)
                xzMax = xzDefault * 1.5f;
            else if(sprinting && strafing)
                xzMax = xzDefault;
        }


        private void Input(float time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                xzInput = true;
                xzSpeed += xzAcceleration;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                xzInput = true;
                xzSpeed -= xzAcceleration;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                xzInput = true;
                strafing = true;
                xzSpeed += xzAcceleration;
                moveDirection = Vector3.Cross(cameraUp, playerDir);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                xzInput = true;
                strafing = true;
                xzSpeed += xzAcceleration;
                moveDirection = -Vector3.Cross(cameraUp, playerDir);
            }
            
            /*playerDir = Vector3.Transform(playerDir,
                Matrix.CreateFromAxisAngle(cameraUp, 
                time * (-MathHelper.PiOver4/4) *
                (Mouse.GetState().X - prevMouseState.X)));

            playerDir = Vector3.Transform(playerDir,
                Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, playerDir),
                time * (MathHelper.PiOver4 / 2.5f) *
                (Mouse.GetState().Y - prevMouseState.Y)));*/
        }
        

        public void shootSound()
        {
            SoundEffectInstance shoot = ((Game1)this.game).shootMusic;
            shoot.Play();
        }

        
        public void DeadSound()
        {
            if (health <= 0)
            {
                ((Game1)game).currentGameState = GameState.GameOver;
                SoundEffectInstance deathSound = ((Game1)this.game).dieMusic;
                deathSound.Play();
            }
        }


        public override void Update(GameTime gameTime)
        {
            currentMouse = Mouse.GetState();
            translation.Translation = playerPos;
            isHit = false;
            playerDir = camera.cameraDir;
            time = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
            xPos = playerPos.X;
            if(!strafing && moveDirection == playerDir)moveDirection = playerDir;

            Input(time);
            MeshModel();
            checkIntersects();
            Jump();
            Gravity(time);
            xzAccelerate(time);

            
            if (health == 0)
            {
                health -= 1;
                DeadSound();
            }
            CreateLookAt();
            base.Update(gameTime);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere sphere = mesh.BoundingSphere;
                BoundingBox box = BoundingBox.CreateFromSphere(sphere);
                
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = transforms[mesh.ParentBone.Index] * GetWorld();
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.TextureEnabled = true;
                    effect.Alpha = 1;
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
