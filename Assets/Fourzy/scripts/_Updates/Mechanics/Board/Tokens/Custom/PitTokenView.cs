//@vadym udod

using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class PitTokenView : TokenView
    {
        public PitToken token { get; private set; }

        public override TokenView SetData(IToken tokenData = null)
        {
            token = tokenData as PitToken;

            if (token.Filled)
            {
                active = false;
                SetAlpha(0f);
            }

            return base.SetData(tokenData);
        }
    }
}
