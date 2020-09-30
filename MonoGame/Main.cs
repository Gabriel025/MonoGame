using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MonoGame
{
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Random rand;

        List<AABB> cubes;
        Renderer renderer;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            rand = new Random();

            cubes = new List<AABB>();
            renderer = new Renderer(GraphicsDevice);

            for (int z = -5; z < 5; z++)
                for (int y = -7; y < 3; y++)
                    for (int x = -5; x < 5; x++)
                        if(rand.Next(100) < 50)
                            cubes.Add(new AABB(new Vector3(0.5f + x, 0.5f + y, 0.5f + z), Vector3.One * 0.9f));

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);


        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            renderer.Draw(cubes, Matrix.Identity);

            base.Draw(gameTime);
        }
    }
}
