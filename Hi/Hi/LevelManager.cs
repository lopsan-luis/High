using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace Hi
{
    public static class LevelManager
    {
        #region Declarations
        private static ContentManager Content;
        private static Player player;
        private static int currentLevel;
        private static Vector2 respawnLocation;

        private static List<Drug> drugs = new List<Drug>();
        private static List<Enemy> enemies = new List<Enemy>();
        private static List<Platform> platforms = new List<Platform>();
        #endregion

        #region Properties
        public static int CurrentLevel
        {
            get { return currentLevel; }
        }

        public static Vector2 RespawnLocation
        {
            get { return respawnLocation; }
            set { respawnLocation = value; }
        }
        #endregion

        #region Initialization
        public static void Initialize(
            ContentManager content,
            Player gamePlayer)
        {
            Content = content;
            player = gamePlayer;
        }
        #endregion

        #region Helper Methods
        private static void checkCurrentCellCode()
        {
            string code = TileMap.CellCodeValue(
                TileMap.GetCellByPixel(player.WorldCenter));

            if (code == "DEAD")
            {
                player.Kill();
            }
        }
        #endregion

        #region Public Methods
        public static void LoadLevel(int levelNumber)
        {
            TileMap.LoadMap((System.IO.FileStream)TitleContainer.OpenStream(
                @"Content\Maps\MAP008.MAP"));// +levelNumber.ToString().PadLeft(3, '0') + ".MAP"));

            drugs.Clear();
            enemies.Clear();

            for (int x = 0; x < TileMap.MapWidth; x++)
            {
                for (int y = 0; y < TileMap.MapHeight; y++)
                {
                    if (TileMap.CellCodeValue(x, y) == "START")
                    {
                        player.WorldLocation = new Vector2(
                            x * TileMap.TileWidth,
                            y * TileMap.TileHeight);
                    }

                    if (TileMap.CellCodeValue(x, y) == "GEM")
                    {
                        drugs.Add(new Drug(Content, x, y));
                    }

                    if (TileMap.CellCodeValue(x, y) == "ENEMY")
                    {
                        enemies.Add(new Enemy(Content, x, y));
                    }

                    if (TileMap.CellCodeValue(x, y) == "PLATFORM") {
                        platforms.Add(new Platform(Content.Load<Texture2D>(@"Textures\Sprites\Gem")
                            ,new Rectangle(x,y,50,50)
                            ,Vector2.Zero,Vector2.Zero));
                    }

                }
            }

            currentLevel = levelNumber;
            respawnLocation = player.WorldLocation;
        }

        public static void ReloadLevel()
        {
            Vector2 saveRespawn = respawnLocation;
            LoadLevel(currentLevel);

            respawnLocation = saveRespawn;
            player.WorldLocation = respawnLocation;
        }

        public static void Update(GameTime gameTime)
        {
            if (!player.Dead)
            {
                checkCurrentCellCode(); 

                for (int x = drugs.Count - 1; x >= 0; x--)
                {
                    drugs[x].Update(gameTime);
                    if (player.CollisionRectangle.Intersects(
                        drugs[x].CollisionRectangle))
                    {
                        drugs.RemoveAt(x);
                        player.Score += 10;
                    }
                }

                for (int x = platforms.Count - 1; x >= 0; x--) {
                    platforms[x].Update(gameTime);
                    if (player.CollisionRectangle.Intersects(platforms[x].CollisionRectangle))
                    {
                        platforms[x].CollisionTest(player,gameTime);
                    }
                
                }
                for (int x = enemies.Count - 1; x >= 0; x--)
                {
                    enemies[x].Update(gameTime);
                    if (!enemies[x].Dead)
                    {
                        if (player.CollisionRectangle.Intersects(
                            enemies[x].CollisionRectangle))
                        {
                            if (player.WorldCenter.Y < enemies[x].WorldLocation.Y)
                            {
                                player.Jump();
                                player.Score += 5;
                                enemies[x].PlayAnimation("die");
                                enemies[x].Dead = true; ;
                            }
                            else
                            {
                                player.Kill();
                            }
                        }
                    }
                    else
                    {
                        if (!enemies[x].Enabled)
                        {
                            enemies.RemoveAt(x);
                        }
                    }
                }

            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Drug gem in drugs)
                gem.Draw(spriteBatch);

            foreach (Enemy enemy in enemies)
                enemy.Draw(spriteBatch);
            foreach (Platform platform in platforms)
                platform.Draw(spriteBatch);
        }

        #endregion

    }
}
                                                                                                                                                                                                                                                                                                   