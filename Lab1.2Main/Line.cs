using SFML.Graphics;
using SFML.System;
using System.Numerics;

namespace EditorMain
{
    [Serializable]
    public class Line: IDrawable
    {
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float Z1 { get; set; }

        public float X2 { get; set; }
        public float Y2 { get; set; }
        public float Z2 { get; set; }

        public int Thickness { get; set; }

        private RectangleShape lineObject;

        private float x1Screen;
        private float x2Screen;
        private float y1Screen;
        private float y2Screen;

        public bool dots = true;


        public Line()
        {
            X1 = 0;
            Y1 = 0;
            Z1 = 0;
            X2 = 0;
            Y2 = 0;
            Z2 = 0;
            Thickness = 2;
            lineObject = new RectangleShape();
        }

        public Line(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.Z1 = z1;
            this.X2 = x2;
            this.Y2 = y2;
            this.Z2 = z2;
            Thickness = 2;
            lineObject = new RectangleShape();
        }

        public Line(float x1, float y1, float z1, float x2, float y2, float z2, bool dots)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.Z1 = z1;
            this.X2 = x2;
            this.Y2 = y2;
            this.Z2 = z2;
            this.dots = dots;
            Thickness = 2;
            lineObject = new RectangleShape();
        }

        public Line(float x1, float y1, float z1, float x2, float y2, float z2, SFML.Graphics.Color color)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.Z1 = z1;
            this.X2 = x2;
            this.Y2 = y2;
            this.Z2 = z2;
            Thickness = 2;
            lineObject = new RectangleShape();
        }

        public virtual bool CheckClick(Vector2i position, uint width, uint height, float phi, float teta, float zc)
        {
            int mX = position.X;
            int mY = position.Y;
            var rectBounds = lineObject.GetGlobalBounds();
            bool result;

            if (Math.Max(Math.Abs(y2Screen - y1Screen), 1) / Math.Max(Math.Abs(x2Screen - x1Screen), 1) >= 10)
            {
                result = rectBounds.Contains(mX, mY);
            }
            else if (Math.Max(Math.Abs(x2Screen - x1Screen), 1) / Math.Max(Math.Abs(y2Screen - y1Screen), 1) >= 10)
            {
                result = rectBounds.Contains(mX, mY);
            }
            else
            {
                result = (Math.Abs((mX - x1Screen) / (x2Screen - x1Screen) - (mY - y1Screen) / (y2Screen - y1Screen)) <= (float)Thickness / 50) && (rectBounds.Contains(mX, mY));
            }

            return result;
        }

        public void Click()
        {
            return;
        }

        public void Draw(RenderWindow window, uint width, uint height, SFML.Graphics.Color color, float phi, float teta, float zc)
        {
            phi /= UserInterface.RADTODEG;
            teta /= UserInterface.RADTODEG;

            if (phi == 0 || teta == 0)
            {
                x1Screen = X1 + width / 2;
                y1Screen = height / 2 - Y1;

                x2Screen = X2 + width / 2;
                y2Screen = height / 2 - Y2;
            }
            else
            {
                var line1 = new Vector4(MathF.Cos(phi), MathF.Sin(phi) * MathF.Sin(teta), 0, MathF.Sin(phi) * MathF.Cos(teta) / zc);
                var line2 = new Vector4(0, MathF.Cos(teta), 0, -MathF.Sin(teta) / zc);
                var line3 = new Vector4(MathF.Sin(phi), -MathF.Cos(phi) * MathF.Sin(teta), 0, -MathF.Cos(phi) * MathF.Cos(teta) / zc);
                var line4 = new Vector4(0, 0, 0, 1);
                var matrix = new Matrix4x4(line1.X, line1.Y, line1.Z, line1.W, line2.X, line2.Y, line2.Z, line2.W, line3.X, line3.Y, line3.Z, line3.W, line4.X, line4.Y, line4.Z, line4.W);

                var oldCoords1 = new Vector4(X1, Y1, Z1, 1);
                var newCoords1 = Vector4.Transform(oldCoords1, matrix);
                newCoords1.X /= newCoords1.W;
                newCoords1.Y /= newCoords1.W;
                x1Screen = newCoords1.X + width / 2;
                y1Screen = height / 2 - newCoords1.Y;

                var oldCoords2 = new Vector4(X2, Y2, Z2, 1);
                var newCoords2 = Vector4.Transform(oldCoords2, matrix);
                newCoords2.X /= newCoords2.W;
                newCoords2.Y /= newCoords2.W;
                x2Screen = newCoords2.X + width / 2;
                y2Screen = height / 2 - newCoords2.Y;
            }

            float dx = x2Screen - x1Screen;
            float dy = y2Screen - y1Screen;

            lineObject.Position = new Vector2f(x1Screen, y1Screen);
            lineObject.Size = new Vector2f((float)Math.Sqrt(Math.Abs(dx) * Math.Abs(dx) + Math.Abs(dy) * Math.Abs(dy)), Thickness);
            lineObject.Origin = new Vector2f(0, Thickness / 2);
            lineObject.Rotation = (float)(Math.Atan2(dy, dx) * UserInterface.RADTODEG);

            lineObject.FillColor = color;
            lineObject.OutlineColor = color;

            window.Draw(lineObject);

            if (dots)
            {
                var dot1 = new CircleShape();
                var dot2 = new CircleShape();

                dot1.Position = new Vector2f(x1Screen, y1Screen);
                dot2.Position = new Vector2f(x2Screen, y2Screen);

                float dotRadius = 3;
                dot1.Radius = dotRadius;
                dot2.Radius = dotRadius;

                dot1.Origin = new Vector2f(dotRadius, dotRadius);
                dot2.Origin = new Vector2f(dotRadius, dotRadius);

                dot1.FillColor = UserInterface.accentColor;
                dot2.FillColor = UserInterface.accentColor;

                window.Draw(dot1);
                window.Draw(dot2);
            }
        }   
    }
}