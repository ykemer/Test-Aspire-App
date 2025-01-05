using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace Aspire_App.Web.Handlers;

public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CustomAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        AuthenticationStateProvider authenticationStateProvider)
        : base(options, logger, encoder, clock)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated ?? false)
        {
            var ticket = new AuthenticationTicket(authState.User, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        return AuthenticateResult.NoResult();
    }
}