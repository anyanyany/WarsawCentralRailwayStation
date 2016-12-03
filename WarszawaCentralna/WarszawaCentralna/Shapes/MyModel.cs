using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarszawaCentralna.Shapes
{
    class MyModel
    {
        Model model;
        Matrix worldMatrix;
        Color color;
        float shininess;
        Texture2D texture;
        bool textureEnabled;

        public MyModel(Model _model, Matrix _worldMatrix, Color _color, float _shininess, bool _textureEnabled, Texture2D _texture = null)
        {
            model = _model;
            worldMatrix = _worldMatrix;
            color = _color;
            shininess = _shininess;
            textureEnabled = _textureEnabled;
            texture = _texture;
        }

        public void ChangeTexture(Texture2D _texture)
        {
            texture = _texture;
        }

        public void Draw(Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    effect.Parameters["World"].SetValue(worldMatrix);
                    effect.Parameters["Ka"].SetValue(color.ToVector3());
                    effect.Parameters["Shininess"].SetValue(shininess);                   
                    if (textureEnabled)
                    {
                        effect.Parameters["TextureEnabled"].SetValue(textureEnabled);
                        effect.Parameters["BasicTexture"].SetValue(texture);
                    }                       
                    part.Effect = effect;
                }
                mesh.Draw();
            }
        }
    }
}

