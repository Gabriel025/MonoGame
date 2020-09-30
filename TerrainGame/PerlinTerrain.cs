using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace TerrainGame
{
    public class PerlinTerrain
    {
        public VertexMultitextured[] vertices;
        int[] indices;
        VertexBuffer vb;
        IndexBuffer ib;
        public Point Size;
        public Vector3 Scale;
        Effect e;
        Thread thr;
        public bool IsGenerating = false;
        bool reloadbuffers = false;

        public Matrix Projection { set { e.Parameters["Projection"].SetValue(value); } }

        public PerlinTerrain(Matrix View, Matrix Projection,
            int Seed, float Persistence, int OctaveCount, float StartFrequency, Point size, Vector3 scale)
        {
            Size = size;
            Scale = scale;
            vertices = new VertexMultitextured[Size.X * Size.Y];
            indices = new int[(Size.X - 1) * (Size.Y - 1) * 6];
            vb = new VertexBuffer(Main.gd, VertexMultitextured.VertexDeclaration,
                vertices.Length, BufferUsage.WriteOnly);
            ib = new IndexBuffer(Main.gd, IndexElementSize.ThirtyTwoBits,
                indices.Length, BufferUsage.WriteOnly);

            e = Main.con.Load<Effect>("Effects");
            e.CurrentTechnique = e.Techniques["Multitextured"];
            e.Parameters["View"].SetValue(View);
            e.Parameters["World"].SetValue(Matrix.CreateScale(Scale));
            e.Parameters["Projection"].SetValue(Projection);
            e.Parameters["DiffuseDirection"]
                .SetValue(Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationZ(MathHelper.ToRadians(-15))));
            //e.Parameters["DiffuseColor"].SetValue(new Vector4(1, 64f/255, 0, 1));
            e.Parameters["Texture1"].SetValue(Main.con.Load<Texture2D>("Texture1"));
            e.Parameters["Texture2"].SetValue(Main.con.Load<Texture2D>("Texture2"));
            e.Parameters["Texture3"].SetValue(Main.con.Load<Texture2D>("Texture3"));
            e.Parameters["Texture4"].SetValue(Main.con.Load<Texture2D>("Texture4"));

            UnthreadedReseed(Seed, Persistence, OctaveCount, StartFrequency);
        }

        public void Reseed(int Seed, float Persistence, int OctaveCount, float StartFrequency)
        {
            thr = new Thread(new ThreadStart(
                                 delegate{ UnthreadedReseed(Seed, Persistence, OctaveCount, StartFrequency); } ));

            if (!IsGenerating) thr.Start();
        }

        public void UnthreadedReseed(int Seed, float Persistence, int OctaveCount, float StartFrequency)
        {
            IsGenerating = true;

            float n;
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                {
                    n = Utility.PerlinNoise(Seed, x, y, Persistence, OctaveCount, StartFrequency);
                    vertices[x + y * Size.X] = new VertexMultitextured(
                            new Vector3(
                                 (x - (float)Size.X / 2),
                                 n,
                                 (y - (float)Size.X / 2)
                            ), Vector3.Zero, new Vector2((float)x/5, (float)y/5),
                            n < -.1f ? new Vector4(1, 0, 0, 0) :
                            n < .5f ? new Vector4(0, 1, 0, 0) :
                            n < .6f ? new Vector4(0, 0, .5f, .5f) : new Vector4(0, 0, 0, 1)
                        );
                }

            int i = 0;
            for (int x = 0; x < Size.X - 1; x++)
                for (int y = 0; y < Size.Y - 1; y++)
                {
                    indices[i++] = x + y * Size.X;
                    indices[i++] = x + 1 + y * Size.X;
                    indices[i++] = x + (y + 1) * Size.X;
                    indices[i++] = x + (y + 1) * Size.X;
                    indices[i++] = x + 1 + y * Size.X;
                    indices[i++] = x + 1 + (y + 1) * Size.X;
                }

            InitializeNormals();

            reloadbuffers = true;

            IsGenerating = false;
            try { thr.Abort(); }catch(Exception){}
        }

        private void InitializeNormals()
        {
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                    vertices[x + y * Size.X].Normal =
                        Vector3.Normalize(
                            Vector3.Cross(
                                (y + 1 >= Size.Y
                                ? vertices[x + y * Size.X].Position * Scale + Vector3.UnitZ
                                : vertices[x + (y + 1) * Size.X].Position * Scale)
                                - vertices[x + y * Size.X].Position * Scale,
                                (x + 1 >= Size.X
                                ? vertices[x + y * Size.X].Position * Scale + Vector3.UnitX
                                : vertices[x + 1 + y * Size.X].Position * Scale)
                                - vertices[x + y * Size.X].Position * Scale
                            )
                        );

        }

        public void Update(Matrix View)
        {
            e.Parameters["View"].SetValue(View);

            if (reloadbuffers)
            {
                vb.SetData<VertexMultitextured>(vertices);
                ib.SetData<int>(indices);
                reloadbuffers = false;
            }
        }

        public void Draw()
        {
            e.CurrentTechnique.Passes[0].Apply();
            Main.gd.SetVertexBuffer(vb);
            Main.gd.Indices = ib;

            Main.gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
            //Main.gd.DrawUserIndexedPrimitives<VertexMultitextured>(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
        }
        
        public float GetHeightAtPos(Vector3 pos)
        {
            pos.X /= Scale.X;
            pos.Z /= Scale.Z;
            pos.X += Size.X / 2;
            pos.Z += Size.Y / 2;
            pos.X = MathHelper.Clamp(pos.X, 0, Size.X - 2);
            pos.Z = MathHelper.Clamp(pos.Z, 0, Size.Y - 2);
            return
                MathHelper.Lerp(
                    MathHelper.Lerp(
                        vertices[(int)pos.X + (int)pos.Z * Size.X].Position.Y * Scale.Y,
                        vertices[(int)pos.X + 1 + (int)pos.Z * Size.X].Position.Y * Scale.Y,
                        pos.X - (float)Math.Floor((double)pos.X)
                    ),
                    MathHelper.Lerp(
                        vertices[(int)pos.X + ((int)pos.Z + 1) * Size.X].Position.Y * Scale.Y,
                        vertices[(int)pos.X + 1 + ((int)pos.Z + 1) * Size.X].Position.Y * Scale.Y,
                        pos.X - (float)Math.Floor((double)pos.X)
                    ),
                    pos.Z - (float)Math.Floor((double)pos.Z)
                );
        }
    }
}