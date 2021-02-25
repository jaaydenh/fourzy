//@vadym udod

using Photon.Realtime;
using System;

namespace Fourzy._Updates
{
    public class OpponentData
    {
        public static Action<int> onRatingUdpate;
        public static Action<int> onTotalGamesUpdate;

        private Player _player;

        private int rating;
        private int totalGames;

        public string Id { get; private set; }
        public int Rating
        {
            get
            {
                if (_player == null)
                {
                    return rating;
                }

                return FourzyPhotonManager.GetPlayerProperty(_player, Constants.REALTIME_RATING_KEY, -1);
            }
            set
            {
                if (_player == null)
                {
                    rating = value;
                }

                onRatingUdpate?.Invoke(value);
            }
        }
        public int TotalGames
        {
            get
            {
                if (_player == null)
                {
                    return totalGames;
                }

                return FourzyPhotonManager.GetPlayerTotalGamesCount(_player);
            }
            set
            {
                if (_player == null)
                {
                    totalGames = value;
                }

                onTotalGamesUpdate?.Invoke(value);
            }
        }

        public OpponentData() { }

        public OpponentData(Player player) :
            this(player.UserId,
                FourzyPhotonManager.GetPlayerProperty(player, Constants.REALTIME_RATING_KEY, -1),
                FourzyPhotonManager.GetPlayerTotalGamesCount(player))
        {
            _player = player;
        }

        public OpponentData(string id, int rating, int totalGames) : this()
        {
            Id = id;

            this.rating = rating;
            this.totalGames = totalGames;
        }
    }
}