using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarszawaCentralna.Lighting
{
    class SpotLight
    {
        public Vector3 Position { get; set; }
        public float Id { get; set; }
        public float Is { get; set; }
        public Color Kd { get; set; }
        public Color Ks { get; set; }
        public float Attenuation { get; set; }
        public float Falloff { get; set; }
        public Vector3 Direction { get; set; }
        public float InnerConeAngle { get; set; }
        public float OuterConeAngle { get; set; }

        public SpotLight(Vector3 _Position, Color _Kd, Color _Ks, float _Id, float _Is, float _Attenuation, float _Falloff, Vector3 _Direction, float _InnerConeAngle, float _OuterConeAngle)
        {
            Position = _Position;
            Id = _Id;
            Is = _Is;
            Kd = _Kd;
            Ks = _Ks;
            Attenuation = _Attenuation;
            Falloff = _Falloff;
            Direction = _Direction;
            InnerConeAngle = _InnerConeAngle;
            OuterConeAngle = _OuterConeAngle;
        }
    }
}

