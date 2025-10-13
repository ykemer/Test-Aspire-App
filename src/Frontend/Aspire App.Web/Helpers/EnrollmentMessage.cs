namespace Aspire_App.Web.Helpers;

public struct EnrollmentMessage
{
  public EnrollmentMessage(string messageType, string message)
  {
    MessageType = messageType;
    Message = message;
  }

  public string MessageType { get;  }
  public string Message { get;  }
}


enum EnrollmentAction
{
  Enroll,
  Leave
}
