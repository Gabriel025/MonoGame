using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TerrainGame
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public static SpriteBatch sb;
        public static GraphicsDevice gd;
        public static ContentManager con;
        public static SpriteFont segoeUI;
        public static bool paused;
        Matrix projection;
        FirstPersonCamera fpc;
        Skybox sky;
        public static PerlinTerrain t;
        int seed = 1;
        public static MouseState ms, pms;
        public static KeyboardState ks, pks;
        public static Texture2D white;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            Window.Title = "Terrain walk";
            Window.ClientSizeChanged +=new EventHandler<EventArgs>(Resize);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            gd = GraphicsDevice;

            sb = new SpriteBatch(gd);
            con = Content;
            segoeUI = con.Load<SpriteFont>("SegoeUI");

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90f),
                gd.Viewport.AspectRatio, 1f, 5000f);
            fpc = new FirstPersonCamera(new Vector3(0f, 0f, 0f), new Vector2(0f, 0f), 0.5f, 2f, 15f);
            pms = ms = Mouse.GetState();
            pks = ks = Keyboard.GetState();
            t = new PerlinTerrain(
                fpc.View, projection,
                seed, .5f, 2, 1f / 32f, new Point(500, 500), new Vector3(8f, 256f, 8f));
            white = new Texture2D(gd, 1, 1);
            white.SetData<Color>(new Color[1]{Color.White});

            sky = new Skybox(500, projection, fpc.View);
        }

        protected override void UnloadContent(){}

        protected override void Update(GameTime gameTime)
        {
            ms = Mouse.GetState();
            ks = Keyboard.GetState();

            if(!t.IsGenerating)
            {
                if (ms.ScrollWheelValue > pms.ScrollWheelValue) seed++;
                if (ms.ScrollWheelValue < pms.ScrollWheelValue) seed--;
                if (ms.ScrollWheelValue != pms.ScrollWheelValue)
                    t.Reseed(seed, .5f, 2, 1f / 32f);
            }

            if (ks.IsKeyDown(Keys.Escape) && pks.IsKeyUp(Keys.Escape)) paused = !paused;

            if (!paused) fpc.Update();
            t.Update(fpc.View);
            sky.Update(fpc.View, fpc.Position);

            pms = ms;
            pks = ks;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            sky.Draw();
            //RasterizerState rs = new RasterizerState();
            //rs.FillMode = FillMode.WireFrame;
            //gd.RasterizerState = rs;
            t.Draw();

            fpc.DrawCrosshair();
            sb.Begin();
                if (paused) sb.Draw(white, gd.Viewport.Bounds, new Color(0, 0, 0, 128));
                sb.DrawString(segoeUI,
                    "Seed: " + seed
                    + (paused ? "\nPAUSED" : "")
                    + (t.IsGenerating ? "\nGENERATING" : ""),
                    new Vector2(10, 10), Color.Blue);
            sb.End();
            //GraphicsDevice.BlendState = BlendState.Opaque;
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            base.Draw(gameTime);
        }

        void Resize(object sender, EventArgs a)
        {
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90f),
                gd.Viewport.AspectRatio, 1f, 5000f);
            sky.Projection = projection;
            t.Projection = projection;
        }
    }
}
