using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace TWB_ass1
{
    class Crate : Microsoft.Xna.Framework.DrawableGameComponent
    {


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;

        VertexPositionNormalTexture[] verts;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        BasicEffect effect;

        Matrix scaling = Matrix.Identity;
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;

        short[] indices;
        MouseState currentMouse;
        MouseState pastMouse;
        Texture2D texture;

        public Crate(Game game, Camera camera, BasicEffect effect, GameTime gameTime) :
            base(game){
                this.camera = camera;
                this.effect = effect;
                LoadContent();
                DrawCrate(gameTime);
                translateCube(new Vector3(10, 60, 10));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
       

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(((Game1)Game).GraphicsDevice);


            Vector3 frontTopLeftPos = new Vector3(-1, 1, 1);
            Vector3 frontTopRightPos = new Vector3(1, 1, 1);
            Vector3 frontBottomRightPos = new Vector3(1, -1, 1);
            Vector3 frontBottomLeftPos = new Vector3(-1, -1, 1);

            Vector3 backTopLeftPos = new Vector3(-1, 1, -1);
            Vector3 backTopRightPos = new Vector3(1, 1, -1);
            Vector3 backBottomRightPos = new Vector3(1, -1, -1);
            Vector3 backBottomLeftPos = new Vector3(-1, -1, -1);

            Vector2 frontTopLeftTex = new Vector2(0f, 0f);
            Vector2 frontTopRightTex = new Vector2(0.5f, 0f);
            Vector2 frontBotRightTex = new Vector2(0.5f, 0.5f);
            Vector2 frontBotLeftTex = new Vector2(0f, 0.5f);

            Vector2 backTopLeftTex = new Vector2(0f, 0f);
            Vector2 backTopRightTex = new Vector2(0.5f, 0f);
            Vector2 backBotRightTex = new Vector2(0.5f, 0.5f);
            Vector2 backBotLeftTex = new Vector2(0f, 0.5f);

            Vector2 leftTopLeftTex = new Vector2(0.5f, 0.5f);
            Vector2 leftTopRightTex = new Vector2(1f, 0.5f);
            Vector2 leftBotRightTex = new Vector2(1f, 0.5f);
            Vector2 leftBotLeftTex = new Vector2(0.5f, 0.5f);

            Vector3 frontNorm = new Vector3(0, 0, -1);
            Vector3 backNorm = new Vector3(0, 0, 1);
            Vector3 leftNorm = new Vector3(-1, 0, 0);
            Vector3 rightNorm = new Vector3(1, 0, 0);
            Vector3 topNorm = new Vector3(0, 1, 0);
            Vector3 botNorm = new Vector3(0, -1, 0);



            verts = new VertexPositionNormalTexture[24]{
                //front
                new VertexPositionNormalTexture(frontTopLeftPos, frontNorm, frontTopLeftTex),
                new VertexPositionNormalTexture(frontTopRightPos, frontNorm, frontTopRightTex),
                new VertexPositionNormalTexture(frontBottomRightPos, frontNorm, frontBotRightTex),
                new VertexPositionNormalTexture(frontBottomLeftPos, frontNorm, frontBotLeftTex),
                //back
                new VertexPositionNormalTexture(backTopLeftPos, backNorm, backTopLeftTex),
                new VertexPositionNormalTexture(backTopRightPos, backNorm, backTopRightTex),
                new VertexPositionNormalTexture(backBottomRightPos, backNorm, backBotRightTex),
                new VertexPositionNormalTexture(backBottomLeftPos, backNorm, backBotLeftTex),
                //left
                new VertexPositionNormalTexture(backTopLeftPos, leftNorm, leftTopLeftTex),
                new VertexPositionNormalTexture(frontTopLeftPos, leftNorm, leftTopRightTex),
                new VertexPositionNormalTexture(frontBottomLeftPos, leftNorm, leftBotRightTex),
                new VertexPositionNormalTexture(backBottomLeftPos, leftNorm, leftBotLeftTex),
                //right
                new VertexPositionNormalTexture(frontTopRightPos, rightNorm, backTopLeftTex),
                new VertexPositionNormalTexture(backTopRightPos, rightNorm, backTopRightTex),
                new VertexPositionNormalTexture(backBottomRightPos, rightNorm, backBotRightTex),
                new VertexPositionNormalTexture(frontBottomRightPos, rightNorm, backBotLeftTex),
                
                new VertexPositionNormalTexture(backTopLeftPos, topNorm, backTopLeftTex),
                new VertexPositionNormalTexture(backTopRightPos, topNorm, backTopRightTex),
                new VertexPositionNormalTexture(backBottomRightPos, topNorm, backBotRightTex),
                new VertexPositionNormalTexture(backBottomLeftPos, topNorm, backBotLeftTex),
                
                new VertexPositionNormalTexture(backTopLeftPos, botNorm, backTopLeftTex),
                new VertexPositionNormalTexture(backTopRightPos, botNorm, backTopRightTex),
                new VertexPositionNormalTexture(backBottomRightPos, botNorm, backBotRightTex),
                new VertexPositionNormalTexture(backBottomLeftPos, botNorm, backBotLeftTex),
           
            };
            indices = new short[]{
                //front
                0, 1, 2,
                0, 2, 3,
                //back
                6, 5, 4,
                6, 4, 7,
                //left
                0, 3, 7,
                0, 7, 4,
                //right
                1, 5, 6,
                1, 6, 2,
                //top
                4, 1, 0,
                4, 5, 1,
                //bottom
                2, 7, 3,
                2, 6, 7
            };
            indexBuffer = new IndexBuffer(((Game1)Game).GraphicsDevice, IndexElementSize.SixteenBits, sizeof(short) * indices.Length, BufferUsage.None);
            indexBuffer.SetData(indices);
            vertexBuffer = new VertexBuffer(((Game1)Game).GraphicsDevice, typeof(VertexPositionNormalTexture), verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);

        }
        public void translateCube(Vector3 target)
        {
            translation.Translation = target;
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void DrawCrate(GameTime gameTime)
        {
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            effect.World = scaling * rotation * translation;
            effect.View = camera.view;
            effect.Projection = camera.projection;

            effect.VertexColorEnabled = false;

            

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                ((Game1)Game).GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 12);
            };
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

    }


}
