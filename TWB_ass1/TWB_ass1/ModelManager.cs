using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TWB_ass1
{
    class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public static List<BasicModel> models = new List<BasicModel>();
        Player player;
        Platform ground;
        ShotManager shotManager;
        float playerShotCooldown = 1.0f;
        float playerShotTimer = 0.0f;
        float elapsed;
        EnemyCube enemyCube;
        public ModelManager(Game game)
            : base(game)
        {

        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public EnemyCube addEnemyCube(Vector3 position)
        {
            return new EnemyCube(
                Game.Content.Load<Model>(@"Models/Objects/Cube"),
                ((Game1)Game).GraphicsDevice,
                ((Game1)Game),
                player,
                shotManager,
                position,
                ((Game1)Game).camera,
                false); 
        }
        protected override void LoadContent()
        {
            //pass the model adding as a parameter to the list of models to load it to the list.
            //BasicModel constructor:
            ground = new Platform(
               Game.Content.Load<Model>(@"Models/Objects/Platform"),
               true);
            ground.platformBoxes.Add(new BoundingBox(new Vector3(float.MinValue, -100, float.MinValue),
                new Vector3(float.MaxValue, 0, float.MaxValue)));
            Platforms.platforms.Add(ground);
            //Platforms.platforms.Add(ground); 
            Platforms.platforms.Add(new Platform(
                 Game.Content.Load<Model>(@"Models/Objects/Platform"),
                 ((Game1)Game).GraphicsDevice,
                 new Vector3(200, 0, 200)));
            Platforms.platforms.Add(new Platform(
                 Game.Content.Load<Model>(@"Models/Objects/Platform"),
                 ((Game1)Game).GraphicsDevice,
                 new Vector3(400, 40, 400)));
            Platforms.platforms.Add(new Platform(
                  Game.Content.Load<Model>(@"Models/Objects/Platform"),
                  ((Game1)Game).GraphicsDevice,
                  new Vector3(600, 80, 600)));
            Walls.walls.Add(new Wall(
               Game.Content.Load<Model>(@"Models/Objects/Wall"),
                ((Game1)Game).GraphicsDevice,
                new Vector3(1000, 0, 1000),
                Matrix.CreateScale(40f, 10f, 0.2f)));
            Walls.walls.Add(new Wall(
               Game.Content.Load<Model>(@"Models/Objects/Wall"),
                ((Game1)Game).GraphicsDevice,
                new Vector3(1000, 0, -1000),
                Matrix.CreateScale(40f, 10f, 0.2f)));
            Walls.walls.Add(new Wall(
               Game.Content.Load<Model>(@"Models/Objects/Wall"),
                ((Game1)Game).GraphicsDevice,
                new Vector3(-1000, 0, -1000),
                Matrix.CreateScale(0.2f, 10f, 40f))); 
            Walls.walls.Add(new Wall(
                Game.Content.Load<Model>(@"Models/Objects/Wall"),
                 ((Game1)Game).GraphicsDevice,
                 new Vector3(1000, 0, -1000),
                 Matrix.CreateScale(0.2f, 10f, 40f)));
            foreach (Wall wall in Walls.walls)
            {
                models.Add(wall);
            }
            foreach (Platform plat in Platforms.platforms)
            {
                models.Add(plat);
            }
            shotManager = new ShotManager(
                  Game.Content.Load<Model>(@"Models/Objects/CubeShot"),
                ((Game1)Game).GraphicsDevice);
            player = new Player(
                  Game.Content.Load<Model>(@"Models/Objects/Cube"),
                  ((Game1)Game).GraphicsDevice,
                  ((Game1)Game));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(400, 40, 400)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(-700, 40, 400)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(-400, 40, -400)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(-800, 40, -400)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(-900, 40, -600)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(-700, 40, -800)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(-600, 40, -600)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(400, 40, -600)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(-700, 40, 800)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(600, 40, -600)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(400, 40, -600)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(700, 40, -800)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(600, 40, -600)));
            Enemies.enemyCubes.Add(addEnemyCube(new Vector3(-400, 40, 600)));
            models.Add(new Ground(
                Game.Content.Load<Model>(@"Models/Ground/Ground")));
            /*models.Add(new SkyBox(
                Game.Content.Load<Model>(@"Models/SkyBox/skybox")));*/
            models.Add(player);
            foreach (EnemyCube enemyCube in Enemies.enemyCubes)
            {
                models.Add(enemyCube);
            }

            foreach (Platform plat in Platforms.platforms)
            {
                models.Add(plat);
            }
            //ground bounds

            
            base.LoadContent();
        }
        private void FireCube()
        {
            if (playerShotTimer >= playerShotCooldown)
            {
                shotManager.AddPlayerShot(player.playerPos, player.playerDir);
                playerShotTimer = 0f;
            }
        }
        public override void Update(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Vector3 fireAngle = Vector3.Zero;
                FireCube();
            }
            for (int i = 0; i <= models.Count-1; i++)
            {
                models[i].Update(gameTime);
            }
            shotManager.Update(gameTime);
            if (playerShotTimer < playerShotCooldown)
                playerShotTimer += elapsed;
            base.Update(gameTime);
        }



        
        /* public override void Draw(GameTime gameTime)
         {   
             base.Draw(gameTime);
         }
         */
        public override void Draw(GameTime gameTime)
        {

            //    //have to write a foreach loop to add each element inside the model list and draw each model
            //    //runs for every element in the list - no repetition needed.
            foreach (BasicModel model in models)
            {//model manager doesnt care how it is drawn, just makes it draw.
                model.Draw(((Game1)Game).GraphicsDevice, ((Game1)Game).camera); //need to pass 2 parameters, graphicsDevice & camera
            }

            shotManager.Draw(((Game1)Game).camera);

           base.Draw(gameTime);
           

        }

    }
}
