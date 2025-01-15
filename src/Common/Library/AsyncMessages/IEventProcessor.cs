namespace Library.AsyncMessages;

public interface IEventProcessor
{
  Task ProcessEvent(string message);
}
