
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MonoGame;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MissileCommand
{

    public enum GameState
    {
        
        Menu,
        Waiting,
        Ready
    }

    public class Game : Microsoft.Xna.Framework.Game
    {
        private NetworkManager _networkManager;
        private EntityManager _entityManager;
        private EnemyManager _enemyManager;
        private PhysicsManager _physicsManager;
        private CameraManager _cameraManager;
        private SpriteFont _font;
        private Texture2D _background;
        private Texture2D _building;
        private Texture2D _silo;
        private KeyboardState _keyState;
        private KeyboardState _oldKeyState;
        private MouseState _mouseState;
        private MouseState _oldMouseState;
        private string _ipAddressValue;
        public SoundEffect _soundMissileLaunch;
        public SoundEffect _soundExplosion;
        public SoundEffect _soundBuildingExplosion;
        private Song _song;

        private GameState _state;
        private int i = 0;

        public PhysicsManager physicsManager { get { return _physicsManager; } }
        public EntityManager entityManager { get { return _entityManager; } }
        public NetworkManager networkManager { get { return _networkManager; } }
        public EnemyManager enemyManager { get { return _enemyManager; } }
        public GameState state { get { return _state; } }
        public KeyboardState keyState { get { return _keyState; } }
        public KeyboardState oldkeyState { get { return _oldKeyState; } }
        public MouseState mouseState { get { return _mouseState; } }
        public MouseState oldmouseState { get { return _oldMouseState; } }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            this.Window.Title = "Missile Command";

            graphics.PreferredBackBufferWidth = 1200;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 800;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            _state = GameState.Menu;
           
            Content.RootDirectory = "Content";

            _networkManager = new NetworkManager(this);
            _ipAddressValue = "127.0.0.1";
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("text");

            _soundMissileLaunch = Content.Load<SoundEffect>("missileshoot");
            _soundExplosion = Content.Load<SoundEffect>("Explosion");
            _soundBuildingExplosion = Content.Load<SoundEffect>("buildingexplosion");
            _song = Content.Load<Song>("music");



            _background = Content.Load<Texture2D>("background5");
            _building = Content.Load<Texture2D>("building");
            _silo = Content.Load<Texture2D>("militarybuilding");
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void startGame()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .4f;
            MediaPlayer.Play(_song);
            

            Console.WriteLine("Game Start");
            _state = GameState.Ready;
            _entityManager = new EntityManager(this);
            _cameraManager = new CameraManager(this);
            _physicsManager = new PhysicsManager(this);
            _enemyManager = new EnemyManager(this);



        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _oldMouseState = _mouseState;
            _mouseState = MouseState.get();
            _oldKeyState = _keyState;
            _keyState = KeyboardState.get();
            
            if (_state == GameState.Menu)
            {
                updateMenu(gameTime);
            }
            else if (_state == GameState.Waiting)
            {
                updateWaiting(gameTime);
            }
            else if (_state == GameState.Ready)
            {
                updateReady(gameTime);
            }
                base.Update(gameTime);
        }

        private void updateReady(GameTime gameTime)
        {
            processInput();
            _physicsManager.update();
            _networkManager.update();
            _entityManager.update();
            
            if (_networkManager.role == NetRole.Server)
            {
                _enemyManager.update(gameTime);
            }
        }

        

        private void updateWaiting(GameTime gameTime)
        {
            if (_networkManager.role == NetRole.Client)
            {
                if (!_networkManager.connected)
                {

                 // if(_keyState.KeysDown()== Keys.OemPeriod)
                //   {
                   //     Console.WriteLine(".");
                  //  }

                    for (int i = 0; i < 10; i++)
                    {
                        Keys key = (Keys)(i + 48);
                        if (_keyState.isPressed(key) && _oldKeyState.isReleased(key))
                        {
                            _ipAddressValue += i.ToString();
                        }
                    }

                    // .
                    if (_keyState.isPressed(Keys.OemPeriod) && _oldKeyState.isReleased(Keys.OemPeriod))
                    {
                       _ipAddressValue += ".";
                       // _ipAddressValue.Insert(_ipAddressValue.Length - 1, ".");
                       // i++;
                       // Keys key = (Keys)(i);
                       // Console.WriteLine(i + ": " + key);
                        

                    }

                    // Backspace
                    if (_keyState.isPressed(Keys.Back) && _oldKeyState.isReleased(Keys.Back))
                    {
                        if (_ipAddressValue.Length > 0)
                        {
                            _ipAddressValue = _ipAddressValue.Substring(0, _ipAddressValue.Length - 1);
                        }
                    }

                    // Enter
                    if (_keyState.isPressed(Keys.Enter) && _oldKeyState.isReleased(Keys.Enter))
                    {
                        
                        _networkManager.connectTo(_ipAddressValue);
                        Console.WriteLine("Attempting to connect to " + _ipAddressValue + ":3456...");
                    }

                   
                   
                }
                else
                {
                    startGame();
                }
            }
            else if (_networkManager.role == NetRole.Server)
            {
                if (_networkManager.connected)
                {
                    
                    startGame();
                    //_entityManager.createBuildings();
                }
            }
            _networkManager.update();
        }

        private void updateMenu(GameTime gameTime)
        {
            if (_keyState.isPressed(Keys.C))
            {
                Console.WriteLine("start client.");
                _networkManager.startClient();
                _state = GameState.Waiting;
            }
            if (_keyState.isPressed(Keys.S))
            {
                Console.WriteLine("start server.");
                _networkManager.startServer();
                _state = GameState.Waiting;
            }
        }

        private void processInput()
        {
           
            if(_keyState.isPressed(Keys.A) && _oldKeyState.isReleased(Keys.A))
            {
                _entityManager.shootmissile(Keys.A);
            }
            if (_keyState.isPressed(Keys.S) && _oldKeyState.isReleased(Keys.S))
            {
                _entityManager.shootmissile(Keys.S);
            }
            if (_keyState.isPressed(Keys.P) && _oldKeyState.isReleased(Keys.P))
            {

                if (networkManager.role == NetRole.Server)
                {
                    resetgame();
                    _networkManager.sendTurretToKill(5);

                }
            }

        }

        public void resetgame() {
            
            _entityManager.reset();
             _enemyManager.reset();


        }

        protected override void Draw(GameTime gameTime)
        {
           
            if (_state == GameState.Menu)
            {

               drawMenu(gameTime);
          
            }
            else if (_state == GameState.Waiting)
            {
                drawWaiting(gameTime);

            }
            else if (_state == GameState.Ready)
            {
                drawReady(gameTime);

            }

            base.Draw(gameTime);
        }

        private void drawReady(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(_font, "Game Ready" + _networkManager.role, new Vector2(100, 100), Color.White);
           
            spriteBatch.Draw(_background, new Vector2(0, 0), Color.Gray);


            Primitives2D.FillRectangle(spriteBatch, new Vector2(0, 750), new Vector2(1200, 50), Color.SaddleBrown);
           
            spriteBatch.DrawString(_font, "Entity Count" + entityManager._missileCount +" net role: "+ _networkManager.role, new Vector2(100, 200), Color.White);
            //  spriteBatch.Draw(_crosshair, new Vector2(_mouseState.X - _crosshair.Width / 2, _mouseState.Y - _crosshair.Height / 2), Color.White);
            drawTurrets();
            drawCrosshair();
           // spriteBatch.Draw(_crosshair, new Vector2(entityManager.networkmouse.X - _crosshair.Width / 2, entityManager.networkmouse.Y - _crosshair.Height / 2), Color.Red);
            foreach (Missile missile in entityManager._missiles)
            {
                //Console.WriteLine("Missile ID: "+missile.id+ " Missile Origin: "+ missile.origin);
              
                Primitives2D.DrawLine(spriteBatch, missile.origin, missile.position+missile.direction*4, Color.White, 3f);
                Primitives2D.DrawLine(spriteBatch, missile.origin, missile.position + missile.direction * 2 , Color.Black, 3f);
                Primitives2D.DrawLine(spriteBatch, missile.origin, missile.position, missile.color, 3f);

            }
            foreach (Explosion explosion in entityManager._explosions)
            {
                //Console.WriteLine("Missile ID: "+missile.id+ " Missile Origin: "+ missile.origin);
                Primitives2D.DrawCircle(spriteBatch, explosion.origin, explosion.radius, 30, explosion.color,3f);

            }
            foreach (Building building in entityManager._buildings)
            {
                
                spriteBatch.Draw(_building, new Vector2(building.origin.X, building.origin.Y-_building.Height), Color.White);

            }
            
            //  Console.WriteLine(_mouseState.postion());
            spriteBatch.End();
        }


        private void drawTurrets()
        {
            if (entityManager.turretA.isAlive) { 
                spriteBatch.Draw(_silo, new Vector2(entityManager.turretA.origin.X - _silo.Width / 2, entityManager.turretA.origin.Y - _silo.Height), Color.White);
                spriteBatch.DrawString(_font, ""+entityManager.turretA.ammunition, new Vector2(entityManager.turretA.origin.X, entityManager.turretA.origin.Y), Color.White);
            }
            if (entityManager.turretB.isAlive)
            {
                spriteBatch.Draw(_silo, new Vector2(entityManager.turretB.origin.X - _silo.Width / 2, entityManager.turretB.origin.Y - _silo.Height), Color.White);
                spriteBatch.DrawString(_font, "" + entityManager.turretB.ammunition, new Vector2(entityManager.turretB.origin.X, entityManager.turretB.origin.Y), Color.White);
            }
            if (entityManager.turretC.isAlive)
            {
                spriteBatch.Draw(_silo, new Vector2(entityManager.turretC.origin.X - _silo.Width / 2, entityManager.turretC.origin.Y - _silo.Height), Color.White);
                spriteBatch.DrawString(_font, "" + entityManager.turretC.ammunition, new Vector2(entityManager.turretC.origin.X, entityManager.turretC.origin.Y), Color.White);
            }
            if (entityManager.turretD.isAlive)
            {
                spriteBatch.Draw(_silo, new Vector2(entityManager.turretD.origin.X - _silo.Width / 2, entityManager.turretD.origin.Y - _silo.Height), Color.White);
                spriteBatch.DrawString(_font, "" + entityManager.turretD.ammunition, new Vector2(entityManager.turretD.origin.X , entityManager.turretD.origin.Y), Color.White);
            }



        }

        private void drawCrosshair()
        {
            Primitives2D.DrawCircle(spriteBatch, entityManager.networkmouse, 25, 24, Color.White, 2f);
            Primitives2D.DrawCircle(spriteBatch, entityManager.networkmouse, 15, 24, Color.White, 2f);
            Primitives2D.DrawLine(spriteBatch, new Vector2(entityManager.networkmouse.X - 25, entityManager.networkmouse.Y), new Vector2(entityManager.networkmouse.X + 25, entityManager.networkmouse.Y), Color.White, 1.5f);
            Primitives2D.DrawLine(spriteBatch, new Vector2(entityManager.networkmouse.X + 1, entityManager.networkmouse.Y - 25), new Vector2(entityManager.networkmouse.X + 1, entityManager.networkmouse.Y + 25), Color.White, 1.5f);

            Primitives2D.DrawCircle(spriteBatch, _mouseState.postion(), 25, 24, Color.White,2f);
            Primitives2D.DrawCircle(spriteBatch, _mouseState.postion(), 15, 24, Color.White,2f);
            Primitives2D.DrawLine(spriteBatch,new Vector2( _mouseState.postion().X-25,_mouseState.postion().Y), new Vector2(_mouseState.postion().X + 25, _mouseState.postion().Y), Color.White,1.5f);
            Primitives2D.DrawLine(spriteBatch, new Vector2(_mouseState.postion().X+1, _mouseState.postion().Y-25), new Vector2(_mouseState.postion().X+1, _mouseState.postion().Y+25), Color.White,1.5f);
        }

        private void drawWaiting(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if(_networkManager.role == NetRole.Client)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(_font, "Connect to server IP:" +_ipAddressValue,new Vector2(100, 100), Color.White);
                //drawCrosshair();
                spriteBatch.End();
            }
            if (_networkManager.role == NetRole.Server)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(_font, "Waiting for client", new Vector2(100, 100), Color.White);
               // drawCrosshair();
                spriteBatch.End();
            }

        }

        private void drawMenu(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(_font, "Press C for Client OR S for Server Network", new Vector2(100, 100), Color.White);
  
            spriteBatch.End();
        }
    }
}
