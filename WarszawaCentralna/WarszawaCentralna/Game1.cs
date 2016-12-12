using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using WarszawaCentralna.Lighting;
using WarszawaCentralna.Shapes;

namespace WarszawaCentralna
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

        private Model bench;
        private Model train;
        private Model ironman;
        private Model trash;
        private Model frame;
        private Texture2D wallTexture;
        private Texture2D concreteTexture;
        private Texture2D lampTexture;
        private Texture2D ironmanTexture;
        private Texture2D linesTexture;
        private Texture2D woodTexture;
        private Texture2D platformTexture;
        private Texture2D sceneTexture;
        private Texture2D perlinTexture;
        Camera camera;
        Camera secondCamera;
        LightManager lightManager;
        List<MyModel> modelsWithoutTexture;
        List<MyModel> modelsWithTexture;
        List<Cuboid> cuboids;
        Effect effectWithTexture;
        Effect effectWithoutTexture;
        List<Effect> effects;
        Scene scene;
        Cuboid platform;
        PointLight changingLight;
        double time;
        int selectedColor;
        bool filterMagLinear;
        KeyboardState keyboardOldState;
        RenderTarget2D renderTarget;
        RenderTarget2D gaussRenderTarget;
        float fogEnabled;
        bool gaussianBlurEnabled;
        SpriteBatch spriteBatch;
        bool multiSamplingEnabled;
        string perlinPath;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            modelsWithoutTexture = new List<MyModel>();
            modelsWithTexture = new List<MyModel>();
            cuboids = new List<Cuboid>();
            effects = new List<Effect>();
            time = 0;
            selectedColor = 0;
            filterMagLinear = true;
            multiSamplingEnabled = false;
            graphics.PreferMultiSampling = true;
            fogEnabled = 1.0f;
            gaussianBlurEnabled = false;
            perlinPath = "PERLIN.png";

        }

        protected override void Initialize()
        {
            base.Initialize();
            Vector3 camTarget = new Vector3(0, 0, 0);
            Vector3 camPosition = new Vector3(0, 0, 100);
            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
            camera = new Camera(camPosition, camTarget, Vector3.Up, projectionMatrix);
            secondCamera = new Camera(camPosition, new Vector3(-30, 0, 0), Vector3.Up, projectionMatrix);
            keyboardOldState = Keyboard.GetState();
            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            gaussRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            bench = Content.Load<Model>("Bench");
            train = Content.Load<Model>("Steam Locomotive");
            ironman = Content.Load<Model>("ironman");
            trash = Content.Load<Model>("cup");
            frame = Content.Load<Model>("ad");
            effectWithTexture = Content.Load<Effect>("LightWithTexture");
            effectWithoutTexture = Content.Load<Effect>("LightWithoutTexture");
            concreteTexture = Content.Load<Texture2D>("concrete");
            ironmanTexture = Content.Load<Texture2D>("ironman.dff");
            linesTexture = Content.Load<Texture2D>("lines");
            wallTexture = Content.Load<Texture2D>("wall");
            lampTexture = Content.Load<Texture2D>("lamp");
            woodTexture = Content.Load<Texture2D>("wood");
            platformTexture = concreteTexture;
            sceneTexture = wallTexture;

            effects.Add(effectWithTexture);
            effects.Add(effectWithoutTexture);
            lightManager = new LightManager(effects);

            PointLight pl1 = new PointLight(new Vector3(50, 19, 0), Color.LightYellow, Color.LightYellow, 0.5f, 0.9f, 50.0f, 3.0f);
            PointLight pl2 = new PointLight(new Vector3(-50, 19, 0), Color.LightYellow, Color.LightYellow, 0.5f, 0.9f, 50.0f, 2.0f);
            changingLight = new PointLight(new Vector3(0, 19, 0), Color.LightYellow, Color.LightYellow, 0.5f, 0.9f, 50.0f, 2.0f);
            SpotLight sl1 = new SpotLight(new Vector3(45, 10, -25), Color.Yellow, Color.Yellow, 1.0f, 1.0f, 100.0f, 2.0f, new Vector3(1, 0, 0), MathHelper.PiOver4 / 2, MathHelper.PiOver4);
            SpotLight sl2 = new SpotLight(new Vector3(-99, 0, 0), Color.HotPink, Color.HotPink, 0.5f, 0.5f, 150.0f, 5.0f, new Vector3(1, 0, 0), MathHelper.PiOver4, MathHelper.PiOver2);

            lightManager.addPointLight(pl1);
            lightManager.addPointLight(pl2);
            lightManager.addPointLight(changingLight);
            lightManager.addSpotLight(sl1);
            lightManager.addSpotLight(sl2);
            lightManager.SetEffectParameters();

            if (File.Exists(perlinPath))
            {
                FileStream filestream = new FileStream(perlinPath, FileMode.Open);
                perlinTexture = Texture2D.FromStream(GraphicsDevice, filestream);
            }
            else
            {
                Stream stream = File.Create(perlinPath);
                perlinTexture = CreatePerlinNoiseTexture(1000, 400);
                perlinTexture.SaveAsPng(stream, 1000, 400);
                stream.Dispose();
            }

            LoadModels();
        }

        private void LoadModels()
        {
            scene = new Scene(200f, 40f, 80f, new Vector3(0, 0, 0), Color.Silver, 50, wallTexture);
            platform = new Cuboid(200f, 10f, 30f, new Vector3(0, -7.5f, 0), Color.DarkGray, 50, true, concreteTexture, linesTexture);

            cuboids.Add(platform);
            Matrix worldMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateScale(0.1f, 0.2f, 0.15f) * Matrix.CreateTranslation(0, 0, -25f);
            Color color = Color.Black;
            float shininess = 250f;
            modelsWithoutTexture.Add(new MyModel(train, worldMatrix, color, 100, false));

            color = Color.Brown;
            worldMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(-7.5f, -8.5f, 0);
            modelsWithoutTexture.Add(new MyModel(bench, worldMatrix, color, shininess, false));
            worldMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(-5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(7.5f, -8.5f, 0);
            modelsWithoutTexture.Add(new MyModel(bench, worldMatrix, color, shininess, false));
            worldMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(-85, -8.5f, 0);
            modelsWithoutTexture.Add(new MyModel(bench, worldMatrix, color, shininess, false));
            worldMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(-5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(-70, -8.5f, 0);
            modelsWithoutTexture.Add(new MyModel(bench, worldMatrix, color, shininess, false));
            worldMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(70, -8.5f, 0);
            modelsWithoutTexture.Add(new MyModel(bench, worldMatrix, color, shininess, false));
            worldMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(-5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(85, -8.5f, 0);
            modelsWithoutTexture.Add(new MyModel(bench, worldMatrix, color, shininess, false));

            color = Color.Maroon;
            worldMatrix = Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(MathHelper.PiOver4 / 2) * Matrix.CreateScale(8) * Matrix.CreateTranslation(-70, -10, -10);
            modelsWithTexture.Add(new MyModel(ironman, worldMatrix, color, shininess, true, ironmanTexture));

            color = Color.LightSteelBlue;
            worldMatrix = Matrix.CreateTranslation(40, -10, 0);
            modelsWithoutTexture.Add(new MyModel(trash, worldMatrix, color, shininess, false));
            worldMatrix = Matrix.CreateTranslation(-40, -10, 0);
            modelsWithoutTexture.Add(new MyModel(trash, worldMatrix, color, shininess, false));

            cuboids.Add(new Cuboid(1f, 11f, 1f, new Vector3(-49f, -2, 0), Color.Black, 100, false));
            cuboids.Add(new Cuboid(1.5f, 4f, 7f, new Vector3(-49f, 0, 0), Color.DarkOliveGreen, 100, true, lampTexture));

            worldMatrix = Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(0.8f) * Matrix.CreateTranslation(100, -10, 0);
            modelsWithTexture.Add(new MyModel(frame, worldMatrix, color, shininess, true, renderTarget));
        }

        protected override void UnloadContent() {}

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            camera.Update();
            changeLights(gameTime);
            KeyboardState keyboardNewState = Keyboard.GetState();

            if (keyboardNewState.IsKeyDown(Keys.Z))
            {
                if (!keyboardOldState.IsKeyDown(Keys.Z))
                {
                    filterMagLinear = !filterMagLinear;
                    effectWithTexture.Parameters["filterMagLinear"].SetValue(filterMagLinear);
                }
            }
            if (keyboardNewState.IsKeyDown(Keys.M)) //zadanie 3. MultiSampleAntiAliasing
            {
                if (!keyboardOldState.IsKeyDown(Keys.M))
                {
                    multiSamplingEnabled = !multiSamplingEnabled;
                }
            }
            if (keyboardNewState.IsKeyDown(Keys.T)) //Multiteksturowanie 
            {
                if (!keyboardOldState.IsKeyDown(Keys.T))
                {
                    if (platformTexture == concreteTexture)
                        platformTexture = woodTexture;
                    else
                        platformTexture = concreteTexture;
                    platform.ChangeTexture(platformTexture);
                }
            }

            if (keyboardNewState.IsKeyDown(Keys.P)) //szum perlina
            {
                if (!keyboardOldState.IsKeyDown(Keys.P))
                {
                    if (sceneTexture == wallTexture)
                        sceneTexture = perlinTexture;
                    else
                        sceneTexture = wallTexture;
                    scene.ChangeTexture(sceneTexture);
                }
            }

            if (keyboardNewState.IsKeyDown(Keys.F)) //mgła
            {
                if (!keyboardOldState.IsKeyDown(Keys.F))
                {
                    fogEnabled = (fogEnabled + 1) % 2;
                }
            }

            if (keyboardNewState.IsKeyDown(Keys.G)) //gauss
            {
                if (!keyboardOldState.IsKeyDown(Keys.G))
                {
                    gaussianBlurEnabled = !gaussianBlurEnabled;
                }
            }

            keyboardOldState = keyboardNewState;
            base.Update(gameTime);
        }

        private void changeLights(GameTime gameTime)
        {
            Color[] colors = { Color.Red, Color.Yellow, Color.GreenYellow, Color.Green, Color.CornflowerBlue, Color.Blue, Color.Violet };
            time += gameTime.ElapsedGameTime.TotalSeconds;
            if (time > 1)
            {
                time = 0;
                selectedColor = (selectedColor + 1) % colors.Length;
                changingLight.Kd = changingLight.Ks = colors[selectedColor];
                lightManager.SetEffectParameters();

                if (gaussianBlurEnabled)
                {
                    Vector3 camTarget = new Vector3(camera.Target.X + 5, camera.Target.Y, camera.Target.Z);
                    Vector3 camPosition = camera.Position;
                    Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
                    camera = new Camera(camPosition, camTarget, Vector3.Up, projectionMatrix);
                }
            }
        }

        protected void DrawScene(Camera _camera)
        {
            GraphicsDevice.Clear(Color.Black);
            //RasterizerState rasterizationState = new RasterizerState { MultiSampleAntiAlias = multiSamplingEnabled };
            //GraphicsDevice.RasterizerState = rasterizationState;
            //https://msdn.microsoft.com/pl-pl/library/bb975403.aspx
            RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;

            effectWithTexture.Parameters["View"].SetValue(_camera.ViewMatrix);
            effectWithoutTexture.Parameters["View"].SetValue(_camera.ViewMatrix);

            effectWithTexture.Parameters["Projection"].SetValue(_camera.ProjectionMatrix);
            effectWithoutTexture.Parameters["Projection"].SetValue(_camera.ProjectionMatrix);

            effectWithTexture.Parameters["CameraPosition"].SetValue(_camera.Position);
            effectWithoutTexture.Parameters["CameraPosition"].SetValue(_camera.Position);

            effectWithTexture.Parameters["FogEnabled"].SetValue(fogEnabled);
            effectWithoutTexture.Parameters["FogEnabled"].SetValue(fogEnabled);

            scene.Draw(effectWithTexture, graphics);
            foreach (MyModel model in modelsWithoutTexture)
                model.Draw(effectWithoutTexture);
            modelsWithTexture[1].ChangeTexture(renderTarget);
            foreach (MyModel model in modelsWithTexture)
                model.Draw(effectWithTexture);
            foreach (Cuboid cuboid in cuboids)
                cuboid.Draw(effectWithTexture, graphics);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawSceneToTexture(renderTarget, camera); //can be camera
            DrawScene(camera);

            if (gaussianBlurEnabled)
            {
                DrawSceneToTexture(gaussRenderTarget, camera); //can be camera
                Texture2D tex = GaussianBlur(gaussRenderTarget);
                Stream stream = File.Create("GaussianBlur.png");
                tex.SaveAsPng(stream, tex.Width, tex.Height);
                stream.Dispose();

                spriteBatch.Begin();
                spriteBatch.Draw(tex, new Rectangle(0, 0, 800, 480), Color.White);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        protected void DrawSceneToTexture(RenderTarget2D renderTarget, Camera _camera)
        {
            //http://rbwhitaker.wikidot.com/render-to-texture
            // Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            // Draw the scene
            DrawScene(_camera);
            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }

        public Texture2D CreatePerlinNoiseTexture(int sizex, int sizey)
        {
            Texture2D t = new Texture2D(GraphicsDevice, sizex, sizey);
            double[,] perlin = new double[sizex, sizey];
            Color[] cor = new Color[sizex * sizey];
            Random r = new Random();

            for (int i = 0; i < sizex; i++)
            {
                for (int j = 0; j < sizey; j++)
                {
                    perlin[i, j] = PerlinNoise.OctavePerlin((double)i / 20, (double)j / 20, 0, 9, 0.5);
                    //http://www.upvector.com/?section=Tutorials&subsection=Intro%20to%20Procedural%20Textures
                    perlin[i, j] = (1 + Math.Sin((i + perlin[i, j] / 2) * 50)) / 2;
                }
            }

            for (int i = 0; i < sizex; i++)
            {
                for (int j = 0; j < sizey; j++)
                {
                    cor[i + j * sizex] = new Color((float)perlin[i, j], (float)perlin[i, j], (float)perlin[i, j]);
                }
            }
            t.SetData(cor);
            return t;
        }

        public Texture2D GaussianBlur(Texture2D image) //https://en.wikipedia.org/wiki/Gaussian_blur
        {
            int sizex = image.Width;
            int sizey = image.Height;
            double minimumBrightness = 120;
            Color[] colors = new Color[sizex * sizey];
            image.GetData<Color>(colors);
            int filterSize = 5;
            double[,] filterMatrix = new double[filterSize, filterSize];

            int filterOffset = (int)((filterSize - 1) / 2);
            double blue;
            double green;
            double red;
            int offset;
            double stddev = 0.8; //radius=2x

            for (int offsetY = filterOffset; offsetY < sizey - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sizex - filterOffset; offsetX++)
                {
                    int r = colors[offsetX + (offsetY * sizex)].R;
                    int g = colors[offsetX + (offsetY * sizex)].G;
                    int b = colors[offsetX + (offsetY * sizex)].B;
                    double L = 0.3 * r + 0.59 * g + 0.11 * b;
                    if (L >= minimumBrightness)
                    {
                        stddev = 0.6+4*(L-minimumBrightness)/(255-minimumBrightness);
                        double sum = 0;
                        for (int x = -filterOffset; x <= filterOffset; x++)
                        {
                            for (int y = -filterOffset; y <= filterOffset; y++)
                            {
                                double value = Math.Exp(-(x * x + y * y) / (2 * stddev * stddev)) / (2 * Math.PI * stddev * stddev);
                                filterMatrix[x + filterOffset, y + filterOffset] = value;
                                sum += value;
                            }
                        }
                        if (sum == 0)
                            sum = 1;

                        blue = 0;
                        green = 0;
                        red = 0;
                        offset = offsetX + (offsetY * sizex);

                        for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                        {
                            for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                            {
                                int calcOffset = offset + filterX + filterY * filterSize;
                                blue += (colors[calcOffset].B) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                                green += (colors[calcOffset].G) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                                red += (colors[calcOffset].R) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                            }
                        }

                        blue /= sum;
                        red /= sum;
                        green /= sum;
                        blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
                        green = (green > 255 ? 255 : (green < 0 ? 0 : green));
                        red = (red > 255 ? 255 : (red < 0 ? 0 : red));

                        colors[offset] = new Color((int)red, (int)green, (int)blue);
                    }
                }
            }

            Texture2D t = new Texture2D(GraphicsDevice, sizex, sizey);
            t.SetData(colors);
            return t;
        }
    }
}
