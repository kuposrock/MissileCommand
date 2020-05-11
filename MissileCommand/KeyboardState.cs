using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MissileCommand
{
    public class KeyboardState
    {
        private bool[] _keysPressed;


        public KeyboardState(bool[] keysPressed)
        {
            _keysPressed = keysPressed;
        }

        public bool isPressed(Keys key)
        {
            int iKey = (int)key;

            if (iKey < 0 || iKey > 500)
            {
                return false;
            }

            return _keysPressed[iKey];
        }

        public bool isReleased(Keys key)
        {
            int iKey = (int)key;

            if (iKey < 0 || iKey > 500)
            {
                return true;
            }

            return !_keysPressed[iKey];
        }

        public static KeyboardState get()
        {
            bool[] keysPressed = new bool[501];
            int[] values = Enum.GetValues(typeof(Keys)) as int[];

            for (int i = -1; i < 500; i++)
            {
                int j = i + 1;
                keysPressed[j] = Keyboard.GetState().IsKeyDown((Keys)j);
            }

            return new KeyboardState(keysPressed);


        }
        /*
        public Keys? KeysDown()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key != Keys.None && Keyboard.GetState().IsKeyDown(key))
                {
                    return key;
                    
                }
            }
            return null;   
        }
        */
    }
}

