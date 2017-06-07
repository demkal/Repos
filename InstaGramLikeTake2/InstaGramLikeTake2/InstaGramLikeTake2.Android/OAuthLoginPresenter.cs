using Xamarin.Auth;

namespace InstaGramLikeTake2.Droid
{
    public class OAuthLoginPresenter
    {
        public void Login(Authenticator authenticator)
        {
            Xamarin.Forms.Forms.Context.StartActivity(authenticator.GetUI(Xamarin.Forms.Forms.Context as MainActivity));
        }
    }
}