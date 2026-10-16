# Security Policy

## Supported versions

Only the latest release of the Contact Conversion Wizard receives security fixes.

## Reporting a vulnerability

Please do **not** open a public issue for security problems. Use
[GitHub's private vulnerability reporting](https://github.com/Rillke/Contact-Conversion-Wizard/security/advisories/new)
instead. You will get an answer as soon as a maintainer finds time; this is a volunteer project.

## Notes for users

- Versions up to 3.5.0.2 could store the password of a Google account in clear text in
  `%APPDATA%\ContactConversionWizard\CCW.config` (option "Google account for direct import/export").
  Newer versions remove the password from this file on start. If you ever entered your password there,
  delete the line starting with `g_pass` (or the whole file) and consider changing the password.
- Up to 3.5.0.2 the optional update check contacted `http://software.nv-systems.net` via unencrypted HTTP.
  That domain no longer belongs to the project. Newer versions only ask the GitHub API via HTTPS.
