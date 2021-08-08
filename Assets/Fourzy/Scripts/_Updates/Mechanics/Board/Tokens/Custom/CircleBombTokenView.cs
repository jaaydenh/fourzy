//@vadym udod

using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class CircleBombTokenView : TokenView
    {
        public CircleBombToken token => base.Token as CircleBombToken;

        public override TokenView SetData(IToken tokenData = null)
        {
            base.SetData(tokenData);

            if (!token.Active)
            {
                active = false;
                SetAlpha(0f);
            }

            return this;
        }

        public override void _Destroy()
        {
            OnActivate();

            base._Destroy();
        }
    }
}