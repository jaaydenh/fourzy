//@vadym udod

using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class PitTokenView : TokenView
    {
        public PitToken token => Token as PitToken;

        public override TokenView SetData(IToken tokenData = null)
        {
            base.SetData(tokenData);

            if (token.Filled)
            {
                active = false;
                SetAlpha(0f);
            }

            return this;
        }
    }
}
