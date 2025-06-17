using System;
using System.Text;

namespace UnityEngine
{
    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int right => new Vector2Int(1, 0);
        public static Vector2Int left => new Vector2Int(-1, 0);
        public static Vector2Int up => new Vector2Int(0, 1);
        public static Vector2Int down => new Vector2Int(0, -1);
        public static Vector2Int zero => new Vector2Int(0, 0);

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public static Vector2Int operator *(Vector2Int a, int m)
        {
            return new Vector2Int(a.x * m, a.y * m);
        }
    }

    public static class Random
    {
        private static System.Random _rand = new System.Random();
        public static float value => (float)_rand.NextDouble();
    }

    public static class Mathf
    {
        public static int FloorToInt(float v)
        {
            return (int)Math.Floor(v);
        }

        public static int CeilToInt(float v)
        {
            return (int)Math.Ceiling(v);
        }

        public static int Abs(int v)
        {
            return Math.Abs(v);
        }

        public static float Abs(float v)
        {
            return Math.Abs(v);
        }

        public static int Max(int a, int b)
        {
            return Math.Max(a, b);
        }
    }
}

namespace Runtime.CardGameplay.Card
{
    using Runtime.CardGameplay.Card.View;

    public class DescriptionBuilder
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private bool _first = true;

        private void NewLine()
        {
            if (!_first)
            {
                _builder.Append('\n');
            }
            _first = false;
        }

        public DescriptionBuilder WithLine(string line)
        {
            if (line != null)
            {
                NewLine();
                _builder.Append(line);
            }
            return this;
        }

        public DescriptionBuilder WithKeyword(Keyword keyword, bool newLine = true)
        {
            if (newLine)
            {
                NewLine();
            }
            _builder.Append(keyword.FormattedText);
            return this;
        }

        public DescriptionBuilder AppendInLine(string text)
        {
            _builder.Append(text);
            return this;
        }

        public string GetFormattedText()
        {
            return _builder.ToString().Trim();
        }
    }
}
