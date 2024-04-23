using SFML.Graphics;
using SFML.System;

namespace EditorMain
{
    public interface IDrawable
    {
        public void Draw(RenderWindow window, uint width, uint height, SFML.Graphics.Color color, float phi, float teta, float zc);

        public bool CheckClick(Vector2i position, uint width, uint height, float phi, float teta, float zc);

        public void Click();
    }
}
