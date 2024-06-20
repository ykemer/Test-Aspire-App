namespace Library.Tools;

public static class Waiter
{
    public static Task Wait(int time)
    {
        return Task.Delay(time);
    }
}