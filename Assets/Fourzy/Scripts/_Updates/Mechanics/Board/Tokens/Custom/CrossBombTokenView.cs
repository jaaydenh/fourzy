//@vadym udod

using FourzyGameModel.Model;


namespace Fourzy._Updates.Mechanics.Board
{
    public class CrossBombTokenView : TokenView
    {
        public CrossBombToken token => Token as CrossBombToken;

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
