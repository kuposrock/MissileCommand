using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MonoGame;
using System.Collections.Generic;

namespace MissileCommand
{
    public class EnemyManager
    {
        private Game _game;

        private int _gametime = 0;
        private Random _randomnumber;
        private int _multiplier = 1;
        private int _maximumMissileToFireForaWave = 2;
        private int _totalMissileForalevel = 10;


        private int _timePerLevel = 0;
        private List<Vector2> locationsToTarget = new List<Vector2>();


        public EnemyManager(Game game)
        {

            _game = game;
            _randomnumber = new Random();
        }

        internal void update(GameTime gameTime)
        {
            locationsToTarget.Clear();
            locationsToTarget.Add(new Vector2(_game.entityManager.turretA.origin.X, _game.entityManager.turretA.origin.Y));
            locationsToTarget.Add(new Vector2(_game.entityManager.turretB.origin.X, _game.entityManager.turretB.origin.Y));
            locationsToTarget.Add(new Vector2(_game.entityManager.turretC.origin.X, _game.entityManager.turretC.origin.Y));
            locationsToTarget.Add(new Vector2(_game.entityManager.turretD.origin.X, _game.entityManager.turretD.origin.Y));
            foreach (Building buildlocation in _game.entityManager._buildings)
            {
                locationsToTarget.Add(new Vector2(buildlocation.origin.X + 20, buildlocation.origin.Y));
            }

            _randomnumber.Next();
            if (_gametime != gameTime.TotalGameTime.Seconds)
            {
                _gametime = gameTime.TotalGameTime.Seconds;
                if (_timePerLevel <= 30)
                {

                    int i = _randomnumber.Next(30 - _timePerLevel);
                    if (i <= _totalMissileForalevel && _totalMissileForalevel>0)
                    {
                       // Console.WriteLine("Shoot missile wave");
                       // i = _randomnumber.Next(2);
                        
                        for (int ammounttofire = 0; ammounttofire <= _randomnumber.Next(_maximumMissileToFireForaWave); ammounttofire++)
                        {
                           // Console.WriteLine("Firing Missile#: "+ammounttofire +"   Missiles left"+i);
                            if (_totalMissileForalevel > 0)
                            { 
                                _totalMissileForalevel--;
                               // Console.WriteLine("Missiles left to fire in wave: "+_totalMissileForalevel);
                                int locationX = _randomnumber.Next(1200);
                                i = _randomnumber.Next(locationsToTarget.Count);
                                _game.entityManager._missiles.Add(new Missile(new Vector2(locationX, 0), new Vector2(locationX, 0), new Vector2(locationsToTarget[i].X, locationsToTarget[i].Y + 3), _multiplier, Color.GreenYellow, true));
                            }
                        }
                    }
                    _timePerLevel++;
                }
                else
                {
                    _maximumMissileToFireForaWave += _multiplier;
                    _totalMissileForalevel += _multiplier;
                    _multiplier++;

                    _totalMissileForalevel = 10 + _multiplier;
                    _timePerLevel = 0;
                    //add game multiplier and check if still alive
                    Console.WriteLine("Multiply Level");
                }
            }

            

            
           /* if (_gametime != _gametimex)
            {
                _game.entityManager._missiles.Add(new Missile(new Vector2(500, 0), new Vector2(500, 0), new Vector2(200, 776), 1f, Color.GreenYellow, true));
                _game.entityManager._missiles.Add(new Missile(new Vector2(500, 0), new Vector2(500, 0), new Vector2(500, 776), 1f, Color.GreenYellow, true));
                _game.entityManager._missiles.Add(new Missile(new Vector2(500, 0), new Vector2(500, 0), new Vector2(790, 776), 1f, Color.GreenYellow, true));
                _game.entityManager._missiles.Add(new Missile(new Vector2(500, 0), new Vector2(500, 0), new Vector2(1070, 776), 1f, Color.GreenYellow, true));
                _gametime = 2;
            }*/

           
                
            


        }

        internal void reset()
        {
            
        }
    }
}
