namespace Aspire_App.Web.Helpers;

public struct CourseMessage
{
  public CourseMessage(string messageType, string message)
  {
    MessageType = messageType;
    Message = message;
  }

  public string MessageType { get;  }
  public string Message { get;  }
}


enum CourseAction
{
  CourseCreated,
  CourseDeleted,
  ClassCreated,
  ClassDeleted
}
