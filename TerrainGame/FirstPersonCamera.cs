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
    public class FirstPersonCamera
    {
        //First Person Camera <modified for this application>
        public Vector3 Position;
        Vector3 anglevector, forward, right;
        public Vector2 Angle;
        public Matrix View;
        public float Sensitivity, Speed, Height;
        float velY;
        bool onGround = false;
        Texture2D cht;

        public FirstPersonCamera(Vector3 StartPosition, Vector2 StartAngle, float Sensivity, float Speed, float Height)
        {
            cht = Main.con.Load<Texture2D>("Crosshair");
            Position = StartPosition;
            Angle = StartAngle;
            anglevector =
                    Vector3.Transform(
                    new Vector3(0, 0, 1),
                    Matrix.CreateRotationX(MathHelper.ToRadians(Angle.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(Angle.Y))
                );
            right = Vector3.Transform(new Vector3(-1, 0, 0), Matrix.CreateRotationY(MathHelper.ToRadians(Angle.Y)));
            Sensitivity = Sensivity;
            this.Speed = Speed;
            this.Height = Height;

            View = Matrix.CreateLookAt(
                Position,
                Position + anglevector,
                Vector3.Up
            );

            Mouse.SetPosition(
                Main.gd.Viewport.Bounds.X + Main.gd.Viewport.Width / 2,
                Main.gd.Viewport.Bounds.Y + Main.gd.Viewport.Height / 2);
        }

        public void Update()
        {   
            if (Main.ks.IsKeyDown(Keys.W)) Position += forward * Speed;
            if (Main.ks.IsKeyDown(Keys.S)) Position -= forward * Speed;

            if (Main.ks.IsKeyDown(Keys.A)) Position -= right * Speed;
            if (Main.ks.IsKeyDown(Keys.D)) Position += right * Speed;

            //Collision and gravity
            if (Main.ks.IsKeyDown(Keys.Space) && onGround)
            {
                velY = 4f;
                onGround = false;
            }
            else velY -= .2f;
            velY = MathHelper.Clamp(velY, -10, 10);
            Position.Y += velY;
            if (Position.Y < Main.t.GetHeightAtPos(Position) + Height)
            {
                Position.Y = Main.t.GetHeightAtPos(Position) + Height;
                onGround = true;
            }
            //if (ks.IsKeyDown(Keys.LeftShift)) Position.Y -= Speed;

            if (Main.ms.Y < Main.pms.Y) Angle.X -= (Main.pms.Y - Main.ms.Y) * Sensitivity;
            else if (Main.ms.Y > Main.pms.Y) Angle.X += (Main.ms.Y - Main.pms.Y) * Sensitivity;
            if (Angle.X < -89) Angle.X = -89;
            else if (Angle.X > 89) Angle.X = 89;
            if (Main.ms.X < Main.pms.X) Angle.Y += (Main.pms.X - Main.ms.X) * Sensitivity;
            else if (Main.ms.X > Main.pms.X) Angle.Y -= (Main.ms.X - Main.pms.X) * Sensitivity;
            if (Angle.Y >= 360) Angle.Y -= 360;
            else if (Angle.Y <= -360) Angle.Y += 360;

            anglevector =
                Vector3.Transform(
                    new Vector3(0, 0, 1),
                    Matrix.CreateRotationX(MathHelper.ToRadians(Angle.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(Angle.Y))
                );
            forward = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(MathHelper.ToRadians(Angle.Y)));
            right = Vector3.Transform(new Vector3(-1, 0, 0), Matrix.CreateRotationY(MathHelper.ToRadians(Angle.Y)));
            View = Matrix.CreateLookAt(Position, Position + anglevector, Vector3.Up);

            Mouse.SetPosition(
            Main.gd.Viewport.Bounds.X + Main.gd.Viewport.Width / 2,
            Main.gd.Viewport.Bounds.Y + Main.gd.Viewport.Height / 2);
            Main.ms = Mouse.GetState();
        }

        public void DrawCrosshair()
        {
            Main.sb.Begin();
            Main.sb.Draw(cht,
                new Rectangle(Main.gd.Viewport.Width / 2 - 25 / 2, Main.gd.Viewport.Height / 2 - 25 / 2, 25, 25),
                Color.White);
            Main.sb.End();
        }
    }
}
