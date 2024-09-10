namespace Bloggest.Components.Bus.Types;

public class HandlerSubscription
{
    public Type HandlerType { get; }
    
    public HandlerSubscription(Type handlerType)
    {
        HandlerType = handlerType;
    }
}