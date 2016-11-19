using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarszawaCentralna.Lighting
{
    class LightManager
    {
        public List<Effect> effects;
        List<PointLight> pointLights;
        List<SpotLight> spotLights;

        public LightManager(List<Effect> _effects)
        {
            effects = _effects;
            pointLights = new List<PointLight>();
            spotLights = new List<SpotLight>();
        }

        public void addPointLight(PointLight _light)
        {
            pointLights.Add(_light);
        }

        public void addSpotLight(SpotLight _light)
        {
            spotLights.Add(_light);
        }

        public void SetEffectParameters()
        {
            int allLights = pointLights.Count + spotLights.Count;
            Vector3[] Position = new Vector3[allLights];
            float[] Id = new float[allLights];
            float[] Is = new float[allLights];
            Vector3[] Kd = new Vector3[allLights];
            Vector3[] Ks = new Vector3[allLights];
            float[] Attenuation = new float[allLights];
            float[] Falloff = new float[allLights];

            Vector3[] Direction = new Vector3[spotLights.Count];
            float[] InnerConeAngle = new float[spotLights.Count];
            float[] OuterConeAngle = new float[spotLights.Count];


            for (int i = 0; i < pointLights.Count; i++)
            {
                Position[i] = pointLights.ElementAt(i).Position;
                Id[i] = pointLights.ElementAt(i).Id;
                Is[i] = pointLights.ElementAt(i).Is;
                Kd[i] = pointLights.ElementAt(i).Kd.ToVector3();
                Ks[i] = pointLights.ElementAt(i).Ks.ToVector3();
                Attenuation[i] = pointLights.ElementAt(i).Attenuation;
                Falloff[i] = pointLights.ElementAt(i).Falloff;
            }

            for (int i = 0; i < spotLights.Count; i++)
            {
                Position[pointLights.Count + i] = spotLights.ElementAt(i).Position;
                Id[pointLights.Count + i] = spotLights.ElementAt(i).Id;
                Is[pointLights.Count + i] = spotLights.ElementAt(i).Is;
                Kd[pointLights.Count + i] = spotLights.ElementAt(i).Kd.ToVector3();
                Ks[pointLights.Count + i] = spotLights.ElementAt(i).Ks.ToVector3();
                Attenuation[pointLights.Count + i] = spotLights.ElementAt(i).Attenuation;
                Falloff[pointLights.Count + i] = spotLights.ElementAt(i).Falloff;

                Direction[i] = spotLights.ElementAt(i).Direction;
                InnerConeAngle[i] = spotLights.ElementAt(i).InnerConeAngle;
                OuterConeAngle[i] = spotLights.ElementAt(i).OuterConeAngle;
            }

            foreach (Effect effect in effects)
            {
                effect.Parameters["LightPosition"].SetValue(Position);
                effect.Parameters["Id"].SetValue(Id);
                effect.Parameters["Is"].SetValue(Is);
                effect.Parameters["Kd"].SetValue(Kd);
                effect.Parameters["Ks"].SetValue(Ks);
                effect.Parameters["Attenuation"].SetValue(Attenuation);
                effect.Parameters["Falloff"].SetValue(Falloff);
                effect.Parameters["LightDirection"].SetValue(Direction);
                effect.Parameters["InnerConeAngle"].SetValue(InnerConeAngle);
                effect.Parameters["OuterConeAngle"].SetValue(OuterConeAngle);
            }
            
        }
    }
}
