using System.Collections;

namespace Server;
class PlayerList
{
    private Hashtable playerList;

    public void Init()
    {
        this.playerList = new Hashtable();
    }
}
