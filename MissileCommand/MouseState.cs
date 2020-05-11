using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MissileCommand
{
    public class MouseState{

        private bool _leftButtonPressed;
        private bool _rightButtonPressed;

        public bool isLeftButtonPressed { get { return _leftButtonPressed; } }
        public bool isRightButtonPressed { get { return _rightButtonPressed; } }

        public int X { get; set; }
        public int Y { get; set; }

        public MouseState(bool leftButtonPressed, bool rightButtonPressed)
        {
            X = Mouse.GetState().X;
            Y = Mouse.GetState().Y;
            _leftButtonPressed = leftButtonPressed;
            _rightButtonPressed = rightButtonPressed;
        }

        public static MouseState get()
        {
            return new MouseState(Mouse.GetState().LeftButton == ButtonState.Pressed, Mouse.GetState().RightButton == ButtonState.Pressed);

        }

        public Vector2 postion()
        {
            return new Vector2(X, Y);
        }
    }
}
