using SFML.Graphics;
using SFML.System;

namespace EditorMain
{
    internal class Field: IDrawable
    {
        public SFML.Graphics.Color insideColor = SFML.Graphics.Color.Black;
        public SFML.Graphics.Color outlineColor = SFML.Graphics.Color.White;

        public float x = 0;
        public float y = 0;

        public float width;
        public float height;

        public Field(float x, float y, float width, float height, SFML.Graphics.Color insideColor, SFML.Graphics.Color outlineColor)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.insideColor = insideColor;
            this.outlineColor = outlineColor;
        }

        public virtual bool CheckClick(Vector2i position, uint width, uint height, float phi, float teta, float zc)
        {
            return false;
        }

        public void Click()
        {

        }

        public void Draw(RenderWindow window, uint windowWidth, uint windowHeight, SFML.Graphics.Color color, float phi, float teta, float zc)
        {
            var rect = new RectangleShape();
            rect.Position = new Vector2f(x, y);
            rect.Size = new Vector2f(this.width, this.height);
            rect.FillColor = insideColor;
            rect.OutlineThickness = 3;
            rect.OutlineColor = outlineColor;
            window.Draw(rect);
        }
    }
}
