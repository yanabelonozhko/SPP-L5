using System.Runtime.InteropServices.ComTypes;

namespace DepInjContainer
{
    public class ImplInfo
    {
        public Enum? Index { get; set; }
        public LivingTime TimeToLive { get; set; }
        public Type ImplType { get; set; }

        public ImplInfo(Enum? index, LivingTime timeToLive, Type implType)
        {
            Index = index;
            TimeToLive = timeToLive;
            ImplType = implType;            
        }
    }
}