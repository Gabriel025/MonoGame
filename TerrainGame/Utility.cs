using System;
using System.IO;
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
    public struct Utility
    {
        /* old_slow_ugly
        public static Texture2D GenerateStatic(GraphicsDevice gd, int Width, int Height, int Seed)
        {
            Random r = new Random(Seed);
            Color[] colors = new Color[Width * Height];
            Texture2D s = new Texture2D(gd, Width, Height);
            int n;
            for(int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    n = r.Next(255);
                    colors[x + y * Width] = new Color(n, 0, 0);
                }
            s.SetData<Color>(colors);
            return s;
        }

        public static Texture2D GeneratePerlinNoise(GraphicsDevice gDev, SpriteBatch sb, int Width, int Height, int Seed)
        {
            Texture2D s = GenerateStatic(gDev, Width, Height, Seed);
            Color[] c = new Color[Width * Height], nc = new Color[Width * Height];
            RenderTarget2D rt = new RenderTarget2D(gDev, Width, Height);

            for (int i = 0; i < 5; i++)
            {
                gDev.SetRenderTarget(rt);
                sb.Begin();
                sb.Draw(
                    s, new Rectangle(0, 0, Width, Height),
                    new Rectangle(0, 0, Width / (int)Math.Pow(2, i + 1), Height / (int)Math.Pow(2, i + 1)),
                    Color.White);
                sb.End();

                gDev.SetRenderTarget(null);
                rt.GetData<Color>(c);

                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        nc[x + y * Width].R += (byte)((double)c[x + y * Width].R / Math.Pow(2, 5 - i));
                    }
            }

            gDev.Textures[0] = null;
            s.SetData<Color>(nc);
            return s;
        }*/

        #region PRNG
        public static float Static(int seed, int x)
        {
            x = seed * 1013 + x * 1619;
            x = (x << 13) ^ x;
            return (1f - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f);
        }

        public static float Static(int seed, int x, int y)
        {
            x = seed * 1013 + x * 1619 + y * 31337;
            x = (x << 13) ^ x;
            return (1f - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f);
        }

        public static float Static(int seed, int x, int y, int z)
        {
            x = seed * 1013 + x * 1619 + y * 31337 + z * 52591;
            x = (x << 13) ^ x;
            return (1f - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f);
        }
        #endregion PRNG

        public static float CosInterpolate(float a, float b, float x)
        {
            float p = (1f - (float)Math.Cos(x * MathHelper.Pi)) * .5f;
            return a * (1f - p) + b * p;
        }

        public static float SmoothStatic(int seed, int x)
        {
            return Static(seed, x - 1) / 4f + Static(seed, x) / 2f + Static(seed, x + 1) / 4f;
        }

        public static float SmoothStatic(int seed, int x, int y)
        {
            float sides = (Static(seed,x-1,y)+Static(seed,x+1,y)+Static(seed,x,y-1)+Static(seed,x,y+1)) / 8;
            float corners = (Static(seed,x-1,y-1)+Static(seed,x+1,y-1)+Static(seed,x-1,y+1)+Static(seed,x+1,y+1)) / 16;
            return Static(seed, x, y) / 4 + sides + corners;
        }

        public static float SmoothStatic(int seed, int x, int y, int z)
        {
            float sides = ( Static(seed, x - 1, y, z) + Static(seed, x + 1, y, z)
                          + Static(seed, x, y - 1, z) + Static(seed, x, y + 1, z)
                          + Static(seed, x, y, z - 1) + Static(seed, x, y, z + 1)) / 24;

            float edges = ( Static(seed, x, y - 1, z - 1) + Static(seed, x, y + 1, z - 1)
                          + Static(seed, x, y - 1, z + 1) + Static(seed, x, y + 1, z + 1)
                          + Static(seed, x - 1, y, z - 1) + Static(seed, x + 1, y, z - 1)
                          + Static(seed, x - 1, y, z + 1) + Static(seed, x + 1, y, z + 1)
                          + Static(seed, x - 1, y - 1, z) + Static(seed, x + 1, y - 1, z)
                          + Static(seed, x - 1, y + 1, z) + Static(seed, x + 1, y + 1, z)) / 48;

            float corners = ( Static(seed, x + 1, y - 1, z - 1) + Static(seed, x + 1, y + 1, z - 1)
                            + Static(seed, x + 1, y - 1, z + 1) + Static(seed, x + 1, y + 1, z + 1)
                            + Static(seed, x - 1, y - 1, z - 1) + Static(seed, x - 1, y + 1, z - 1)
                            + Static(seed, x - 1, y - 1, z + 1) + Static(seed, x - 1, y + 1, z + 1)) / 32;

            return Static(seed, x, y) / 4 + sides + edges + corners;
        }

        #region InterpolateStatic
        public static float InterpolateStatic(int seed, float x)
        {
            int intx = (int)Math.Floor(x);
            float frac = x - intx;

            return CosInterpolate(SmoothStatic(seed, intx), SmoothStatic(seed, intx + 1), frac);
        }

        public static float InterpolateStatic(int seed, float x, float y)
        {
            int intx = (int)(x > 0f ? Math.Floor(x) : Math.Ceiling(x));
            float fracx = x - (float)intx;
            int inty = (int)(y > 0f ? Math.Floor(y) : Math.Ceiling(y));
            float fracy = y - (float)inty;

            float i1 = CosInterpolate(SmoothStatic(seed, intx, inty), SmoothStatic(seed, intx + 1, inty), fracx);
            float i2 = CosInterpolate(SmoothStatic(seed, intx, inty + 1), SmoothStatic(seed, intx + 1, inty + 1), fracx);

            return CosInterpolate(i1, i2, fracy);
        }

        public static float InterpolateStatic(int seed, float x, float y, float z)
        {
            int intx = (int)(x > 0f ? Math.Floor(x) : Math.Ceiling(x));
            float fracx = x - (float)intx;
            int inty = (int)(y > 0f ? Math.Floor(y) : Math.Ceiling(y));
            float fracy = y - (float)inty;
            int intz = (int)(z > 0f ? Math.Floor(z) : Math.Ceiling(z));
            float fracz = z - (float)intz;

            float ix1 = CosInterpolate(SmoothStatic(seed, intx, inty, intz), SmoothStatic(seed, intx + 1, inty, intz), fracx);
            float ix2 = CosInterpolate(SmoothStatic(seed, intx, inty + 1, intz), SmoothStatic(seed, intx + 1, inty + 1, intz), fracx);
            float ixz1 = CosInterpolate(SmoothStatic(seed, intx, inty, intz + 1), SmoothStatic(seed, intx + 1, inty, intz + 1), fracx);
            float ixz2 = CosInterpolate(SmoothStatic(seed, intx, inty + 1, intz + 1), SmoothStatic(seed, intx + 1, inty + 1, intz + 1), fracx);
            return CosInterpolate(CosInterpolate(ix1, ix2, fracy), CosInterpolate(ixz1, ixz2, fracy), fracz);
        }
        #endregion InterpolateStatic

        public static float PerlinNoise(int seed, float x, float Persistence, int OctaveCount, float StartFrequency)
        {
            float result = 0f, amp = 1f, freq = StartFrequency;
            for (int i = 0; i < OctaveCount; i++)
            {
                result += InterpolateStatic(seed, x * freq) * amp;
                amp *= Persistence;
                freq *= 2;
            }
            return result;
        }

        public static float PerlinNoise(int seed, float x, float y, float Persistence, int OctaveCount, float StartFrequency)
        {
            float result = 0f, amp = 1f, freq = StartFrequency;
            for (int i = 0; i < OctaveCount; i++)
            {
                result += InterpolateStatic(seed, x * freq, y * freq) * amp;
                amp *= Persistence;
                freq *= 2f;
            }
            return result;
        }

        public static float PerlinNoise(int seed, float x, float y, float z, float Persistence, int OctaveCount, float StartFrequency)
        {
            float result = 0f, amp = 1f, freq = StartFrequency;
            for (int i = 0; i < OctaveCount; i++)
            {
                result += InterpolateStatic(seed, x * freq, y * freq, z * freq) * amp;
                amp *= Persistence;
                freq *= 2f;
            }
            return result;
        }

        /* unused
        public static Texture2D GPN(GraphicsDevice gDev, int Width, int Height, int Seed)
        {
            Color[] colors = new Color[Width * Height];
            Texture2D t = new Texture2D(gDev, Width, Height);
            float n;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    n = PerlinNoise(Seed, x, y, .5f, 5, 1f / 32f);
                    if(n < 0)
                        colors[x + y * Width] = new Color((byte)(127f - (Math.Abs(n) * 100f)), 0, 0);
                    else
                        colors[x + y * Width] = new Color((byte)(127f + (n * 100f)), 0, 0);
                }
            t.SetData<Color>(colors);
            return t;
        }

        public static void GeneratePerlinCubemap(int Width, int Height, int Length, int Seed)
        {
            StreamWriter sr = new StreamWriter("c:\\Users\\root\\Desktop\\Cubemap.cm");
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    for (int z = 0; z < Length; z++)
                        if (PerlinNoise(Seed, x, y, z, .5f, 5, 1f / 32f) > -0.1f)
                            sr.WriteLine("cube pos:"+x+","+y+","+z+" size:1,1,1 color:175,175,175,255");
            sr.Close();
            sr.Dispose();
        }

        public static void GeneratePerlinTexture(GraphicsDevice gDev, int Seed, Point Size, Texture2D Scale)
        {
            Color[] colors = new Color[Size.X * Size.Y], scale = new Color[256];
            Scale.GetData<Color>(scale);
            Texture2D t = new Texture2D(gDev, Size.X, Size.Y);
            float n;
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                {
                    n = PerlinNoise(Seed, x, y, .5f, 5, 1f / 32f);
                    if (n > 1)
                        n = 1;
                    if (n < -1)
                        n = -1;

                    if (n < 0)
                        colors[x + y * Size.X] = scale[(int)(127f + n * 127f)];
                    else
                        colors[x + y * Size.X] = scale[(int)(127f + n * 127f)];
                }
            t.SetData<Color>(colors);
            t.SaveAsPng(
                new FileStream("C:\\Users\\root\\Desktop\\firenoise" + Seed + ".png", FileMode.Create), Size.X, Size.Y);
        }*/
    }
}
