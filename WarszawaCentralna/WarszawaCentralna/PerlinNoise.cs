using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarszawaCentralna
{
    class PerlinNoise
    {
        int WIDTH = 256;
        int HEIGHT = 256;

        public PerlinNoise(int width, int height)
        {
            this.WIDTH = width;
            this.HEIGHT = height;
        }

        
        /// Gets the value for a specific X and Y coordinate
        /// results in range [-1, 1] * maxHeight
        public float GetRandomHeight(float X, float Y, float MaxHeight, float Frequency, float Amplitude, float Persistance, int Octaves)
        {
            GenerateNoise();
            float FinalValue = 0.0f;
            for (int i = 0; i < Octaves; ++i)
            {
                FinalValue += GetSmoothNoise(X * Frequency, Y * Frequency) * Amplitude;
                Frequency *= 2.0f;
                Amplitude *= Persistance;
            }
            if (FinalValue < -1.0f)
            {
                FinalValue = -1.0f;
            }
            else if (FinalValue > 1.0f)
            {
                FinalValue = 1.0f;
            }
            return FinalValue * MaxHeight;
        }

        //This function is a simple bilinear filtering function which is good (and easy) enough.        
        private float GetSmoothNoise(float X, float Y)
        {
            float FractionX = X - (int)X;
            float FractionY = Y - (int)Y;
            int X1 = ((int)X + WIDTH) % WIDTH;
            int Y1 = ((int)Y + HEIGHT) % HEIGHT;
            //for cool art deco looking images, do +1 for X2 and Y2 instead of -1...
            int X2 = ((int)X + WIDTH - 1) % WIDTH;
            int Y2 = ((int)Y + HEIGHT - 1) % HEIGHT;
            float FinalValue = 0.0f;
            FinalValue += FractionX * FractionY * Noise[X1, Y1];
            FinalValue += FractionX * (1 - FractionY) * Noise[X1, Y2];
            FinalValue += (1 - FractionX) * FractionY * Noise[X2, Y1];
            FinalValue += (1 - FractionX) * (1 - FractionY) * Noise[X2, Y2];
            return FinalValue;
        }

        float[,] Noise;
        bool NoiseInitialized = false;
        /// create a array of randoms
        private void GenerateNoise()
        {
            if (NoiseInitialized)                //A boolean variable in the class to make sure we only do this once
                return;
            Random random = new Random();
            Noise = new float[WIDTH, HEIGHT];    //Create the noise table where WIDTH and HEIGHT are set to some value>0            
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    Noise[x, y] = ((float)(random.NextDouble()) - 0.5f) * 2.0f;  //Generate noise between -1 and 1
                }
            }
            NoiseInitialized = true;
        }

        

    }
}
