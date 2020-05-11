using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace MissileCommand
{
    public class Building
    {
        public Vector2 _location;
        private int _ammunition = 150;
        private bool _isAlive;
        private bool _canShoot;

        public Vector2 origin { get { return _location; } }
        public int ammunition { get { return _ammunition; } }
        public bool isAlive { get { return _isAlive; } }
        public bool canShoot { get { return _canShoot; } }

        public Building(Vector2 location, bool isAlive, int ammunition, bool canshoot)
        {
            _location = location;
            _isAlive = isAlive;
            _ammunition = ammunition;
            _canShoot = canshoot;
        }

        public void removeAmmo()
        {
            _ammunition--;
        }

        internal void kill()
        {
            _isAlive = false;
        }

        internal void getammo(int ammo)
        {
            _ammunition = ammo;
        }

        internal void reset()
        {
            if (canShoot)
            {
                _ammunition = 30;
            }
            _isAlive = true;
        }
    }
}
