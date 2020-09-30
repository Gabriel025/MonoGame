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
    public class Skybox
    {
        //Skybox <modified for this application>
        BasicEffect b;
        short[] indices;
        VertexPositionTexture[] verts;
        VertexBuffer vb;
        IndexBuffer ib;
        public Matrix Projection { set { b.Projection = value; } }

        public Skybox(int Size, Matrix Projection, Matrix View)
        {
            b = new BasicEffect(Main.gd);
            b.Projection = Projection;
            b.View = View;
            b.World = Matrix.Identity;
            b.TextureEnabled = true;
            b.Texture = Main.con.Load<Texture2D>("Skybox");
            float hsize = Size / 2;

            verts = new VertexPositionTexture[24]
            {
                //Up
                new VertexPositionTexture(new Vector3(-hsize, hsize, hsize), new Vector2(0f, 0f)),
                new VertexPositionTexture(new Vector3(hsize, hsize, hsize), new Vector2(1f / 4f, 0f)),
                new VertexPositionTexture(new Vector3(hsize, hsize, -hsize), new Vector2(1f / 4f, 1f / 3f)),
                new VertexPositionTexture(new Vector3(-hsize, hsize, -hsize), new Vector2(0f, 1f / 3f)),

                //Down
                new VertexPositionTexture(new Vector3(-hsize, -hsize, -hsize), new Vector2(0f, 1f / 3f * 2f)),
                new VertexPositionTexture(new Vector3(hsize, -hsize, -hsize), new Vector2(1f / 4f, 1f / 3f * 2f)),
                new VertexPositionTexture(new Vector3(hsize, -hsize, hsize), new Vector2(1f / 4f, 1f)),
                new VertexPositionTexture(new Vector3(-hsize, -hsize, hsize), new Vector2(0f, 1f)),

                //Back
                new VertexPositionTexture(new Vector3(-hsize, hsize, -hsize), new Vector2(0f, 1f / 3f)),
                new VertexPositionTexture(new Vector3(hsize, hsize, -hsize), new Vector2(1f / 4f, 1f / 3f)),
                new VertexPositionTexture(new Vector3(hsize, -hsize, -hsize), new Vector2(1f / 4f, 1f / 3f * 2f)),
                new VertexPositionTexture(new Vector3(-hsize, -hsize, -hsize), new Vector2(0f, 1f / 3f * 2f)),

                //Right
                new VertexPositionTexture(new Vector3(hsize, hsize, -hsize), new Vector2(1f / 4f, 1f / 3f)),
                new VertexPositionTexture(new Vector3(hsize, hsize, hsize), new Vector2(1f / 4f * 2f, 1f / 3f)),
                new VertexPositionTexture(new Vector3(hsize, -hsize, hsize), new Vector2(1f / 4f * 2f, 1f / 3f * 2f)),
                new VertexPositionTexture(new Vector3(hsize, -hsize, -hsize), new Vector2(1f / 4f, 1f / 3f * 2f)),

                //Front
                new VertexPositionTexture(new Vector3(hsize, hsize, hsize), new Vector2(1f / 4f * 2f, 1f / 3f)),
                new VertexPositionTexture(new Vector3(-hsize, hsize, hsize), new Vector2(1f / 4f * 3f, 1f / 3f)),
                new VertexPositionTexture(new Vector3(-hsize, -hsize, hsize), new Vector2(1f / 4f * 3f, 1f / 3f * 2f)),
                new VertexPositionTexture(new Vector3(hsize, -hsize, hsize), new Vector2(1f / 4f * 2f, 1f / 3f * 2f)),

                //Left
                new VertexPositionTexture(new Vector3(-hsize, hsize, hsize), new Vector2(1f / 4f * 3f, 1f / 3f)),
                new VertexPositionTexture(new Vector3(-hsize, hsize, -hsize), new Vector2(1f, 1f / 3f)),
                new VertexPositionTexture(new Vector3(-hsize, -hsize, -hsize), new Vector2(1f, 1f / 3f * 2f)),
                new VertexPositionTexture(new Vector3(-hsize, -hsize, hsize), new Vector2(1f / 4f * 3f, 1f / 3f * 2f))
            };
            indices = new short[36]
            {
                0,1,2, 0,2,3,
                4,5,6, 6,7,4,
                8,9,10, 10,11,8,
                12,13,14, 12,14,15,
                16,17,18, 16,18,19,
                20,21,22, 20,22,23
            };

            vb = new VertexBuffer(Main.gd, VertexPositionTexture.VertexDeclaration,
                verts.Length, BufferUsage.WriteOnly);
            ib = new IndexBuffer(Main.gd, IndexElementSize.SixteenBits,
                indices.Length, BufferUsage.WriteOnly);
            vb.SetData<VertexPositionTexture>(verts);
            ib.SetData<short>(indices);
        }

        public void Update(Matrix newView, Vector3 CamPosition)
        {
            b.View = newView;
            b.World = Matrix.CreateTranslation(CamPosition);
        }

        public void Draw()
        {
            b.CurrentTechnique.Passes[0].Apply();
            Main.gd.SetVertexBuffer(vb);
            Main.gd.Indices = ib;
            Main.gd.DepthStencilState = DepthStencilState.None;
            Main.gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12);
            Main.gd.DepthStencilState = DepthStencilState.Default;
        }
    }
}
