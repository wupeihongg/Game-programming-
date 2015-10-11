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
    public class ShotManager
    {
        Model ShotModel;
        public List<CubeProjectile> CubeProjectiles = new List<CubeProjectile>();
        Vector3 Gravity = new Vector3(0, -0.05f, 0);
        Matrix scale = Matrix.CreateScale(0.25f, 0.25f, 0.25f);
        GraphicsDevice device;
        public bool isColliding = false;
        public ShotManager(Model model, GraphicsDevice device)
        {
            ShotModel = model;
            this.device = device;
        }
        public void checkIntersects()
        {
            isColliding = false;
            for(int h = 0; h < Enemies.enemyCubes.Count() -1; h++)
            {
                for (int i = 0; i <= Enemies.enemyCubes[h].enemyBoxes.Count() - 1; i++)
                {
                    BoundingBox box2 = Enemies.enemyCubes[h].enemyBoxes[i];
                    for (int j = 0; j < CubeProjectiles.Count() - 1; j++)
                    {
                        for (int k = 0; k < CubeProjectiles[j].cubeProjectileBoxes.Count() - 1; k++)
                        {
                            BoundingBox box1 = CubeProjectiles[j].cubeProjectileBoxes[k];
                            if (box1.Contains(box2) != ContainmentType.Disjoint)
                            {
                                CubeProjectiles[j].cubeProjectileBoxes.RemoveAt(k);
                                k--;
                                CubeProjectiles[j].IsActive = false;
                                Enemies.enemyCubes[h].enemyBoxes.RemoveAt(i);
                                i--;
                                ModelManager.models.Remove(Enemies.enemyCubes[h]);
                                Enemies.enemyCubes.RemoveAt(h);
                                h--;
                                Console.Out.WriteLine("k");
                            }
                           
                        }
                    }
                }
            }
        }
        
        public void AddPlayerShot(Vector3 startingPos, Vector3 direction)
        {
            bool reusedShot = false;

            foreach (CubeProjectile cubeProjectile in CubeProjectiles)
            {
                if (!cubeProjectile.IsActive)
                {
                    reusedShot = true;
                    cubeProjectile.ResetShot(startingPos, direction);
                    continue;
                }
            }
            if (!reusedShot)
            {
                CubeProjectiles.Add(new CubeProjectile(ShotModel, device, startingPos, direction));
            }
            
        }
        public void Update(GameTime gameTime)
        {
            //Console.Out.WriteLine(CubeProjectiles.Count());
            foreach (CubeProjectile cubeProjectile in CubeProjectiles)
            {
                if (cubeProjectile.IsActive)
                {
                    cubeProjectile.Update(gameTime);
                }
            }
            //checkIntersects();
            
        }
        public void Draw(Camera camera)
        {
            foreach (CubeProjectile cubeProjectile in CubeProjectiles)
            {
                if (cubeProjectile.IsActive)
                {
                    cubeProjectile.Draw(camera);
                }
            }
        }
    }
}
