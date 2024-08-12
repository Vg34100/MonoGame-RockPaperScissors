using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockPaperScissors.Managers;
using System;
using System.Collections.Generic;
using System.IO;

namespace RockPaperScissors.Models
{
    public class Map
    {
        private readonly List<Layer> _layers;
        public Point TileSize { get; private set; }
        public Point MapSize { get; private set; }
        private Texture2D _tileset;
        private int _tilesPerRow;
        public float Scale { get; private set; }

        public Map(List<string> csvFilePaths, Texture2D tileset, int tileWidth, int tileHeight, float scale = 1f)
        {
            _tileset = tileset;
            TileSize = new Point(tileWidth, tileHeight);
            _tilesPerRow = tileset.Width / tileWidth;
            Scale = scale;
            _layers = new List<Layer>();

            foreach (var csvFilePath in csvFilePaths)
            {
                var layer = new Layer(LoadMapFromCSV(csvFilePath), tileset, TileSize, _tilesPerRow, Scale);
                _layers.Add(layer);
            }

            if (_layers.Count > 0)
            {
                MapSize = new Point(
                    (int)(_layers[0].Width * TileSize.X * Scale),
                    (int)(_layers[0].Height * TileSize.Y * Scale)
                );
            }
        }

        private int[,] LoadMapFromCSV(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            int height = lines.Length;
            int width = lines[0].Split(',').Length;
            int[,] mapData = new int[height, width];

            for (int y = 0; y < height; y++)
            {
                string[] tiles = lines[y].Split(',');
                for (int x = 0; x < width; x++)
                {
                    mapData[y, x] = int.Parse(tiles[x]);
                }
            }

            return mapData;
        }

        public void Draw()
        {
            float baseDepth = 1f; // Start with 1.0 for the back-most layer

            foreach (var layer in _layers)
            {
                layer.Draw(baseDepth);
                baseDepth -= 0.1f; // Decrease the base depth for the next layer
            }
        }

        public void DrawBackgroundLayers()
        {
            // Draw only the first layer (background)
            if (_layers.Count > 0)
            {
                _layers[0].DrawBG(1f);
            }
        }

        public void DrawForegroundLayers()
        {
            // Draw all layers except the first one


            for (int i = 1; i < _layers.Count; i++)
            {
                _layers[i].Draw(1 - i * 0.001f);
            }
        }

        public bool CheckCollision(Vector2 position)
        {
            // Convert world position to tile coordinates
            int tileX = (int)(position.X / (TileSize.X * Scale));
            int tileY = (int)(position.Y / (TileSize.Y * Scale));

            // Check layers 2 and 3 for collisions
            for (int i = 1; i < _layers.Count; i++)
            {
                if (_layers[i].HasTileAt(tileX, tileY))
                {
                    return true; // Collision detected
                }
            }

            return false; // No collision
        }
    }

    public class Layer
    {
        private readonly Sprite[,] _tiles;
        public int Width { get; private set; }
        public int Height { get; private set; }
        private float _scale;

        public Layer(int[,] mapData, Texture2D tileset, Point tileSize, int tilesPerRow, float scale)
        {
            Height = mapData.GetLength(0);
            Width = mapData.GetLength(1);
            _tiles = new Sprite[Width, Height];
            _scale = scale;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int tileIndex = mapData[y, x];
                    if (tileIndex >= 0)
                    {
                        Rectangle sourceRect = GetTileSourceRectangle(tileIndex, tileSize, tilesPerRow);
                        Texture2D tileTexture = CreateTileTexture(tileset, sourceRect);
                        Vector2 position = new Vector2(
                            x * tileSize.X * scale + (tileSize.X * scale / 2),
                            y * tileSize.Y * scale + (tileSize.Y * scale / 2)
                        );
                        _tiles[x, y] = new Sprite(tileTexture, position);
                    }
                    else
                    {
                        _tiles[x, y] = null; // No tile for this index
                    }
                }
            }
        }

        private Rectangle GetTileSourceRectangle(int tileIndex, Point tileSize, int tilesPerRow)
        {
            int x = (tileIndex % tilesPerRow) * tileSize.X;
            int y = (tileIndex / tilesPerRow) * tileSize.Y;
            return new Rectangle(x, y, tileSize.X, tileSize.Y);
        }

        private Texture2D CreateTileTexture(Texture2D tileset, Rectangle sourceRect)
        {
            Color[] data = new Color[sourceRect.Width * sourceRect.Height];
            tileset.GetData(0, sourceRect, data, 0, data.Length);

            Texture2D tileTexture = new Texture2D(tileset.GraphicsDevice, sourceRect.Width, sourceRect.Height);
            tileTexture.SetData(data);

            return tileTexture;
        }

        public bool HasTileAt(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return _tiles[x, y] != null;
            }
            return false;
        }

        public void DrawBG(float baseDepth)
        {


            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (_tiles[x, y] != null)
                    {


                        Globals.SpriteBatch.Draw(
                            _tiles[x, y]._texture,
                            _tiles[x, y].Position,
                            null,
                            Color.White,
                            0f,
                            _tiles[x, y].Origin,
                            _scale,
                            SpriteEffects.None,
                            baseDepth
                        );
                    }
                }
            }
        }

        public void Draw(float baseDepth)
        {

            Vector2 cameraPosition = new Vector2(-Globals.GameStateManager._translation.Translation.X, -Globals.GameStateManager._translation.Translation.Y);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (_tiles[x, y] != null)
                    {
                        float heroDepth = baseDepth - ((_tiles[x, y].Position.Y - cameraPosition.Y) / Globals.GameStateManager._map.MapSize.Y);


                        Globals.SpriteBatch.Draw(
                            _tiles[x, y]._texture,
                            _tiles[x, y].Position,
                            null,
                            Color.White,
                            0f,
                            _tiles[x, y].Origin,
                            _scale,
                            SpriteEffects.None,
                            heroDepth
                        );
                    }
                }
            }
        }




    }
}