# BlazorIdentity

Microsoft Identity MVC pages (.cshtml, cs), refactored to Blazor (.razor) components (primarily so that they will more easily conform to the styles and layout of the Blazor application, and remove the need for jquery for validation.)

Born out of frustration that there are not convienient Blazor versions of the Microsoft Identity pages, I thought that it may be possible to convert these into Blazor components, this seems possible for the most part, although some SignInManager calls are in a 'standard' form submit to a controller.

Work in progress:

login - working
logout - working
most components created as placeholder
signin manager linked with standard controller 'IdentityController'.

TODO:

Finish first pass creating components.
User self management components
2fa components
Handling / better handling of validation messages
URL redirects
Remove scaffold components once complete

Feel free to use, but at your own risk
