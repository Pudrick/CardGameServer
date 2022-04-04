namespace Server;

class Player
{
    private int ID;
    private string userName;
    private string password;

    private enum OnlineCondition
    {
        Offline,
        Available,
        Playing
    };
}