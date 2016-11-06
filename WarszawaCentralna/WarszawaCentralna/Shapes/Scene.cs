using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarszawaCentralna.Shapes
{
    class Scene
    {
        public VertexPositionNormalTexture[] vertices;
        Matrix worldMatrix;
        Vector3 position;
        Color color;
        float shininess;

        public Scene(float length, float height, float width, Vector3 _position, Color _color, float _shininess)
        {
            position = _position;
            color = _color;
            shininess = _shininess;
            vertices = new VertexPositionNormalTexture[36];
            Vector2 texture = new Vector2(0, 0);
            worldMatrix = Matrix.CreateTranslation(position);
            length = length / 2;
            height = height / 2;
            width = width / 2;

            Vector3 DownLeftFar = position + new Vector3(-length, -height, -width);
            Vector3 DownLeftNear = position + new Vector3(-length, -height, width);
            Vector3 DownRightFar = position + new Vector3(length, -height, -width);
            Vector3 DownRightNear = position + new Vector3(length, -height, width);
            Vector3 UpLeftFar = position + new Vector3(-length, height, -width);
            Vector3 UpLeftNear = position + new Vector3(-length, height, width);
            Vector3 UpRightFar = position + new Vector3(length, height, -width);
            Vector3 UpRightNear = position + new Vector3(length, height, width);

            CreateWall(UpLeftFar, UpRightFar, DownRightFar, DownLeftFar, 0);
            CreateWall(DownLeftNear, DownRightNear, UpRightNear, UpLeftNear, 6);
            CreateWall(UpLeftNear, UpRightNear, UpRightFar, UpLeftFar, 12);
            CreateWall(DownRightFar, DownRightNear, DownLeftNear, DownLeftFar, 18);
            CreateWall(UpLeftFar, DownLeftFar, DownLeftNear, UpLeftNear, 24);
            CreateWall(DownRightNear, DownRightFar, UpRightFar, UpRightNear, 30);
        }


        private void CreateWall(Vector3 DownLeft, Vector3 UpLeft, Vector3 UpRight, Vector3 DownRight, int index)
        {
            Vector2 texture = Vector2.Zero;
            Vector3 avg = (DownLeft + UpLeft + UpRight + DownRight) / 4;
            Vector3 normalVector = position - avg;
            normalVector.Normalize();

            vertices[index + 0] = new VertexPositionNormalTexture(UpRight, normalVector, texture);
            vertices[index + 1] = new VertexPositionNormalTexture(DownLeft, normalVector, texture);
            vertices[index + 2] = new VertexPositionNormalTexture(UpLeft, normalVector, texture);
            vertices[index + 3] = new VertexPositionNormalTexture(UpRight, normalVector, texture);
            vertices[index + 4] = new VertexPositionNormalTexture(DownRight, normalVector, texture);
            vertices[index + 5] = new VertexPositionNormalTexture(DownLeft, normalVector, texture);
        }

        public void Draw(Effect effect, GraphicsDeviceManager graphics)
        {
            effect.Parameters["World"].SetValue(worldMatrix);
            effect.Parameters["Ka"].SetValue(color.ToVector3());
            effect.Parameters["Shininess"].SetValue(shininess);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }
        }
    }
}