//@vadym udod

using FourzyGameModel.Model;


namespace Fourzy._Updates.Mechanics.Board
{
    public class CrossBombTokenView : TokenView
    {
        public CrossBombToken token { get; private set; }

        public override TokenView SetData(IToken tokenData = null)
        {
            token = tokenData as CrossBombToken;

            if (!token.Active)
            {
                active = false;
                SetAlpha(0f);
            }

            return base.SetData(tokenData);
        }

        public override void _Destroy()
        {
            OnActivate();

            base._Destroy();
        }
    }
}
