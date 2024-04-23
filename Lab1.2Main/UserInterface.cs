using SFML.Graphics;
using SFML.Window;
using System.Numerics;
using System.Text.Json;

namespace EditorMain
{
    public delegate void ClickHandler();

    internal static class UserInterface
    {
        public const float RADTODEG = 180 / MathF.PI;

        private const uint FIELD_WIDTH = 900;
        private const uint WINDOW_WIDTH = 1180;
        private const uint WINDOW_HEIGHT = 790;
        private const string WINDOW_TITLE = "Графический редактор";
        public const string FONT = "calibri.ttf";

        private const int FIRST_BUTTON_X = (int)FIELD_WIDTH + 20;
        private const int SECOND_BUTTON_X = FIRST_BUTTON_X + THIN_BUTTON_WIDTH + 10;
        private const int WIDE_BUTTON_WIDTH = 240;
        private const int THIN_BUTTON_WIDTH = 115;
        private const int BUTTON_HEIGHT = 20;

        public static SFML.Graphics.Color backgroundColor = SFML.Graphics.Color.Black;
        public static SFML.Graphics.Color mainColor = SFML.Graphics.Color.Green;
        public static SFML.Graphics.Color accentColor = SFML.Graphics.Color.White;
        public static SFML.Graphics.Color activeColor = SFML.Graphics.Color.Red;

        private static bool controlPressed = false;
        private static bool rmbPressed = false;

        private static List<int> selectedObjects = new List<int>();
        private static float startX;
        private static float startY;

        private static float phi = -30;
        private static float teta = 45;
        private static float zc = 600;

        private static RenderWindow MAIN_WINDOW = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), WINDOW_TITLE, Styles.Close);


        private static Field fieldBackground = new Field(0, 0, FIELD_WIDTH, WINDOW_HEIGHT, backgroundColor, accentColor);

        private static List<Line> userObjects = new List<Line>();

        private static List<Line> axes = new List<Line>()
        {
            new Line(-FIELD_WIDTH / 2, 0, 0, FIELD_WIDTH / 2, 0, 0, false),
            new Line(0, -WINDOW_HEIGHT / 2, 0, 0, WINDOW_HEIGHT / 2, 0, false),
            new Line(0, 0, -500, 0, 0, 500, false),
        };

        private static Dictionary<string, IDrawable> ui = new Dictionary<string, IDrawable>()
        {
            { "Control panel background", new Field(FIELD_WIDTH, 0, WINDOW_WIDTH - FIELD_WIDTH, WINDOW_HEIGHT, backgroundColor, accentColor) },

            { "Delete button", new Button(FIRST_BUTTON_X, 10, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Удалить выбранные", DeleteSelectedObjects) },

            { "Draw button", new Button(FIRST_BUTTON_X, 50, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Нарисовать", DrawLine) },
            { "Draw x1 text field", new TextField(FIRST_BUTTON_X, 80, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Координата X1", 0.0f) },
            { "Draw y1 text field", new TextField(FIRST_BUTTON_X, 110, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Координата Y1", 0.0f) },
            { "Draw z1 text field", new TextField(FIRST_BUTTON_X, 140, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Координата Z1", 0.0f) },
            { "Draw x2 text field", new TextField(FIRST_BUTTON_X, 170, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Координата X2", 0.0f) },
            { "Draw y2 text field", new TextField(FIRST_BUTTON_X, 200, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Координата Y2", 0.0f) },
            { "Draw z2 text field", new TextField(FIRST_BUTTON_X, 230, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Координата Z2", 0.0f) },

            { "Transition button", new Button(FIRST_BUTTON_X, 270, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Переместить...", Transit) },
            { "Trans x text field", new TextField(FIRST_BUTTON_X, 300, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "по X на", 0.0f) },
            { "Trans y text field", new TextField(FIRST_BUTTON_X, 330, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "по Y на", 0.0f) },
            { "Trans z text field", new TextField(FIRST_BUTTON_X, 360, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "по Z на", 0.0f) },

            { "Rotation button X", new Button(FIRST_BUTTON_X, 400, THIN_BUTTON_WIDTH, BUTTON_HEIGHT, "Повернуть...", RotateX) },
            { "Rotation button Y", new Button(FIRST_BUTTON_X, 430, THIN_BUTTON_WIDTH, BUTTON_HEIGHT, "Повернуть...", RotateY) },
            { "Rotation button Z", new Button(FIRST_BUTTON_X, 460, THIN_BUTTON_WIDTH, BUTTON_HEIGHT, "Повернуть...", RotateZ) },
            { "Rotate x text field", new TextField(SECOND_BUTTON_X, 400, THIN_BUTTON_WIDTH, BUTTON_HEIGHT, "вокруг X на", 0.0f) },
            { "Rotate y text field", new TextField(SECOND_BUTTON_X, 430, THIN_BUTTON_WIDTH, BUTTON_HEIGHT, "вокруг Y на", 0.0f) },
            { "Rotate z text field", new TextField(SECOND_BUTTON_X, 460, THIN_BUTTON_WIDTH, BUTTON_HEIGHT, "вокруг Z на", 0.0f) },

            { "Scaling button", new Button(FIRST_BUTTON_X, 500, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Масштаб", Scale) },
            { "Scale x text field", new TextField(FIRST_BUTTON_X, 530, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Коэффициент X", 1.0f) },
            { "Scale y text field", new TextField(FIRST_BUTTON_X, 560, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Коэффициент Y", 1.0f) },
            { "Scale z text field", new TextField(FIRST_BUTTON_X, 590, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Коэффициент Z", 1.0f) },

            { "Mirror x button", new Button(FIRST_BUTTON_X, 630, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Отразить по X", MirrorX) },
            { "Mirror y button", new Button(FIRST_BUTTON_X, 660, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Отразить по Y", MirrorY) },
            { "Mirror z button", new Button(FIRST_BUTTON_X, 690, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Отразить по Z", MirrorZ) },

            { "Save button", new Button(FIRST_BUTTON_X, 730, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Сохранить", Save) },
            { "Load button", new Button(FIRST_BUTTON_X, 760, WIDE_BUTTON_WIDTH, BUTTON_HEIGHT, "Загрузить", Load) },
        };

        public static Dictionary<string, string> keyCodes = new Dictionary<string, string>()
        {
            {"Num1", "1" },
            {"Num2", "2" },
            {"Num3", "3" },
            {"Num4", "4" },
            {"Num5", "5" },
            {"Num6", "6" },
            {"Num7", "7" },
            {"Num8", "8" },
            {"Num9", "9" },
            {"Num0", "0" },
            {"Period", "." },
            {"Dash", "-" },
            {"Hyphen", "-" },
        };

        private static MainLoopHandler mainLoopHandler = new MainLoopHandler(MAIN_WINDOW, userObjects, RADTODEG, selectedObjects);
        private static KeyPressHandler keyPressHandler = new KeyPressHandler(userObjects, selectedObjects);
        private static MousePressHandler mousePressHandler = new MousePressHandler(userObjects, selectedObjects, ui);


        private static void Draw(RenderWindow window)
        {
            window.SetActive(true);
            window.Clear(backgroundColor);

            fieldBackground.Draw(window, FIELD_WIDTH, WINDOW_HEIGHT, mainColor, phi, teta, zc);

            foreach (Line axis in axes)
            {
                axis.Draw(window, FIELD_WIDTH, WINDOW_HEIGHT, accentColor, phi, teta, zc);
            }

            var curIndex = 0;
            foreach (IDrawable userObject in userObjects)
            {
                if (selectedObjects.Contains(curIndex))
                {
                    userObject.Draw(window, FIELD_WIDTH, WINDOW_HEIGHT, activeColor, phi, teta, zc);
                }
                else
                {
                    userObject.Draw(window, FIELD_WIDTH, WINDOW_HEIGHT, mainColor, phi, teta, zc);
                }

                curIndex++;
            }

            foreach (KeyValuePair<string, IDrawable> uiElement in ui)
            {
                if (uiElement.Value is Button b)
                {
                    b.Draw(window, FIELD_WIDTH, WINDOW_HEIGHT, accentColor, phi, teta, zc);
                }
                else
                {
                    uiElement.Value.Draw(window, FIELD_WIDTH, WINDOW_HEIGHT, mainColor, phi, teta, zc);
                }
            }

            window.Display();
        }

        private static void Close(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var window = (RenderWindow)sender;
                window.Close();
            }
        }

        private static void MousePressed(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var window = (RenderWindow)sender;
                var position = Mouse.GetPosition(window);

                if (((MouseButtonEventArgs)e).Button == Mouse.Button.Left)
                {
                    if (controlPressed)
                    {
                        mousePressHandler.ControlLMBPressed(position, phi, teta, zc);
                    }
                    else
                    {
                        mousePressHandler.LMBPressed(position, phi, teta, zc);
                    }
                }

                if (((MouseButtonEventArgs)e).Button == Mouse.Button.Right && !rmbPressed)
                {
                    mousePressHandler.RMBPressed(MAIN_WINDOW, ref startX, ref startY, ref rmbPressed);
                }
            }
        }

        private static void MouseWheelScrolled(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var d = ((MouseWheelScrollEventArgs)e).Delta;
                mousePressHandler.MouseWheelScrolled(d, ref zc);
            }
        }

        private static void MouseReleased(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                if (((MouseButtonEventArgs)e).Button == Mouse.Button.Right && rmbPressed)
                {
                    rmbPressed = false;
                }
            }
        }

        private static void KeyPressed(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                var key = ((SFML.Window.KeyEventArgs)e).Code;

                foreach (KeyValuePair<string, IDrawable> element in ui)
                {
                    if (element.Value is TextField tf)
                    {
                        if (tf.active)
                        {
                            keyPressHandler.TextFieldType(tf, key);
                            return;
                        }
                    }
                }

                if (key == Keyboard.Key.LControl)
                {
                    controlPressed = true;
                    return;
                }
            }
        }

        private static void KeyReleased(object? sender, EventArgs e)
        {
            if (((SFML.Window.KeyEventArgs)e).Code == Keyboard.Key.LControl)
            {
                controlPressed = false;
            }
        }

        private static void SubscribeEvents(RenderWindow window)
        {
            window.Closed += Close;
            window.MouseButtonPressed += MousePressed;
            window.MouseButtonReleased += MouseReleased;
            window.KeyPressed += KeyPressed;
            window.KeyReleased += KeyReleased;
            window.MouseWheelScrolled += MouseWheelScrolled;
        }

        private static void DrawLine()
        {
            DeleteSelectedObjects();

            float x1 = ((TextField)ui["Draw x1 text field"]).GetFloat();
            float y1 = ((TextField)ui["Draw y1 text field"]).GetFloat();
            float z1 = ((TextField)ui["Draw z1 text field"]).GetFloat();

            float x2 = ((TextField)ui["Draw x2 text field"]).GetFloat();
            float y2 = ((TextField)ui["Draw y2 text field"]).GetFloat();
            float z2 = ((TextField)ui["Draw z2 text field"]).GetFloat();

            userObjects.Add(new Line(x1, y1, z1, x2, y2, z2));
        }

        private static void Transit()
        {
            var dX = ((TextField)ui["Trans x text field"]).GetFloat();
            var dY = ((TextField)ui["Trans y text field"]).GetFloat();
            var dZ = ((TextField)ui["Trans z text field"]).GetFloat();

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    line.X1 += dX;
                    line.X2 += dX;
                    line.Y1 += dY;
                    line.Y2 += dY;
                    line.Z1 += dZ;
                    line.Z2 += dZ;
                }

                ++currentIndex;
            }
        }

        private static void RotateX()
        {
            var angle = ((TextField)ui["Rotate x text field"]).GetFloat();
            angle /= RADTODEG;

            var line1 = new Vector4(1, 0, 0, 0);
            var line2 = new Vector4(0, MathF.Cos(angle), -MathF.Sin(angle), 0);
            var line3 = new Vector4(0, MathF.Sin(angle), MathF.Cos(angle), 0);
            var line4 = new Vector4(0, 0, 0, 1);
            var matrix = new Matrix4x4(line1.X, line1.Y, line1.Z, line1.W, line2.X, line2.Y, line2.Z, line2.W, line3.X, line3.Y, line3.Z, line3.W, line4.X, line4.Y, line4.Z, line4.W);

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    var oldCoords1 = new Vector4(line.X1, line.Y1, line.Z1, 1);
                    var newCoords1 = Vector4.Transform(oldCoords1, matrix);
                    newCoords1.X /= newCoords1.W;
                    newCoords1.Y /= newCoords1.W;
                    line.X1 = newCoords1.X;
                    line.Y1 = newCoords1.Y;
                    line.Z1 = newCoords1.Z;

                    var oldCoords2 = new Vector4(line.X2, line.Y2, line.Z2, 1);
                    var newCoords2 = Vector4.Transform(oldCoords2, matrix);
                    newCoords2.X /= newCoords2.W;
                    newCoords2.Y /= newCoords2.W;
                    line.X2 = newCoords2.X;
                    line.Y2 = newCoords2.Y;
                    line.Z2 = newCoords2.Z;
                }

                ++currentIndex;
            }
        }

        private static void RotateY()
        {
            var angle = ((TextField)ui["Rotate y text field"]).GetFloat();
            angle /= RADTODEG;

            var line1 = new Vector4(MathF.Cos(angle), 0, MathF.Sin(angle), 0);
            var line2 = new Vector4(0, 1, 0, 0);
            var line3 = new Vector4(-MathF.Sin(angle), 0, MathF.Cos(angle), 0);
            var line4 = new Vector4(0, 0, 0, 1);
            var matrix = new Matrix4x4(line1.X, line1.Y, line1.Z, line1.W, line2.X, line2.Y, line2.Z, line2.W, line3.X, line3.Y, line3.Z, line3.W, line4.X, line4.Y, line4.Z, line4.W);

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    var oldCoords1 = new Vector4(line.X1, line.Y1, line.Z1, 1);
                    var newCoords1 = Vector4.Transform(oldCoords1, matrix);
                    newCoords1.X /= newCoords1.W;
                    newCoords1.Y /= newCoords1.W;
                    line.X1 = newCoords1.X;
                    line.Y1 = newCoords1.Y;
                    line.Z1 = newCoords1.Z;

                    var oldCoords2 = new Vector4(line.X2, line.Y2, line.Z2, 1);
                    var newCoords2 = Vector4.Transform(oldCoords2, matrix);
                    newCoords2.X /= newCoords2.W;
                    newCoords2.Y /= newCoords2.W;
                    line.X2 = newCoords2.X;
                    line.Y2 = newCoords2.Y;
                    line.Z2 = newCoords2.Z;
                }

                ++currentIndex;
            }
        }

        private static void RotateZ()
        {
            var angle = ((TextField)ui["Rotate z text field"]).GetFloat();
            angle /= RADTODEG;

            var line1 = new Vector4(MathF.Cos(angle), -MathF.Sin(angle), 0, 0);
            var line2 = new Vector4(MathF.Sin(angle), MathF.Cos(angle), 0, 0);
            var line3 = new Vector4(0, 0, 1, 0);
            var line4 = new Vector4(0, 0, 0, 1);
            var matrix = new Matrix4x4(line1.X, line1.Y, line1.Z, line1.W, line2.X, line2.Y, line2.Z, line2.W, line3.X, line3.Y, line3.Z, line3.W, line4.X, line4.Y, line4.Z, line4.W);

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    var oldCoords1 = new Vector4(line.X1, line.Y1, line.Z1, 1);
                    var newCoords1 = Vector4.Transform(oldCoords1, matrix);
                    newCoords1.X /= newCoords1.W;
                    newCoords1.Y /= newCoords1.W;
                    line.X1 = newCoords1.X;
                    line.Y1 = newCoords1.Y;
                    line.Z1 = newCoords1.Z;

                    var oldCoords2 = new Vector4(line.X2, line.Y2, line.Z2, 1);
                    var newCoords2 = Vector4.Transform(oldCoords2, matrix);
                    newCoords2.X /= newCoords2.W;
                    newCoords2.Y /= newCoords2.W;
                    line.X2 = newCoords2.X;
                    line.Y2 = newCoords2.Y;
                    line.Z2 = newCoords2.Z;
                }

                ++currentIndex;
            }
        }

        private static void MirrorX()
        {
            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    line.X1 = -line.X1;
                    line.X2 = -line.X2;
                }

                ++currentIndex;
            }
        }

        private static void MirrorY()
        {
            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    line.Y1 = -line.Y1;
                    line.Y2 = -line.Y2;
                }

                ++currentIndex;
            }
        }

        private static void MirrorZ()
        {
            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    line.Z1 = -line.Z1;
                    line.Z2 = -line.Z2;
                }

                ++currentIndex;
            }
        }

        private static void Scale()
        {
            var sX = ((TextField)ui["Scale x text field"]).GetFloat();
            var sY = ((TextField)ui["Scale y text field"]).GetFloat();
            var sZ = ((TextField)ui["Scale z text field"]).GetFloat();

            var line1 = new Vector4(sX, 0, 0, 0);
            var line2 = new Vector4(0, sY, 0, 0);
            var line3 = new Vector4(0, 0, sZ, 0);
            var line4 = new Vector4(0, 0, 0, 1);
            var matrix = new Matrix4x4(line1.X, line1.Y, line1.Z, line1.W, line2.X, line2.Y, line2.Z, line2.W, line3.X, line3.Y, line3.Z, line3.W, line4.X, line4.Y, line4.Z, line4.W);

            var currentIndex = 0;
            foreach (Line line in userObjects)
            {
                if (selectedObjects.Contains(currentIndex))
                {
                    var oldCoords1 = new Vector4(line.X1, line.Y1, line.Z1, 1);
                    var newCoords1 = Vector4.Transform(oldCoords1, matrix);
                    newCoords1.X /= newCoords1.W;
                    newCoords1.Y /= newCoords1.W;
                    line.X1 = newCoords1.X;
                    line.Y1 = newCoords1.Y;
                    line.Z1 = newCoords1.Z;

                    var oldCoords2 = new Vector4(line.X2, line.Y2, line.Z2, 1);
                    var newCoords2 = Vector4.Transform(oldCoords2, matrix);
                    newCoords2.X /= newCoords2.W;
                    newCoords2.Y /= newCoords2.W;
                    line.X2 = newCoords2.X;
                    line.Y2 = newCoords2.Y;
                    line.Z2 = newCoords2.Z;
                }

                ++currentIndex;
            }
        }

        private static void DeleteSelectedObjects()
        {
            selectedObjects.Sort();

            foreach (int i in selectedObjects.FastReverse())
            {
                userObjects.RemoveAt(i);
            }

            selectedObjects.Clear();
        }

        private static void Save()
        {
            Stream stream;
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "json files (*.json)|*.json";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    JsonSerializer.Serialize(stream, userObjects, options);
                    stream.Dispose();
                    stream.Close();
                }
            }
        }

        private static void Load()
        {
            Stream stream;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "json files (*.json)|*.json";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((stream = openFileDialog.OpenFile()) != null)
                {
                    List<Line>? newList = JsonSerializer.Deserialize<List<Line>>(stream);
                    selectedObjects.Clear();

                    if (newList == null)
                    {
                        userObjects = new List<Line>();
                        return;
                    }

                    userObjects.Clear();

                    foreach(Line l in newList)
                    {
                        userObjects.Add(new Line(l.X1, l.Y1, l.Z1, l.X2, l.Y2, l.Z2));
                    }

                    stream.Dispose();
                    stream.Close();
                }
            }
        }

        private static void DrawExample()
        {
            //Пол
            userObjects.Add(new Line(0, 0, 0, 300, 0, 0));
            userObjects.Add(new Line(300, 0, 0, 300, 0, 100));
            userObjects.Add(new Line(300, 0, 100, 0, 0, 100));
            userObjects.Add(new Line(0, 0, 100, 0, 0, 0));

            //Потолок
            userObjects.Add(new Line(0, 100, 0, 300, 100, 0));
            userObjects.Add(new Line(300, 100, 0, 300, 100, 100));
            userObjects.Add(new Line(300, 100, 100, 0, 100, 100));
            userObjects.Add(new Line(0, 100, 100, 0, 100, 0));

            //Стены
            userObjects.Add(new Line(0, 0, 0, 0, 100, 0));
            userObjects.Add(new Line(300, 0, 0, 300, 100, 0));
            userObjects.Add(new Line(300, 0, 100, 300, 100, 100));
            userObjects.Add(new Line(0, 0, 100, 0, 100, 100));

            //Дверь
            userObjects.Add(new Line(33, 0, 100, 33, 80, 100));
            userObjects.Add(new Line(67, 0, 100, 67, 80, 100));
            userObjects.Add(new Line(33, 80, 100, 67, 80, 100));

            //Окно у двери
            userObjects.Add(new Line(100, 33, 100, 150, 33, 100));
            userObjects.Add(new Line(150, 33, 100, 150, 67, 100));
            userObjects.Add(new Line(150, 67, 100, 100, 67, 100));
            userObjects.Add(new Line(100, 67, 100, 100, 33, 100));

            //Окно не у двери
            userObjects.Add(new Line(200, 33, 100, 250, 33, 100));
            userObjects.Add(new Line(250, 33, 100, 250, 67, 100));
            userObjects.Add(new Line(250, 67, 100, 200, 67, 100));
            userObjects.Add(new Line(200, 67, 100, 200, 33, 100));

            //Заднее окно
            userObjects.Add(new Line(200, 33, 0, 250, 33, 0));
            userObjects.Add(new Line(250, 33, 0, 250, 67, 0));
            userObjects.Add(new Line(250, 67, 0, 200, 67, 0));
            userObjects.Add(new Line(200, 67, 0, 200, 33, 0));

            //Крыша
            userObjects.Add(new Line(0, 100, 0, 0, 150, 50));
            userObjects.Add(new Line(0, 100, 100, 0, 150, 50));
            userObjects.Add(new Line(300, 100, 0, 300, 150, 50));
            userObjects.Add(new Line(300, 100, 100, 300, 150, 50));
            userObjects.Add(new Line(0, 150, 50, 300, 150, 50));

            //Кровать
            userObjects.Add(new Line(300, 0, 33, 300, 40, 33));
            userObjects.Add(new Line(300, 40, 33, 300, 40, 0));
            userObjects.Add(new Line(300, 40, 0, 300, 0, 0));
            userObjects.Add(new Line(300, 20, 33, 300, 20, 0));
            userObjects.Add(new Line(300, 20, 0, 220, 20, 0));
            userObjects.Add(new Line(220, 20, 0, 220, 20, 33));
            userObjects.Add(new Line(220, 20, 33, 300, 20, 33));
            userObjects.Add(new Line(220, 20, 0, 220, 0, 0));
            userObjects.Add(new Line(220, 20, 33, 220, 0, 33));
            userObjects.Add(new Line(220, 0, 33, 220, 30, 33));
            userObjects.Add(new Line(220, 30, 33, 220, 30, 0));
            userObjects.Add(new Line(220, 30, 0, 220, 0, 0));

            //Стол
            userObjects.Add(new Line(300, 0, 100, 300, 30, 100));
            userObjects.Add(new Line(300, 0, 67, 300, 30, 67));
            userObjects.Add(new Line(220, 0, 100, 220, 30, 100));
            userObjects.Add(new Line(220, 0, 67, 220, 30, 67));
            userObjects.Add(new Line(300, 30, 100, 300, 30, 67));
            userObjects.Add(new Line(300, 30, 67, 220, 30, 67));
            userObjects.Add(new Line(220, 30, 67, 220, 30, 100));
            userObjects.Add(new Line(220, 30, 100, 300, 30, 100));
        }

        public static void Start()
        {
            SubscribeEvents(MAIN_WINDOW);
            Draw(MAIN_WINDOW);
            //DrawExample();

            while (MAIN_WINDOW.IsOpen)
            { 
                MAIN_WINDOW.DispatchEvents();
                
                if (rmbPressed)
                {
                    mainLoopHandler.HandleCameraRotation(ref startX, ref startY, ref phi, ref teta);
                }

                Draw(MAIN_WINDOW);
            }
        }
    }
}