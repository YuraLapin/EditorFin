using SFML.Graphics;
using SFML.System;
using System.ComponentModel.Design;
using System.Numerics;

namespace EditorMain
{
    public class Button: IDrawable
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public string? Txt;
        public SFML.Graphics.Font Font = new SFML.Graphics.Font(UserInterface.FONT);
        public ClickHandler ClickFunction;
        private int active = 0;
        private const int FRAMES_FOR_ACTIVE = 200;

        public Button()
        {
            X = 100;
            Y = 50;
            Width = 100;
            Height = 50;
            Txt = "???";
            ClickFunction = () => { };
        }

        public Button(float x, float y, float width, float height, string txt, ClickHandler clickFunction)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Txt = txt;
            this.ClickFunction = clickFunction;
        }        

        public virtual bool CheckClick(Vector2i position, uint width, uint height, float phi, float teta, float zc)
        {
            int mX = position.X;
            int mY = position.Y;
            var frame = new RectangleShape();
            frame.Size = new Vector2f(Width, Height);
            frame.OutlineThickness = 2;
            frame.Position = new Vector2f(X, Y);
            var rectBounds = frame.GetGlobalBounds();
            return rectBounds.Contains(mX, mY);
        }

        public void Click()
        {
            active = FRAMES_FOR_ACTIVE;
            ClickFunction();
        }

        public void Draw(RenderWindow window, uint windowWidth, uint windowHeight, SFML.Graphics.Color color, float phi, float teta, float zc)
        {
            var frame = new RectangleShape();
            frame.FillColor = SFML.Graphics.Color.Black;
            frame.Size = new Vector2f(Width, Height);

            if (active > 0)
            {
                frame.OutlineColor = UserInterface.activeColor;
                --active;
            }
            else
            {
                frame.OutlineColor = color;
            }

            frame.OutlineThickness = 1;
            frame.Position = new Vector2f(X, Y);

            var text = new Text(Txt, Font);
            text.FillColor = color;
            text.Scale = new Vector2f(0.6f, 0.6f);
            text.Origin = new Vector2f(text.GetLocalBounds().Width / 2, text.GetLocalBounds().Height);
            text.Position = new Vector2f(X + Width / 2, Y + Height / 2);

            window.Draw(frame);
            window.Draw(text);
        }        
    }
}