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

        public MyModel(Model _model, Matrix _worldMatrix, Color _color, float _shininess)
        {
            model = _model;
            worldMatrix = _worldMatrix;
            color = _color;
            shininess = _shininess;
        }

        public void Draw(Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    effect.Parameters["World"].SetValue(worldMatrix);
                    effect.Parameters["Ka"].SetValue(color.ToVector3());
                    //effect.Parameters["Kd"].SetValue(color.ToVector3());
                    //effect.Parameters["Ks"].SetValue(color.ToVector3());
                    effect.Parameters["Shininess"].SetValue(shininess);
                    part.Effect = effect;
                }
                mesh.Draw();
            }
        }
    }
}

