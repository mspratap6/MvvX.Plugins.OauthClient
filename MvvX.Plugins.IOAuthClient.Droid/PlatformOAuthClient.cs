using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using Xamarin.Auth;

namespace MvvX.Plugins.IOAuthClient.Droid
{
    public class PlatformOAuthClient : IOAuthClient
    {
        #region Fields

        private OAuth2Authenticator auth;

        public bool AllowCancel
        {
            get
            {
                return auth.AllowCancel;
            }

            set
            {
                auth.AllowCancel = value;
            }
        }

        public string AccessTokenName
        {
            get
            {
                return auth.AccessTokenName;
            }

            set
            {
                auth.AccessTokenName = value;
            }
        }

        public string ClientId
        {
            get
            {
                return auth.ClientId;
            }
        }

        public string ClientSecret
        {
            get
            {
                return auth.ClientSecret;
            }
        }

        public bool DoNotEscapeScope
        {
            get
            {
                return auth.DoNotEscapeScope;
            }

            set
            {
                auth.DoNotEscapeScope = value;
            }
        }

        public Dictionary<string, string> RequestParameters
        {
            get
            {
                return auth.RequestParameters;
            }
        }

        #endregion

        #region Events
        
        public event EventHandler<IAuthenticatorCompletedEventArgs> Completed;

        private void OAuth2Authenticator_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (Completed != null)
            {
                this.Completed(sender, new PlatformAuthenticatorCompletedEventArgs(e));
            }
        }
        
        event EventHandler<IAuthenticatorErrorEventArgs> Error;

        private void OAuth2Authenticator_Error(object sender, AuthenticatorErrorEventArgs e)
        {
            if (Completed != null)
            {
                this.Error(sender, new PlatformAuthenticatorErrorEventArgs(e));
            }
        }

        #endregion

        #region Methods
        
        public void Start(object parameter, string title)
        {
            if (!(parameter is Context))
                throw new ArgumentException("parameter must be a Context object");

            var intent = auth.GetUI(parameter as Context);
            (parameter as Context).StartActivity(intent);
        }

        public void New(string clientId, string scope, Uri authorizeUrl, Uri redirectUrl)
        {
            auth = new OAuth2Authenticator(
                clientId: clientId,
                scope: scope,
                authorizeUrl: authorizeUrl,
                redirectUrl: redirectUrl);

            auth.Completed += OAuth2Authenticator_Completed;
            auth.Error += OAuth2Authenticator_Error;
        }

        public void New(string clientId, string clientSecret, string scope, Uri authorizeUrl, Uri redirectUrl, Uri accessTokenUrl)
        {
            auth = new OAuth2Authenticator(
                clientId: clientId,
                clientSecret: clientSecret,
                scope: scope,
                authorizeUrl: authorizeUrl,
                redirectUrl: redirectUrl,
                accessTokenUrl: accessTokenUrl);

            auth.Completed += OAuth2Authenticator_Completed;
            auth.Error += OAuth2Authenticator_Error;
        }

        public IOAuth2Request CreateRequest(string method, Uri url, IDictionary<string, string> parameters, IAccount account)
        {
            var request = new OAuth2Request(method, url, parameters, new Account(account.Username, account.Properties, account.Cookies));
            return new PlatformOAuth2Request(request);
        }

        #endregion
    }
}