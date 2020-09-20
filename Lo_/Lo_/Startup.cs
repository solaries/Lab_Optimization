using System; 
using System.Collections.Generic; 
using System.Linq; 
using Microsoft.Owin; 
using System.Threading.Tasks; 
using Microsoft.Owin.Security.OAuth; 
using System.Web; 
using Owin; 
using System.Web.Http;  
  
[assembly: OwinStartup(typeof(Lo.Startup))] 
namespace  Lo  
{ 
 
    public class Startup 
    { 
        public void Configuration(IAppBuilder app) 
        { 
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll); 
 
            var theProvider = new AuthorizationProvider(); 
            OAuthAuthorizationServerOptions option = new OAuthAuthorizationServerOptions 
            { 
                AllowInsecureHttp = true, 
                TokenEndpointPath = new PathString("/token"), 
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1), 
                Provider = theProvider 
            }; 
            app.UseOAuthAuthorizationServer(option); 
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()); 
 
            HttpConfiguration config = new HttpConfiguration(); 
            WebApiConfig.Register(config); 
        } 
    } 
} 
