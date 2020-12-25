//@vadym udod

using FourzyGameModel.Model;
using System;

[Serializable]
public class ClientPlayer : Player
{
    public string playfabID;

    public ClientPlayer(Player original) : base(original) { }

    public ClientPlayer(ClientPlayer original) : base(original)
    {
        this.playfabID = original.playfabID;
    }
}
