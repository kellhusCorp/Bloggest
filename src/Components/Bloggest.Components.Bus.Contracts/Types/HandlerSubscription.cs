namespace Bloggest.Components.Bus.Contracts.Types;

public class HandlerSubscription
{
    public Type HandlerType { get; }
    
    public HandlerSubscription(Type handlerType)
    {
        HandlerType = handlerType;
    }
}