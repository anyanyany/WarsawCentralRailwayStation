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
        LightManager lightManager;
        List<MyModel> models;
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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            models = new List<MyModel>();
            cuboids = new List<Cuboid>();
            effects = new List<Effect>();
            time = 0;
            selectedColor = 0;
            filterMagLinear = true;
            graphics.PreferMultiSampling = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Vector3 camTarget = new Vector3(0, 0, 0);
            Vector3 camPosition = new Vector3(0, 0, 100);
            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
            camera = new Camera(camPosition, camTarget, Vector3.Up, projectionMatrix);
            keyboardOldState = Keyboard.GetState();
         

            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        protected override void LoadContent()
        {
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

            Stream stream = File.Create("PERLIN.png");
            perlinTexture = CreatePerlinNoiseTexture(1000, 400);
            perlinTexture.SaveAsPng(stream, 1000, 400);
            stream.Dispose();

            effects.Add(effectWithTexture);
            effects.Add(effectWithoutTexture);
            lightManager = new LightManager(effects);

            scene = new Scene(200f, 40f, 80f, new Vector3(0, 0, 0), Color.Silver, 50, wallTexture);
            platform = new Cuboid(200f, 10f, 30f, new Vector3(0, -7.5f, 0), Color.DarkGray, 50, true, concreteTexture, linesTexture);
            cuboids.Add(platform);

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

            LoadModels();
        }

        private void LoadModels()
        {
            Matrix worldMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateScale(0.1f, 0.2f, 0.15f) * Matrix.CreateTranslation(0, 0, -25f);
            Color color = Color.Black;
            float shininess = 250f;
            models.Add(new MyModel(train, worldMatrix, color, 100));

            color = Color.Brown;
            worldMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(-7.5f, -8.5f, 0);
            models.Add(new MyModel(bench, worldMatrix, color, shininess));
            worldMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(-5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(7.5f, -8.5f, 0);
            models.Add(new MyModel(bench, worldMatrix, color, shininess));
            worldMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(-85, -8.5f, 0);
            models.Add(new MyModel(bench, worldMatrix, color, shininess));
            worldMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(-5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(-70, -8.5f, 0);
            models.Add(new MyModel(bench, worldMatrix, color, shininess));
            worldMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(70, -8.5f, 0);
            models.Add(new MyModel(bench, worldMatrix, color, shininess));
            worldMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateRotationX(-MathHelper.PiOver2 + MathHelper.ToRadians(-5)) * Matrix.CreateScale(8f, 0.07f, 0.3f) * Matrix.CreateTranslation(85, -8.5f, 0);
            models.Add(new MyModel(bench, worldMatrix, color, shininess));

            color = Color.Maroon;
            worldMatrix = Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(MathHelper.PiOver4 / 2) * Matrix.CreateScale(8) * Matrix.CreateTranslation(-70, -10, -10);
            models.Add(new MyModel(ironman, worldMatrix, color, shininess));

            color = Color.LightSteelBlue;
            worldMatrix = Matrix.CreateTranslation(40, -10, 0);
            models.Add(new MyModel(trash, worldMatrix, color, shininess));
            worldMatrix = Matrix.CreateTranslation(-40, -10, 0);
            models.Add(new MyModel(trash, worldMatrix, color, shininess));

            cuboids.Add(new Cuboid(1f, 11f, 1f, new Vector3(-49f, -2, 0), Color.Black, 100, false));
            cuboids.Add(new Cuboid(1.5f, 4f, 7f, new Vector3(-49f, 0, 0), Color.DarkOliveGreen, 100, true, lampTexture));

            worldMatrix = Matrix.CreateTranslation(40, -10, 20);
            worldMatrix = Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(0.8f) * Matrix.CreateTranslation(100, -10, 0);

            models.Add(new MyModel(frame, worldMatrix, color, shininess));
        }

        protected override void UnloadContent()
        {

        }

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
                    graphics.PreferMultiSampling = !graphics.PreferMultiSampling;
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

            if (keyboardNewState.IsKeyDown(Keys.P))
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
            }
        }

        protected void DrawScene()
        {
            GraphicsDevice.Clear(Color.Black);
            RasterizerState rasterizationState = new RasterizerState { MultiSampleAntiAlias = true };
            GraphicsDevice.RasterizerState = rasterizationState;

            effectWithTexture.Parameters["View"].SetValue(camera.ViewMatrix);
            effectWithTexture.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            effectWithTexture.Parameters["CameraPosition"].SetValue(camera.Position);


            effectWithoutTexture.Parameters["View"].SetValue(camera.ViewMatrix);
            effectWithoutTexture.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            effectWithoutTexture.Parameters["CameraPosition"].SetValue(camera.Position);
            scene.Draw(effectWithTexture, graphics);

            foreach (MyModel model in models)
                model.Draw(effectWithoutTexture);
            foreach (Cuboid cuboid in cuboids)
                cuboid.Draw(effectWithTexture, graphics);
            effectWithTexture.Parameters["BasicTexture"].SetValue(ironmanTexture);
            models[7].Draw(effectWithTexture);
            effectWithTexture.Parameters["BasicTexture"].SetValue(renderTarget);
            models[10].Draw(effectWithTexture);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawSceneToTexture(renderTarget);
            //scene.ChangeTexture(renderTarget); // CHANGE THIS

            DrawScene();

            base.Draw(gameTime);
        }

        protected void DrawSceneToTexture(RenderTarget2D renderTarget)
        {
            //http://rbwhitaker.wikidot.com/render-to-texture

            // Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            // Draw the scene
            DrawScene();
            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }

        public Texture2D CreatePerlinNoiseTexture(int sizex, int sizey)
        {
            PerlinNoise pn = new PerlinNoise();
            Texture2D t = new Texture2D(GraphicsDevice, sizex, sizey);
            double[,] perlin = new double[sizex, sizey];
            Color[] cor = new Color[sizex * sizey];
            Random r = new Random();
            
            for (int i = 0; i < sizex; i++)
            {
                for (int j = 0; j < sizey; j++)
                {
                    double value = pn.OctavePerlin((double)i /20, (double)j/20, 0,9,0.5);
                    perlin[i, j] = value;
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

    }
}
