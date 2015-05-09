using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FuncWorks.XNA.XTiled;

namespace TestGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Rectangle mapView;
        Map map;

        bool didCollide = false;
        bool canMoveRight = true;
        bool canMoveLeft = true;
        bool canMoveUp = true ;
        bool canMoveDown = true;
        
        Point lastPosition;
        Point startPosition = new Point(84, 129);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            mapView = graphics.GraphicsDevice.Viewport.Bounds;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            map = Content.Load<Map>("desert");
            Map.InitObjectDrawing(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            ObjectLayerList layers = map.ObjectLayers;
            Rectangle delta = mapView;
            ObjectLayer colissionAreas = layers[0];
            ObjectLayer deathAreas = layers[1];
            ObjectLayer mapItems = layers[2];
            MapObject player = mapItems.MapObjects[0];
            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.Escape))
                this.Exit();

            if (keys.IsKeyDown(Keys.S) && canMoveDown)
                player.Bounds.Y += Convert.ToInt32(gameTime.ElapsedGameTime.TotalMilliseconds / 4);
            if (keys.IsKeyDown(Keys.W) && canMoveUp)
                player.Bounds.Y -= Convert.ToInt32(gameTime.ElapsedGameTime.TotalMilliseconds / 4);
            if (keys.IsKeyDown(Keys.D) && canMoveRight)
                player.Bounds.X += Convert.ToInt32(gameTime.ElapsedGameTime.TotalMilliseconds / 4);
            if (keys.IsKeyDown(Keys.A) && canMoveLeft)
                player.Bounds.X -= Convert.ToInt32(gameTime.ElapsedGameTime.TotalMilliseconds / 4);


            if (delta.X != player.Bounds.X && player.Bounds.X > 400 && player.Bounds.X < (map.Bounds.Width - 400))
            {
                delta.X = player.Bounds.X - 400;
            }
            if (delta.Y != player.Bounds.Y && player.Bounds.Y > 240 && player.Bounds.Y < (map.Bounds.Height - 240))
            {
                delta.Y = player.Bounds.Y - 240;
            }

            if(PlayerAndEnemyCollide(player))
            {
                player.Bounds.X = startPosition.X;
                player.Bounds.Y = startPosition.Y;
                delta.Y = 0;
                delta.X = 0;
            }

            if (map.Bounds.Contains(delta))
            {
                mapView = delta;
            }

            if (PlayerAndTileCollide(player))
            {
                player.Bounds.X = lastPosition.X;
                player.Bounds.Y = lastPosition.Y;
                didCollide = true;
            }
            else
            {
                didCollide = false;

                if (!(player.Bounds.X == lastPosition.X
                    && player.Bounds.Y == lastPosition.Y))
                {
                    canMoveDown = true;
                    canMoveLeft = true;
                    canMoveRight = true;
                    canMoveUp = true;
                }

                lastPosition.X = player.Bounds.X;
                lastPosition.Y = player.Bounds.Y;
            }


            base.Update(gameTime);
        }

        public bool PlayerAndTileCollide(MapObject player)
        {
            int collisionLayer = 0;
            Rectangle playerRec = new Rectangle(player.Bounds.X, player.Bounds.Y, player.Bounds.Width, player.Bounds.Height);

            foreach (var obj in map.GetObjectsInRegion(collisionLayer, mapView))
            {
                if (obj.Bounds.Intersects(playerRec))
                {
                    if (player.Bounds.X > lastPosition.X)
                    {
                        canMoveRight = false;
                    }
                    if (player.Bounds.X < lastPosition.X)
                    {
                        canMoveLeft = false;
                    }
                    if (player.Bounds.Y > lastPosition.Y)
                    {
                        canMoveDown = false;
                    }
                    if (player.Bounds.Y < lastPosition.Y)
                    {
                        canMoveUp = false;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool PlayerAndEnemyCollide(MapObject player)
        {
            int enemyLayer = 1;
            Rectangle playerRec = new Rectangle(player.Bounds.X, player.Bounds.Y, player.Bounds.Width, player.Bounds.Height);

            foreach (var obj in map.GetObjectsInRegion(enemyLayer, mapView))
            {
                if (obj.Bounds.Intersects(playerRec))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            map.Draw(spriteBatch, mapView);
            //map.DrawObjectLayer(spriteBatch, 0, mapView, 0f);
            //map.DrawObjectLayer(spriteBatch, 1, mapView, 0f);
            map.DrawObjectLayer(spriteBatch, 2, mapView, 0f);
            
            
            //map.DrawMapObject(spriteBatch, 
            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}













////Rectangle delta = mapView;
//if (keys.IsKeyDown(Keys.Down))
//    delta.Y += Convert.ToInt32(gameTime.ElapsedGameTime.TotalMilliseconds / 4);
//if (keys.IsKeyDown(Keys.Up))
//    delta.Y -= Convert.ToInt32(gameTime.ElapsedGameTime.TotalMilliseconds / 4);
//if (keys.IsKeyDown(Keys.Right))
//    delta.X += Convert.ToInt32(gameTime.ElapsedGameTime.TotalMilliseconds / 4);
//if (keys.IsKeyDown(Keys.Left))
//    delta.X -= Convert.ToInt32(gameTime.ElapsedGameTime.TotalMilliseconds / 4);

//delta.X += player.Bounds.X;