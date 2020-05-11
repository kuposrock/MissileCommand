using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MissileCommand
{
    public class EntityManager
    {
        private Game _game;
        private Vector2 _networkMouse;
        public List<Missile> _missiles = new List<Missile>();
        public List<Missile> _missilesToRemove = new List<Missile>();
        public List<Explosion> _explosions = new List<Explosion>();
        public List<Explosion> _explosionsToRemove = new List<Explosion>();
        public List<Building> _buildings = new List<Building>();
        public List<Building> _buildingsToRemove = new List<Building>();
        private Building _turrentA;
        private Building _turrentB;
        private Building _turrentC;
        private Building _turrentD;
        private float speedMultipler = 7f;

        public Vector2 networkmouse { get { return _networkMouse; } }
        public int _missileCount { get { return _missiles.Count; } }
        public int _explosionsCount { get { return _explosions.Count; } }
        public int _buildingsCount { get { return _buildings.Count; } }
        public int _buildingsToRemoveCount { get { return _buildingsToRemove.Count; } }
        public Building turretA { get { return _turrentA; } }
        public Building turretB { get { return _turrentB; } }
        public Building turretC { get { return _turrentC; } }
        public Building turretD { get { return _turrentD; } }

        public EntityManager(Game game)
        {
            _game = game;
            _networkMouse = new Vector2(0, 0);
            _turrentA = new Building(new Vector2(188, 775), true, 15, true);
            _turrentB = new Building(new Vector2(474, 775), true, 15, true);
            _turrentC = new Building(new Vector2(760, 775), true, 15, true);
            _turrentD = new Building(new Vector2(1046, 775), true, 15, true);


            _buildings.Add(new Building(new Vector2(66, 775), true, 0, false));//Silo1
            _buildings.Add(new Building(new Vector2(264, 775), true, 0, false));//silo2

            _buildings.Add(new Building(new Vector2(352, 775), true, 0, false));//silo2
            _buildings.Add(new Building(new Vector2(570, 775), true, 0, false));//silo2

            _buildings.Add(new Building(new Vector2(638, 775), true, 0, false));
            _buildings.Add(new Building(new Vector2(836, 775), true, 0, false));

            _buildings.Add(new Building(new Vector2(924, 775), true, 0, false));
            _buildings.Add(new Building(new Vector2(1112, 775), true, 0, false));

        }
 

        public void updateMultiplayerMouse(int x, int y)
        {
            _networkMouse.X = x;
            _networkMouse.Y = y;
        }

        internal void reset()
        {
            _missiles.Clear();
            _missilesToRemove.Clear();
            _explosions.Clear();
            _explosionsToRemove.Clear();
            _buildings.Clear();
            _buildingsToRemove.Clear();

            _turrentA = new Building(new Vector2(188, 775), true, 150, true);
            _turrentB = new Building(new Vector2(474, 775), true, 150, true);
            _turrentC = new Building(new Vector2(760, 775), true, 150, true);
            _turrentD = new Building(new Vector2(1046, 775), true, 150, true);

            _buildings.Add(new Building(new Vector2(66, 775), true, 0, false));//Silo1
            _buildings.Add(new Building(new Vector2(264, 775), true, 0, false));//silo2

            _buildings.Add(new Building(new Vector2(352, 775), true, 0, false));//silo2
            _buildings.Add(new Building(new Vector2(570, 775), true, 0, false));//silo2

            _buildings.Add(new Building(new Vector2(638, 775), true, 0, false));
            _buildings.Add(new Building(new Vector2(836, 775), true, 0, false));

            _buildings.Add(new Building(new Vector2(924, 775), true, 0, false));
            _buildings.Add(new Building(new Vector2(1112, 775), true, 0, false));
        }


        public void shootmissile(Keys keys)
        {
            if (_game.networkManager.role == NetRole.Client)
            {
                if (keys == Keys.A)
                {
                    if (_turrentA.isAlive && _turrentA.ammunition > 0 && _game.mouseState.Y < 775)
                    {
                        Console.WriteLine("Client MISSILE");
                        
                        _turrentA.removeAmmo();
                        _game._soundMissileLaunch.Play();
                        Missile clientMissile = new Missile(_turrentA._location, _turrentA._location, new Vector2(_game.mouseState.X, _game.mouseState.Y), speedMultipler, Color.LightBlue, true);
                        _game.networkManager.sendMissileUpdate(clientMissile);
                        //_missiles.Add(new Missile(_turrentA._location, _turrentA._location, new Vector2(_game.mouseState.X, _game.mouseState.Y)));
                    }
                }

                else if (keys == Keys.S)
                {
                    if (_turrentB.isAlive && _turrentB.ammunition > 0 && _game.mouseState.Y < 775)
                    {
                        Console.WriteLine("Client MISSILE");
                        _turrentB.removeAmmo();
                        _game._soundMissileLaunch.Play();
                        Missile clientMissile = new Missile(_turrentB._location, _turrentB._location, new Vector2(_game.mouseState.X, _game.mouseState.Y), speedMultipler, Color.LightBlue, true);
                        _game.networkManager.sendMissileUpdate(clientMissile);
                       // _missiles.Add(new Missile(_turrentB._location, _turrentB._location, new Vector2(_game.mouseState.X, _game.mouseState.Y)));
                    }
                }
            } 

            if (_game.networkManager.role == NetRole.Server)
            {
                if (keys == Keys.A)
                {
                    if (_turrentC.isAlive && _turrentC.ammunition > 0 && _game.mouseState.Y < 775)
                    {
                        _turrentC.removeAmmo();
                        Console.WriteLine("Server MISSILE");
                        _game._soundMissileLaunch.Play();
                        _missiles.Add(new Missile(_turrentC._location, _turrentC._location, new Vector2(_game.mouseState.X, _game.mouseState.Y), speedMultipler, Color.Red,true));
                    }
                }

                else if (keys == Keys.S)
                {
                    if (_turrentD.isAlive && _turrentD.ammunition > 0 && _game.mouseState.Y < 775)
                    {
                        _turrentD.removeAmmo();
                        Console.WriteLine("Server MISSILE");
                        _game._soundMissileLaunch.Play();
                        _missiles.Add(new Missile(_turrentD._location, _turrentD._location, new Vector2(_game.mouseState.X, _game.mouseState.Y), speedMultipler, Color.Red, true));
                    }
                }
            }
        }

        internal void update()
        {
            updateMissile();
            updateExplosions();
            updateBuildings();

        }

        private void updateBuildings()
        {
            //_turrentA.update();
            foreach (Building building in _buildings)
            {
                //building.update();

                if (building.isAlive == false)
                {
                    Console.WriteLine("Building Removed");
                    if (_game.networkManager.role == NetRole.Server)
                    {
                        _buildingsToRemove.Add(building);
                    }
                }
            }
           
            foreach (Building removebuilding in _buildingsToRemove)
            {
                _game._soundBuildingExplosion.Play();
                _game.networkManager.sendsound(2);
                _buildings.Remove(removebuilding);
                
            }
            _buildingsToRemove.Clear();





        }

        private void updateExplosions()
        {
            foreach (Explosion explosion in _explosions)
            {
                explosion.update();

                if (explosion.isAlive == false)
                {
                    Console.WriteLine("Explosion Removed");
                    if (_game.networkManager.role == NetRole.Server)
                    {
                        _explosionsToRemove.Add(explosion);
                    }
                }
            }

            foreach (Explosion removeexplosions in _explosionsToRemove)
            {

                _explosions.Remove(removeexplosions);
            }
            _explosionsToRemove.Clear();
        }

        public void updateMissile()
        {
            foreach (Missile missiles in _missiles)
            {
                missiles.update();
                
                if (missiles.isAlive == false)
                {
                    Console.WriteLine("Missile Removed");
                    if (_game.networkManager.role == NetRole.Server)
                    {
                        _missilesToRemove.Add(missiles);
                        
                    }
                }
            }

            foreach (Missile removemissile in _missilesToRemove)
            {
                _game._soundExplosion.Play();
                _game.networkManager.sendsound(1);
                _explosions.Add(new Explosion(removemissile.position, removemissile.color, true));
                _missiles.Remove(removemissile);
            }
            _missilesToRemove.Clear();
        }

       
    }

}
