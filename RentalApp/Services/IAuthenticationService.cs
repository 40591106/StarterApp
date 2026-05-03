using RentalApp.Database.Models;

namespace RentalApp.Services;

/// <summary>
/// Contract for authentication services supporting login, registration, and role checks.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Fired when the authentication state changes.
    /// </summary>
    event EventHandler<bool>? AuthenticationStateChanged;

    /// <summary>
    /// Gets whether a user is currently authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the currently authenticated user, or null if not authenticated.
    /// </summary>
    User? CurrentUser { get; }

    /// <summary>
    /// Gets the roles assigned to the current user.
    /// </summary>
    List<string> CurrentUserRoles { get; }

    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    Task<AuthenticationResult> LoginAsync(string email, string password);

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    Task<AuthenticationResult> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password
    );

    /// <summary>
    /// Logs out the current user and clears authentication state.
    /// </summary>
    Task LogoutAsync();

    /// <summary>
    /// Checks whether the current user has the specified role.
    /// </summary>
    bool HasRole(string roleName);

    /// <summary>
    /// Checks whether the current user has any of the specified roles.
    /// </summary>
    bool HasAnyRole(params string[] roleNames);

    /// <summary>
    /// Checks whether the current user has all of the specified roles.
    /// </summary>
    bool HasAllRoles(params string[] roleNames);

    /// <summary>
    /// Changes the current user's password after verifying the current password.
    /// </summary>
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
}