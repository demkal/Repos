namespace InstaGramLikeTake2
{
    // A class to easily access constant values
    class Constants
    {
        // Name of your app
        public static string AppName = "ExampleName";

        // Your client id. You get it from your registered client on Instagram. (https://www.instagram.com/developer/clients/manage/)
        public static string ClientId = "af2caa0aeaed43f3bb97f3ae04fd272e";

        // Specify what you want to access from the Instagram API. I didn't use all of them in this example.
        // For more scope parameters look at the Instagram API documentation. (https://www.instagram.com/developer/authorization/)
        public static string Scopes = "basic public_content likes";

        // The authorization URL of Instagram. Note that I use client-side authentication in this example.
        // Change the response_type to "code" to use server-side authentication. (https://www.instagram.com/developer/authentication/)
        // Don't forget to insert your client_id and redirect_uri in the url.
        public static string AuthorizationUrl = "https://api.instagram.com/oauth/authorize/?client_id=af2caa0aeaed43f3bb97f3ae04fd272e&redirect_uri=http://mobileapp120170703104520.azurewebsites.net/api/values/get?ZUMO-API-VERSION=2.0.0&response_type=code";

        // Your redirect URI. The URI must match with the registered redirect URI of your Instagram client. (https://www.instagram.com/developer/clients/manage/)
        public static string RedirectUri = "http://mobileapp120170703104520.azurewebsites.net/api/values/get?ZUMO-API-VERSION=2.0.0";

        public static string InstagramAccessToken = "instagram.accesstoken";
    }
}
