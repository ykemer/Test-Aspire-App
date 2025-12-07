namespace Aspire_App.Web.Helpers;

public struct ClassMessage
{
  public ClassMessage(string messageType, string message)
  {
    MessageType = messageType;
    Message = message;
  }

  public string MessageType { get;  }
  public string Message { get;  }
}
