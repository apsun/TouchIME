using TouchIME.Input;
using TouchIME.Input.Synaptics;

namespace TouchIME
{
    public class TouchInputFactory
    {
        public static ITouchInput CreateInput()
        {
            return new SynTouchpadInput();
        }
    }
}
