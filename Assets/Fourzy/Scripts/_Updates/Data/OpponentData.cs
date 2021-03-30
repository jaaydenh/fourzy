//@vadym udod

using Photon.Realtime;
using System;

namespace Fourzy._Updates
{
    public class OpponentData
    {
        public static Action<int> onRatingUdpate;
        public static Action<int> onTotalGamesUpdate;

        public bool useExternalRatingValue = false;
        public bool useExternalTotalGamesValue = false;

        private Player _player;

        private int rating;
        private int totalGames;

        public string Id { get; private set; }
        public int Rating
        {
            get
            {
                if (_player == null || useExternalRatingValue)
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
                if (_player == null || useExternalTotalGamesValue)
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

        /// <summary>
        /// This value will be used as current before new value received from opponent (if any)
        /// </summary>
        /// <param name="rating"></param>
        public void SetExternalRating(int rating)
        {
            this.rating = rating;
            useExternalRatingValue = true;

            onRatingUdpate?.Invoke(rating);
        }

        public void SetExternalTotalGames(int totalGames)
        {
            this.totalGames = totalGames;
            useExternalTotalGamesValue = true;

            onTotalGamesUpdate?.Invoke(totalGames);
        }
    }
}