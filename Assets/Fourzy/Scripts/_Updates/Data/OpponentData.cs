//@vadym udod

#if !MOBILE_SKILLZ
using Photon.Realtime;
#endif

using System;

namespace Fourzy._Updates
{
    public class OpponentData
    {
        public static Action<int> onRatingUdpate;
        public static Action<int> onTotalGamesUpdate;

        public bool useExternalRatingValue = false;
        public bool useExternalTotalGamesValue = false;

#if !MOBILE_SKILLZ
        private Player _player;
#endif

        private int rating;
        private int totalGames;

        public string Id { get; private set; }
        public int Rating
        {
            get
            {
#if !MOBILE_SKILLZ
                if (_player == null || useExternalRatingValue)
                {
                    return rating;
                }

                return FourzyPhotonManager.GetPlayerProperty(_player, Constants.REALTIME_RATING_KEY, -1);
#else
                return 0;
#endif
            }
            set
            {
#if !MOBILE_SKILLZ
                if (_player == null)
                {
                    rating = value;
                }
#endif

                onRatingUdpate?.Invoke(value);
            }
        }
        public int TotalGames
        {
            get
            {
#if !MOBILE_SKILLZ
                if (_player == null || useExternalTotalGamesValue)
                {
                    return totalGames;
                }

                return FourzyPhotonManager.GetPlayerTotalGamesCount(_player);
#else
                return 0;
#endif
            }
            set
            {
#if !MOBILE_SKILLZ
                if (_player == null)
                {
                    totalGames = value;
                }
#endif

                onTotalGamesUpdate?.Invoke(value);
            }
        }

        public OpponentData() { }

#if !MOBILE_SKILLZ
        public OpponentData(Player player) : this(player.UserId, FourzyPhotonManager.GetPlayerProperty(player, Constants.REALTIME_RATING_KEY, -1), FourzyPhotonManager.GetPlayerTotalGamesCount(player))
        {
            _player = player;
        }
#endif

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