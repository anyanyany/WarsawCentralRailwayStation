using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
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
        Camera camera;
        LightManager lightManager;
        List<MyModel> models;
        List<Cuboid> cuboids;
        Effect effect;
        Scene scene;
        PointLight changingLight;
        double time;
        int selectedColor;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            models = new List<MyModel>();
            cuboids = new List<Cuboid>();
            time = 0;
            selectedColor = 0;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Vector3 camTarget = new Vector3(0, 0, 0);
            Vector3 camPosition = new Vector3(0, 0, 100);
            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
            camera = new Camera(camPosition, camTarget, Vector3.Up, projectionMatrix);
            scene = new Scene(200f, 40f, 80f, new Vector3(0, 0, 0), Color.DimGray, 50);
            cuboids.Add(new Cuboid(200f, 10f, 30f, new Vector3(0, -7.5f, 0), Color.DarkGray, 50));
        }

        protected override void LoadContent()
        {
            bench = Content.Load<Model>("Bench");
            train = Content.Load<Model>("Steam Locomotive");
            ironman = Content.Load<Model>("ironman");
            trash = Content.Load<Model>("cup");
            effect = Content.Load<Effect>("Light");

            lightManager = new LightManager(effect);

            PointLight pl1 = new PointLight(new Vector3(50, 19, 0), Color.LightYellow, Color.LightYellow, 0.5f, 0.9f, 50.0f, 3.0f);
            PointLight pl2 = new PointLight(new Vector3(-50, 19, 0), Color.LightYellow, Color.LightYellow, 0.5f, 0.9f, 50.0f, 2.0f);
            changingLight = new PointLight(new Vector3(0, 19, 0), Color.LightYellow, Color.LightYellow, 0.5f, 0.9f, 50.0f, 2.0f);
            SpotLight sl1 = new SpotLight(new Vector3(45, 10, -25), Color.Yellow, Color.Yellow, 1.0f, 1.0f, 100.0f, 2.0f, new Vector3(1, 0, 0), MathHelper.PiOver4 / 2, MathHelper.PiOver4);
            SpotLight sl2 = new SpotLight(new Vector3(-99, 0, 0), Color.DarkOliveGreen, Color.DarkOliveGreen, 0.5f, 0.5f, 150.0f, 5.0f, new Vector3(1, 0, 0), MathHelper.PiOver4, MathHelper.PiOver2);

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
            float shininess = 10f;
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

            cuboids.Add(new Cuboid(1f, 11f, 1f, new Vector3(-49f, -2, 0), Color.Black, 100));
            cuboids.Add(new Cuboid(1.5f, 4f, 7f, new Vector3(-49f, 0, 0), Color.DarkOliveGreen, 100));
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


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            effect.Parameters["View"].SetValue(camera.ViewMatrix);
            effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            effect.Parameters["CameraPosition"].SetValue(camera.Position);
            scene.Draw(effect, graphics);
            foreach (MyModel model in models)
                model.Draw(effect);
            foreach (Cuboid cuboid in cuboids)
                cuboid.Draw(effect, graphics);
            base.Draw(gameTime);
        }
    }
}
