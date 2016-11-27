using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarszawaCentralna.Shapes
{
    class Cuboid
    {
        public VertexPositionNormalTexture[] vertices;
        Matrix worldMatrix;
        Vector3 position;
        Color color;
        float shininess;
        Texture2D texture;
        Texture2D secondTexture;
        bool textureEnabled;

        public Cuboid(float length, float height, float width, Vector3 _position, Color _color, float _shininess, bool _textureEnabled, Texture2D _texture = null, Texture2D _secondTexture = null)
        {
            position = _position;
            color = _color;
            shininess = _shininess;
            vertices = new VertexPositionNormalTexture[36];
            worldMatrix = Matrix.CreateTranslation(position);
            length = length / 2;
            height = height / 2;
            width = width / 2;
            textureEnabled = _textureEnabled;
            texture = _texture;
            secondTexture = _secondTexture;

            Vector3 UpLeftNear = position + new Vector3(-length, height, -width);
            Vector3 UpLeftFar = position + new Vector3(-length, height, width);
            Vector3 UpRightNear = position + new Vector3(length, height, -width);
            Vector3 UpRightFar = position + new Vector3(length, height, width);

            Vector3 DownLeftNear = position + new Vector3(-length, -height, -width);
            Vector3 DownLeftFar = position + new Vector3(-length, -height, width);
            Vector3 DownRightNear = position + new Vector3(length, -height, -width);
            Vector3 DownRightFar = position + new Vector3(length, -height, width);

            CreateWall(UpLeftFar, UpRightFar, DownRightFar, DownLeftFar, 0, new Vector3(0, 0, -1));
            CreateWall(DownLeftNear, DownRightNear, UpRightNear, UpLeftNear, 6, new Vector3(0, 0, 1));
            CreateWall(UpLeftNear, UpRightNear, UpRightFar, UpLeftFar, 12, new Vector3(0, 1, 0));
            CreateWall(DownRightFar, DownRightNear, DownLeftNear, DownLeftFar, 18, new Vector3(0, -1, 0));
            CreateWall(UpLeftFar, DownLeftFar, DownLeftNear, UpLeftNear, 24, new Vector3(-1, 0, 0));
            CreateWall(DownRightNear, DownRightFar, UpRightFar, UpRightNear, 30, new Vector3(1, 0, 0));
        }


        private void CreateWall(Vector3 DownLeft, Vector3 UpLeft, Vector3 UpRight, Vector3 DownRight, int index, Vector3 normalVector)
        {
            Vector3 avg = (DownLeft + UpLeft + UpRight + DownRight) / 4;
            normalVector = avg - position;
            normalVector.Normalize();

            vertices[index + 0] = new VertexPositionNormalTexture(UpRight, normalVector, new Vector2(1, 0));
            vertices[index + 1] = new VertexPositionNormalTexture(DownLeft, normalVector, new Vector2(0, 1));
            vertices[index + 2] = new VertexPositionNormalTexture(UpLeft, normalVector, new Vector2(0, 0));
            vertices[index + 3] = new VertexPositionNormalTexture(UpRight, normalVector, new Vector2(1, 0));
            vertices[index + 4] = new VertexPositionNormalTexture(DownRight, normalVector, new Vector2(1, 1));
            vertices[index + 5] = new VertexPositionNormalTexture(DownLeft, normalVector, new Vector2(0, 1));

        }

        public void ChangeTexture(Texture2D _texture)
        {
            texture = _texture;
        }

        public void Draw(Effect effect, GraphicsDeviceManager graphics)
        {
            effect.Parameters["World"].SetValue(worldMatrix);
            effect.Parameters["Ka"].SetValue(color.ToVector3());
            effect.Parameters["Shininess"].SetValue(shininess);
            effect.Parameters["TextureEnabled"].SetValue(textureEnabled);
            effect.Parameters["SecondTextureEnabled"].SetValue(false);
            if (textureEnabled)
            {
                effect.Parameters["BasicTexture"].SetValue(texture);
                if (secondTexture != null)
                {
                    effect.Parameters["SecondTextureEnabled"].SetValue(true);
                    effect.Parameters["AdditionalTexture"].SetValue(secondTexture);
                }
            }

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }
        }
    }
}
