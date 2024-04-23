using SFML.Window;

namespace EditorMain
{
    internal class KeyPressHandler
    {
        public List<Line> userObjects;
        public List<int> selectedObjects;

        public KeyPressHandler(List<Line> userObjects, List<int> selectedObjectIndex)
        {
            this.userObjects = userObjects;
            this.selectedObjects = selectedObjectIndex;
        }

        public void TextFieldType(TextField tf, Keyboard.Key key)
        {
            if (key == Keyboard.Key.Backspace && tf.text.Count() > 0) 
            {
                tf.text = tf.text.Remove(tf.text.Count() - 1);
                return;    
            }

            string abc = tf.alphabet;
            if (tf.text == "")
            {
                abc += '-';
            }
            else 
            if (!tf.text.Contains('.'))
            {
                abc += '.';
            }

            string keyString = key.ToString();
            if (UserInterface.keyCodes.Keys.Contains(keyString))
            {
                string toAdd = UserInterface.keyCodes[keyString];
                if (abc.Contains(toAdd) && tf.text.Count() < tf.frameWidth / 10)
                {
                    tf.text += toAdd;
                }
            }
        }
    }
}
