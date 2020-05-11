using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;

namespace MissileCommand
{

    public enum NetRole
    {
        Undefined,
        Server,
        Client
    };

    public enum MessageType
    {
        UpdateMissile,
        UpdateMouse,
        UpdateExplosion,
        UpdateBuildings,
        PlaySound,
        UpdateTurrets
    };

    public class NetworkManager
    {
        private Game _game;
        private NetRole _role;
        private NetPeer _peer;

        public NetRole role { get { return _role; } }
        public bool connected
        {
            get
            {
                if (_role == NetRole.Client)
                {
                    return (_peer as NetClient).ConnectionStatus == NetConnectionStatus.Connected;
                }
                else if (_role == NetRole.Server)
                {
                    return (_peer as NetServer).ConnectionsCount == 1;
                }
                else
                {
                    return false;
                }
            }
        }

        public NetworkManager(Game game)
        {
            _game = game;
            _role = NetRole.Undefined;
        }

        public void startClient()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Missile Command");

            _peer = new NetClient(config);
            _peer.Start();
            _role = NetRole.Client;
            Console.WriteLine("Started Client");
        }

        public void startServer()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Missile Command");

            config.Port = 23073;
            _peer = new NetServer(config);
            _peer.Start();
            _role = NetRole.Server;
            Console.WriteLine("Started Server");
        }

        public void connectTo(string ip)
        {
            _peer.Connect(ip, 23073);
        }

        private void processIncomingMessages()
        {
            if (_peer != null)
            {
                NetIncomingMessage im;

                while ((im = _peer.ReadMessage()) != null)
                {
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                            if (status == NetConnectionStatus.Connected)
                            {
                                if (_role == NetRole.Client)
                                {
                                    Console.WriteLine("Connected to Server.");
                                }
                                else
                                {
                                    Console.WriteLine("Connected to Client.");
                                }
                            }
                            else
                            {
                               // _game.addMessage(status.ToString() + ": " + im.ReadString());
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            MessageType messageType = (MessageType)im.ReadInt32();

                            if (messageType == MessageType.UpdateMouse)
                            {
                                receiveMouseUpdate(im);
                            }
                            else if (messageType == MessageType.UpdateMissile)
                            {
                               receiveMissiles(im);
                            }
                            else if (messageType == MessageType.UpdateExplosion)
                            {
                                receiveExplosionUpdate(im);
                            }
                            else if (messageType == MessageType.UpdateBuildings)
                            {
                                receiveBuildingUpdate(im);
                            }
                            else if (messageType == MessageType.PlaySound)
                            {
                                receiveSoundUpdate(im);
                            }
                            else if (messageType == MessageType.UpdateTurrets)
                            {
                                updateTurrets(im);
                            }

                            break;
                        default:
                            Console.WriteLine("Unhandled message type: " + im.MessageType);
                            break;
                    }
                    _peer.Recycle(im);
                }
            }
        }


        private void updateTurrets(NetIncomingMessage im)
        {
            if (_game.state == GameState.Ready)
            {
                if (_role == NetRole.Client)
                {
                    int turretthatdied = im.ReadInt32();
                    if (turretthatdied == 1)
                    {
                        _game.entityManager.turretA.kill();
                    }
                    if (turretthatdied == 2)
                    {
                        _game.entityManager.turretB.kill();
                    }
                    if (turretthatdied == 3)
                    {
                        _game.entityManager.turretC.kill();
                    }
                    if (turretthatdied == 4)
                    {
                        _game.entityManager.turretD.kill();
                    }
                    if (turretthatdied == 5)
                    {
                        _game.resetgame();
                    }//create turrets!!!
                    if (turretthatdied == 6)
                    {
                        _game.entityManager.turretA.reset();
                    }
                    if (turretthatdied == 7)
                    {
                        _game.entityManager.turretB.reset();
                    }
                    if (turretthatdied == 8)
                    {
                        _game.entityManager.turretC.reset();
                    }
                    if (turretthatdied == 9)
                    {
                        _game.entityManager.turretD.reset();
                    }

                }
            }

        }
        internal void sendTurretToKill(int v)
        {
            if (_game.state == GameState.Ready)
            {
                if (_role == NetRole.Server)
                {
                    NetOutgoingMessage om = _peer.CreateMessage();
                    om.Write((int)MessageType.UpdateTurrets);
                    om.Write(v);
                    _peer.SendMessage(om, _peer.Connections[0], NetDeliveryMethod.Unreliable);
                }
            }
        }



        private void receiveSoundUpdate(NetIncomingMessage im)
        {
            if (_game.state == GameState.Ready)
            {
                if (_role == NetRole.Client)
                {
                    int sound = im.ReadInt32();
                    if(sound == 1)
                    {
                        _game._soundExplosion.Play();
                    }
                    if (sound == 2)
                    {
                        _game._soundBuildingExplosion.Play();
                    }

                    
                }
            }

        }
        internal void sendsound(int v)
        {
            if (_game.state == GameState.Ready)
            {
                if (_role == NetRole.Server)
                {
                    NetOutgoingMessage om = _peer.CreateMessage();
                    om.Write((int)MessageType.PlaySound);
                    om.Write(v);
                    _peer.SendMessage(om, _peer.Connections[0], NetDeliveryMethod.Unreliable);
                }
            }
        }

        private void receiveBuildingUpdate(NetIncomingMessage im)
        {
            if (_game.state == GameState.Ready)
            {
                int entityCount = im.ReadInt32();
                if (_role == NetRole.Client)
                {
                    _game.entityManager._buildings.Clear();

                    for (int i = 0; i < entityCount; i++)
                    { 
                        float originX = im.ReadFloat();
                        float originY = im.ReadFloat();
                        int ammunition = im.ReadInt32();
                        bool alive = im.ReadBoolean();
                        bool canshoot = im.ReadBoolean();
                        Building newbuilding = new Building(new Vector2(originX, originY), alive, ammunition, canshoot);
                        _game.entityManager._buildings.Add(newbuilding);

                    }
                }
            }
        }

        private void sendbuildingsList()
        {
            if (_game.state == GameState.Ready)
            {
                if (_role == NetRole.Server)
                {
                    NetOutgoingMessage om = _peer.CreateMessage();
                    om.Write((int)MessageType.UpdateBuildings);
                    om.Write(_game.entityManager._buildingsCount);

                    foreach (Building buildingtosend in _game.entityManager._buildings)
                    {
                        om.Write(buildingtosend.origin.X);
                        om.Write(buildingtosend.origin.Y);
                        om.Write(buildingtosend.ammunition);
                        om.Write(buildingtosend.isAlive);
                        om.Write(buildingtosend.canShoot);
                    }
                    _peer.SendMessage(om, _peer.Connections[0], NetDeliveryMethod.Unreliable);
                }
            }
        }


        private void receiveExplosionUpdate(NetIncomingMessage im)
        {
            if (_game.state == GameState.Ready)
            {
                int entityCount = im.ReadInt32();
                if (_role == NetRole.Client)
                {
                    _game.entityManager._explosions.Clear();

                    for (int i = 0; i < entityCount; i++)
                    {

                        int id = im.ReadInt32();
                        float originX = im.ReadFloat();
                        float originY = im.ReadFloat();
                        Color color = new Color(im.ReadUInt32());
                        bool alive = im.ReadBoolean();
                        float radius = im.ReadFloat();
                        Explosion newexplosion = new Explosion(new Vector2(originX, originY), color, alive, id,radius);
                        _game.entityManager._explosions.Add(newexplosion);



                    }
                }
            }

        }
        private void sendExplosionList()
        {
            if (_game.state == GameState.Ready)
            {
                if (_role == NetRole.Server)
                {
                    NetOutgoingMessage om = _peer.CreateMessage();

                    om.Write((int)MessageType.UpdateExplosion);
                    om.Write(_game.entityManager._explosionsCount);

                    foreach (Explosion explosiontosend in _game.entityManager._explosions)
                    {
                        om.Write(explosiontosend.id);
                        om.Write(explosiontosend.origin.X);
                        om.Write(explosiontosend.origin.Y);
                        om.Write(explosiontosend.colorint);
                        om.Write(explosiontosend.isAlive);
                        om.Write(explosiontosend.radius);
                    }
                    _peer.SendMessage(om, _peer.Connections[0], NetDeliveryMethod.Unreliable);
                }
            }

        }

   

        public void update()
        {
            sendinformation();
            processIncomingMessages();

        }

        public void sendinformation()
        {
            sendMouseUpdate();
            sendMissileList();
            sendExplosionList();
            sendbuildingsList();
        }

       

        private void sendMissileList()
        {
            if (_game.state == GameState.Ready)
            {
                if (_role == NetRole.Server)
                {
                    NetOutgoingMessage om = _peer.CreateMessage();

                    om.Write((int)MessageType.UpdateMissile);
                    om.Write(_game.entityManager._missileCount);

                    foreach (Missile missilestosend in _game.entityManager._missiles)
                    {
                        om.Write(missilestosend.id);
                        om.Write(missilestosend.origin.X);
                        om.Write(missilestosend.origin.Y);
                        om.Write(missilestosend.position.X);
                        om.Write(missilestosend.position.Y);
                        om.Write(missilestosend.destination.X);
                        om.Write(missilestosend.destination.Y);
                        om.Write(missilestosend.speed);
                        om.Write(missilestosend.colorint);
                        om.Write(missilestosend.isAlive);

                    }
                    _peer.SendMessage(om, _peer.Connections[0], NetDeliveryMethod.Unreliable);
                }
            }

        }
        private void receiveMissiles(NetIncomingMessage im)
        {
            if (_game.state == GameState.Ready)
            {
                int entityCount = im.ReadInt32();
                if (_role == NetRole.Client)
                {
                    _game.entityManager._missiles.Clear();
                }

                for (int i = 0; i < entityCount; i++)
                {

                    int id = im.ReadInt32();
                    float originX = im.ReadFloat();
                    float originY = im.ReadFloat();
                    float positionX = im.ReadFloat();
                    float positionY = im.ReadFloat();
                    float destinationX = im.ReadFloat();
                    float destinationY = im.ReadFloat();
                    float speed = im.ReadFloat();
                    Color color = new Color(im.ReadUInt32());
                    bool alive = im.ReadBoolean();

                    Missile newmissile = new Missile(new Vector2(originX, originY), new Vector2(positionX, positionY), new Vector2(destinationX, destinationY), speed, color, alive, id);
                    _game.entityManager._missiles.Add(newmissile);



                }
            }
        }
        public void sendMissileUpdate(Missile clientMissile)
        {
            if (_game.state == GameState.Ready)
            {

                NetOutgoingMessage om = _peer.CreateMessage();

                om.Write((int)MessageType.UpdateMissile);
                om.Write(1); // indicates number of missiles being sent


                om.Write(clientMissile.id);
                om.Write(clientMissile.origin.X);
                om.Write(clientMissile.origin.Y);
                om.Write(clientMissile.position.X);
                om.Write(clientMissile.position.Y);
                om.Write(clientMissile.destination.X);
                om.Write(clientMissile.destination.Y);
                om.Write(clientMissile.speed);
                om.Write(clientMissile.colorint);
                om.Write(clientMissile.isAlive);


                _peer.SendMessage(om, _peer.Connections[0], NetDeliveryMethod.Unreliable);

            }
        }

        public void sendMouseUpdate()
        {
            if (_game.state == GameState.Ready)
            {
                NetOutgoingMessage om = _peer.CreateMessage();
                om.Write((int)MessageType.UpdateMouse);
                om.Write(_game.mouseState.X);
                om.Write(_game.mouseState.Y);
                if (_role == NetRole.Client)
                {
                    om.Write(_game.entityManager.turretA.ammunition);
                    om.Write(_game.entityManager.turretB.ammunition);
                }
                if (_role == NetRole.Server)
                {
                    om.Write(_game.entityManager.turretC.ammunition);
                    om.Write(_game.entityManager.turretD.ammunition);
                }
                _peer.SendMessage(om, _peer.Connections[0], NetDeliveryMethod.Unreliable);
                
            }
        }
        public void receiveMouseUpdate(NetIncomingMessage im)
        {
            if (_game.state == GameState.Ready)
            {
                int x, y;
                x = im.ReadInt32();
                y = im.ReadInt32();
                _game.entityManager.updateMultiplayerMouse(x, y);
                if (_role == NetRole.Client)
                {
                    int t1, t2;
                    t1 = im.ReadInt32();
                    t2 = im.ReadInt32();
                    _game.entityManager.turretC.getammo(t1);
                    _game.entityManager.turretD.getammo(t2);
                }
                if (_role == NetRole.Server)
                {
                    int t1, t2;
                    t1 = im.ReadInt32();
                    t2 = im.ReadInt32();
                    _game.entityManager.turretA.getammo(t1);
                    _game.entityManager.turretB.getammo(t2);
                }
            }
        }
    }
}
