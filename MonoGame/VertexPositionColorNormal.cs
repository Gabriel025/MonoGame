using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame
{
    struct VertexPositionColorNormal:IVertexType
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
}
