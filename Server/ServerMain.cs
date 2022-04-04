
namespace Server;// namespace of the file

class MainService
{
    public static void Main(string[] args)
    {
        int port = InitPort(args);
        GameServer cardServer = new GameServer();
        cardServer.SetPort(port);
        cardServer.InitServer();

    }

    private static int InitPort(string[] args)
    {
        if (args.Length > 0)
            return int.Parse(args[0]);
        else
            return 22737;
    }
}

