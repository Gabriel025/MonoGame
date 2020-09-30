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
    public struct VertexPositionColorNormal:IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public VertexPositionColorNormal(Vector3 Position, Color Color, Vector3 Normal)
        {
            this.Position = Position;
            this.Color = Color;
            this.Normal = Normal;
        }

        public readonly static VertexDeclaration VertexDeclaration =
            new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    }

    public struct VertexMultitextured:IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector4 TextureWeight;

        public VertexMultitextured(Vector3 Position, Vector3 Normal,
            Vector2 TextureCoordinate, Vector4 TextureWeight)
        {
            this.Position = Position;
            this.Normal = Normal;
            this.TextureCoordinate = TextureCoordinate;
            this.TextureWeight = TextureWeight;
        }

        public readonly static VertexDeclaration VertexDeclaration =
            new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1)
            );
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    }
}
