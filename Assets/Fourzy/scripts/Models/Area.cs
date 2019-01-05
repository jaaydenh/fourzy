using Fourzy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An area is an unlockable part of the Fourzy Game. 
//   Players spend money to unlock an area.
//   Areas have several types of game elements associated with them.

public class GameArea {
    public string Name { get; set; }
    public string Description { get; set; }

    //Initial Time in Seconds
    public int InitialTime = 30;

    //Additional Time in Second added after each move.
    public int AdditionalTime = 12;
    
    public GameArea()
    {
        this.Name = "Random Area Name";
        this.Description= "Random Area Description";
     }

    //Return Information on a Game Area.
    public static GameArea GetGameArea(Area area)
    {
        return new GameArea();
    }
}
