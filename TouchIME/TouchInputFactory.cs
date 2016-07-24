using SynapticsInput;
using TouchInputAPI;

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
