using SFML.System;
using SFML.Window;

namespace EditorMain
{
    internal class MousePressHandler
    {
        public List<Line> userObjects;
        public List<int> selectedObjects;
        public Dictionary<string, IDrawable> ui;
        public uint width;
        public uint height;

        public MousePressHandler(List<Line> userObjects, List<int> selectedObjects, Dictionary<string, IDrawable> ui)
        {
            this.userObjects = userObjects;
            this.selectedObjects = selectedObjects;
            this.ui = ui;
        }

        public void ControlLMBPressed(Vector2i position, float phi, float teta, float zc)
        {
            var currentIndex = userObjects.Count - 1;

            foreach (KeyValuePair<string, IDrawable> element in ui)
            {
                if (element.Value is TextField tf)
                {
                    tf.active = false;
                }
            }

            foreach (IDrawable element in userObjects.FastReverse())
            {
                if (element.CheckClick(position, width, height, phi, teta, zc))
                {
                    if (selectedObjects.Contains(currentIndex))
                    {
                        selectedObjects.Remove(currentIndex);
                        return;
                    }

                    selectedObjects.Add(currentIndex);
                    element.Click();
                    return;
                }

                --currentIndex;
            }
        }

        public void LMBPressed(Vector2i position, float phi, float teta, float zc)
        {
            foreach (KeyValuePair<string, IDrawable> element in ui)
            {
                if (element.Value is TextField tf)
                {
                    tf.active = false;
                }
            }

            foreach (KeyValuePair<string, IDrawable> element in ui)
            {
                if (!(element.Value is Line) && (element.Value.CheckClick(position, width, height, phi, teta, zc)))
                {
                    element.Value.Click();
                    return;
                }
            }

            var currentIndex = userObjects.Count - 1;

            foreach (IDrawable element in userObjects.FastReverse())
            {
                if (element.CheckClick(position, width, height, phi, teta, zc))
                {
                    selectedObjects.Clear();
                    selectedObjects.Add(currentIndex);
                    element.Click();
                    return;
                }

                --currentIndex;
            }

            selectedObjects.Clear();
        }

        public void RMBPressed(SFML.Graphics.RenderWindow window, ref float startX, ref float startY, ref bool rmbPressed)
        {
            rmbPressed = true;
            var position = Mouse.GetPosition(window);

            startX = position.X;
            startY = position.Y;
        }

        public void MouseWheelScrolled(float delta, ref float zc)
        {
            zc -= delta * 100;

            float maxVal = 3000;
            if (zc > maxVal)
            {
                zc = maxVal;
                return;
            }

            float minVal = 600;
            if (zc < minVal)
            {
                zc = minVal;
                return;
            }
        }
    }
}
