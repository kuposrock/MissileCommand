using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MissileCommand
{
    public class PhysicsManager
    {
        private Game _game;

        public PhysicsManager(Game game)
        {
            _game = game;
        }

        public void update()
        {
            if (_game.networkManager.role == NetRole.Server)
            {
                foreach (Missile missiletocheck in _game.entityManager._missiles)
                {
                    foreach (Explosion explosiontocheck in _game.entityManager._explosions)
                    {
                        if (explosiontocheck.radius >= Vector2.Distance(missiletocheck.position, explosiontocheck.origin))
                        {
                            missiletocheck.kill();
                        }
                    }
                    foreach (Building buildingtocheck in _game.entityManager._buildings)
                    {
                        if (missiletocheck.position.X >= buildingtocheck.origin.X && missiletocheck.position.X <= buildingtocheck.origin.X + 38 && missiletocheck.position.Y >= 774)
                        {

                            buildingtocheck.kill();
                        }
                    }

                    if (missiletocheck.position.X >= _game.entityManager.turretA.origin.X - 30 && missiletocheck.position.X <= _game.entityManager.turretA.origin.X + 30 && missiletocheck.position.Y > 775)
                    {
                        Console.WriteLine("THIS IS HOW I DIED3");
                        _game.entityManager.turretA.kill();
                        _game.networkManager.sendTurretToKill(1);
                    }
                    if (missiletocheck.position.X >= _game.entityManager.turretB.origin.X - 30 && missiletocheck.position.X <= _game.entityManager.turretB.origin.X + 30 && missiletocheck.position.Y > 775)
                    {
                        Console.WriteLine("THIS IS HOW I DIED4");
                        _game.entityManager.turretB.kill();
                        _game.networkManager.sendTurretToKill(2);
                    }
                    if (missiletocheck.position.X >= _game.entityManager.turretC.origin.X - 30 && missiletocheck.position.X <= _game.entityManager.turretC.origin.X + 30 && missiletocheck.position.Y > 775)
                    {
                        Console.WriteLine("THIS IS HOW I DIED");

                        _game.entityManager.turretC.kill();
                        _game.networkManager.sendTurretToKill(3);
                    }
                    if (missiletocheck.position.X >= _game.entityManager.turretD.origin.X-30 && missiletocheck.position.X <= _game.entityManager.turretD.origin.X + 30 && missiletocheck.position.Y > 775)
                    {
                        Console.WriteLine("THIS IS HOW I DIED x2");
                        _game.entityManager.turretD.kill();
                        _game.networkManager.sendTurretToKill(4);
                    }
                }
            }
        }
    }
}
