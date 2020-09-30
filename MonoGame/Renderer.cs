using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame
{
    class Renderer
    {
        GraphicsDevice graphicsDevice;
        VertexBuffer unitCubeVerts;
        IndexBuffer unitCubeIndices;
        BasicEffect effect;

        public Renderer(GraphicsDevice graphicsDevice)
        {

            this.graphicsDevice = graphicsDevice;
            unitCubeVerts = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorNormal), 24, BufferUsage.WriteOnly);
            unitCubeIndices = new IndexBuffer(graphicsDevice, typeof(short), 36, BufferUsage.WriteOnly);
            setupBuffers();

            effect = new BasicEffect(graphicsDevice);
            effect.VertexColorEnabled = true;
            effect.PreferPerPixelLighting = true;
            effect.EnableDefaultLighting();

        }

        private void setupBuffers()
        {
            int[] rawVerts =
            {
                0, 0, 0, 255, 255, 255, -1, 0, 0,
                0, 1, 0, 255, 255, 255, -1, 0, 0,
                0, 1, 1, 255, 255, 255, -1, 0, 0,
                0, 0, 1, 255, 255, 255, -1, 0, 0,

                0, 0, 0, 255, 255, 255, 0, -1, 0,
                0, 0, 1, 255, 255, 255, 0, -1, 0,
                1, 0, 1, 255, 255, 255, 0, -1, 0,
                1, 0, 0, 255, 255, 255, 0, -1, 0,

                0, 0, 0, 255, 255, 255, 0, 0, -1,
                1, 0, 0, 255, 255, 255, 0, 0, -1,
                1, 1, 0, 255, 255, 255, 0, 0, -1,
                0, 1, 0, 255, 255, 255, 0, 0, -1,

                1, 0, 0, 255, 255, 255, 1, 0, 0,
                1, 0, 1, 255, 255, 255, 1, 0, 0,
                1, 1, 1, 255, 255, 255, 1, 0, 0,
                1, 1, 0, 255, 255, 255, 1, 0, 0,

                0, 1, 0, 255, 255, 255, 0, 1, 0,
                1, 1, 0, 255, 255, 255, 0, 1, 0,
                1, 1, 1, 255, 255, 255, 0, 1, 0,
                0, 1, 1, 255, 255, 255, 0, 1, 0,

                0, 0, 1, 255, 255, 255, 0, 0, 1,
                0, 1, 1, 255, 255, 255, 0, 0, 1,
                1, 1, 1, 255, 255, 255, 0, 0, 1,
                1, 0, 1, 255, 255, 255, 0, 0, 1,
            };

            VertexPositionColorNormal[] verts = new VertexPositionColorNormal[24];

            int iRaw = 0;
            for(int i = 0; i < 24; i++)
                verts[i] = new VertexPositionColorNormal(
                    new Vector3(-0.5f + rawVerts[iRaw++], -0.5f + rawVerts[iRaw++], -0.5f + rawVerts[iRaw++]),
                    new Color(rawVerts[iRaw++], rawVerts[iRaw++], rawVerts[iRaw++]),
                    new Vector3(rawVerts[iRaw++], rawVerts[iRaw++], rawVerts[iRaw++]));

            short[] indices =
            {
                0, 1, 2, 0, 2, 3,
                4, 5, 6, 4, 6, 7,
                8, 9, 10, 8, 10, 11,
                12, 13, 14, 12, 14, 15,
                16, 17, 18, 16, 18, 19,
                20, 21, 22, 20, 22, 23
            };

            unitCubeVerts.SetData<VertexPositionColorNormal>(verts);
            unitCubeIndices.SetData<short>(indices);
        }

        Vector3 camPosition = new Vector3(0, 10, 10);
        public void Draw(List<AABB> cubes, Matrix viewMatrix)
        {
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60f), graphicsDevice.Viewport.AspectRatio, 1f, 1000f);
            effect.View = Matrix.CreateLookAt(camPosition, Vector3.Zero, Vector3.Up);
            camPosition = Vector3.Transform(camPosition, Matrix.CreateRotationY(0.02f));
            graphicsDevice.SetVertexBuffer(unitCubeVerts);
            graphicsDevice.Indices = unitCubeIndices;

            foreach (AABB cube in cubes)
            {
                effect.World = Matrix.CreateScale(cube.size) * Matrix.CreateTranslation(cube.center);
                effect.CurrentTechnique.Passes[0].Apply();

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
            }
        }
    }
}
