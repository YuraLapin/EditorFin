using SFML.Graphics;
using SFML.Window;

namespace EditorMain
{
    internal class MainLoopHandler
    {
        public RenderWindow mainWindow;
        public List<Line> userObjects;
        public List<int> selectedObjecs;
        public float radtodeg;
        public int SCALE_COEF = 100;

        public MainLoopHandler(RenderWindow mainWindow, List<Line> userObjects, float radtodeg, List<int> selectedObjectIndex)
        {
            this.mainWindow = mainWindow;
            this.userObjects = userObjects;
            this.radtodeg = radtodeg;
            this.selectedObjecs = selectedObjectIndex; 
        }

        public void HandleCameraRotation(ref float startX, ref float startY, ref float phi, ref float teta)
        {
            var position = Mouse.GetPosition(mainWindow);
            var dX = position.X - startX;
            var dY = position.Y - startY;
            startX = position.X;
            startY = position.Y;

            phi += dX / 10;
            teta += dY / 10;

            if ((int)phi / 90 == 0)
            {
                phi += (float)0.00001;
            }

            if ((int)teta / 90 == 0)
            {
                phi += (float)0.00001;
            }
        }
    }
}
