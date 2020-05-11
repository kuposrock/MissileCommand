using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MissileCommand
{
    public class Explosion
    {
        private Vector2 _origin;
        private Color _color;
        private bool _isAlive;
        private int _id;
        private float _radius;
        private bool alive;

        public Vector2 origin { get { return _origin; } }
        public uint colorint { get { return _color.PackedValue; } }
        public Color color { get { return _color; } }
        public bool isAlive { get { return _isAlive; } }
        public int id { get { return _id; } }
        public float radius { get { return _radius; } }

        public Explosion(Vector2 origin,  Color color, bool alive, float radius)
        {
            _origin = origin;
            _color = color;
            _isAlive = alive;
            _radius = radius;
            Random random = new Random();
            _id = random.Next(10000);
        }
        public Explosion(Vector2 origin, Color color, bool alive, int id, float radius)
        {
            _origin = origin;
            _color = color;
            _radius = radius;
            _isAlive = alive;
            Random random = new Random();
            _id = id;
        }
        public Explosion(Vector2 origin, Color color, bool alive)
        {
            _origin = origin;
            _color = color;
            _isAlive = alive;
            Random random = new Random();
            _id = random.Next(10000);
        }

        public void update()
        {
            _radius+=.6f;
            if(radius > 40)
            {
                _isAlive = false;
            }
        }

    }
}
