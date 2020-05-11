using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MissileCommand
{
    public class Missile
    {
        private Vector2 _origin;
        private Vector2 _position;
        private Vector2 _direction;
        private float _distance;
        private Vector2 _destination;
        private float _speed;
        private Color _color;
        private bool _isAlive;
        private int _id;

    

        public Vector2 origin { get { return _origin; } }
        public Vector2 position { get { return _position; } }
        public Vector2 destination { get { return _destination; } }
        public Vector2 direction { get { return _direction; } }
        public float speed { get { return _speed; } }
        public uint colorint { get { return _color.PackedValue; } }
        public Color color { get { return _color; } }
        public bool isAlive { get { return _isAlive; } }
        public int id { get { return _id; } }


        public Missile(Vector2 origin, Vector2 position, Vector2 destination, float speed, Color color, bool alive)
        {
            _origin = origin;
            _position = position;
            _destination = destination;
            _speed = speed;
            _color = color;
            _isAlive = alive;
            Random random = new Random();
            _id = random.Next(10000);
            calculatedistance();

        }
        internal void kill()
        {
            _isAlive = false;
        }
        private void calculatedistance()
        {
            _distance = Vector2.Distance(_origin, _destination);
        }

        public Missile(Vector2 origin, Vector2 position, Vector2 destination, float speed, Color color, bool alive, int id)
        {
            _origin = origin;
            _position = position;
            _destination = destination;
            _speed = speed;
            _color = color;
            _isAlive = alive;
            Random random = new Random();
            _id = id;
            calculatedistance();

        }

        public void update()
        {
            _direction = _destination - _origin;
            _direction.Normalize();
            _position += _direction * _speed;

            if(_distance< Vector2.Distance(_origin, _position))
            {
                _isAlive = false;
            }
        }
        public void calculateDirection()
        {
            _direction = _destination - _origin;
            _direction.Normalize();

        }

    }
}
